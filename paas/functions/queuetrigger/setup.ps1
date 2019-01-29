$acct = "laaz203functionssa"
$rg = "functions"
$queue = "incoming-orders"

az group create -n $rg -l westus

az storage account create `
 -n $acct `
 -g $rg `
 -l westus `
 --sku Standard_LRS `
 --kind StorageV2 `
 --access-tier Hot

$key = $(az storage account keys list `
 --account-name $acct `
 -g $rg `
 --query "[0].value" `
 --output tsv)

az storage queue create `
 -n $queue `
 --account-name $acct `
 --account-key $key

az storage account show-connection-string `
 -n $acct `
 --query "connectionString"


$order1json = Get-Content -Path order1.json

az storage message put `
 --account-name $acct `
 --account-key $key `
 --queue-name $queue `
 --content $order1json

az group delete -n $rg -y


