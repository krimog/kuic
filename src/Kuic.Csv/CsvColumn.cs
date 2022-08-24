using Kuic.Core.Extensions;
using System;

namespace Kuic.Csv
{
    /// <summary>
    /// Represents a column in a CSV file.
    /// </summary>
    /// <typeparam name="TSource">The type of the source data.</typeparam>
    public abstract class CsvColumnBase<TSource>
    {
        protected CsvColumnBase(string? header)
        {
            Header = header;
        }

        public string? Header { get; }

        public abstract string GetStringValue(TSource element, IFormatProvider formatProvider);
    }

    /// <summary>
    /// Represents a column in a CSV file.
    /// </summary>
    /// <typeparam name="TSource">The type of the source data.</typeparam>
    /// <typeparam name="TColumn">The type of the column data.</typeparam>
    /// <seealso cref="Kuic.Csv.CsvColumnBase{TSource}" />
    public class CsvColumn<TSource, TColumn> : CsvColumnBase<TSource>
    {
        private readonly Func<TSource, TColumn> _accessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvColumn{TSource, TColumn}"/> class.
        /// </summary>
        /// <param name="header">The column header.</param>
        /// <param name="accessor">The column data accessor.</param>
        /// <exception cref="System.ArgumentNullException">accessor</exception>
        public CsvColumn(string header, Func<TSource, TColumn> accessor)
            : base(header)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvColumn{TSource, TColumn}"/> class.
        /// </summary>
        /// <param name="accessor">The column data accessor.</param>
        /// <exception cref="System.ArgumentNullException">accessor</exception>
        public CsvColumn(Func<TSource, TColumn> accessor)
            : base(null)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        /// <summary>
        /// Gets the string reprensation of the column for some source element with a specific format provider.
        /// </summary>
        /// <param name="element">The source element.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>The string reprensentation of the data.</returns>
        public override string GetStringValue(TSource element, IFormatProvider formatProvider)
        {
            return _accessor(element)?.ToCulturedString(formatProvider) ?? string.Empty;
        }
    }
}
