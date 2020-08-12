using System;

namespace Kuic.Core.Extensions
{
    public static class DecimalExtensions
    {
        public static decimal Round(this decimal value, int decimals = 2)
        {
            if (decimals < 0) throw new ArgumentOutOfRangeException(nameof(decimals), $"{nameof(decimals)} should not be negative.");
            return Decimal.Round(value, decimals);
        }
    }
}
