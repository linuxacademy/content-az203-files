$rgname = "eventgrid"
$location = "westus"
$stgacctname = "laaz203eventgridsa"
$sitename = "laaz203eventviewer"
az group create -n $rgName -l $location

az storage account create `
  -n $stgacctname `
  -l $location `
  -g $rgname `
  --sku Standard_LRS `
  --kind StorageV2 `
  --access-tier Hot 

$stgacctkey = $(az storage account keys list `
 --account-name $stgacctname `
 -g $egrgname  `
 --query "[0].value" `
 --output tsv)

az storage container create `
 --account-name $stgacctname `
 --account-key $stgacctkey `
 --name testcontainer

$sitename = "laaz203eventviewer"
$hostingplanname = "eventviewerplan"
az group deployment create `   
  -g $rgname `
  --template-uri "https://raw.githubusercontent.com/Azure-Samples/azure-event-grid-viewer/master/azuredeploy.json" `
  --parameters siteName=$sitename hostingPlanName=$hostingplanname

az provider register --namespace Microsoft.EventGrid
az provider show --namespace Microsoft.EventGrid --query "registrationState"


$eventViewerEndpoint = "https://laaz203eventviewer.azurewebsites.net/api/updates"

$storageid = $(az storage account show `
 -n $stgacctname `
 -g $rgName `
 --query id `
 --output tsv)

$endpoint = "https://$sitename.azurewebsites.net/api/updates"
  
az eventgrid event-subscription create `
  -g $egrgname `
  --resource-id $storageid `
  --name storagesubscription `
  --endpoint $endpoint

az eventgrid event-subscription create `
  --resource-id $storageid `
  --name storagesubscription `
  --endpoint-type WebHook `
  --endpoint $endpoint `
  --included-event-types "Microsoft.Storage.BlobCreated" 

az eventgrid event-subscription create `
  --resource-id $storageid `
  --name storagesubscription `
  --endpoint-type WebHook `
  --endpoint $endpoint `
  --included-event-types "Microsoft.Storage.BlobCreated" `
  --subject-begins-with "/blobServices/default/containers/testcontainer/"

az eventgrid event-subscription list -g $egrgname --location $location
  
touch testfile.txt
az storage blob upload `
 --account-name $stgacctname `
 --account-key $stgacctkey `
 --file testfile.txt `
 --container-name testcontainer  `
 --name testfile.txt

az storage container create `
 --account-name $stgacctname `
 --account-key $stgacctkey `
 --name testcontainer2

az storage blob upload `
 --account-name $stgacctname `
 --account-key $stgacctkey `
 --file testfile.txt `
 --container-name testcontainer2  `
 --name testfile.txt


az storage blob delete `
 --account-name $stgacctname `
 --account-key $stgacctkey `
 --container-name testcontainer  `
 --name testfile.txt

az eventgrid event-subscription delete `
  --resource-id $storageid `
  --name storagesubscription 