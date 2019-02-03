$location="westus"
$resourceGroup = "msistg"
$stgacct = "laaz203msistg"

az group create --n $resourceGroup -l $location

az storage account create `
 -g $resourceGroup `
 -n $stgacct `
 -l $location `
 --sku Standard_LRS

$subscriptionId = az account show --query id -o tsv
$subscriptionId
$tenantId = az account show --query tenantId -o tsv
$tenantId

$stgAcctId = az storage account show -n $stgacct --query id -o tsv
$stgAcctId

$spNone = az ad sp create-for-rbac --name LaAz203StgMsiNoAccess | ConvertFrom-Json
$spAll = az ad sp create-for-rbac --name LaAz203StgMsiFullAccess | ConvertFrom-Json

az role assignment list --assignee $spNone.appId
az role assignment list --assignee $spAll.appId

az role definition list --output json | jq '.[] | {"roleName":.roleName, "description":.description}'
az role definition list --custom-role-only false --output json | jq '.[] | {"roleName":.roleName, "description":.description, "roleType":.roleType}'
az role definition list --name "Contributor"
az role definition list --name "Contributor" --output json | jq '.[] | {"actions":.permissions[0].actions, "notActions":.permissions[0].notActions}'

az ad sp show --id $spAll.appId
$spAllObjId = az ad sp show --id $spAll.appId --query objectId -o tsv
$spAllObjId
$spNoneObjId = az ad sp show --id $spNone.appId --query objectId -o tsv
$spNoneObjId

az account show --query id -o tsv

$spNonePwd = az ad sp credential reset --name $spNone.appId --query password -o tsv
$spNonePwd
$spAllPwd = az ad sp credential reset --name $spAll.appId --query password -o tsv
$spAllPwd

az role assignment delete --assignee $spNone.appId --role Contributor
az role assignment create --assignee $spNone.appId --role Reader

az role assignment list --assignee $spNone.appId
az role assignment list --assignee $spAll.appId

az role assignment create `
 --role "Storage Account Contributor" `
 --assignee-object-id $spAllObjId `
 --scope $stgAcctId

az login --service-principal --username $spNone.appId --password $spNonePwd --tenant $tenantId 
az storage container create --name newcontainer2 --account-name $stgacct

az login --service-principal --username $spAll.appId --password $spAllPwd --tenant $tenantId 
az storage container create --name newcontainer2 --account-name $stgacct

az account show

az ad sp credential reset --name $spNone.appId
az ad sp credential reset --name $spAll.appId

az ad sp delete --id $spNone.appId
az ad sp delete --id $spAll.appId

az group delete -y -n $resourceGroup

