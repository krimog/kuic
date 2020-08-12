using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Kuic.Core.Collections
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TElement> DistinctAs<TElement, TProperty>(this IEnumerable<TElement> source, Func<TElement, TProperty> selector)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            return Implementation(source, selector);

            static IEnumerable<TElement> Implementation(IEnumerable<TElement> source, Func<TElement, TProperty> selector)
            {
                var hashSet = new HashSet<TProperty>();

                foreach (var element in source)
                {
                    if (hashSet.Add(selector(element)))
                        yield return element;
                }
            }
        }

        public static IEnumerable<TElement> Union<TElement>(this IEnumerable<TElement> source, params TElement[] otherElements)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = otherElements ?? throw new ArgumentNullException(nameof(otherElements));

            return Enumerable.Union(source, otherElements);
        }

        public static bool IsNullOrEmpty<TElement>(this IEnumerable<TElement>? source)
        {
            return source is null || !source.Any();
        }

        public static bool IsNotNullNorEmpty<TElement>(this IEnumerable<TElement>? source) => !source.IsNullOrEmpty();

        public static IEnumerable<TElement> EmptyIfNull<TElement>(this IEnumerable<TElement>? source)
        {
            return source ?? Enumerable.Empty<TElement>();
        }

        [return: MaybeNull]
        public static TElement MaxOrDefault<TElement>(this IEnumerable<TElement> source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return source.Any() ? source.Max() : default;
        }

        [return: MaybeNull]
        public static TResult MaxOrDefault<TElement, TResult>(this IEnumerable<TElement> source, Func<TElement, TResult> selector)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            return source.Any() ? source.Max(selector) : default;
        }

        [return: MaybeNull]
        public static TElement MinOrDefault<TElement>(this IEnumerable<TElement> source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return source.Any() ? source.Min() : default;
        }

        [return: MaybeNull]
        public static TResult MinOrDefault<TElement, TResult>(this IEnumerable<TElement> source, Func<TElement, TResult> selector)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            return source.Any() ? source.Min(selector) : default;
        }

        public static IEnumerable<TSource> WhereIsMax<TSource, TCompare>(this IEnumerable<TSource> source, Func<TSource, TCompare> selector) where TCompare : IComparable
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            if (!source.Any()) return source;

            var maxValue = default(TCompare);
            var maxElements = new List<TSource>();
            source.ForFirstAndForRest(
            elt =>
            {
                maxValue = selector(elt);
                maxElements.Add(elt);
            },
            elt =>
            {
                TCompare currentValue = selector(elt);
                if (currentValue == null) return;
                int comparison = currentValue.CompareTo(maxValue);
                if (comparison == 0) maxElements.Add(elt);
                else if (comparison > 0)
                {
                    maxValue = currentValue;
                    maxElements.Clear();
                    maxElements.Add(elt);
                }
            });

            return maxElements;
        }

        public static IEnumerable<TSource> WhereIsMin<TSource, TCompare>(this IEnumerable<TSource> source, Func<TSource, TCompare> selector) where TCompare : IComparable
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            if (!source.Any()) return source;

            var minValue = default(TCompare);
            var minElements = new List<TSource>();
            source.ForFirstAndForRest(
            elt =>
            {
                minValue = selector(elt);
                minElements.Add(elt);
            },
            elt =>
            {
                TCompare currentValue = selector(elt);
                int comparison = currentValue == null ? -1 : currentValue.CompareTo(minValue);
                if (comparison == 0) minElements.Add(elt);
                else if (comparison < 0)
                {
                    minValue = currentValue;
                    minElements.Clear();
                    minElements.Add(elt);
                }
            });

            return minElements;
        }

        public static decimal? SumOrDefault(this IEnumerable<decimal?> source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            if (!source.Any()) return null;
            decimal total = 0M;
            foreach (var element in source)
            {
                if (element == null) return null;
                total += element.Value;
            }

            return total;
        }

        public static decimal? SumOrDefault<T>(this IEnumerable<T> source, Func<T, decimal?> accessor)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = accessor ?? throw new ArgumentNullException(nameof(accessor));

            if (!source.Any()) return null;
            decimal total = 0M;
            foreach (var element in source)
            {
                var value = accessor(element);
                if (value == null) return null;
                total += value.Value;
            }

            return total;
        }

        public static decimal? SumOrDefault(this IEnumerable<int?> source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            if (!source.Any()) return null;
            int total = 0;
            foreach (var element in source)
            {
                if (element == null) return null;
                total += element.Value;
            }

            return total;
        }

        public static decimal? SumOrDefault<T>(this IEnumerable<T> source, Func<T, int?> accessor)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = accessor ?? throw new ArgumentNullException(nameof(accessor));

            if (!source.Any()) return null;
            int total = 0;
            foreach (var element in source)
            {
                var value = accessor(element);
                if (value == null) return null;
                total += value.Value;
            }

            return total;
        }

        public static void ForFirstAndForRest<TElement>(this IEnumerable<TElement> source, Action<TElement> actionOnFirst, Action<TElement> actionOnRest)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = actionOnFirst ?? throw new ArgumentNullException(nameof(actionOnFirst));
            _ = actionOnRest ?? throw new ArgumentNullException(nameof(actionOnRest));

            IEnumerator<TElement> enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext()) return;
            actionOnFirst(enumerator.Current);
            while (enumerator.MoveNext()) actionOnRest(enumerator.Current);
        }
    }
}
