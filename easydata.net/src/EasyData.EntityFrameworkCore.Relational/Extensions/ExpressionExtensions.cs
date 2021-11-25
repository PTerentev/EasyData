using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EasyData.EntityFrameworkCore
{
    public static class ExpressionExtensions
    {
        public static Expression ContainsString(this Expression sourceStringExpression, Expression value)
        {
            return Expression.Call(sourceStringExpression, typeof(string).GetMethod("Contains", new[] { typeof(string) }), value);
        }
    }
}
