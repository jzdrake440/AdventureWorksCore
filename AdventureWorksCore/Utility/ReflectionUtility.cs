using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace AdventureWorksCore.Utility
{

    public static class ReflectionUtility
    {
        public static Expression GetterExpression(PropertyInfo prop, Expression member)
        {
            if (!prop.IsDefined(typeof(CustomGetterExpressionAttribute)))
                return Expression.Property(member, prop);

            return prop.GetCustomAttribute<CustomGetterExpressionAttribute>().GetExpression(member);
        }
    }
}
