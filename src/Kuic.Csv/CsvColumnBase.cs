using System;

namespace Kuic.Csv
{
    /// <summary>
    /// Represents a CSV column with accessors to read and/or write into an object.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    public abstract class CsvColumnBase<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvColumnBase{TElement}"/> class.
        /// </summary>
        /// <param name="header">The header of the column.</param>
        protected CsvColumnBase(string header)
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvColumnBase{TElement}"/> class.
        /// </summary>
        /// <param name="index">The zero-based index of the column.</param>
        protected CsvColumnBase(int index)
        {
            Index = index;
        }

        /// <summary>
        /// Gets the zero-based index of the column; or <c>null</c> if the column has a header.
        /// </summary>
        /// <value>
        /// The index of the column.
        /// </value>
        public int? Index { get; }
        /// <summary>
        /// Gets the header of the column; or <c>null</c> if the column has an index.
        /// </summary>
        /// <value>
        /// The header of the column.
        /// </value>
        public string? Header { get; }

        /// <summary>
        /// Gets a value indicating whether you can write into the column or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if you can write into the column; otherwise, false.
        /// </value>
        public abstract bool CanWrite { get; }
        /// <summary>
        /// Gets a value indicating whether you can read the column or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if you can read the column; otherwise, false.
        /// </value>
        public abstract bool CanRead { get; }

        /// <summary>
        /// Gets the string representation of the value to write into the column.
        /// </summary>
        /// <param name="element">The source object.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>The string representation of the value.</returns>
        public virtual string GetStringValue(TElement element, IFormatProvider formatProvider)
        {
            if (CanRead) throw new NotImplementedException();
            else throw new InvalidOperationException();
        }

        /// <summary>
        /// Sets the provided value into the element.
        /// </summary>
        /// <param name="element">The source object.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="value">The value to write.</param>
        public virtual void PopulateElement(TElement element, IFormatProvider formatProvider, string value)
        {
            if (CanWrite) throw new NotImplementedException();
            else throw new InvalidOperationException();
        }
    }
}
