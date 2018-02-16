using AdventureWorksCore.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

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
        [CustomGetterExpression(typeof(Customer), nameof(AccountTypeExpression))]
        public string AccountType
        {
            get
            {
                return (Person != null) ? "Person" :
                    (Store != null) ? "Store" :
                    "Unknown";
            }
        }

        public static Expression AccountTypeExpression(Expression parameter)
        {
            Expression storeProperty = Expression.Property(parameter, nameof(Store));
            Expression personProperty = Expression.Property(parameter, nameof(Person));

            Expression isStoreNotNull = ExpressionUtility.IsNotNull(storeProperty);
            Expression isPersonNotNull = ExpressionUtility.IsNotNull(personProperty);

            return Expression.Condition(isStoreNotNull,
                Expression.Constant("Store"),
                Expression.Condition(isPersonNotNull,
                    Expression.Constant("Person"),
                    Expression.Constant("Unknown")));
        }

        [NotMapped]
        [CustomGetterExpression(typeof(Customer), nameof(DisplayNameExpression))]
        public string DisplayName
        {
            get
            {
                return Person?.DisplayName ?? Store?.Name ?? "Unknown";
            }
        }

        public static Expression DisplayNameExpression(Expression parameter)
        {
            Expression storeProperty = Expression.Property(parameter, nameof(Store));
            Expression personProperty = Expression.Property(parameter, nameof(Person));

            Expression storeName = Expression.Property(storeProperty, nameof(Entity.Store.Name));
            Expression personName = Person.DisplayNameExpression(personProperty);

            Expression isStoreNotNull = ExpressionUtility.IsNotNull(storeProperty);
            Expression isPersonNotNull = ExpressionUtility.IsNotNull(personProperty);

            return Expression.Condition(isStoreNotNull,
                storeName,
                Expression.Condition(isPersonNotNull,
                    personName,
                    Expression.Constant("Unknown")));
        }
    }
}
