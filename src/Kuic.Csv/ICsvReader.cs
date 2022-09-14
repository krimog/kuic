using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace Kuic.Csv
{
    /// <summary>
    /// An interface describing an object that can generate a collection of objects from a comma separated value (CSV) file.
    /// </summary>
    /// <typeparam name="TElement">The type of an element of the collection.</typeparam>
    public interface ICsvReader<TElement> : IDisposable
    {
        /// <summary>
        /// Gets or sets the CSV source stream.
        /// </summary>
        /// <value>
        /// A stream containing the CSV data.
        /// </value>
        Stream? SourceStream { get; set; }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        ICsvReaderConfiguration Configuration { get; set; }
        /// <summary>
        /// Gets the CSV columns.
        /// </summary>
        /// <value>
        /// The CSV columns.
        /// </value>
        CsvColumnCollection<TElement> Columns { get; }

        /// <summary>
        /// Asynchronously reads the CSV stream and return the deserialized elements.
        /// </summary>
        /// <returns>The deserialized elements.</returns>
        /// <exception cref="InvalidOperationException">The SourceStream property is not initialized.</exception>
        /// <exception cref="CsvFormatException"></exception>
        IAsyncEnumerable<TElement> ReadAsAsyncEnumerable();

        /// <summary>
        /// Adds a column with a header.
        /// </summary>
        /// <param name="objectPopulator">A delegate to set the value on the object with the specified format provider.</param>
        /// <param name="header">The column header.</param>
        /// <returns>The current reader. Calls can be chained.</returns>
        ICsvReader<TElement> AddColumn(Action<TElement, IFormatProvider, string> objectPopulator, string header);
        /// <summary>
        /// Adds a column with a header.
        /// </summary>
        /// <typeparam name="TColumn">The type of the property to populate.</typeparam>
        /// <param name="objectPopulator">A delegate to set the value on the object.</param>
        /// <param name="header">The column header.</param>
        /// <returns>The current reader. Calls can be chained.</returns>
        ICsvReader<TElement> AddColumn<TColumn>(Action<TElement, TColumn?> objectPopulator, string header);
        /// <summary>
        /// Adds a column with a header.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="headerIfDifferent">If set, will be the column header. If not set, the property name will be used.</param>
        /// <returns>The current reader. Calls can be chained.</returns>
        ICsvReader<TElement> AddColumn(string propertyName, string? headerIfDifferent = null);
        /// <summary>
        /// Adds a column with a header.
        /// </summary>
        /// <param name="propertyAccessor">The accessor to the property.</param>
        /// <param name="headerIfDifferent">If set, will be the column header. If not set, the property name will be used.</param>
        /// <returns>The current reader. Calls can be chained.</returns>
        ICsvReader<TElement> AddColumn(Expression<Func<TElement, object?>> propertyAccessor, string? headerIfDifferent = null);
        /// <summary>
        /// Adds a column with its index.
        /// </summary>
        /// <param name="objectPopulator">A delegate to set the value on the object with the specified format provider.</param>
        /// <param name="index">The zero-based index of the column.</param>
        /// <returns>The current reader. Calls can be chained.</returns>
        ICsvReader<TElement> AddColumn(Action<TElement, IFormatProvider, string> objectPopulator, int index);
        /// <summary>
        /// Adds a column with its index.
        /// </summary>
        /// <typeparam name="TColumn">The type of the property to populate.</typeparam>
        /// <param name="objectPopulator">A delegate to set the value on the object.</param>
        /// <param name="index">The zero-based index of the column.</param>
        /// <returns>The current reader. Calls can be chained.</returns>
        ICsvReader<TElement> AddColumn<TColumn>(Action<TElement, TColumn?> objectPopulator, int index);
        /// <summary>
        /// Adds a column with its index.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="index">The zero-based index of the column.</param>
        /// <returns>The current reader. Calls can be chained.</returns>
        ICsvReader<TElement> AddColumn(string propertyName, int index);
        /// <summary>
        /// Adds a column with its index.
        /// </summary>
        /// <param name="propertyAccessor">The accessor to the property.</param>
        /// <param name="index">The zero-based index of the column.</param>
        /// <returns>The current reader. Calls can be chained.</returns>
        ICsvReader<TElement> AddColumn(Expression<Func<TElement, object?>> propertyAccessor, int index);
    }
}
