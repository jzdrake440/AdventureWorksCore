using System;
using System.Linq.Expressions;

namespace AdventureWorks.BLL.Services
{
    public interface IPpeService
    {
        void AddExpression<T>(string propertyName, Func<Expression, Expression> expressionFunc);
        Expression GetExpression<T>(string propertyName, Expression parameter);
    }
}