using System;
using System.Linq.Expressions;

namespace AdventureWorks.BLL.Utility
{
    public static class ExpressionUtility
    {
        public static BinaryExpression IsNull(Expression expression)
        {
            return Expression.Equal(expression, Expression.Constant(null));
        }

        public static BinaryExpression IsNotNull(Expression expression)
        {
            return Expression.NotEqual(expression, Expression.Constant(null));
        }

        public static Expression CallToString(Expression e)
        {
            if (e.Type == typeof(string))
                return e;
            return Expression.Call(e, nameof(object.ToString), Type.EmptyTypes);
        }

        public static Expression CallContains(Expression expression, Expression stringExpression)
        {
            if (expression.Type != typeof(string))
                throw new ArgumentException("Argument \"expression\" must be a string.");

            if (stringExpression.Type != typeof(string))
                throw new ArgumentException("Argument \"stringExpression\" must be a string.");

            return Expression.Call(expression, nameof(string.Contains), Type.EmptyTypes, stringExpression);
        }

        public static Expression OrElseIgnoreNull(Expression left, Expression right)
        {
            if (left == null && right == null)
                return null;

            if (left == null)
                return right;

            if (right == null)
                return left;

            return Expression.OrElse(left, right);
        }

        public static Expression AndAlsoIgnoreNull(Expression left, Expression right)
        {
            if (left == null && right == null)
                return null;

            if (left == null)
                return right;

            if (right == null)
                return left;

            return Expression.AndAlso(left, right);
        }
    }
}
