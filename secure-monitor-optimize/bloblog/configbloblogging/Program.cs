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
            var resourceGroupName = "monitorwebapps";
            var webSiteName = "laaz203bloblog";

            var clientId = "e6f36702-fecf-4182-a0f0-974b93d0485f";
            var clientSecret = "8a13f374-d0e7-4099-9d2b-9bb56ae271ad";
            var subscriptionId = "298c6f7d-4dac-465f-b7e4-65e216d1dbe9";
            var tenantId = "8c7d3a28-a492-4475-866a-f54c96d0f7d7";
            var sasUrl = "https://laaz20bloglogstg.blob.core.windows.net/logs?sv=2018-03-28&si=logpolicy&sr=c&sig=xy4YdzNIFlkCv7QZue5WdjlfUSLdTX0p15ym/V2Rn04%3D";
 
            var serviceCredentials = 
                await ApplicationTokenProvider.LoginSilentAsync(
                    tenantId, clientId, clientSecret);
            var client = new WebSiteManagementClient(
                serviceCredentials);
            client.SubscriptionId = subscriptionId;

            var appSettings = new StringDictionary(
                name: "properties",
                properties: new Dictionary<string, string> {
                    { "DIAGNOSTICS_AZUREBLOBCONTAINERSASURL", sasUrl },
                    { "DIAGNOSTICS_AZUREBLOBRETENTIONINDAYS", "30" },
                }
            );
            client.WebApps.UpdateApplicationSettings(
                resourceGroupName: resourceGroupName, 
                name: webSiteName,
                appSettings: appSettings
            );
       
            Console.WriteLine("done");
        }
    }
}
