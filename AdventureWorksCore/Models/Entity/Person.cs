using AdventureWorksCore.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace AdventureWorksCore.Models.Entity
{
    public partial class Person
    {
        public Person()
        {
            BusinessEntityContact = new HashSet<BusinessEntityContact>();
            Customer = new HashSet<Customer>();
            EmailAddress = new HashSet<EmailAddress>();
            PersonCreditCard = new HashSet<PersonCreditCard>();
            PersonPhone = new HashSet<PersonPhone>();
        }

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
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }

        public BusinessEntity BusinessEntity { get; set; }
        public Employee Employee { get; set; }
        public Password Password { get; set; }
        public ICollection<BusinessEntityContact> BusinessEntityContact { get; set; }
        public ICollection<Customer> Customer { get; set; }
        public ICollection<EmailAddress> EmailAddress { get; set; }
        public ICollection<PersonCreditCard> PersonCreditCard { get; set; }
        public ICollection<PersonPhone> PersonPhone { get; set; }

        [NotMapped]
        [CustomGetterExpression(typeof(Person), nameof(DisplayNameExpression))]
        public string DisplayName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }

        public static Expression DisplayNameExpression(Expression expression)
        {
            Expression lastNameProperty = Expression.Property(expression, nameof(LastName));
            Expression firstNameProperty = Expression.Property(expression, nameof(FirstName));
            Expression separatorExpression = Expression.Constant(", ");

            MethodInfo concatMethod = typeof(string).GetMethod("Concat", new []{ typeof(string[]) });

            Expression arrayExpression = Expression.NewArrayInit(typeof(string), new[] { lastNameProperty, separatorExpression, firstNameProperty });

            return Expression.Call(null, concatMethod, new[] { arrayExpression });
        }
    }
}
