using AdventureWorksCore.Models.DataTables;
using AdventureWorksCore.Models.Dto;
using AdventureWorksCore.Models.Entity;
using AdventureWorksCore.Utility;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;

namespace AdventureWorks.Controllers.Api
{
    [Route("/api/customers")]
    public class CustomersController : Controller
    {
        private IMapper _mapper;
        private AdventureWorks2017Context _context;
        
        public CustomersController(IMapper mapper, AdventureWorks2017Context context)
        {
            _mapper = mapper;
            _context = context;
        }
        
        [HttpGet]
        public IActionResult GetCustomers()
        {
            return Ok(_mapper.Map<List<CustomerDetailDto>>(_context.Customer.ToList()));
        }
        
        [HttpGet("{id}")]
        public IActionResult GetCustomer(int id)
        {
            var customer = _context.Customer.SingleOrDefault(c => c.CustomerId == id);

            if (customer == null)
                return NotFound();

            return Ok(_mapper.Map<CustomerDetailDto>(customer));
        }
        
        [HttpPost]
        public IActionResult AddCustomer(CustomerDetailDto customerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customer = _mapper.Map<Customer>(customerDto);

            _context.Customer.Add(customer);
            _context.SaveChanges();
            
            return Created(new Uri(Request.GetDisplayUrl() + customer.CustomerId),
                _mapper.Map<CustomerDetailDto>(customer));
        }
        
        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(int id, CustomerDetailDto customerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customer = _context.Customer.SingleOrDefault(c => c.CustomerId == customerDto.CustomerID);

            if (customer == null)
                return NotFound();

            _mapper.Map<CustomerDetailDto, Customer>(customerDto, customer);

            _context.SaveChanges();

            return Ok(_mapper.Map<CustomerDetailDto>(customer)); //remap customer to dto in case of backend updates
        }
        
        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            var customer = _context.Customer.SingleOrDefault(c => c.CustomerId == id);

            if (customer == null)
                return BadRequest();

            _context.Customer.Remove(customer);
            _context.SaveChanges();

            return Ok();
        }
    }
}