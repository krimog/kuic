using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Kuic.Core.Extensions;
using Kuic.Core.Helpers;

namespace Kuic.Csv
{
    /// <summary>
    /// An object that can generate a collection of objects from a comma separated value (CSV) file.
    /// </summary>
    /// <typeparam name="TElement">The type of an element of the collection.</typeparam>
    public sealed class CsvReader<TElement> : ICsvReader<TElement>, IDisposable
    {
        private ICsvReaderConfiguration _configuration;
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader{TElement}"/> class.
        /// </summary>
        /// <param name="sourceStream">The CSV source stream.</param>
        public CsvReader(Stream sourceStream) : this(sourceStream, new CsvConfiguration()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader{TElement}"/> class.
        /// </summary>
        /// <param name="sourceStream">The CSV source stream.</param>
        /// <param name="configuration">The configuration.</param>
        public CsvReader(Stream sourceStream, ICsvReaderConfiguration configuration)
        {
            SourceStream = sourceStream ?? throw new ArgumentNullException(nameof(sourceStream));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader{TElement}"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public CsvReader(ICsvReaderConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader{TElement}"/> class.
        /// </summary>
        public CsvReader() : this(new CsvConfiguration()) { }

        /// <summary>
        /// Gets or sets the CSV source stream.
        /// </summary>
        /// <value>
        /// A stream containing the CSV data.
        /// </value>
        public Stream? SourceStream { get; set; }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public ICsvReaderConfiguration Configuration
        {
            get => _configuration;
            set => _configuration = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets the CSV columns.
        /// </summary>
        /// <value>
        /// The CSV columns.
        /// </value>
        public CsvColumnCollection<TElement> Columns { get; } = new();

        /// <summary>
        /// Adds a column with a header.
        /// </summary>
        /// <param name="objectPopulator">A delegate to set the value on the object with the specified format provider.</param>
        /// <param name="header">The column header.</param>
        /// <returns>The current reader. Calls can be chained.</returns>
        public ICsvReader<TElement> AddColumn(Action<TElement, IFormatProvider, string> objectPopulator, string header)
        {
            Columns.Add(CsvColumn.ReadableWithHeader(objectPopulator, header));
            _configuration.HasHeaders = true;
            return this;
        }

        /// <summary>
        /// Adds a column with a header.
        /// </summary>
        /// <typeparam name="TColumn">The type of the property to populate.</typeparam>
        /// <param name="objectPopulator">A delegate to set the value on the object.</param>
        /// <param name="header">The column header.</param>
        /// <returns>The current reader. Calls can be chained.</returns>
        public ICsvReader<TElement> AddColumn<TColumn>(Action<TElement, TColumn?> objectPopulator, string header)
            => AddColumn((o, f, v) => objectPopulator(o, Conversion.TryParse(v, f, out TColumn? val) ? val : default), header);

        /// <summary>
        /// Adds a column with a header.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="headerIfDifferent">If set, will be the column header. If not set, the property name will be used.</param>
        /// <returns>The current reader. Calls can be chained.</returns>
        public ICsvReader<TElement> AddColumn(string propertyName, string? headerIfDifferent = null)
            => AddColumn(CsvColumn.ForPropertyWithHeader<TElement>(propertyName, headerIfDifferent));

        /// <summary>
        /// Adds a column with a header.
        /// </summary>
        /// <param name="propertyAccessor">The accessor to the property.</param>
        /// <param name="headerIfDifferent">If set, will be the column header. If not set, the property name will be used.</param>
        /// <returns>The current reader. Calls can be chained.</returns>
        public ICsvReader<TElement> AddColumn(Expression<Func<TElement, object?>> propertyAccessor, string? headerIfDifferent = null)
            => AddColumn(CsvColumn.ForPropertyWithHeader(propertyAccessor, headerIfDifferent));

        /// <summary>
        /// Adds a column with its index.
        /// </summary>
        /// <param name="objectPopulator">A delegate to set the value on the object with the specified format provider.</param>
        /// <param name="index">The zero-based index of the column.</param>
        /// <returns>The current reader. Calls can be chained.</returns>
        public ICsvReader<TElement> AddColumn(Action<TElement, IFormatProvider, string> objectPopulator, int index)
        {
            Columns.Add(CsvColumn.ReadableWithIndex(objectPopulator, index));
            return this;
        }

        /// <summary>
        /// Adds a column with its index.
        /// </summary>
        /// <typeparam name="TColumn">The type of the property to populate.</typeparam>
        /// <param name="objectPopulator">A delegate to set the value on the object.</param>
        /// <param name="index">The zero-based index of the column.</param>
        /// <returns>The current reader. Calls can be chained.</returns>
        public ICsvReader<TElement> AddColumn<TColumn>(Action<TElement, TColumn?> objectPopulator, int index)
            => AddColumn((o, f, v) => objectPopulator(o, Conversion.TryParse(v, f, out TColumn? val) ? val : default), index);

        /// <summary>
        /// Adds a column with its index.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="index">The zero-based index of the column.</param>
        /// <returns>The current reader. Calls can be chained.</returns>
        public ICsvReader<TElement> AddColumn(string propertyName, int index)
            => AddColumn(CsvColumn.ForPropertyWithIndex<TElement>(propertyName, index));

        /// <summary>
        /// Adds a column with its index.
        /// </summary>
        /// <param name="propertyAccessor">The accessor to the property.</param>
        /// <param name="index">The zero-based index of the column.</param>
        /// <returns>The current reader. Calls can be chained.</returns>
        public ICsvReader<TElement> AddColumn(Expression<Func<TElement, object?>> propertyAccessor, int index)
            => AddColumn(CsvColumn.ForPropertyWithIndex(propertyAccessor, index));

        private ICsvReader<TElement> AddColumn(CsvColumnBase<TElement> column)
        {
            if (column is null) throw new ArgumentNullException(nameof(column));
            if (!column.CanRead) throw new ArgumentException("The column must be readable.", nameof(column));
            Columns.Add(column);
            return this;
        }

        private int _currentCharLine;
        private int _currentCharColumn;
        /// <summary>
        /// Asynchronously reads the CSV stream and return the deserialized elements.
        /// </summary>
        /// <returns>The deserialized elements.</returns>
        /// <exception cref="InvalidOperationException">The SourceStream property is not initialized.</exception>
        /// <exception cref="CsvFormatException"></exception>
        public async IAsyncEnumerable<TElement> ReadAsAsyncEnumerable()
        {
            if (SourceStream is null) throw new InvalidOperationException($"The {nameof(SourceStream)} property is not initialized.");
            if (_isDisposed) throw new ObjectDisposedException(nameof(CsvReader<TElement>));

            var separator = _configuration.Separator;
            var hasHeaders = _configuration.HasHeaders;
            var autoGenerate = _configuration.AutoGenerate;
            var encoding = _configuration.Encoding;
            var culture = _configuration.Culture;
            _currentCharLine = 0;
            _currentCharColumn = 0;

            CsvColumnBase<TElement>[] columns;
            if (autoGenerate == true || (!autoGenerate.HasValue && !Columns.Any()))
            {
                columns = typeof(TElement)
                    .GetProperties()
                    .Where(p => p.CanWrite && !p.IsIndexer())
                    .Select(p => CsvColumn.ForPropertyWithHeader<TElement>(p))
                    .Concat(Columns)
                    .Where(c => c.CanRead)
                    .ToArray();
            }
            else columns = Columns.Where(c => c.CanRead).ToArray();

            if (!hasHeaders && columns.Any(c => !c.Index.HasValue)) throw new InvalidOperationException("A column requires a header and the source doesn't provide them.");

            CsvColumnBase<TElement>?[]? columnsInCsvOrder = null;
            var reader = new StreamReader(SourceStream, encoding);
            int? csvColumnCount = null;
            if (hasHeaders)
            {
                var headers = await ParseLineAsync(reader, separator);
                if (headers is null) yield break;
                columnsInCsvOrder = new CsvColumnBase<TElement>?[headers.Count];
                csvColumnCount = headers.Count;
                for (int i = 0; i < headers.Count; i++)
                {
                    var matchingColumn = columns.FirstOrDefault(c => (c.Index.HasValue && c.Index == i) || (c.Header == headers[i]));
                    columnsInCsvOrder[i] = matchingColumn;
                }
            }

            while (true)
            {
                var lineData = await ParseLineAsync(reader, separator);
                if (lineData is null) yield break;
                if (csvColumnCount.HasValue && lineData.Count > 0)
                {
                    if (lineData.Count > csvColumnCount.Value) throw new CsvFormatException($"Too many columns in CSV at ln: {_currentCharLine}.", _currentCharLine);
                    if (lineData.Count < csvColumnCount.Value) throw new CsvFormatException($"Too few columns in CSV at ln: {_currentCharLine}.", _currentCharLine);
                }
                else if (!csvColumnCount.HasValue && lineData.Count > 0)
                    csvColumnCount = lineData.Count;

                if (columnsInCsvOrder is null)
                {
                    columnsInCsvOrder = new CsvColumnBase<TElement>?[lineData.Count];
                    for (int i = 0; i < lineData.Count; i++)
                    {
                        var matchingColumn = columns.FirstOrDefault(c => c.Index.HasValue && c.Index == i);
                        columnsInCsvOrder[i] = matchingColumn;
                    }
                }

                var element = Activator.CreateInstance<TElement>()!;
                for (int i = 0; i < columnsInCsvOrder.Length; i++)
                {
                    var currentColumn = columnsInCsvOrder[i];
                    if (currentColumn is null) continue;
                    currentColumn.PopulateElement(element, culture, lineData[i]);
                }

                yield return element;
            }
        }

        private async Task<List<string>?> ParseLineAsync(StreamReader reader, string separator)
        {
            var result = new List<string>();
            var line = await reader.ReadLineAsync();
            _currentCharLine++;
            _currentCharColumn = 1;
            if (line is null) return null;
            bool isEscaping = false;
            bool wasEscaping = false;
            var builder = new StringBuilder();
            for (int i = 0; i < line.Length; i++, _currentCharColumn++)
            {
                if (line[i] == '"')
                {
                    if (!isEscaping)
                    {
                        if (builder.Length > 0) throw new CsvFormatException($"Unexpected '\"' character in CSV at ln: {_currentCharLine}, ch: {_currentCharColumn}.", _currentCharLine, _currentCharColumn);
                        isEscaping = true;
                    }
                    else if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        builder.Append('"');
                        i++;
                        _currentCharColumn++;
                    }
                    else
                    {
                        isEscaping = false;
                        wasEscaping = true;
                    }
                }
                else if (!isEscaping && StringContinuesWith(line, i, separator))
                {
                    wasEscaping = false;
                    i += separator.Length - 1;
                    _currentCharColumn += separator.Length - 1;
                    result.Add(builder.ToString());
                    builder.Clear();
                }
                else
                {
                    if (wasEscaping) throw new CsvFormatException($"Unexpected '{line[i]}' character in CSV at ln: {_currentCharLine}, ch: {_currentCharColumn}.", _currentCharLine, _currentCharColumn);
                    builder.Append(line[i]);
                }

                if (i == line.Length - 1 && isEscaping)
                {
                    var nextLine = await reader.ReadLineAsync();
                    if (nextLine is null) throw new CsvFormatException($"Unclosed escaping quote in CSV at ln: {_currentCharLine}.", _currentCharLine);
                    _currentCharLine++;
                    _currentCharColumn = 0;
                    line = $"{line}{Environment.NewLine}{nextLine}";
                }
            }

            result.Add(builder.ToString());
            return result;
        }

        private bool StringContinuesWith(string value, int currentIndex, string next)
        {
            if (currentIndex + next.Length > value.Length) return false;
            for (int i = 0; i < next.Length; i++)
            {
                if (value[currentIndex + i] != next[i]) return false;
            }

            return true;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                if (!_configuration.KeepStreamOpen)
                {
                    SourceStream?.Dispose();
                    SourceStream = null;
                }

                _isDisposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}
