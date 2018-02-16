using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace AdventureWorksCore.Utility
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomGetterExpressionAttribute : Attribute
    {
        public MethodInfo Method { get; }

        public CustomGetterExpressionAttribute(Type invokedType, string invokedMethodName)
        {
            Method = invokedType.GetMethod(invokedMethodName); //TODO validation
        }

        public Expression GetExpression(Expression member)
        {
            return (Expression)Method.Invoke(null, new[] { member });
        }
    }
}
