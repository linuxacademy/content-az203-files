using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;

namespace app.Controllers
{
    public class KeyVaultSecretController : Controller
    {
        public KeyVaultSecretController(){

        }

        public async Task<IActionResult> Index()
        {
            var azureServiceTokenProvider1 = new AzureServiceTokenProvider();
            var kvc = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(
                    azureServiceTokenProvider1.KeyVaultTokenCallback));

            var kvBaseUrl = "https://laaz203kvsecrets.vault.azure.net/";
            var secret = await kvc.GetSecretAsync(kvBaseUrl, "connectionString");

            ViewData["Message"] = secret.Value; //secret.value;
            return View();
        }
    }
}