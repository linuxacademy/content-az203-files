$location = "westus"
$rgname = "kvsecrets"
$kvname = "laaz203kvsecrets"
$spname = "LaAz203WebAppSecrets"

az group create --n $rgname -l $location

az keyvault create `
 -n $kvname `
 -g $rgname `
 --sku standard 

az keyvault secret set `
 --vault-name $kvname `
 --name "connectionString" `
 --value "this is the connection string"

az keyvault secret show `
 --vault-name $kvname `
 --name connectionString

# run the app

$sp = az ad sp create-for-rbac --name $spname | ConvertFrom-Json
$sp

$tenantid = az account show --query tenantId -o tsv
az login --service-principal `
  --username $sp.appId `
  --password $sp.password `
  --tenant $tenantid

az keyvault secret show `
 --vault-name $kvname `
 --name connectionString

# run the app - no access to secret

az login #back to main account

az keyvault set-policy `
 --name $kvname `
 --spn $sp.Name `
 --secret-permissions get

az login --service-principal --username $sp.appId --password $sp.password --tenant $tenantid

az keyvault secret show `
 --vault-name $kvname `
 --name connectionString

# run the app

az ad sp delete --id $sp.appId
az keyvault delete --name $kvname

az group delete -n $rgname --yes
