using System;

namespace AdventureWorks.BLL
{
    public class StoreBl
    {
        public static readonly CustomerType CustomerType = CustomerType.STORE;

        public int BusinessEntityId { get; set; }
        public string Name { get; set; }
        public int? SalesPersonId { get; set; }
        public string Demographics { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}