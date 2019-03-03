using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using webbloblogging.Models;
using Microsoft.Extensions.Logging;

namespace webbloblogging.Controllers
{
    public class HomeController : Controller
    {
        private ILogger _logger = null;

public HomeController(ILogger<HomeController> logger) 
{ 
    _logger = logger;
    _logger.LogInformation("In HomeController.HomeController");
}

public IActionResult Index()
{
    _logger.LogInformation("In HomeController.Index");
    return View();
}

        public IActionResult Privacy()
        {
            _logger.LogInformation("In HomeController.Privacy");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
