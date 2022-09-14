using System;
using System.Globalization;
using System.Text;

namespace Kuic.Csv
{
    /// <summary>
    /// Provides configuration for both <see cref="CsvBuilder{TElement}"/> and <see cref="CsvReader{TElement}"/>.
    /// </summary>
    public class CsvConfiguration : ICsvBuilderConfiguration, ICsvReaderConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvConfiguration"/> class.
        /// </summary>
        public CsvConfiguration()
        {
            _culture = CultureInfo.CurrentCulture;
            _encoding = Encoding.UTF8;
            _separator = _culture.TextInfo.ListSeparator;

            AddBomInFile = true;
            HasHeaders = true;
            KeepStreamOpen = true;
        }

        private bool _hasSeparatorBeenSet = false;
        private string _separator;
        private Encoding _encoding;
        private CultureInfo _culture;

        /// <summary>
        /// Gets or sets a value indicating whether the CSV file has headers or not.
        /// Default is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the file has headers; otherwise, <c>false</c>.
        /// </value>
        public bool HasHeaders { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the columns should be generated automatically or not.
        /// Default is <c>null</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the columns should be generated automatically; <c>false</c> if not; <c>null</c> if it depends on whether you have manually added columns.
        /// </value>
        public bool? AutoGenerate { get; set; }

        /// <summary>
        /// Gets or sets the encoding of the CSV file.
        /// Default is UTF8.
        /// </summary>
        /// <value>
        /// The encoding of the CSV file.
        /// </value>
        public Encoding Encoding
        {
            get => _encoding;
            set => _encoding = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets or sets the separator of the CSV file.
        /// Default is the culture's separator.
        /// </summary>
        /// <value>
        /// The separator of the CSV file.
        /// </value>
        public string Separator
        {
            get => _separator;
            set
            {
                if (value is null) throw new ArgumentNullException(nameof(value));
                if (value.Length == 0) throw new ArgumentException("The separator can't be empty.", nameof(value));
                if (value.Contains('"')) throw new ArgumentException("The separator can't contain \".", nameof(value));
                if (value.Contains('\n') || value.Contains('\r')) throw new ArgumentException("The separator can't contain a new line character.", nameof(value));
                _separator = value;
                _hasSeparatorBeenSet = true;
            }
        }

        /// <summary>
        /// Gets or sets the culture of the CSV file.
        /// Default is CurrentCulture.
        /// </summary>
        /// <value>
        /// The culture of the CSV file.
        /// </value>
        public CultureInfo Culture
        {
            get => _culture;
            set
            {
                _culture = value ?? throw new ArgumentNullException(nameof(value));
                if (!_hasSeparatorBeenSet) _separator = _culture.TextInfo.ListSeparator;
            }
        }

        /// <summary>
        /// Sets the culture of the CSV file.
        /// </summary>
        /// <param name="cultureName">The name of the culture in the "en-US" format.</param>
        public void SetCulture(string cultureName)
        {
            _ = cultureName ?? throw new ArgumentNullException(nameof(cultureName));
            var cultureInfo = new CultureInfo(cultureName);
            Culture = cultureInfo;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the BOM should be written in the CSV file or not.
        /// Only used by the <see cref="CsvBuilder{TElement}"/>, and only while using the <see cref="CsvBuilder{TElement}.ToFileAsync(string)"/> method.
        /// Default is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the BOM should be written; otherwise, <c>false</c>.
        /// </value>
        public bool AddBomInFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the source stream should be left open after disposing the <see cref="CsvReader{TElement}"/>.
        /// Default is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the stream should be left open; otherwise, <c>false</c>.
        /// </value>
        public bool KeepStreamOpen { get; set; }
    }
}
