using System;
using System.IO;
using System.Threading.Tasks;

namespace Shared
{
    public class StreamingService : IStreamingService
    {
        public async Task UploadAsync(Stream stream)
        {
            var buffer = new byte[1024 * 1024 * 5];
            long totalBytesRead = default;
            int bytesRead;
            DateTime nextUpdate = default;
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                totalBytesRead += bytesRead;
                if (nextUpdate < DateTime.Now)
                {
                    Console.WriteLine($"Received {totalBytesRead} bytes");
                    nextUpdate = DateTime.Now.AddSeconds(2);

                }
            }

            Console.WriteLine($"Received {totalBytesRead} bytes");
        }
    }
}
