using System.Globalization;
using System.Text;

namespace Kuic.Csv
{
    /// <summary>
    /// Provides configuration for <see cref="CsvReader{TElement}"/>.
    /// </summary>
    public interface ICsvReaderConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether the CSV file has headers or not.
        /// Default is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the file has headers; otherwise, <c>false</c>.
        /// </value>
        bool HasHeaders { get; set; }

        /// <summary>
        /// Gets or sets the encoding of the CSV file.
        /// Default is UTF8.
        /// </summary>
        /// <value>
        /// The encoding of the CSV file.
        /// </value>
        Encoding Encoding { get; set; }

        /// <summary>
        /// Gets or sets the culture of the CSV file.
        /// Default is CurrentCulture.
        /// </summary>
        /// <value>
        /// The culture of the CSV file.
        /// </value>
        CultureInfo Culture { get; set; }

        /// <summary>
        /// Sets the culture of the CSV file.
        /// </summary>
        /// <param name="cultureName">The name of the culture in the "en-US" format.</param>
        void SetCulture(string cultureName);

        /// <summary>
        /// Gets or sets the separator of the CSV file.
        /// Default is the culture's separator.
        /// </summary>
        /// <value>
        /// The separator of the CSV file.
        /// </value>
        string Separator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the source stream should be left open after disposing the <see cref="CsvReader{TElement}"/>.
        /// Default is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the stream should be left open; otherwise, <c>false</c>.
        /// </value>
        bool KeepStreamOpen { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the columns should be generated automatically or not.
        /// Default is <c>null</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the columns should be generated automatically; <c>false</c> if not; <c>null</c> if it depends on whether you have manually added columns.
        /// </value>
        bool? AutoGenerate { get; set; }
    }
}