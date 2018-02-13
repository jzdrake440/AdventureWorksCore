using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdventureWorksCore.Models.Entity;
using Microsoft.AspNetCore.Mvc;

namespace AdventureWorksCore.Controllers
{
    public class CustomersController : Controller
    {
        private AdventureWorks2017Context _context;

        public CustomersController(AdventureWorks2017Context context)
        {
            _context = context;
        }

        [Route("")]//TODO remove for home page
        [Route("Customers")]
        [Route("Customers/Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}