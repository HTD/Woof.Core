using System.IO;

namespace Woof.SystemEx {

    public class CountingStream : Stream {

        public Stream BaseStream { get; }

        public int BytesRead { get; private set; }

        public int BytesWritten { get; private set; }

        public override bool CanRead => BaseStream.CanRead;

        public override bool CanSeek => BaseStream.CanSeek;

        public override bool CanWrite => BaseStream.CanWrite;

        public override long Length => BaseStream.Length;

        public override long Position {
            get => BaseStream.Position; set => BaseStream.Position = value;
        }

        public CountingStream(Stream stream) => BaseStream = stream;
        public override void Flush() => BaseStream.Flush();

        public override long Seek(long offset, SeekOrigin origin) => BaseStream.Seek(offset, origin);

        public override void SetLength(long value) => BaseStream.SetLength(value);

        public override int Read(byte[] buffer, int offset, int count) {
            var bytesRead = BaseStream.Read(buffer, offset, count);
            BytesRead += bytesRead;
            return bytesRead;
        }

        public override void Write(byte[] buffer, int offset, int count) => BaseStream.Write(buffer, offset, BytesWritten += count);

    }

}