using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Kuic.Core.Extensions
{
    public static class ReflectionExtensions
    {
        public static Func<object, object> ToLambda(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            if (propertyInfo.IsIndexer()) throw new InvalidOperationException("This method doesn't support indexed properties.");

            var parameter = Expression.Parameter(typeof(object));
            var typedParameter = Expression.Convert(parameter, propertyInfo.DeclaringType);
            var access = Expression.Property(typedParameter, propertyInfo);
            var finalCast = Expression.Convert(access, typeof(object));
            return Expression.Lambda<Func<object, object>>(finalCast, parameter).Compile();
        }

        public static Func<TSource, object> ToLambda<TSource>(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            if (!propertyInfo.DeclaringType.IsAssignableFrom(typeof(TSource))) throw new InvalidOperationException($"The type {typeof(TSource).FullName} doesn't inherit from / implement the type {propertyInfo.DeclaringType.FullName}.");
            if (propertyInfo.IsIndexer()) throw new InvalidOperationException("This method doesn't support indexed properties.");

            var parameter = Expression.Parameter(typeof(TSource));
            var access = Expression.Property(parameter, propertyInfo);
            var finalCast = Expression.Convert(access, typeof(object));
            return Expression.Lambda<Func<TSource, object>>(finalCast, parameter).Compile();
        }

        public static Func<TSource, TDest> ToLambda<TSource, TDest>(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            if (!propertyInfo.DeclaringType.IsAssignableFrom(typeof(TSource))) throw new InvalidOperationException($"The type {typeof(TSource).FullName} doesn't inherit from / implement the type {propertyInfo.DeclaringType.FullName}.");
            if (!typeof(TDest).IsAssignableFrom(propertyInfo.PropertyType)) throw new InvalidOperationException($"The type {propertyInfo.DeclaringType.FullName} doesn't inherit from / implement the type {typeof(TDest).FullName}.");
            if (propertyInfo.IsIndexer()) throw new InvalidOperationException("This method doesn't support indexed properties.");

            var parameter = Expression.Parameter(typeof(TSource));
            var access = Expression.Property(parameter, propertyInfo);
            var finalCast = Expression.Convert(access, typeof(TDest));
            return Expression.Lambda<Func<TSource, TDest>>(finalCast, parameter).Compile();
        }

        public static bool IsIndexer(this PropertyInfo propertyInfo) => propertyInfo.GetIndexParameters().Length > 0;
    }
}
