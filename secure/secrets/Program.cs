using System;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;

namespace secrets
{
    class Program
    {
        static void Main(string[] args)
        {
            // Instantiate a new KeyVaultClient object, with an access token to Key Vault
            var azureServiceTokenProvider1 = new AzureServiceTokenProvider();
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider1.KeyVaultTokenCallback));

            var kvBaseUrl = "https://webappsecrets.vault.azure.net/
            kv.GetSecretAsync()
        }
    }
}
