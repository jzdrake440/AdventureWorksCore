using AdventureWorks.Models.Dto;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Extensions;
using AdventureWorks.BLL;
using AdventureWorks.BLL.Services;

namespace AdventureWorks.Controllers.Api
{
    [Route("/api/customers")]
    public class CustomersController : Controller
    {
        private IMapper _mapper;
        private ICustomerService _customers;
        
        public CustomersController(IMapper mapper, ICustomerService customers)
        {
            _mapper = mapper;
            _customers = customers;
        }
        
        [HttpGet]
        public IActionResult GetCustomers()
        {
            return Ok(_mapper.Map<List<CustomerDetailDto>>(_customers.GetCustomers()));
        }
        
        [HttpGet("{id}")]
        public IActionResult GetCustomer(int id)
        {
            var customer = _customers.GetCustomer(id);

            if (customer == null)
                return NotFound();

            return Ok(_mapper.Map<CustomerDetailDto>(customer));
        }
        
        [HttpPost]
        public IActionResult AddCustomer(CustomerDetailDto customerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customerBl = _mapper.Map<CustomerBl>(customerDto);
            customerBl = _customers.AddCustomer(customerBl);
            
            return Created(new Uri(Request.GetDisplayUrl() + customerBl.CustomerId),
                _mapper.Map<CustomerDetailDto>(customerBl));
        }
        
        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(int id, CustomerDetailDto customerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customerBl = _customers.GetCustomer(id);

            if (customerBl == null)
                return NotFound();

            _mapper.Map(customerDto, customerBl);

            customerBl = _customers.UpdateCustomer(customerBl);

            return Ok(_mapper.Map<CustomerDetailDto>(customerBl));
        }
        
        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            var customerBl = _customers.GetCustomer(id);

            if (customerBl == null)
                return BadRequest();

            _customers.DeleteCustomer(customerBl);

            return Ok();
        }
    }
}