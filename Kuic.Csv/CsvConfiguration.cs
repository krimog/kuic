using System;
using System.Globalization;
using System.Text;

namespace Kuic.Csv
{
    /// <summary>
    /// Reprensents the configuration of the <see cref="CsvBuilder{TSource}"/>.
    /// </summary>
    public class CsvConfiguration
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
            AddHeaders = true;
        }

        private bool _hasSeparatorBeenSet = false;
        private string _separator;
        private Encoding _encoding;
        private CultureInfo _culture;

        /// <summary>
        /// Gets or sets a value indicating whether the BOM should be added in the file.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the BOM should be added in the file; otherwise, <c>false</c>.
        /// </value>
        public bool AddBomInFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the headers line should be added.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the headers line should be added; otherwise, <c>false</c>.
        /// </value>
        public bool AddHeaders { get; set; }

        /// <summary>
        /// Gets or sets the builder's encoding.
        /// </summary>
        /// <value>
        /// The encoding.
        /// </value>
        /// <exception cref="System.ArgumentNullException">value</exception>
        public Encoding Encoding
        {
            get => _encoding;
            set => _encoding = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets or sets the separator.
        /// </summary>
        /// <value>
        /// The separator.
        /// </value>
        /// <exception cref="System.ArgumentNullException">value</exception>
        /// <exception cref="System.ArgumentException">
        /// The separator can't be empty.
        /// or
        /// The separator can't contain ".
        /// or
        /// The separator can't contain a new line character.
        /// </exception>
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
        /// Gets or sets the culture.
        /// </summary>
        /// <value>
        /// The culture.
        /// </value>
        /// <exception cref="System.ArgumentNullException">value</exception>
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
        /// Sets the culture.
        /// </summary>
        /// <param name="cultureName">Name of the culture.</param>
        /// <example><c>SetCulture("en-US");</c></example>
        /// <exception cref="System.ArgumentNullException">cultureName</exception>
        public void SetCulture(string cultureName)
        {
            _ = cultureName ?? throw new ArgumentNullException(nameof(cultureName));
            var cultureInfo = new CultureInfo(cultureName);
            Culture = cultureInfo;
        }
    }
}
