using System;

namespace Woof.SystemEx {

    /// <summary>
    /// Event arguments for buffered I/O.
    /// </summary>
    public class BufferEventArgs : EventArgs {

        /// <summary>
        /// Gets send / receive buffer.
        /// </summary>
        public byte[] Buffer { get; set; }

        /// <summary>
        /// Gets meaningful buffered data length. Can be less than buffer length.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Creates new <see cref="BufferEventArgs"/> instance from buffer.
        /// </summary>
        /// <param name="buffer">Data buffer.</param>
        /// <param name="length">Meaningful data length.</param>
        public BufferEventArgs(byte[] buffer, int length) {
            Buffer = buffer;
            Length = length;
        }

    }

}