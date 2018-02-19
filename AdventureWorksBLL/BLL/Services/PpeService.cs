using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AdventureWorks.BLL.Services
{
    public class PpeService : IPpeService
    {
        private Dictionary<Tuple<Type, string>, Delegate> expressions = new Dictionary<Tuple<Type, string>, Delegate>();

        public void AddExpression<T>(string propertyName, Func<Expression, Expression> expressionFunc)
        {
            expressions.Add(new Tuple<Type, string>(typeof(T), propertyName.ToUpper()), expressionFunc);
        }

        public Expression GetExpression<T>(Expression parameter, string propertyName)
        {
            var func = (Func<Expression, Expression>)expressions[new Tuple<Type, string>(typeof(T), propertyName.ToUpper())];

            if (func == null)
                throw new KeyNotFoundException(typeof(T).Name + "." + propertyName);

            return func.Invoke(parameter);
        }
    }
}
