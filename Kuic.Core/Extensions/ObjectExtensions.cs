using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Kuic.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsOneOf<T>(this T value, params T[] values)
        {
            _ = values ?? throw new ArgumentNullException(nameof(values));
            return values.Contains(value);
        }

        public static IEnumerable<T> AsEnumerable<T>(this T value)
        {
            yield return value;
        }

        public static bool IsOneOf<T>(this T value, IEqualityComparer<T> comparer, params T[] values)
        {
            _ = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _ = values ?? throw new ArgumentNullException(nameof(values));

            return values.Contains(value, comparer);
        }

        public static bool IsNotOneOf<T>(this T value, params T[] values)
        {
            _ = values ?? throw new ArgumentNullException(nameof(values));
            return !values.Contains(value);
        }

        public static bool IsNotOneOf<T>(this T value, IEqualityComparer<T> comparer, params T[] values)
        {
            _ = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _ = values ?? throw new ArgumentNullException(nameof(values));

            return !values.Contains(value, comparer);
        }

        private static Dictionary<Type, Func<object, IFormatProvider, string?>> _culturedStringAccessors = new();
        public static string? ToCulturedString<T>(this T value, IFormatProvider formatProvider)
        {
            _ = formatProvider ?? throw new ArgumentNullException(nameof(formatProvider));
            if (value is null) return null;

            return value switch
            {
                string s => s,
                bool b => b.ToString(formatProvider),
                byte b => b.ToString(formatProvider),
                sbyte b => b.ToString(formatProvider),
                short s => s.ToString(formatProvider),
                ushort s => s.ToString(formatProvider),
                int i => i.ToString(formatProvider),
                uint i => i.ToString(formatProvider),
                long l => l.ToString(formatProvider),
                ulong l => l.ToString(formatProvider),
                float f => f.ToString(formatProvider),
                double d => d.ToString(formatProvider),
                decimal d => d.ToString(formatProvider),
                DateTime dt => dt.ToString(formatProvider),
                DateTimeOffset dt => dt.ToString(formatProvider),
                _ => GetOrCreateValueThroughAccessor(value, formatProvider),
            };

            static string? GetOrCreateValueThroughAccessor(object value, IFormatProvider formatProvider)
            {
                var objectType = value.GetType();
                if (!_culturedStringAccessors.TryGetValue(objectType, out var accessor))
                {
                    var toStringMethodInfo = objectType.GetMethod(nameof(object.ToString), new[] { typeof(IFormatProvider) });
                    if (toStringMethodInfo is null)
                    {
                        accessor = (val, _) => val.ToString();
                    }
                    else
                    {
                        var valParam = Expression.Parameter(typeof(object));
                        var formatParam = Expression.Parameter(typeof(IFormatProvider));
                        var typedParam = Expression.Convert(valParam, objectType);
                        var call = Expression.Call(typedParam, toStringMethodInfo, formatParam);
                        accessor = Expression.Lambda<Func<object, IFormatProvider, string?>>(call, valParam, formatParam).Compile();
                    }

                    _culturedStringAccessors[objectType] = accessor;
                }

                return accessor(value!, formatProvider);
            }
        }
    }
}
