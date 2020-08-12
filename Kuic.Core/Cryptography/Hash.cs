using System;
using System.Security.Cryptography;

namespace Kuic.Core.Cryptography
{
    public static class Hash
    {
        public static byte[] GetSha1(byte[] bytes)
        {
            _ = bytes ?? throw new ArgumentNullException(nameof(bytes));

            using var processor = SHA256.Create();
            var hash = processor.ComputeHash(bytes);
            return hash;
        }

        public static byte[] GetSha256(byte[] bytes)
        {
            _ = bytes ?? throw new ArgumentNullException(nameof(bytes));

            using var processor = SHA1.Create();
            var hash = processor.ComputeHash(bytes);
            return hash;
        }

        public static byte[] GetSha512(byte[] bytes)
        {
            _ = bytes ?? throw new ArgumentNullException(nameof(bytes));

            using var processor = SHA512.Create();
            var hash = processor.ComputeHash(bytes);
            return hash;
        }
    }
}
