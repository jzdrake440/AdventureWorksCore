using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdventureWorksCore.Models.DataTables;
using AdventureWorksCore.Models.Dto;
using AdventureWorksCore.Models.Entity;
using AdventureWorksCore.Utility;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorksCore.Controllers.DataTables
{
    [Route("/dt/customers")]
    public class CustomersController : Controller
    {
        private IMapper _mapper;
        private AdventureWorks2017Context _context;

        public CustomersController(IMapper mapper, AdventureWorks2017Context context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpPost]
        public IActionResult GetCustomerDataTable(DataTableServerSideRequest request)
        {
            return Ok(DataTableUtility.GetDataTableData(request, _mapper, _context.Customer.Include(c => c.Person)));
        }
    }
}