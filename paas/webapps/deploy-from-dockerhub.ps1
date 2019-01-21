$rg = "webapps"
$planname = "dockerhubdeployasp"
$appname = "laaz203dockerhubdeploy"
$container = "microsoft/dotnet-samples:aspnetapp"

az group create -n $rg -l westus
az appservice plan create `
 -n $planname `
 -g $rg `
 --sku B1 `
 --is-linux
 
az webapp create `
 -n $appname `
 -g $rg `
 --plan $planname `
 --deployment-container-image-name $container 
 
az webapp config appsettings set `
 -g $rg `
 -n $appname `
 --settings WEBSITES_PORT=80

az webapp show -n $appname -g $rg
az webapp show -n $appname -g $rg --query "defaultHostName" -o tsv
az group delete -n $rg --yes
