using System;
using System.Collections;
using System.Collections.Generic;

namespace Kuic.Csv
{
    /// <summary>
    /// Represents a list of CSV columns.
    /// </summary>
    /// <typeparam name="TElement">The type of an element of the CSV.</typeparam>
    public sealed class CsvColumnCollection<TElement> : IList<CsvColumnBase<TElement>>, IReadOnlyCollection<CsvColumnBase<TElement>>, ICollection<CsvColumnBase<TElement>>, IEnumerable<CsvColumnBase<TElement>>
    {
        private readonly List<CsvColumnBase<TElement>> _columns;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvColumnCollection{TElement}"/> class.
        /// </summary>
        public CsvColumnCollection()
        {
            _columns = new();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvColumnCollection{TElement}"/> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public CsvColumnCollection(IEnumerable<CsvColumnBase<TElement>> collection)
        {
            _columns = new(collection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvColumnCollection{TElement}"/> class.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        public CsvColumnCollection(int capacity)
        {
            _columns = new(capacity);
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public CsvColumnBase<TElement> this[int index] { get => _columns[index]; set => _columns[index] = value; }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="CsvColumnCollection{TElement}"/>.
        /// </summary>
        public int Count => _columns.Count;

        public bool IsReadOnly => false;

        /// <summary>
        /// Adds an object to the end of the <see cref="CsvColumnCollection{TElement}"/>.
        /// </summary>
        /// <param name="item">The object to be added.</param>
        public void Add(CsvColumnBase<TElement> item) => _columns.Add(item ?? throw new ArgumentNullException(nameof(item)));

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="CsvColumnCollection{TElement}"/>.
        /// </summary>
        /// <param name="columns">The collection whose elements should be added.</param>
        public void AddRange(IEnumerable<CsvColumnBase<TElement>> columns)
        {
            foreach (var column in columns ?? throw new ArgumentNullException(nameof(columns)))
                Add(column);
        }

        /// <summary>
        /// Removes all elements from the <see cref="CsvColumnCollection{TElement}"/>.
        /// </summary>
        public void Clear() => _columns.Clear();

        public bool Contains(CsvColumnBase<TElement> item) => _columns.Contains(item);

        public void CopyTo(CsvColumnBase<TElement>[] array, int arrayIndex) => _columns.CopyTo(array, arrayIndex);

        public IEnumerator<CsvColumnBase<TElement>> GetEnumerator() => _columns.GetEnumerator();

        public int IndexOf(CsvColumnBase<TElement> item) => _columns.IndexOf(item);

        public void Insert(int index, CsvColumnBase<TElement> item) => _columns.Insert(index, item);

        public bool Remove(CsvColumnBase<TElement> item) => _columns.Remove(item);

        public void RemoveAt(int index) => _columns.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => _columns.GetEnumerator();
    }
}
