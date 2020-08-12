using Kuic.Core.Cryptography;
using System;

namespace Kuic.Core.Extensions
{
    public static class BytesExtensions
    {
        public static string ToBase64(this byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            return Convert.ToBase64String(bytes);
        }

        public static string GetHashString(this byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            return Hash.GetSha256(bytes).ToBase64();
        }
    }
}
