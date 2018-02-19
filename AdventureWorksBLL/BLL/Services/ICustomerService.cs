using System;
using System.Collections.Generic;
using AdventureWorks.BLL.DataTables;
using AdventureWorks.DAL;

namespace AdventureWorks.BLL.Services
{
    public interface ICustomerService
    {
        CustomerBl AddCustomer(CustomerBl customerBl);
        void DeleteCustomer(CustomerBl customerBl);
        CustomerBl GetCustomer(int id);
        List<CustomerBl> GetCustomers();
        DataTableServerSideResponse<CustomerBl> GetCustomersDataTableResponse(DataTableServerSideRequest request);
        List<CustomerBl> GetCustomers(Func<Customer, bool> criteria);
        List<CustomerBl> GetCustomers(IEnumerable<int> ids);
        CustomerBl UpdateCustomer(CustomerBl customerBl);
    }
}