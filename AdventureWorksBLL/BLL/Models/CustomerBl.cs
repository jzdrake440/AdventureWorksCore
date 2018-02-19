using AdventureWorks.BLL.Utility;
using AdventureWorks.DAL;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using AdventureWorks.BLL.DataTables;

namespace AdventureWorks.BLL
{
    public class CustomerBl
    {
        public int? CustomerId { get; set; }
        public int? PersonId { get; set; }
        public int? StoreId { get; set; }
        public int? TerritoryId { get; set; }
        public string AccountNumber { get; set; }
        public DateTime ModifiedDate { get; set; }
        public PersonBl Person { get; set; }
        public StoreBl Store { get; set; }
        public SalesTerritoryBl Territory { get; set; }
        public ICollection<SalesOrderHeaderBl> SalesOrderHeader { get; set; }
        
        public string CustomerType { get { return GetCustomerType().GetDisplayValue(); } }
        public string DisplayName
        {
            get
            {
                return Person?.DisplayName ?? Store?.Name ?? BLL.CustomerType.UNKNOWN.GetDisplayValue();
            }
        }

        public CustomerType GetCustomerType()
        {
            if (Store != null)
                return BLL.CustomerType.STORE;

            if (Person != null)
                return BLL.CustomerType.PERSON;

            return BLL.CustomerType.UNKNOWN;
        }
    }
}
