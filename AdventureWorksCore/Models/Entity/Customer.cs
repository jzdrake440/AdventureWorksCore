using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdventureWorksCore.Models.Entity
{
    public partial class Customer
    {
        public Customer()
        {
            SalesOrderHeader = new HashSet<SalesOrderHeader>();
        }

        public int CustomerId { get; set; }
        public int? PersonId { get; set; }
        public int? StoreId { get; set; }
        public int? TerritoryId { get; set; }
        public string AccountNumber { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Person Person { get; set; }
        public Store Store { get; set; }
        public SalesTerritory Territory { get; set; }
        public ICollection<SalesOrderHeader> SalesOrderHeader { get; set; }

        [NotMapped]
        public string AccountType
        {
            get
            {
                if (Person != null)
                    return "Person";

                if (Store != null)
                    return "Store";

                return "Unknown";
            }
        }

        [NotMapped]
        public string DisplayName
        {
            get
            {
                return Person?.DisplayName ?? Store?.Name ?? "Unknown";
            }
        }
    }
}
