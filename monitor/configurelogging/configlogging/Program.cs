using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Management.WebSites;
using Microsoft.Azure.Management.WebSites.Models;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Common;
using Microsoft.Rest;
using Microsoft.Rest.Azure;
using Microsoft.Rest.Azure.Authentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace configlogging
{
    class Program
    {
        static void Main(string[] args)
        {
            runAsync().Wait();
        }

        private static async Task runAsync()
        {
            //var environments = AzureEnvironment.KnownEnvironments.ToArray();
            //var environment = AzureEnvironment.AzureGlobalCloud;
            //var resourceManagerEndpoint = environment.ResourceManagerEndpoint;
            //var client = new WebSiteManagementClient(
              //  _environment.GetEndpointAsUri(AzureEnvironment.Endpoint.ResourceManager), tokenCreds, loggingHandler
            //);

            var tenantId = "8c7d3a28-a492-4475-866a-f54c96d0f7d7";
            var subscriptionId = "298c6f7d-4dac-465f-b7e4-65e216d1dbe9";
            var clientId = "a7520b14-40f8-466f-90a5-372c789781bc";
            var clientSecret = "4e2862ed-48b9-49eb-a021-73093148c2e3";

            //var authority = String.Format("{0}{1}", environment.AuthenticationEndpoint, tenantId);

            //var authContext = new AuthenticationContext(authority);
            //var credential = new ClientCredential(clientId, clientSecret);
            //var authResult = await authContext.AcquireTokenAsync(environment.AuthenticationEndpoint, credential);

            //var tokenCloudCredentials = new TokenCloudCredentials(subscriptionId, authResult.AccessToken);

            //var tokenCredentials = new TokenCredentials(tokenCloudCredentials.Token);

            //var httpClient = new HttpClient();
          
            var resourceGroupName = "monitorwebapps";
            var webSiteName = "laaz203monitorwalogs";

            var sasUrl = "https://laaz203monitorwastg.blob.core.windows.net/logs?sv=2018-03-28&si=logpolicy&sr=c&sig=PhqddKMzYScijvRZlm7brQSm54aG5sKVOrjACQeqhOs%3D";

            
            
            /*
            var rmClient = new Microsoft.Azure.Management.ResourceManager.ResourceManagementClient(serviceCredentials);
            rmClient.SubscriptionId = subscriptionId;
            var rgs = await rmClient.ResourceGroups.ListAsync();
/* 
            var sasConstraints = new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(24),
                SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Write
            };

            var sasContainerToken = container.GetSharedAccessSignature(sasConstraints);
*/
            var appSettings = new StringDictionary(
                name: "properties",
                properties: new Dictionary<string, string> {
                    { "DIAGNOSTICS_AZUREBLOBCONTAINERSASURL", sasUrl },
                    { "DIAGNOSTICS_AZUREBLOBRETENTIONINDAYS", "30" },
                }
            );
 
 
            var serviceCredentials = await ApplicationTokenProvider.LoginSilentAsync(tenantId, clientId, clientSecret);
            var client = new WebSiteManagementClient(serviceCredentials);
            client.SubscriptionId = subscriptionId;
            client.WebApps.UpdateApplicationSettings(
                resourceGroupName: resourceGroupName, 
                name: webSiteName,
                appSettings: appSettings
            );
        }
    }
}
