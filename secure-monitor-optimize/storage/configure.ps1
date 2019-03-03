$location = "westus"
$rgname = "rbacmsistg"
$stgacct = "laaz203rbacmsistg"

az group create --n $rgname -l $location

az storage account create `
 -g $rgname `
 -n $stgacct `
 -l $location `
 --sku Standard_LRS

$sp = az ad sp create-for-rbac `
 -n LaAz203StgSp | ConvertFrom-Json

az role assignment list --assignee $sp.appId

az role assignment delete `
 --assignee $sp.appId --role Contributor

az role assignment list --assignee $sp.appId

$tenantid = az account show `
 --query tenantId -o tsv

az login --service-principal `
  --username $sp.appId `
  --password $sp.password `
  --tenant $tenantid 

az role assignment create `
 --assignee $sp.appId --role Reader

az login --service-principal `
 --username $sp.appId `
 --password $sp.password `
 --tenant $tenantid

az storage container list `
 --account-name $stgacct

# run code

$stgacctid = az storage account show `
 -n $stgacct --query id -o tsv
$spobjid = az ad sp show `
 --id $sp.appId --query objectId -o tsv
$stgacctid
$spobjid

az login

az role assignment create `
 --role "Storage Account Contributor" `
 --assignee-object-id $spobjid `
 --scope $stgacctid

az login --service-principal `
 --username $sp.appId `
 --password $sp.password `
 --tenant $tenantid

az storage container create `
 --name contribcli `
 --account-name $stgacct

az storage container list `
 --account-name $stgacct

# run program

az login

az ad sp delete --id $sp.appId
az group delete -y -n $rgname

