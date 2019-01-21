$rgName = "dml"
$primaryAcctName = "laaz200dmlprimary"
$secondaryAcctName = "laaz200dmlsecondary"

az group create -n $rgName -l westus
az storage account create -g $rgName -l westus -n $primaryAcctName
az storage account create -g $rgName -l eastus -n $secondaryAcctName

$primaryAcctKey = ""
$secondaryAcctKey = ""

az storage container create `
 -n "files" `
 --account-name $primaryAcctName `
--account-key $primaryAcctKey
az storage container create `
 -n "files" `
 --account-name $secondaryAcctName `
 --account-key $secondaryAcctKey

azcopy `
 --source files `
 --destination https://$primaryAcctName.blob.core.windows.net/files `
 --dest-key $primaryAcctKey `
 --recursive 

azcopy `
 --source https://$primaryAcctName.blob.core.windows.net/files `
 --source-key $primaryAcctKey `
 --destination https://$secondaryAcctName.blob.core.windows.net/files `
 --dest-key $secondaryAcctKey `
 --recursive

