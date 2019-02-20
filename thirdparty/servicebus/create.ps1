az group create -n servicebus -l westus

az servicebus namespace create --n laaz203sb -g servicebus

az servicebus namespace authorization-rule keys list `
 -g servicebus `
 --namespace-name laaz203sb `
 --name RootManageSharedAccessKey `
 --query primaryConnectionString

az servicebus queue create `
 --namespace-name laaz203sb `
 -g servicebus `
 -n testqueue 

New-AzureRmServiceBusQueue `
 -ResourceGroupName servicebus `
 -NamespaceName laaz203sb `
 -name testqueue `
 -EnablePartitioning $false