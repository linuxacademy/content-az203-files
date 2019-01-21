$rgName = "laaz203logicapp"
$stgAcctName = "laaz203logicappstorage"
$locaiton = "westus"

az group create -n $rgName

az storage account create `
 -g $rgName `
 -n $stgAcctName `
 -l $location `
 --sku Standard_LRS

