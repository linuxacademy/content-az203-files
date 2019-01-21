$rg = "webapps"
$planname = "githubdeployasp"
$appname = "laaz203githubdeploy"
$repourl = "https://github.com/Azure-Samples/php-docs-hello-world"

az group create `
 -n $rg -l westus

az appservice plan create `
 -n $planname `
 -g $rg `
 --sku FREE 

az webapp create `
 -n $appname `
 -g $rg `
 --plan $planname 

az webapp deployment source config `
 -n $appname `
 -g $rg `
 --repo-url $repourl `
 --branch master `
 --manual-integration

az webapp deployment source show `
 -n $appname `
 -g $rg

az webapp show `
 -n $appname `
 -g $rg
 
az webapp show `
 -n $appname `
 -g $rg `
 --query "defaultHostName" -o tsv

az webapp deployment source sync -n $appname -g $rg
az group delete -n $rg --yes
