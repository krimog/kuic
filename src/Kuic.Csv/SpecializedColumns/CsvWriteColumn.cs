using System;
using Kuic.Core.Extensions;

namespace Kuic.Csv.SpecializedColumns
{
    internal class CsvWriteColumn<TElement, TColumn> : CsvColumnBase<TElement>
    {
        private readonly Func<TElement, TColumn> _accessor;

        public CsvWriteColumn(Func<TElement, TColumn> accessor, int index)
            : base(index)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        public CsvWriteColumn(Func<TElement, TColumn> accessor, string header)
            : base(header)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        public override bool CanWrite => true;
        public override bool CanRead => false;

        public override string GetStringValue(TElement element, IFormatProvider formatProvider)
        {
            return _accessor(element)?.ToCulturedString(formatProvider) ?? string.Empty;
        }
    }
}
