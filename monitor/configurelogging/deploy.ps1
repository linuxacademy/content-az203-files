$publishFolder = "publish"
$publishDir = "./web/" + $publishFolder
if (Test-path $publishFolder) {Remove-Item -Recurse -Force $publishDir}
$destination = "publish.zip"
if(Test-path $destination) {Remove-item $destination}

dotnet publish -c release -o $publishFolder ./web/web.csproj 

Add-Type -assembly "system.io.compression.filesystem"
[io.compression.zipfile]::CreateFromDirectory($publishDir, $destination)

# create a new webapp
$planName="laaz203monitorwaplan"
$resourceGroup = "monitorwebapps"
$appName="laaz203monitorwalogs"
$location="eastus"
az group create -n $resourceGroup -l $location
az appservice plan create -n $planName -g $resourceGroup -l $location --sku B1
az webapp create -n $appName -g $resourceGroup --plan $planName

# deploy publish.zip using the kudu zip api
az webapp deployment source config-zip -n $appName -g $resourceGroup --src $destination

# launch the site in a browser
$site = az webapp show -n $appName -g $resourceGroup --query "defaultHostName" -o tsv
Start-Process https://$site

$stgacct = "laaz203monitorwastg"
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

az storage container generate-sas `
 --name logs `
 --policy-name logpolicy `
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

az storage container show `
 -n logs `
 --account-name $stgacct `
 --account-key $key

az storage container policy create `
 -c logs `
 --account-name $stgacct `
 --account-key $key





az ad sp create-for-rbac --name LaAz203WebSiteManager

az account show --query "id" -o tsv
az account show --query "tenantId" -o tsv

az account get-access-token
az account list

az ad sp list


$spId = az ad sp list --display-name LaAz203WebSiteManager --query '[].{appId:appId}' --o tsv
az ad sp credential list --id $spId

$keyId = az ad sp credential list --id $spId --query "[].{keyId:keyId}" --o tsv
$keyId

az ad sp credential delete --id $spId --key-id $keyid
az ad sp credential reset --name LaAz203WebSiteManager --query "password" -o tsv

az group create



