using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdventureWorks.Models.Dto
{
    public class CustomerDto
    {
        public int? CustomerID { get; set; }
        public int? PersonID { get; set; }
        public int? StoreID { get; set; }
        public int? TerritoryID { get; set; }
        public string AccountNumber { get; set; }
        public string DisplayName { get; set; }
        public string AccountType { get; set; }
    }

    public class CustomerDetailDto : CustomerDto
    {
        public DateTime ModifiedDate { get; set; }

        public virtual PersonDto Person { get; set; }
        public SalesTerritoryDto SalesTerritory { get; set; }
        public StoreDto Store { get; set; }

        //public virtual ICollection<SalesOrderHeader> SalesOrderHeaders { get; set; }
    }
}