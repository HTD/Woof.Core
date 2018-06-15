using System;
using System.IO;

namespace Woof.DebugEx {

    public class DebugStream : Stream {

        public event EventHandler DataRead;
        public event EventHandler DataWritten;

        public byte[] InputBuffer { get; private set; }

        public byte[] OutputBuffer { get; private set; }

        public int HeadPosition { get; private set; }

        public int BytesRead { get; private set; }

        public int TotalRead { get; private set; }

        public int BytesWritten { get; private set; }

        public int TotalWritten { get; private set; }

        public DebugStream(Stream target) => Target = target;

        public override bool CanRead => Target.CanRead;

        public override bool CanSeek => Target.CanSeek;

        public override bool CanWrite => Target.CanWrite;

        public override long Length => Target.Length;

        public override long Position { get => Target.Position; set => Target.Position = value; }

        public override void Flush() => Target.Flush();

        public override int Read(byte[] buffer, int offset, int count) {
            if (InputBuffer == null || InputBuffer.Length < count) InputBuffer = new byte[count];
            int length = Target.Read(InputBuffer, 0, count);
            Buffer.BlockCopy(InputBuffer, 0, buffer, offset, length);
            HeadPosition += length;
            BytesRead = length;
            TotalRead += length;
            DataRead?.Invoke(this, EventArgs.Empty);
            return length;
        }
        public override long Seek(long offset, SeekOrigin origin) => Target.Seek(offset, origin);

        public override void SetLength(long value) => Target.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) {
            if (OutputBuffer == null || OutputBuffer.Length < count) OutputBuffer = new byte[count];
            Buffer.BlockCopy(buffer, offset, OutputBuffer, 0, count);
            Target.Write(OutputBuffer, 0, count);
            HeadPosition += count;
            BytesWritten = count;
            TotalWritten += count;
            DataWritten?.Invoke(this, EventArgs.Empty);
        }

        private readonly Stream Target;

    }

}
