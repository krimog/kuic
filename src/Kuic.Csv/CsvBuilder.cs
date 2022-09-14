using Kuic.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuic.Csv
{
    /// <summary>
    /// An object that can generate a comma separated value (CSV) file from a collection of objects.
    /// </summary>
    /// <typeparam name="TElement">The type of an element of the collection.</typeparam>
    public sealed class CsvBuilder<TElement> : ICsvBuilder<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvBuilder{TElement}"/> class.
        /// </summary>
        /// <param name="sourceData">The source data.</param>
        /// <exception cref="ArgumentNullException">sourceData</exception>
        public CsvBuilder(IEnumerable<TElement> sourceData)
            : this(sourceData, new CsvConfiguration()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvBuilder{TElement}"/> class.
        /// </summary>
        /// <param name="sourceData">The source data.</param>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="ArgumentNullException">
        /// sourceData
        /// or
        /// configuration
        /// </exception>
        public CsvBuilder(IEnumerable<TElement> sourceData, ICsvBuilderConfiguration configuration)
        {
            SourceData = sourceData ?? throw new ArgumentNullException(nameof(sourceData));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvBuilder{TElement}"/> class.
        /// </summary>
        /// <param name="sourceData">The asynchronous source data.</param>
        /// <exception cref="ArgumentNullException">asyncSourceData</exception>
        public CsvBuilder(IAsyncEnumerable<TElement> asyncSourceData)
            : this(asyncSourceData, new CsvConfiguration()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvBuilder{TElement}"/> class.
        /// </summary>
        /// <param name="sourceData">The asynchronous source data.</param>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="ArgumentNullException">
        /// asyncSourceData
        /// or
        /// configuration
        /// </exception>
        public CsvBuilder(IAsyncEnumerable<TElement> asyncSourceData, ICsvBuilderConfiguration configuration)
        {
            AsyncSourceData = asyncSourceData ?? throw new ArgumentNullException(nameof(asyncSourceData));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvBuilder{TElement}"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="ArgumentNullException">configuration</exception>
        public CsvBuilder(ICsvBuilderConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvBuilder{TElement}"/> class.
        /// </summary>
        public CsvBuilder() : this(new CsvConfiguration()) { }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        /// <exception cref="ArgumentNullException">value</exception>
        public ICsvBuilderConfiguration Configuration
        {
            get => _configuration;
            set => _configuration = value ?? throw new ArgumentNullException(nameof(value));
        }
        private ICsvBuilderConfiguration _configuration;

        /// <summary>
        /// Gets the CSV columns.
        /// </summary>
        /// <value>
        /// The CSV columns.
        /// </value>
        public CsvColumnCollection<TElement> Columns { get; } = new();

        /// <summary>
        /// Gets or sets the source data.
        /// </summary>
        /// <value>
        /// The source data.
        /// </value>
        public IEnumerable<TElement>? SourceData { get => _sourceData; set { if ((_sourceData = value) is not null) AsyncSourceData = null; } }
        private IEnumerable<TElement>? _sourceData;

        /// <summary>
        /// Gets or sets the asynchronous source data.
        /// </summary>
        /// <value>
        /// The asynchronous source data.
        /// </value>
        public IAsyncEnumerable<TElement>? AsyncSourceData { get => _asyncSourceData; set { if ((_asyncSourceData = value) is not null) SourceData = null; } }
        private IAsyncEnumerable<TElement>? _asyncSourceData;

        /// <summary>
        /// Adds a column.
        /// </summary>
        /// <typeparam name="TColumn">The type of the column data.</typeparam>
        /// <param name="header">The column header.</param>
        /// <param name="accessor">The column data accessor.</param>
        /// <returns>The current builder. Calls can be chained.</returns>
        public ICsvBuilder<TElement> AddColumn<TColumn>(string header, Func<TElement, TColumn> accessor)
        {
            Columns.Add(CsvColumn.WritableWithHeader(accessor, header));
            return this;
        }

        /// <summary>
        /// Adds a headerless column.
        /// Sets the configuration's <c>AddHeader</c> property to <c>false</c>.
        /// </summary>
        /// <typeparam name="TColumn">The type of the column data.</typeparam>
        /// <param name="accessor">The column data accessor.</param>
        /// <returns>The current builder. Calls can be chained.</returns>
        public ICsvBuilder<TElement> AddColumn<TColumn>(Func<TElement, TColumn> accessor)
        {
            Columns.Add(CsvColumn.WritableWithIndex(accessor, Columns.Count));
            Configuration.HasHeaders = false;
            return this;
        }

        /// <summary>
        /// Asynchronously writes the CSV into a file.
        /// </summary>
        /// <param name="filePath">The path to the file to create or overwrite.</param>
        /// <exception cref="ArgumentNullException">filePath</exception>
        /// <exception cref="InvalidOperationException">Unable to write headers if a column has no header.</exception>
        public async Task ToFileAsync(string filePath)
        {
            if (SourceData is null && AsyncSourceData is null) throw new InvalidOperationException($"The {nameof(SourceData)} property is not initialized.");
            _ = filePath ?? throw new ArgumentNullException(nameof(filePath));

            // To prevent problems with change in configuration during method execution
            var addBom = _configuration.AddBomInFile;
            var separator = _configuration.Separator;
            var addHeaders = _configuration.HasHeaders;
            var encoding = _configuration.Encoding;
            var culture = _configuration.Culture;
            var autoGenerate = _configuration.AutoGenerate;

            CsvColumnBase<TElement>[] columns;
            if (autoGenerate == true || (!autoGenerate.HasValue && !Columns.Any()))
            {
                columns = typeof(TElement)
                    .GetProperties()
                    .Where(p => p.CanRead && !p.IsIndexer())
                    .Select(p => CsvColumn.ForPropertyWithHeader<TElement>(p))
                    .Concat(Columns)
                    .Where(c => c.CanWrite)
                    .ToArray();
            }
            else columns = Columns.Where(c => c.CanWrite).ToArray();
            if (!columns.Any()) throw new InvalidOperationException("No column to generate.");

            using var fileStream = File.Create(filePath);

            if (addBom)
            {
                var bom = encoding.GetPreamble();
                await fileStream.WriteAsync(bom);
            }

            await WriteContentAsync(fileStream, columns, separator, addHeaders, encoding, culture);
        }

        /// <summary>
        /// Asynchronously writes the CSV into either an existing stream or a new <see cref="MemoryStream"/>.
        /// </summary>
        /// <param name="stream">The stream to write into. If <c>null</c>, a new <see cref="MemoryStream"/> is created.</param>
        /// <returns>The stream the CSV has be written into.</returns>
        /// <exception cref="InvalidOperationException">Unable to write headers if a column has no header.</exception>
        public async Task<Stream> ToStreamAsync(Stream? stream)
        {
            if (SourceData is null && AsyncSourceData is null) throw new InvalidOperationException($"The {nameof(SourceData)} property is not initialized.");
            stream ??= new MemoryStream();

            // To prevent problems with change in configuration during method execution
            var addBom = _configuration.AddBomInFile;
            var separator = _configuration.Separator;
            var addHeaders = _configuration.HasHeaders;
            var encoding = _configuration.Encoding;
            var culture = _configuration.Culture;
            var autoGenerate = _configuration.AutoGenerate;

            CsvColumnBase<TElement>[] columns;
            if (autoGenerate == true || (!autoGenerate.HasValue && !Columns.Any()))
            {
                columns = typeof(TElement)
                    .GetProperties()
                    .Where(p => p.CanRead && !p.IsIndexer())
                    .Select(p => CsvColumn.ForPropertyWithHeader<TElement>(p))
                    .Concat(Columns)
                    .Where(c => c.CanWrite)
                    .ToArray();
            }
            else columns = Columns.Where(c => c.CanWrite).ToArray();
            if (!columns.Any()) throw new InvalidOperationException("No column to generate.");

            if (addBom)
            {
                var bom = encoding.GetPreamble();
                await stream.WriteAsync(bom);
            }

            await WriteContentAsync(stream, columns, separator, addHeaders, encoding, culture);
            return stream;
        }

        private async Task WriteContentAsync(Stream stream, CsvColumnBase<TElement>[] columns, string separator, bool addHeaders, Encoding encoding, CultureInfo culture)
        {
            var writer = new StreamWriter(stream, encoding);
            if (addHeaders)
            {
                if (columns.Any(c => c.Header is null)) throw new InvalidOperationException("Unable to write headers if a column has no header.");
                var headerLine = string.Join(separator, columns.Select(c => Escape(c.Header!, separator)));
                await writer.WriteLineAsync(headerLine);
            }

            if (SourceData is not null)
            {
                foreach (var element in SourceData)
                {
                    var line = string.Join(separator, columns.Select(c => Escape(c.GetStringValue(element, culture), separator)));
                    await writer.WriteLineAsync(line);
                }
            }
            else if (AsyncSourceData is not null)
            {
                await foreach (var element in AsyncSourceData)
                {
                    var line = string.Join(separator, columns.Select(c => Escape(c.GetStringValue(element, culture), separator)));
                    await writer.WriteLineAsync(line);
                }
            }
            else throw new InvalidOperationException($"The {nameof(SourceData)} property is not initialized.");

            await writer.FlushAsync();
        }

        private static string Escape(string value, string separator)
        {
            bool shouldEscape = value.Contains(separator) || value.Contains('"') || value.Contains('\n') || value.Contains('\r');
            if (!shouldEscape) return value;
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
    }
}
