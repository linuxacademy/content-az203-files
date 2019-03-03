$stgacctname = "laaz203blobsas"
$container = "images"
$rgname = "blobsas"
$location = "westus"

az group create -n $rgname -l $location

az storage account create `
 -g $rgName `
 -n $stgacctname `
 -l $location `
 --sku Standard_LRS

$stgacctkey = $(az storage account keys list `
 -g $rgname `
 --account-name $stgacctname `
  --query "[0].value" `
  --output tsv)
 $stgacctkey

az storage container create `
 --account-name $stgacctname `
 --account-key $stgacctkey `
 --name $container

az storage blob upload `
 --account-name $stgacctname `
 --account-key $stgacctkey `
 --file bleu.jpg `
 --container-name $container  `
 --name bleu.jpg

az storage blob url `
 --account-name $stgacctname `
 --account-key $stgacctkey `
 --container-name $container `
 --name bleu.jpg

$now = [DateTime]::UtcNow
$now

$start = $now.ToString('yyyy-MM-ddTHH:mmZ')
$end = $now.AddMinutes(5).ToString('yyyy-MM-ddTHH:mmZ')
$start
$end

$sas = az storage blob generate-sas `
 --account-name $stgacctname `
 --account-key $stgacctkey `
 --container-name $container `
 --name bleu.jpg `
 --permissions r `
 --start $start `
 --expiry $end
$sas

az storage blob url `
 --account-name $stgacctname `
 --account-key $stgacctkey `
 --container-name $container `
 --name bleu.jpg `
 --sas $sas `
 -o tsv



