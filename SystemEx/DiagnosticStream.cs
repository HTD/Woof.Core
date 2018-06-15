using System;
using System.IO;

namespace Woof.SystemEx {

    /// <summary>
    /// Stream wrapper reporting I/O operations progress.
    /// </summary>
    public class DiagnosticStream : Stream {

        /// <summary>
        /// Occurs when single read operation has completed.
        /// </summary>
        public event EventHandler ReadCompleted;

        /// <summary>
        /// Occurs when signle write operation has completed.
        /// </summary>
        public event EventHandler WriteCompleted;

        /// <summary>
        /// Occurs when read operation reached end of content.
        /// </summary>
        public event EventHandler EndOfContent;

        /// <summary>
        /// Occurs when the inner stream is closed.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Gets the total number of bytes read from the stream.
        /// </summary>
        public long BytesRead { get; private set; }

        /// <summary>
        /// Gets the total number of bytes written to the stream.
        /// </summary>
        public long BytesWritten { get; private set; }

        /// <summary>
        /// Inner stream instance.
        /// </summary>
        public readonly Stream InnerStream;

        /// <summary>
        /// Known content length. Null if unknown.
        /// </summary>
        public readonly long? ContentLength;

        /// <summary>
        /// True if the inner stream is disposed.
        /// </summary>
        public bool IsDisposed;

        /// <summary>
        /// Creates diagnostic wrapper over inner stream.
        /// </summary>
        /// <param name="innerStream">Inner stream.</param>
        /// <param name="contentLength">Known content length.</param>
        public DiagnosticStream(Stream innerStream, long? contentLength) {
            InnerStream = innerStream;
            ContentLength = contentLength;
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead => InnerStream.CanRead;

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek => InnerStream.CanSeek;

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite => InnerStream.CanWrite;

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        /// <exception cref="System.NotSupportedException">A class derived from Stream does not support seeking.</exception>
        /// <exception cref="System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override long Length => ContentLength ?? InnerStream.Length;

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="System.NotSupportedException">The stream does not support seeking.</exception>
        /// <exception cref="System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override long Position { get => InnerStream.Position; set => InnerStream.Position = value; }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        public override void Flush() => InnerStream.Flush();

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. When this method returns, the buffer contains the specified
        /// byte array with the values between offset and (offset + count - 1) replaced by
        /// the bytes read from the current source.
        /// </param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number
        /// of bytes requested if that many bytes are not currently available, or zero (0)
        /// if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="System.ArgumentException">The sum of offset and count is larger than the buffer length.</exception>
        /// <exception cref="System.ArgumentNullException">Buffer is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Offset or count is negative.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="System.NotSupportedException">The stream does not support reading.</exception>
        /// <exception cref="System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override int Read(byte[] buffer, int offset, int count) {
            int length = InnerStream.Read(buffer, offset, count);
            BytesRead += length;
            ReadCompleted?.Invoke(this, EventArgs.Empty);
            if (length == 0) EndOfContent?.Invoke(this, EventArgs.Empty);
            return length;
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type System.IO.SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="System.NotSupportedException">The stream does not support seeking.</exception>
        /// <exception cref="System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override long Seek(long offset, SeekOrigin origin) => InnerStream.Seek(offset, origin);

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="System.NotSupportedException">The stream does not support seeking.</exception>
        /// <exception cref="System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override void SetLength(long value) => InnerStream.SetLength(value);

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="System.ArgumentException">The sum of offset and count is larger than the buffer length.</exception>
        /// <exception cref="System.ArgumentNullException">Buffer is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Offset or count is negative.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="System.NotSupportedException">The stream does not support writing.</exception>
        /// <exception cref="System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override void Write(byte[] buffer, int offset, int count) {
            InnerStream.Write(buffer, offset, count);
            BytesWritten += count;
            WriteCompleted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Closes the current stream and releases any resources (such as sockets and file
        /// handles) associated with the current stream. Instead of calling this method,
        /// ensure that the stream is properly disposed.
        /// </summary>
        public override void Close() {
            if (IsDisposed) return;
            InnerStream.Dispose();
            Closed?.Invoke(this, EventArgs.Empty);
            IsDisposed = true;
        }

        /// <summary>
        /// Releases all resources used by the System.IO.Stream.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing) {
            if (disposing && !IsDisposed) Close();
        }

    }

}