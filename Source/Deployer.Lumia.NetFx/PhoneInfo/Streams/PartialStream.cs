using System;
using System.IO;
using System.Linq;

namespace Deployer.Lumia.NetFx.PhoneInfo.Streams
{
    internal class PartialStream : Stream
    {
        private Stream innerstream;

        private bool disposed;
        private long start;
        private long end;

        public PartialStream(Stream stream, long StartOffset, long EndOffset)
        {
            stream.Seek(StartOffset, SeekOrigin.Begin);
            start = StartOffset;
            end = EndOffset;
            innerstream = stream;
        }

        public override bool CanRead => innerstream.CanRead;
        public override bool CanSeek => innerstream.CanSeek;
        public override bool CanWrite => innerstream.CanWrite;
        public override long Length => end - start;
        public override long Position { get => innerstream.Position - start; set => innerstream.Position = value + start; }

        public override void Flush()
        {
            innerstream.Flush();
        }

        // Some devices can only be read sector per sector, this aims to solve this issue.
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count % 512 != 0)
            {
                long extrastart = innerstream.Position % 512;
                if (extrastart != 0)
                {
                    innerstream.Seek(-extrastart, SeekOrigin.Current);
                }

                var addedcount = 512 - count % 512;
                var ncount = count + addedcount;
                byte[] tmpbuffer = new byte[extrastart + buffer.Length + addedcount];
                buffer.CopyTo(tmpbuffer, extrastart);
                innerstream.Read(tmpbuffer, offset, ncount);
                tmpbuffer.ToList().Skip((int)extrastart).Take(count + offset).ToArray().CopyTo(buffer, 0);
                return count;
            }

            return innerstream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin)
                return innerstream.Seek(offset + start, origin);
            if (origin == SeekOrigin.End)
                return innerstream.Seek(end + offset, origin);
            return innerstream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            innerstream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            innerstream.Write(buffer, offset, count);
        }
        
        public override void Close()
        {
            innerstream.Dispose();
            innerstream = null;
            base.Close();
        }

        new void Dispose()
        {
            Dispose(true);
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        new void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!disposed)
            {
                if (disposing)
                    if (innerstream != null)
                    {
                        innerstream.Dispose();
                        innerstream = null;
                    }

                // Note disposing has been done.
                disposed = true;
            }
        }
    }
}