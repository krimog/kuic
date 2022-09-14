using System;

namespace Kuic.Csv.SpecializedColumns
{
    internal class CsvReadColumn<TElement> : CsvColumnBase<TElement>
    {
        private readonly Action<TElement, IFormatProvider, string> _populator;

        public CsvReadColumn(Action<TElement, IFormatProvider, string> populator, int index)
            : base(index)
        {
            _populator = populator ?? throw new ArgumentNullException(nameof(populator));
        }

        public CsvReadColumn(Action<TElement, IFormatProvider, string> populator, string header)
            : base(header)
        {
            _populator = populator ?? throw new ArgumentNullException(nameof(populator));
        }

        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override void PopulateElement(TElement element, IFormatProvider formatProvider, string value)
        {
            _populator(element, formatProvider, value);
        }
    }
}
