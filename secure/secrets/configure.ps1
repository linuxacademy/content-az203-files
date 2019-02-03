$location="westus"
$resourceGroup = "msistg"
$kvname = "laaz203wakv"

az group create --n $resourceGroup -l $location

az keyvault create `
 -vault-name $kvname `

az keyvault secret set `


$subscriptionId = az account show --query id -o tsv
$subscriptionId

$sp = az ad sp create-for-rbac --name LaAz203WebSecrets | ConvertFrom-Json

#az role assignment delete --assignee $sp.appId --role Contributor
#az role assignment create --assignee $sp.appId --role Reader

$tenantId = az account show --query tenantId -o tsv
$tenantId
$spAllObjId = az ad sp show --id $spAll.appId --query objectId -o tsv
$spAllObjId



az ad sp delete --id $spNone.appId
az ad sp delete --id $spAll.appId
