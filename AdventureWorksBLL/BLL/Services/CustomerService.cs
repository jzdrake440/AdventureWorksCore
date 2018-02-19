using AdventureWorks.BLL.DataTables;
using AdventureWorks.BLL.Utility;
using AdventureWorks.DAL;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorks.BLL.Services
{

    public class CustomerService : ICustomerService
    {
        private readonly AdventureWorks2017Context _context;
        private readonly IMapper _mapper;
        private readonly IDataTableService _dataTableService;

        public CustomerService(AdventureWorks2017Context context, IMapper mapper, IDataTableService dataTableService)
        {
            _context = context;
            _mapper = mapper;
            _dataTableService = dataTableService;
        }

        public CustomerBl GetCustomer(int id)
        {
            return GetCustomers(c => c.CustomerId == id).SingleOrDefault();
        }

        public List<CustomerBl> GetCustomers(IEnumerable<int> ids)
        {
            return GetCustomers(c => ids.Contains(c.CustomerId));
        }

        public List<CustomerBl> GetCustomers(Func<Customer, bool> criteria)
        {
            return _mapper.Map<List<CustomerBl>>(_context.Customer.Where(criteria).ToList());
        }

        public List<CustomerBl> GetCustomers()
        {
            return _mapper.Map<List<CustomerBl>>(_context.Customer.ToList());
        }

        public DataTableServerSideResponse<CustomerBl> GetCustomersDataTableResponse(DataTableServerSideRequest request)
        {
            return _dataTableService.GetDataTableData<Customer, CustomerBl>(
                request, 
                _context.Customer
                    .Include(nameof(Customer.Store))
                    .Include(nameof(Customer.Person)));
        }

        public CustomerBl AddCustomer(CustomerBl customerBl)
        {
            if (customerBl.CustomerId.GetValueOrDefault() != 0)
                throw new ArgumentException("customerBl cannot define it's own id");

            var customer = _mapper.Map<Customer>(customerBl);
            _context.Customer.Add(customer);
            _context.SaveChanges();

            return _mapper.Map<CustomerBl>(customer);
        }

        public CustomerBl UpdateCustomer(CustomerBl customerBl)
        {
            if (customerBl == null)
                throw new ArgumentException("customerBl cannot be null");

            var customer = _context.Customer.SingleOrDefault(c => c.CustomerId == customerBl.CustomerId);

            if (customer == null)
                throw new ArgumentException(String.Format("customer {0} does not exist", customerBl.CustomerId));

            _mapper.Map(customerBl, customer);
            _context.SaveChanges();

            return _mapper.Map<CustomerBl>(customer);
        }

        public void DeleteCustomer(CustomerBl customerBl)
        {
            if (customerBl == null)
                throw new ArgumentException("customerBl cannot be null");

            var customer = _context.Customer.SingleOrDefault(c => c.CustomerId == customerBl.CustomerId);

            if (customer == null)
                throw new ArgumentException(String.Format("customer {0} does not exist", customerBl.CustomerId));

            _context.Customer.Remove(customer);
            _context.SaveChanges();
        }
    }
}
