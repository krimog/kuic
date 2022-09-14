using System;
using System.Linq.Expressions;
using System.Reflection;
using Kuic.Core.Extensions;
using Kuic.Core.Helpers;
using Kuic.Csv.SpecializedColumns;

namespace Kuic.Csv
{
    /// <summary>
    /// Provides static methods to create CSV columns for both <see cref="CsvBuilder{TElement}" /> and <see cref="CsvReader{TElement}" />.
    /// </summary>
    public static class CsvColumn
    {
        /// <summary>
        /// Returns a column with a header to read and/or write into a property.
        /// </summary>
        /// <typeparam name="TElement">The element type.</typeparam>
        /// <param name="propertyInfo">The property to read and/or write into.</param>
        /// <param name="headerIfDifferent">If set, will be the column header. If not set, the property name will be used.</param>
        /// <returns>The CSV column.</returns>
        /// <exception cref="ArgumentNullException">propertyInfo</exception>
        public static CsvColumnBase<TElement> ForPropertyWithHeader<TElement>(PropertyInfo propertyInfo, string? headerIfDifferent = null)
        {
            _ = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            headerIfDifferent ??= propertyInfo.Name;

            Func<TElement, object>? accessor = null;
            if (propertyInfo.CanRead) accessor = propertyInfo.ToGetterLambda<TElement>();

            Action<TElement, IFormatProvider, string>? populator = null;
            if (propertyInfo.CanWrite)
            {
                var setter = propertyInfo.ToSetterLambda<TElement>();
                populator = (elt, formatProvider, value) =>
                {
                    string? nullableValue = value == string.Empty ? null : value;
                    if (Conversion.TryParse(nullableValue, propertyInfo.PropertyType, formatProvider, out var typedValue))
                        setter(elt, typedValue);
                };
            }

            if (propertyInfo.CanRead && propertyInfo.CanWrite)
            {
                return new CsvReadWriteColumn<TElement, object?>(accessor!, populator!, headerIfDifferent);
            }

            if (propertyInfo.CanRead)
            {
                return new CsvWriteColumn<TElement, object?>(accessor!, headerIfDifferent);
            }

            if (propertyInfo.CanWrite)
            {
                return new CsvReadColumn<TElement>(populator!, headerIfDifferent);
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Returns a column with a header to read and/or write into a property.
        /// </summary>
        /// <typeparam name="TElement">The element type.</typeparam>
        /// <param name="propertyExpression">The accessor to the property.</param>
        /// <param name="headerIfDifferent">If set, will be the column header. If not set, the property name will be used.</param>
        /// <returns>The CSV column.</returns>
        /// <exception cref="ArgumentNullException">propertyExpression</exception>
        /// <exception cref="ArgumentException">Unable to get the property from the expression.</exception>
        public static CsvColumnBase<TElement> ForPropertyWithHeader<TElement>(Expression<Func<TElement, object?>> propertyExpression, string? headerIfDifferent = null)
        {
            _ = propertyExpression ?? throw new ArgumentNullException(nameof(propertyExpression));
            var propertyInfo = propertyExpression.GetPropertyInfo();
            if (propertyInfo is null) throw new ArgumentException("Unable to get the property from the expression.", nameof(propertyExpression));
            return ForPropertyWithHeader<TElement>(propertyInfo, headerIfDifferent);
        }

        /// <summary>
        /// Returns a column with a header to read and/or write into a property.
        /// </summary>
        /// <typeparam name="TElement">The element type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="headerIfDifferent">If set, will be the column header. If not set, the property name will be used.</param>
        /// <returns>The CSV column.</returns>
        /// <exception cref="ArgumentNullException">propertyName</exception>
        /// <exception cref="ArgumentException">Unable to find the property.</exception>
        public static CsvColumnBase<TElement> ForPropertyWithHeader<TElement>(string propertyName, string? headerIfDifferent = null)
        {
            _ = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            var propertyInfo = typeof(TElement).GetProperty(propertyName);
            if (propertyInfo is null) throw new ArgumentException($"Unable to find the property {propertyName} in type {typeof(TElement).Name}.", nameof(propertyName));
            return ForPropertyWithHeader<TElement>(propertyInfo, headerIfDifferent);
        }

        /// <summary>
        /// Returns a column with its index to read and/or write into a property.
        /// </summary>
        /// <typeparam name="TElement">The element type.</typeparam>
        /// <param name="propertyInfo">The property to read and/or write into.</param>
        /// <param name="index">The zero-based index of the column.</param>
        /// <returns>The CSV column.</returns>
        /// <exception cref="ArgumentNullException">propertyInfo</exception>
        public static CsvColumnBase<TElement> ForPropertyWithIndex<TElement>(PropertyInfo propertyInfo, int index)
        {
            _ = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));

            Func<TElement, object>? accessor = null;
            if (propertyInfo.CanRead) accessor = propertyInfo.ToGetterLambda<TElement>();

            Action<TElement, IFormatProvider, string>? populator = null;
            if (propertyInfo.CanWrite)
            {
                var setter = propertyInfo.ToSetterLambda<TElement>();
                populator = (elt, formatProvider, value) =>
                {
                    string? nullableValue = value == string.Empty ? null : value;
                    if (Conversion.TryParse(nullableValue, propertyInfo.PropertyType, formatProvider, out var typedValue))
                        setter(elt, typedValue);
                };
            }

            if (propertyInfo.CanRead && propertyInfo.CanWrite)
            {
                return new CsvReadWriteColumn<TElement, object?>(accessor!, populator!, index);
            }

            if (propertyInfo.CanRead)
            {
                return new CsvWriteColumn<TElement, object?>(accessor!, index);
            }

            if (propertyInfo.CanWrite)
            {
                return new CsvReadColumn<TElement>(populator!, index);
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Returns a column with its index to read and/or write into a property.
        /// </summary>
        /// <typeparam name="TElement">The element type.</typeparam>
        /// <param name="propertyExpression">The accessor to the property.</param>
        /// <param name="index">The zero-based index of the column.</param>
        /// <returns>The CSV column.</returns>
        /// <exception cref="ArgumentNullException">propertyExpression</exception>
        /// <exception cref="ArgumentException">Unable to get the property from the expression.</exception>
        public static CsvColumnBase<TElement> ForPropertyWithIndex<TElement>(Expression<Func<TElement, object?>> propertyExpression, int index)
        {
            _ = propertyExpression ?? throw new ArgumentNullException(nameof(propertyExpression));
            var propertyInfo = propertyExpression.GetPropertyInfo();
            if (propertyInfo is null) throw new ArgumentException("Unable to get the property from the expression.", nameof(propertyExpression));
            return ForPropertyWithIndex<TElement>(propertyInfo, index);
        }

        /// <summary>
        /// Returns a column with its index to read and/or write into a property.
        /// </summary>
        /// <typeparam name="TElement">The element type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="index">The zero-based index of the column.</param>
        /// <returns>The CSV column.</returns>
        /// <exception cref="ArgumentNullException">propertyExpression</exception>
        /// <exception cref="ArgumentException">Unable to find the property.</exception>
        public static CsvColumnBase<TElement> ForPropertyWithIndex<TElement>(string propertyName, int index)
        {
            _ = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            var propertyInfo = typeof(TElement).GetProperty(propertyName);
            if (propertyInfo is null) throw new ArgumentException($"Unable to find the property {propertyName} in type {typeof(TElement).Name}.", nameof(propertyName));
            return ForPropertyWithIndex<TElement>(propertyInfo, index);
        }

        /// <summary>
        /// Returns a column for a <see cref="CsvReader{TElement}" /> with a header to write into an object.
        /// </summary>
        /// <typeparam name="TElement">The element type.</typeparam>
        /// <param name="objectPopulator">A delegate to set the value on the object with the specified format provider.</param>
        /// <param name="header">The column header.</param>
        /// <returns>The CSV column.</returns>
        public static CsvColumnBase<TElement> ReadableWithHeader<TElement>(Action<TElement, IFormatProvider, string> objectPopulator, string header)
        {
            return new CsvReadColumn<TElement>(objectPopulator, header);
        }

        /// <summary>
        /// Returns a column for a <see cref="CsvReader{TElement}" /> with its index to write into an object.
        /// </summary>
        /// <typeparam name="TElement">The element type.</typeparam>
        /// <param name="objectPopulator">A delegate to set the value on the object with the specified format provider.</param>
        /// <param name="index">The zero-based index of the column.</param>
        /// <returns>The CSV column.</returns>
        public static CsvColumnBase<TElement> ReadableWithIndex<TElement>(Action<TElement, IFormatProvider, string> objectPopulator, int index)
        {
            return new CsvReadColumn<TElement>(objectPopulator, index);
        }

        /// <summary>
        /// Returns a column for a <see cref="CsvBuilder{TElement}" /> with a header to read from an object.
        /// </summary>
        /// <typeparam name="TElement">The element type.</typeparam>
        /// <typeparam name="TDest">The type of the property.</typeparam>
        /// <param name="valueAccessor">A delegate to get the value from the object.</param>
        /// <param name="header">The column header.</param>
        /// <returns>The CSV column.</returns>
        public static CsvColumnBase<TElement> WritableWithHeader<TElement, TDest>(Func<TElement, TDest> valueAccessor, string header)
        {
            return new CsvWriteColumn<TElement, TDest>(valueAccessor, header);
        }

        /// <summary>
        /// Returns a column for a <see cref="CsvBuilder{TElement}" /> with its index to read from an object.
        /// </summary>
        /// <typeparam name="TElement">The element type.</typeparam>
        /// <typeparam name="TDest">The type of the property.</typeparam>
        /// <param name="valueAccessor">A delegate to get the value from the object.</param>
        /// <param name="index">The zero-based index of the column.</param>
        /// <returns>The CSV column.</returns>
        public static CsvColumnBase<TElement> WritableWithIndex<TElement, TDest>(Func<TElement, TDest> valueAccessor, int index)
        {
            return new CsvWriteColumn<TElement, TDest>(valueAccessor, index);
        }

        /// <summary>
        /// Returns a column for a <see cref="CsvBuilder{TElement}" /> with a header to read from an object.
        /// </summary>
        /// <typeparam name="TElement">The element type.</typeparam>
        /// <param name="valueAccessor">A delegate to get the value from the object.</param>
        /// <param name="header">The column header.</param>
        /// <returns>The CSV column.</returns>
        public static CsvColumnBase<TElement> WritableWithHeader<TElement>(Func<TElement, object?> valueAccessor, string header)
        {
            return new CsvWriteColumn<TElement, object?>(valueAccessor, header);
        }

        /// <summary>
        /// Returns a column for a <see cref="CsvBuilder{TElement}" /> with its index to read from an object.
        /// </summary>
        /// <typeparam name="TElement">The element type.</typeparam>
        /// <param name="valueAccessor">A delegate to get the value from the object.</param>
        /// <param name="index">The zero-based index of the column.</param>
        /// <returns>The CSV column.</returns>
        public static CsvColumnBase<TElement> WritableWithIndex<TElement>(Func<TElement, object?> valueAccessor, int index)
        {
            return new CsvWriteColumn<TElement, object?>(valueAccessor, index);
        }

        /// <summary>
        /// Returns a column with a header to read and write into an object.
        /// </summary>
        /// <typeparam name="TElement">The element type.</typeparam>
        /// <typeparam name="TDest">The type of the property.</typeparam>
        /// <param name="valueAccessor">A delegate to get the value from the object.</param>
        /// <param name="objectPopulator">A delegate to set the value on the object with the specified format provider.</param>
        /// <param name="header">The column header.</param>
        /// <returns>The CSV column.</returns>
        public static CsvColumnBase<TElement> ReadWritableWithHeader<TElement, TDest>(Func<TElement, TDest> valueAccessor, Action<TElement, IFormatProvider, string> objectPopulator, string header)
        {
            return new CsvReadWriteColumn<TElement, TDest>(valueAccessor, objectPopulator, header);
        }

        /// <summary>
        /// Returns a column with its index to read and write into an object.
        /// </summary>
        /// <typeparam name="TElement">The element type.</typeparam>
        /// <typeparam name="TDest">The type of the property.</typeparam>
        /// <param name="valueAccessor">A delegate to get the value from the object.</param>
        /// <param name="objectPopulator">A delegate to set the value on the object with the specified format provider.</param>
        /// <param name="index">The zero-based index of the column.</param>
        /// <returns>The CSV column.</returns>
        public static CsvColumnBase<TElement> ReadWritableWithIndex<TElement, TDest>(Func<TElement, TDest> valueAccessor, Action<TElement, IFormatProvider, string> objectPopulator, int index)
        {
            return new CsvReadWriteColumn<TElement, TDest>(valueAccessor, objectPopulator, index);
        }

        /// <summary>
        /// Returns a column with a header to read and write into an object.
        /// </summary>
        /// <typeparam name="TElement">The element type.</typeparam>
        /// <param name="valueAccessor">A delegate to get the value from the object.</param>
        /// <param name="objectPopulator">A delegate to set the value on the object with the specified format provider.</param>
        /// <param name="header">The column header.</param>
        /// <returns>The CSV column.</returns>
        public static CsvColumnBase<TElement> ReadWritableWithHeader<TElement>(Func<TElement, object?> valueAccessor, Action<TElement, IFormatProvider, string> objectPopulator, string header)
        {
            return new CsvReadWriteColumn<TElement, object?>(valueAccessor, objectPopulator, header);
        }

        /// <summary>
        /// Returns a column with its index to read and write into an object.
        /// </summary>
        /// <typeparam name="TElement">The element type.</typeparam>
        /// <param name="valueAccessor">A delegate to get the value from the object.</param>
        /// <param name="objectPopulator">A delegate to set the value on the object with the specified format provider.</param>
        /// <param name="index">The zero-based index of the column.</param>
        /// <returns>The CSV column.</returns>
        public static CsvColumnBase<TElement> ReadWritableWithIndex<TElement>(Func<TElement, object?> valueAccessor, Action<TElement, IFormatProvider, string> objectPopulator, int index)
        {
            return new CsvReadWriteColumn<TElement, object?>(valueAccessor, objectPopulator, index);
        }
    }
}
