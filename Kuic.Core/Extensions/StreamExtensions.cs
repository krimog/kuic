using System;
using System.Collections.Generic;
using System.IO;
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
    }
}
