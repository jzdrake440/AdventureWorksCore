using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AdventureWorksCore.Utility
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

        public static MethodCallExpression CallToString(Expression e)
        {
            return Expression.Call(e, nameof(object.ToString), Type.EmptyTypes);
        }

        public static Expression CallContains(Expression expression, Expression stringExpression)
        {
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
