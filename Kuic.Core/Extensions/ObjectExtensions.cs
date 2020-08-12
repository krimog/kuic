using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
