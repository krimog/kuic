using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuic.Core.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<string> ReadAllTextAsync(this Stream stream, bool seekOrigin = true, bool keepOpen = false)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            if (seekOrigin && stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);

            var reader = new StreamReader(stream);
            var result = await reader.ReadToEndAsync();

            if (!keepOpen) stream.Close();
            return result;
        }

        public static async Task<byte[]> ReadAllBytesAsync(this Stream stream, bool seekOrigin = true, bool keepOpen = false)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));
            const int bufferSize = 4096;

            if (seekOrigin && stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);

            var bytesRead = new List<byte>(bufferSize);
            var buffer = new byte[bufferSize];
            int readCount;
            do
            {
                readCount = await stream.ReadAsync(buffer, 0, bufferSize);
                if (readCount == 0) break;
                bytesRead.AddRange(buffer[..readCount]);
                if (readCount < bufferSize) break;
            } while (true);

            if (!keepOpen) stream.Close();
            return bytesRead.ToArray();
        }

        public static bool TryGetEncoding(this Stream stream, out Encoding? encoding)
        {
            if (!stream.CanSeek) throw new NotSupportedException($"{nameof(TryGetEncoding)} only supports seekable streams.");

            var encodings = new[]
            {
                Encoding.ASCII,
                Encoding.BigEndianUnicode,
                Encoding.Unicode,
                Encoding.UTF32,
                Encoding.UTF8,
                Encoding.UTF7,
            };

            var boms = encodings.Select(e => e.GetPreamble()).ToArray();
            var longestBom = boms.Max(b => b.Length);

            byte[] streamStart = new byte[longestBom];
            var readBytesLength = stream.Read(streamStart, 0, longestBom);
            stream.Seek(-readBytesLength, SeekOrigin.Current);
            for (int b = 0; b < boms.Length; b++)
            {
                int currentBomLength = boms[b].Length;
                if (readBytesLength < currentBomLength) continue;
                if (IsMatch(streamStart, boms[b]))
                {
                    stream.Seek(currentBomLength, SeekOrigin.Current);
                    encoding = encodings[b];
                    return true;
                }
            }

            encoding = default;
            return false;

            bool IsMatch(byte[] streamStart, byte[] bom)
            {
                for (int i = 0; i < bom.Length; i++)
                    if (streamStart[i] != bom[i]) return false;
                return true;
            }
        }
    }
}
