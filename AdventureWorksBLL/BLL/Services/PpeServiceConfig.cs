using AdventureWorks.BLL;
using AdventureWorks.BLL.Utility;
using AdventureWorks.DAL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AdventureWorks.BLL.Services
{
    public static class PpeServiceConfig
    {
        public static void AddPpeService(this IServiceCollection services)
        {

            services.AddTransient<IPpeService, PpeService>(s => BuildService());
        }

        public static PpeService BuildService()
        {
            PpeService ppeService = new PpeService();

            //Customer.CustomerType
            ppeService.AddExpression<Customer>(nameof(CustomerBl.CustomerType), pe =>
            {
                Expression storeProperty = Expression.Property(pe, nameof(Customer.Store));
                Expression personProperty = Expression.Property(pe, nameof(Customer.Person));

                Expression isStoreNotNull = ExpressionUtility.IsNotNull(storeProperty);
                Expression isPersonNotNull = ExpressionUtility.IsNotNull(personProperty);

                return Expression.Condition(isPersonNotNull,
                    Expression.Constant(CustomerType.PERSON.GetDisplayValue()),
                    Expression.Condition(isStoreNotNull,
                        Expression.Constant(CustomerType.STORE.GetDisplayValue()),
                        Expression.Constant(CustomerType.UNKNOWN.GetDisplayValue())));
            });

            //Customer.DisplayName
            ppeService.AddExpression<Customer>(nameof(CustomerBl.DisplayName), pe =>
            {
                Expression storeProperty = Expression.Property(pe, nameof(Customer.Store));
                Expression personProperty = Expression.Property(pe, nameof(Customer.Person));

                Expression storeName = Expression.Property(storeProperty, nameof(Store.Name));
                Expression personName = ppeService.GetExpression<Person>(nameof(PersonBl.DisplayName), personProperty);

                Expression isStoreNotNull = ExpressionUtility.IsNotNull(storeProperty);
                Expression isPersonNotNull = ExpressionUtility.IsNotNull(personProperty);

                return Expression.Condition(isPersonNotNull,
                    personName,
                    Expression.Condition(isStoreNotNull,
                        storeName,
                        Expression.Constant(CustomerType.UNKNOWN.GetDisplayValue())));
            });

            //Person.DisplayName
            ppeService.AddExpression<Person>(nameof(PersonBl.DisplayName), pe =>
            {
                Expression lastNameProperty = Expression.Property(pe, nameof(Person.LastName));
                Expression firstNameProperty = Expression.Property(pe, nameof(Person.FirstName));
                Expression separatorExpression = Expression.Constant(", ");

                MethodInfo concatMethod = typeof(string).GetMethod(nameof(String.Concat), new[] { typeof(string[]) });

                Expression arrayExpression = Expression.NewArrayInit(typeof(string), new[] { lastNameProperty, separatorExpression, firstNameProperty });

                return Expression.Call(null, concatMethod, new[] { arrayExpression });
            });

            return ppeService;
        }
    }
}
