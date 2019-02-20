using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Models;

namespace DotNetCoreSqlDb.Controllers
{
    public class CustomersController : Controller
    {
        private readonly MyDatabaseContext _context;

        public CustomersController(MyDatabaseContext context)
        {
            _context = context;    
        }

        // GET: Todos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customers.Take(5).ToListAsync());
        }
    }
}