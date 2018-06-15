using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CodeDog.Core {

    /// <summary>
    /// Paged memory buffer for data bigger than 2GB.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    public class Paged<T> {

        #region Public fields

        /// <summary>
        /// Total size of paged memory object in bytes.
        /// </summary>
        public readonly ulong Size;

        /// <summary>
        /// Element size in bytes.
        /// </summary>
        public readonly int ElementSize;

        /// <summary>
        /// Total length of paged memory object as a number of T elements.
        /// </summary>
        public readonly ulong Length;

        /// <summary>
        /// Page size in bytes.
        /// </summary>
        public readonly int PageSize;

        /// <summary>
        /// Page length as a number of T elements per page.
        /// </summary>
        public readonly int PageLength;

        /// <summary>
        /// Total pages in object.
        /// </summary>
        public readonly int PageCount;

        /// <summary>
        /// Pages of arrays of T elements.
        /// </summary>
        public readonly T[][] Pages;

        #endregion

        #region Private fields

        /// <summary>
        /// The number of the highest bit in PageSize.
        /// </summary>
        private const int PAGE_SIZE_LOG2 = 17; // PageSize 1MB

        /// <summary>
        /// Unsigned 64-bit integer containing bit mask for page offset part of the 64-bit index.
        /// </summary>
        private readonly ulong OffsetMask;

        #endregion

        /// <summary>
        /// Accesses an element of the paged memory.
        /// </summary>
        /// <param name="i">Element index.</param>
        /// <returns>Element.</returns>
        public T this[ulong i] {
            get => Pages[(int)(i >> PAGE_SIZE_LOG2)][(int)(i & OffsetMask)]; set => Pages[(int)(i >> PAGE_SIZE_LOG2)][(int)(i & OffsetMask)] = value;
        }

        /// <summary>
        /// Accesses an element of the paged memory.
        /// </summary>
        /// <param name="page">Page index.</param>
        /// <param name="offset">Element offset.</param>
        /// <returns>Element.</returns>
        public T this[int page, int offset] {
            get => Pages[page][offset]; set => Pages[page][offset] = value;
        }

        /// <summary>
        /// Allocates paged memory (in large amounts).
        /// </summary>
        /// <param name="size">Size in bytes (will be truncated to page size, default 1MB).</param>
        /// <param name="clearLsb">Target size will have following bits cleared (to provide block divisibility).</param>
        public Paged(ulong size, ulong clearLsb = 0x01) {
            ElementSize = Marshal.SizeOf(typeof(T));
            Length = size / (ulong)ElementSize;
            Length++;
            Length >>= PAGE_SIZE_LOG2;
            Length <<= PAGE_SIZE_LOG2;
            if (clearLsb > 0) Length &= ~clearLsb;
            Size = Length * (ulong)ElementSize;
            PageLength = 1 << PAGE_SIZE_LOG2;
            PageSize = PageLength * ElementSize;
            PageCount = (int)(Length >> PAGE_SIZE_LOG2);
            Pages = new T[PageCount][];
            OffsetMask = (ulong)(PageLength - 1);
            for (int i = 0; i < PageCount; i++) Pages[i] = new T[PageLength];
        }

        /// <summary>
        /// Returns specified page as byte buffer (slow, since the buffer is copied from page).
        /// </summary>
        /// <param name="i">Page index.</param>
        /// <param name="pageOffset">Optional page offset in bytes.</param>
        /// <param name="bufferSize">Optional buffer size, 0 means PageSize.</param>
        /// <returns></returns>
        public byte[] GetPageAsByteBuffer(int i, int pageOffset = 0, int bufferSize = 0) {
            if (bufferSize == 0) bufferSize = PageSize - pageOffset;
            var buffer = new byte[bufferSize];
            Buffer.BlockCopy(Pages[i], pageOffset, buffer, 0, bufferSize);
            return buffer;
        }

        /// <summary>
        /// Copies specified byte buffer to specified page (slow, since copying is involved).
        /// </summary>
        /// <param name="i">Page index.</param>
        /// <param name="buffer">Fixed length byte buffer.</param>
        /// <param name="bufferOffset">Optional buffer offset in bytes.</param>
        /// <param name="pageOffset">Optional page offset in bytes.</param>
        public void GetPageFromByteBuffer(int i, byte[] buffer, int bufferOffset = 0, int pageOffset = 0) => Buffer.BlockCopy(buffer, bufferOffset, Pages[i], pageOffset, buffer.Length - bufferOffset);

        /// <summary>
        /// Reads pages from stream, if the stream is larger than paged memory, a part of the stream will be read.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        public void Read(Stream stream) {
            for (int i = 0; i < PageCount; i++) {
                int read = stream.Read(GetPageAsByteBuffer(i), 0, PageSize);
                if (read < PageSize) break;
            }
        }

        /// <summary>
        /// Writes all paged memory to a stream.
        /// </summary>
        /// <param name="stream">Output stream.</param>
        /// <param name="size">Optional size in bytes, 0 means all pages</param>
        public void Write(Stream stream, ulong size = 0) {
            if (size == 0) for (int i = 0; i < PageCount; i++) stream.Write(GetPageAsByteBuffer(i), 0, PageSize);
            else {
                var pageCount = (int)(size / (ulong)PageSize);
                var lastPageSize = (int)(size - (ulong)pageCount * (ulong)PageSize);
                for (int i = 0; i < pageCount; i++) stream.Write(GetPageAsByteBuffer(i), 0, PageSize);
                stream.Write(GetPageAsByteBuffer(pageCount, 0, lastPageSize), 0, lastPageSize);
            }
        }

        /// <summary>
        /// Returns memory size in gigabytes (gibibytes)
        /// </summary>
        /// <returns></returns>
        public double ToGibi() => Size / (double)0x40000000;

        /// <summary>
        /// Returns string description of paged memory object
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"[{ToGibi():0.000}GB]";

    }

}