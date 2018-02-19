using AdventureWorks.BLL.Utility;
using AdventureWorks.DAL;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AdventureWorks.BLL
{
    public class PersonBl
    {
        public static readonly CustomerType CustomerType = CustomerType.PERSON;

        public int BusinessEntityId { get; set; }
        public string PersonType { get; set; }
        public bool NameStyle { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public int EmailPromotion { get; set; }
        public string AdditionalContactInfo { get; set; }
        public string Demographics { get; set; }
        public DateTime ModifiedDate { get; set; }
        public BusinessEntityBl BusinessEntity { get; set; }
        public EmployeeBl Employee { get; set; }
        public PasswordBl Password { get; set; }

        public string DisplayName { get { return LastName + ", " + FirstName; } }
    }
}
