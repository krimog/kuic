using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Kuic.Core.Collections
{
    public static class DictionaryExtensions
    {
        [return: MaybeNull]
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));

            return source.TryGetValue(key, out var val) ? val : default;
        }
    }
}
