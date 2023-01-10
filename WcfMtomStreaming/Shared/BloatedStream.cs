using System;
using System.IO;

namespace Shared
{
    public class BloatedStream : Stream
    {
        private readonly long length;
        private long position;

        public BloatedStream(long length)
        {
            this.length = length;
            position = 0;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (position < length)
            {
                var numberOfBytesInThisRead = (int)Math.Min(count, length - position);
                position += numberOfBytesInThisRead;
                return numberOfBytesInThisRead;
            }

            return 0;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
