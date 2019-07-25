using System;
using System.Collections.Concurrent;

namespace Woof.SystemEx {

    /// <summary>
    /// A simple fixed-size, thread-safe buffer pool.
    /// Used to avoid frequent memory collections / allocations.
    /// </summary>
    public sealed class BufferPool {

        /// <summary>
        /// Default buffer size.
        /// </summary>
        private readonly int Size;

        /// <summary>
        /// Actual buffer collection.
        /// </summary>
        private readonly ConcurrentBag<byte[]> Pool = new ConcurrentBag<byte[]>();

        /// <summary>
        /// Creates new buffer pool with specified buffer size.
        /// </summary>
        /// <param name="size">Buffer size in bytes, default 1KB.</param>
        public BufferPool(int size = 0x400) => Size = size;

        /// <summary>
        /// Gets a buffer from the pool.
        /// </summary>
        /// <returns></returns>
        public byte[] Get() {
            if (Pool.TryTake(out var buffer)) return buffer;
            else return new byte[Size];
        }

        /// <summary>
        /// Returns a buffer to the pool.
        /// </summary>
        /// <param name="buffer"></param>
        public void Put(byte[] buffer) {
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (buffer.Length != Size) throw new ArgumentOutOfRangeException("buffer");
            Array.Clear(buffer, 0, Size);
            Pool.Add(buffer);
        }

    }

}