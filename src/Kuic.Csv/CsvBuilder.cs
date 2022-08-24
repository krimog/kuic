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
    /// <typeparam name="TSource">The type of an element of the collection.</typeparam>
    public class CsvBuilder<TSource>
    {
        private CsvConfiguration _configuration;
        private readonly List<CsvColumnBase<TSource>> _columns = new();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvBuilder{TSource}"/> class.
        /// </summary>
        /// <param name="sourceData">The source data.</param>
        /// <exception cref="ArgumentNullException">sourceData</exception>
        public CsvBuilder(IEnumerable<TSource> sourceData)
            : this(sourceData, new CsvConfiguration()) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvBuilder{TSource}"/> class.
        /// </summary>
        /// <param name="sourceData">The source data.</param>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="ArgumentNullException">
        /// sourceData
        /// or
        /// configuration
        /// </exception>
        public CsvBuilder(IEnumerable<TSource> sourceData, CsvConfiguration configuration)
        {
            SourceData = sourceData ?? throw new ArgumentNullException(nameof(sourceData));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        
        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        /// <exception cref="ArgumentNullException">value</exception>
        public CsvConfiguration Configuration
        {
            get => _configuration;
            set => _configuration = value ?? throw new ArgumentNullException(nameof(value));
        }
        
        /// <summary>
        /// Gets the source data.
        /// </summary>
        /// <value>
        /// The source data.
        /// </value>
        public IEnumerable<TSource> SourceData { get; }
        
        /// <summary>
        /// Adds a column.
        /// Adding a column also prevents the automatic generation of columns.
        /// </summary>
        /// <typeparam name="TColumn">The type of the column data.</typeparam>
        /// <param name="header">The column header.</param>
        /// <param name="accessor">The column data accessor.</param>
        /// <returns>The current builder. Calls can be chained.</returns>
        public CsvBuilder<TSource> AddColumn<TColumn>(string header, Func<TSource, TColumn> accessor)
        {
            _columns.Add(new CsvColumn<TSource, TColumn>(header, accessor));
            return this;
        }
        
        /// <summary>
        /// Adds a headerless column.
        /// Sets the configuration's <c>AddHeader</c> property to <c>false</c>.
        /// Adding a column also prevents the automatic generation of columns.
        /// </summary>
        /// <typeparam name="TColumn">The type of the column data.</typeparam>
        /// <param name="accessor">The column data accessor.</param>
        /// <returns>The current builder. Calls can be chained.</returns>
        public CsvBuilder<TSource> AddColumn<TColumn>(Func<TSource, TColumn> accessor)
        {
            _columns.Add(new CsvColumn<TSource, TColumn>(accessor));
            Configuration.AddHeaders = false;
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
            _ = filePath ?? throw new ArgumentNullException(nameof(filePath));

            // To prevent problems with change in configuration during method execution
            var addBom = _configuration.AddBomInFile;
            var separator = _configuration.Separator;
            var addHeaders = _configuration.AddHeaders;
            var encoding = _configuration.Encoding;
            var culture = _configuration.Culture;

            var columns = _columns.ToArray();
            if (columns.Length == 0)
            {
                columns = typeof(TSource)
                    .GetProperties()
                    .Where(p => p.CanRead && !p.IsIndexer())
                    .Select(p => new CsvColumn<TSource, object>(p.Name, p.ToLambda<TSource>()))
                    .ToArray();
            }

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
            stream ??= new MemoryStream();

            // To prevent problems with change in configuration during method execution
            var addBom = _configuration.AddBomInFile;
            var separator = _configuration.Separator;
            var addHeaders = _configuration.AddHeaders;
            var encoding = _configuration.Encoding;
            var culture = _configuration.Culture;

            var columns = _columns.ToArray();
            if (columns.Length == 0)
            {
                columns = typeof(TSource)
                    .GetProperties()
                    .Where(p => p.CanRead && !p.IsIndexer())
                    .Select(p => new CsvColumn<TSource, object>(p.Name, p.ToLambda<TSource>()))
                    .ToArray();
            }

            if (addBom)
            {
                var bom = encoding.GetPreamble();
                await stream.WriteAsync(bom);
            }

            await WriteContentAsync(stream, columns, separator, addHeaders, encoding, culture);
            return stream;
        }

        private async Task WriteContentAsync(Stream stream, CsvColumnBase<TSource>[] columns, string separator, bool addHeaders, Encoding encoding, CultureInfo culture)
        {
            var writer = new StreamWriter(stream, encoding);
            if (addHeaders)
            {
                if (columns.Any(c => c.Header is null)) throw new InvalidOperationException("Unable to write headers if a column has no header.");
                var headerLine = string.Join(separator, columns.Select(c => Escape(c.Header!, separator)));
                await writer.WriteLineAsync(headerLine);
            }

            foreach (var element in SourceData)
            {
                var line = string.Join(separator, columns.Select(c => Escape(c.GetStringValue(element, culture), separator)));
                await writer.WriteLineAsync(line);
            }

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
