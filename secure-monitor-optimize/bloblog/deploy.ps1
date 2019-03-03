Push-Location ../aicore
$publishDir = "publish"
$package = "publish.zip"
if (Test-path $publishDir) {Remove-Item -Recurse -Force $publishDir}
if (Test-path $package) {Remove-item $package}
dotnet publish -c release -o $publishDir web.csproj 
Compress-Archive -Path $publishDir/* -DestinationPath $package -Force
Move-Item -Path $package -Destination ../bloglog -Force
Pop-Location

# create a new webapp
$planName="laaz203bloblog"
$resourceGroup = "monitorwebapps"
$appName="laaz203bloblog"
$location="westus"
$stgacct = "laaz20bloglogstg"

az group create -n $resourceGroup -l $location

az appservice plan create -n $planName -g $resourceGroup -l $location --sku B1
az webapp create -n $appName -g $resourceGroup --plan $planName

# deploy publish.zip using the kudu zip api
az webapp deployment source config-zip -n $appName -g $resourceGroup --src $package

# launch the site in a browser
$site = az webapp show -n $appName -g $resourceGroup --query "defaultHostName" -o tsv
Start-Process https://$site

az storage account create `
 -g $resourceGroup `
 -n $stgacct `
 -l $location `
 --sku Standard_LRS

$key = $(az storage account keys list `
 --account-name $stgacct `
 -g $resourceGroup `
 --query "[0].value" `
 --output tsv)
$key

az storage container create `
 --name logs `
 --account-name $stgacct `
 --account-key $key

az storage container list `
 --account-name $stgacct `
 --account-key $key

az storage container policy list `
 -c logs `
 --account-name $stgacct `
 --account-key $key

$today = Get-Date 
$tomorrow = $today.AddDays(1)
$today.ToString("yyyy-MM-dd")
$tomorrow.ToString("yyyy-MM-dd")

az storage container policy create `
 -c logs `
 --name "logpolicy" `
 --start $today.ToString("yyyy-MM-dd") `
 --expiry $tomorrow.ToString("yyyy-MM-dd") `
 --permissions lwrd `
 --account-name $stgacct `
 --account-key $key

$sas = az storage container generate-sas `
 --name logs `
 --policy-name logpolicy `
 --account-name $stgacct `
 --account-key $key `
 -o tsv
$sas

$containerSasUrl = "https://$stgacct.blob.core.windows.net/logs?$sas"
$containerSasUrl

az ad sp list --display-name LaAz203WebSiteManager
az ad sp delete --id "a7520b14-40f8-466f-90a5-372c789781bc"

$sp = az ad sp create-for-rbac --name LaAz203WebSiteManager | ConvertFrom-Json

$subid = az account show --query "id" -o tsv
$tenantid = az account show --query "tenantId" -o tsv

$clientId = $sp.appId
$clientSecret = $sp.password
"var clientId = `"$clientId`";" 
"var clientSecret = `"$clientSecret`";"
"var subscriptionId = `"$subid`";"
"var tenantId = `"$tenantid`";"
"var sasUrl = `"$containerSasUrl`";"

az webapp log config -n $appName -g $resourceGroup --level information --application-logging true

az webapp log show -n $appName -g $resourceGroup


az webapp delete -g $resourceGroup -n $appName
az group delete -g $resourceGroup --yes



