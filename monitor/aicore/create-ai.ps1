$resourceGroup = "appinsights"
$propsFile = "props.json"
'{"Application_Type":"web"}' | Out-File $propsFile
$appInsightsName = "laaz203aisample"
az group create -n $resourceGroup -l eastus

az resource create `
    -g $resourceGroup -n $appInsightsName `
    --resource-type "Microsoft.Insights/components" `
    --properties "@$propsFile"
Remove-Item $propsFile

az resource show -g $resourceGroup -n $appInsightsName `
    --resource-type "Microsoft.Insights/components" `
    --query "properties.InstrumentationKey" -o tsv
az group delete -n $resourceGroup

az resource show -g $RESOURCEGROUPNAME -n $APPLICATION_NAME --resource-type "Microsoft.Insights/components" --query properties.InstrumentationKey

az provider register --namespace Microsoft.insights/components
