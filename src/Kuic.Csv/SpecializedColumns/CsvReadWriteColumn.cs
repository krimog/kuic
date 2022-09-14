using System;
using Kuic.Core.Extensions;

namespace Kuic.Csv.SpecializedColumns
{
    internal class CsvReadWriteColumn<TElement, TColumn> : CsvColumnBase<TElement>
    {
        private readonly Func<TElement, TColumn> _accessor;
        private readonly Action<TElement, IFormatProvider, string> _populator;

        public CsvReadWriteColumn(Func<TElement, TColumn> accessor, Action<TElement, IFormatProvider, string> populator, int index)
            : base(index)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _populator = populator ?? throw new ArgumentNullException(nameof(populator));
        }

        public CsvReadWriteColumn(Func<TElement, TColumn> accessor, Action<TElement, IFormatProvider, string> populator, string header)
            : base(header)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _populator = populator ?? throw new ArgumentNullException(nameof(populator));
        }

        public override bool CanWrite => true;
        public override bool CanRead => true;

        public override string GetStringValue(TElement element, IFormatProvider formatProvider)
        {
            return _accessor(element)?.ToCulturedString(formatProvider) ?? string.Empty;
        }

        public override void PopulateElement(TElement element, IFormatProvider formatProvider, string value)
        {
            _populator(element, formatProvider, value);
        }
    }
}
