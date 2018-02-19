using AdventureWorks.BLL.DataTables;
using AdventureWorks.Models.Dto;
using AdventureWorks.DAL;
using AdventureWorks.BLL.Utility;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdventureWorks.BLL.Services;

namespace AdventureWorks.Controllers.DataTables
{
    [Route("/dt/customers")]
    public class CustomersController : Controller
    {
        private IMapper _mapper;
        private ICustomerService _customers;

        public CustomersController(IMapper mapper, ICustomerService customers)
        {
            _mapper = mapper;
            _customers = customers;
        }

        [HttpPost]
        public IActionResult GetCustomerDataTable(DataTableServerSideRequest request)
        {
            return Ok(_customers.GetCustomersDataTableResponse(request));
        }
    }
}