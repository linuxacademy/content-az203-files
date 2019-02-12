$rg = "search"
$location = "westus"
$serviceName = "laaz203search"

az group create -n $rg -l $location

az search service create `
 --name $serviceName `
 -g $rg `
 --sku free

az search admin-key show `
 --service-name $serviceName `
 -g $rg `
 --query "primaryKey"
 
az search query-key list `
 --service-name $serviceName `
 -g $rg `
 --query "[0].key"

az group delete -n $rg --yes
