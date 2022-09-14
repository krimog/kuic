using System;
using System.Collections.Generic;

namespace Kuic.Csv
{
    /// <summary>
    /// Provides extension methods for CSV
    /// </summary>
    public static class CsvExtensions
    {
        /// <summary>
        /// Gets an object to build a comma separated value (CSV) file or stream from this collection.
        /// </summary>
        /// <typeparam name="T">The type of the collection's elements.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>The <see cref="CsvBuilder{TSource}"/>.</returns>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static ICsvBuilder<T> ToCsv<T>(this IEnumerable<T> source) => ToCsv(source, new CsvConfiguration());

        /// <summary>
        /// Gets an object to build a comma separated value (CSV) file or stream from this collection.
        /// </summary>
        /// <typeparam name="T">The type of the collection's elements.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="configuration">The CSV configuration.</param>
        /// <returns>The <see cref="CsvBuilder{TSource}"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// source
        /// or
        /// configuration
        /// </exception>
        public static ICsvBuilder<T> ToCsv<T>(this IEnumerable<T> source, ICsvBuilderConfiguration configuration)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var builder = new CsvBuilder<T>(source, configuration);
            return builder;
        }

        /// <summary>
        /// Gets an object to build a comma separated value (CSV) file or stream from this collection.
        /// </summary>
        /// <typeparam name="T">The type of the collection's elements.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="configure">An action to configure the builder.</param>
        /// <returns>The <see cref="CsvBuilder{TSource}"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// source
        /// or
        /// configure
        /// </exception>
        public static ICsvBuilder<T> ToCsv<T>(this IEnumerable<T> source, Action<ICsvBuilderConfiguration> configure)
        {
            _ = configure ?? throw new ArgumentNullException(nameof(configure));

            var configuration = new CsvConfiguration();
            configure.Invoke(configuration);
            return ToCsv(source, configuration);
        }

        /// <summary>
        /// Gets an object to build a comma separated value (CSV) file or stream from this collection.
        /// </summary>
        /// <typeparam name="T">The type of the collection's elements.</typeparam>
        /// <param name="source">The asynchronous source collection.</param>
        /// <returns>The <see cref="CsvBuilder{TSource}"/>.</returns>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static ICsvBuilder<T> ToCsv<T>(this IAsyncEnumerable<T> source) => ToCsv(source, new CsvConfiguration());

        /// <summary>
        /// Gets an object to build a comma separated value (CSV) file or stream from this collection.
        /// </summary>
        /// <typeparam name="T">The type of the collection's elements.</typeparam>
        /// <param name="source">The asynchronous source collection.</param>
        /// <param name="configuration">The CSV configuration.</param>
        /// <returns>The <see cref="CsvBuilder{TSource}"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// source
        /// or
        /// configuration
        /// </exception>
        public static ICsvBuilder<T> ToCsv<T>(this IAsyncEnumerable<T> source, ICsvBuilderConfiguration configuration)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var builder = new CsvBuilder<T>(source, configuration);
            return builder;
        }

        /// <summary>
        /// Gets an object to build a comma separated value (CSV) file or stream from this collection.
        /// </summary>
        /// <typeparam name="T">The type of the collection's elements.</typeparam>
        /// <param name="source">The asynchronous source collection.</param>
        /// <param name="configure">An action to configure the builder.</param>
        /// <returns>The <see cref="CsvBuilder{TSource}"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// source
        /// or
        /// configure
        /// </exception>
        public static ICsvBuilder<T> ToCsv<T>(this IAsyncEnumerable<T> source, Action<ICsvBuilderConfiguration> configure)
        {
            _ = configure ?? throw new ArgumentNullException(nameof(configure));

            var configuration = new CsvConfiguration();
            configure.Invoke(configuration);
            return ToCsv(source, configuration);
        }
    }
}
