using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdventureWorks.Models.Dto
{
    public class StoreDto
    {
        public int? BusinessEntityID { get; set; }
        public string Name { get; set; }
    }

    public class StoreDetailDto
    {
        public int? BusinessEntityID { get; set; }
        public string Name { get; set; }
        public int? SalesPersonID { get; set; }
        public string Demographics { get; set; }
        public DateTime ModifiedDate { get; set; }

        //public virtual BusinessEntity BusinessEntity { get; set; }
        public virtual ICollection<CustomerDto> Customers { get; set; }
        //public virtual SalesPerson SalesPerson { get; set; }
    }
}