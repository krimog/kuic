using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Kuic.Core.Extensions
{
    public static class ExpressionExtensions
    {
        public static PropertyInfo? GetPropertyInfo<TObject, TProperty>(this Expression<Func<TObject, TProperty>> expression)
        {
            _ = expression ?? throw new ArgumentNullException(nameof(expression));
            var memberExpression = GetPropertyExpression(expression.Body);
            if (memberExpression is null) return null;
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            if (propertyInfo.DeclaringType != typeof(TObject)) return null;
            return propertyInfo;
        }

        private static MemberExpression? GetPropertyExpression(Expression? source, bool canReturnSelf = true)
        {
            if (source is MemberExpression me)
            {
                if (me.Member is PropertyInfo && canReturnSelf) return me;
                return GetPropertyExpression(me.Expression);
            }

            if (source is UnaryExpression ue) return GetPropertyExpression(ue.Operand);

            return null;
        }
    }
}
