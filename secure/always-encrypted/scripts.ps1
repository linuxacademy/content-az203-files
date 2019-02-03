$subscriptionName = '<your Azure subscription name>'
$userPrincipalName = '<username@domain.com>'
$applicationId = '<application ID from your AAD application>'
$resourceGroupName = '<resource group name>'
$location = '<datacenter location>'
$vaultName = 'AeKeyVault'


Connect-AzureRmAccount
$subscriptionId = (Get-AzureRmSubscription -SubscriptionName $subscriptionName).Id
Set-AzureRmContext -SubscriptionId $subscriptionId

New-AzureRmResourceGroup -Name $resourceGroupName -Location $location
New-AzureRmKeyVault -VaultName $vaultName -ResourceGroupName $resourceGroupName -Location $location

Set-AzureRmKeyVaultAccessPolicy -VaultName $vaultName -ResourceGroupName $resourceGroupName -PermissionsToKeys create,get,wrapKey,unwrapKey,sign,verify,list -UserPrincipalName $userPrincipalName
Set-AzureRmKeyVaultAccessPolicy  -VaultName $vaultName  -ResourceGroupName $resourceGroupName -ServicePrincipalName $applicationId -PermissionsToKeys get,wrapKey,unwrapKey,sign,verify,list
