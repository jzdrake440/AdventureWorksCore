using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdventureWorks.BLL.Services;
using AdventureWorks.DAL;
using Microsoft.AspNetCore.Mvc;

namespace AdventureWorks.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customers;

        public CustomersController(ICustomerService customers)
        {
            _customers = customers;
        }

        [Route("")]//TODO remove for home page
        [Route("Customers")]
        [Route("Customers/Search")]
        public IActionResult Search()
        {
            return View();
        }
    }
}