$rgName = "logicapps"
$stgAcctName = "laaz203lasa"
$location = "westus"
$queueName = "toarchive"
$containerName = "images"

az group create -n $rgName -l $location

az storage account create `
 -g $rgName `
 -n $stgAcctName `
 -l $location `
 --sku Standard_LRS

$key = $(az storage account keys list `
 --account-name $stgAcctName `
 -g $rgName `
 --query "[0].value" `
 --output tsv)
$key

az storage queue create `
 --name $queueName `
 --account-name $stgAcctName `
 --account-key $key

az storage container create `
 --name $containerName `
 --account-name $stgAcctName `
 --account-key $key

az storage blob upload `
 --container-name $containerName `
 --name bleu.jpg `
 --file bleu.jpg `
 --account-name $stgAcctName `
 --account-key $key

az storage message peek `
 --queue-name $queueName `
 --account-name $stgAcctName `
 --account-key $key `
 --num-messages 10

az group delete --name $rgName --yes

