$resourceGroup = "appinsights"
$appInsightsName = "laaz203aisample"
az group create -n $resourceGroup -l eastus

$propsFile = "props.json"
'{"Application_Type":"web"}' | Out-File $propsFile
az resource create `
    -g $resourceGroup -n $appInsightsName `
    --resource-type "Microsoft.Insights/components" `
    --properties "@$propsFile"
Remove-Item $propsFile

az resource show -g $resourceGroup -n $appInsightsName `
    --resource-type "Microsoft.Insights/components" `
    --query "properties.InstrumentationKey" -o tsv

az group delete -n $resourceGroup