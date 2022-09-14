using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Kuic.Csv
{
    /// <summary>
    /// An interface describing an object that can generate a comma separated value (CSV) file from a collection of objects.
    /// </summary>
    /// <typeparam name="TElement">The type of an element of the collection.</typeparam>
    public interface ICsvBuilder<TElement>
    {
        /// <summary>
        /// Gets or sets the source data.
        /// </summary>
        /// <value>
        /// The source data.
        /// </value>
        IEnumerable<TElement>? SourceData { get; set; }
        /// <summary>
        /// Gets or sets the asynchronous source data.
        /// </summary>
        /// <value>
        /// The asynchronous source data.
        /// </value>
        IAsyncEnumerable<TElement>? AsyncSourceData { get; set; }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        /// <exception cref="ArgumentNullException">value</exception>
        ICsvBuilderConfiguration Configuration { get; set; }
        /// <summary>
        /// Gets the CSV columns.
        /// </summary>
        /// <value>
        /// The CSV columns.
        /// </value>
        CsvColumnCollection<TElement> Columns { get; }

        /// <summary>
        /// Asynchronously writes the CSV into a file.
        /// </summary>
        /// <param name="filePath">The path to the file to create or overwrite.</param>
        /// <exception cref="ArgumentNullException">filePath</exception>
        /// <exception cref="InvalidOperationException">Unable to write headers if a column has no header.</exception>
        Task ToFileAsync(string filePath);
        /// <summary>
        /// Asynchronously writes the CSV into either an existing stream or a new <see cref="MemoryStream"/>.
        /// </summary>
        /// <param name="stream">The stream to write into. If <c>null</c>, a new <see cref="MemoryStream"/> is created.</param>
        /// <returns>The stream the CSV has be written into.</returns>
        /// <exception cref="InvalidOperationException">Unable to write headers if a column has no header.</exception>
        Task<Stream> ToStreamAsync(Stream? stream);

        /// <summary>
        /// Adds a headerless column.
        /// Sets the configuration's <c>AddHeader</c> property to <c>false</c>.
        /// </summary>
        /// <typeparam name="TColumn">The type of the column data.</typeparam>
        /// <param name="accessor">The column data accessor.</param>
        /// <returns>The current builder. Calls can be chained.</returns>
        ICsvBuilder<TElement> AddColumn<TColumn>(Func<TElement, TColumn> accessor);
        /// <summary>
        /// Adds a column.
        /// </summary>
        /// <typeparam name="TColumn">The type of the column data.</typeparam>
        /// <param name="header">The column header.</param>
        /// <param name="accessor">The column data accessor.</param>
        /// <returns>The current builder. Calls can be chained.</returns>
        ICsvBuilder<TElement> AddColumn<TColumn>(string header, Func<TElement, TColumn> accessor);
    }
}
