using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Kuic.Core.Extensions
{
    public static class ReflectionExtensions
    {
        public static Func<object, object> ToGetterLambda(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            if (!propertyInfo.CanRead) throw new ArgumentException("The property is not readable.", nameof(propertyInfo));
            if (propertyInfo.IsIndexer()) throw new InvalidOperationException("This method doesn't support indexed properties.");

            var parameter = Expression.Parameter(typeof(object));
            var typedParameter = Expression.Convert(parameter, propertyInfo.DeclaringType);
            var access = Expression.Property(typedParameter, propertyInfo);
            var finalCast = Expression.Convert(access, typeof(object));
            return Expression.Lambda<Func<object, object>>(finalCast, parameter).Compile();
        }

        public static Func<TSource, object> ToGetterLambda<TSource>(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            if(!propertyInfo.CanRead) throw new ArgumentException("The property is not readable.", nameof(propertyInfo));
            if (!propertyInfo.DeclaringType.IsAssignableFrom(typeof(TSource))) throw new InvalidOperationException($"The type {typeof(TSource).FullName} doesn't inherit from / implement the type {propertyInfo.DeclaringType.FullName}.");
            if (propertyInfo.IsIndexer()) throw new InvalidOperationException("This method doesn't support indexed properties.");

            var parameter = Expression.Parameter(typeof(TSource));
            var access = Expression.Property(parameter, propertyInfo);
            var finalCast = Expression.Convert(access, typeof(object));
            return Expression.Lambda<Func<TSource, object>>(finalCast, parameter).Compile();
        }

        public static Func<TSource, TDest> ToGetterLambda<TSource, TDest>(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            if (!propertyInfo.CanRead) throw new ArgumentException("The property is not readable.", nameof(propertyInfo));
            if (!propertyInfo.DeclaringType.IsAssignableFrom(typeof(TSource))) throw new InvalidOperationException($"The type {typeof(TSource).FullName} doesn't inherit from / implement the type {propertyInfo.DeclaringType.FullName}.");
            if (!typeof(TDest).IsAssignableFrom(propertyInfo.PropertyType)) throw new InvalidOperationException($"The type {propertyInfo.DeclaringType.FullName} doesn't inherit from / implement the type {typeof(TDest).FullName}.");
            if (propertyInfo.IsIndexer()) throw new InvalidOperationException("This method doesn't support indexed properties.");

            var parameter = Expression.Parameter(typeof(TSource));
            var access = Expression.Property(parameter, propertyInfo);
            var finalCast = Expression.Convert(access, typeof(TDest));
            return Expression.Lambda<Func<TSource, TDest>>(finalCast, parameter).Compile();
        }

        public static Action<object, object?> ToSetterLambda(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            if (!propertyInfo.CanRead) throw new ArgumentException("The property is not readable.", nameof(propertyInfo));
            if (propertyInfo.IsIndexer()) throw new InvalidOperationException("This method doesn't support indexed properties.");

            var objParam = Expression.Parameter(typeof(object));
            var valParam = Expression.Parameter(typeof(object));
            var objCast = Expression.Convert(objParam, propertyInfo.DeclaringType);
            var access = Expression.Property(objCast, propertyInfo);
            var valCast = Expression.Convert(valParam, propertyInfo.PropertyType);
            var body = Expression.Assign(access, valCast);
            return Expression.Lambda<Action<object, object?>>(body, objParam, valParam).Compile();
        }

        public static Action<TSource, object?> ToSetterLambda<TSource>(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            if (!propertyInfo.CanWrite) throw new ArgumentException("The property is not writable.", nameof(propertyInfo));
            if (!propertyInfo.DeclaringType.IsAssignableFrom(typeof(TSource))) throw new InvalidOperationException($"The type {typeof(TSource).FullName} doesn't inherit from / implement the type {propertyInfo.DeclaringType.FullName}.");
            if (propertyInfo.IsIndexer()) throw new InvalidOperationException("This method doesn't support indexed properties.");

            var objParam = Expression.Parameter(typeof(TSource));
            var valParam = Expression.Parameter(typeof(object));
            var access = Expression.Property(objParam, propertyInfo);
            var valCast = Expression.Convert(valParam, propertyInfo.PropertyType);
            var body = Expression.Assign(access, valCast);
            return Expression.Lambda<Action<TSource, object?>>(body, objParam, valParam).Compile();
        }

        public static Action<TSource, TDest> ToSetterLambda<TSource, TDest>(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            if (!propertyInfo.CanWrite) throw new ArgumentException("The property is not writable.", nameof(propertyInfo));
            if (!propertyInfo.DeclaringType.IsAssignableFrom(typeof(TSource))) throw new InvalidOperationException($"The type {typeof(TSource).FullName} doesn't inherit from / implement the type {propertyInfo.DeclaringType.FullName}.");
            if (!typeof(TDest).IsAssignableFrom(propertyInfo.PropertyType)) throw new InvalidOperationException($"The type {propertyInfo.DeclaringType.FullName} doesn't inherit from / implement the type {typeof(TDest).FullName}.");
            if (propertyInfo.IsIndexer()) throw new InvalidOperationException("This method doesn't support indexed properties.");

            var objParam = Expression.Parameter(typeof(TSource));
            var valParam = Expression.Parameter(typeof(TDest));
            var access = Expression.Property(objParam, propertyInfo);
            var body = Expression.Assign(access, valParam);
            return Expression.Lambda<Action<TSource, TDest>>(body, objParam, valParam).Compile();
        }

        public static bool IsIndexer(this PropertyInfo propertyInfo) => propertyInfo.GetIndexParameters().Length > 0;
    }
}
