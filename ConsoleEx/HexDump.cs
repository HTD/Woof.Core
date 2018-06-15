using System;
using System.IO;
using System.Text;

namespace Woof.ConsoleEx {

    /// <summary>
    /// A stream to output hexadecimal dump to <see cref="Console"/>.
    /// </summary>
    public class HexDump : Stream {

        /// <summary>
        /// Output formmats.
        /// </summary>
        public enum Formats {
            /// <summary>
            /// Plain text.
            /// </summary>
            Plain,
            /// <summary>
            /// Color formatting usable with HexColor console filter.
            /// </summary>
            HexColor
        };

        /// <summary>
        /// Gets or sets HexDump output format.
        /// DEFAULT: PlainText.
        /// </summary>
        public Formats Format {
            get => _Format; set {
                switch (_Format = value) {
                    case Formats.Plain:
                        FormatOffset = "{0:x8} : ";
                        FormatByte = "{0:x2} ";
                        FormatBlockSeparator = " ";
                        FormatTextSeparator = ": ";
                        FormatText = "{0}";
                        FormatNull = "   ";
                        break;
                    case Formats.HexColor:
                        FormatOffset = "{0:x8} `0ff`:` ";
                        FormatByte = "`077`{0:x2}` ";
                        FormatBlockSeparator = " ";
                        FormatTextSeparator = "`f00`:` ";
                        FormatText = "`444`{0}`";
                        FormatNull = "   ";
                        break;
                }
            }
        }

        /// <summary>
        /// Text encoding used to decode data.
        /// DEFAULT: ASCII.
        /// </summary>
        public Encoding TextEncoding { get; set; }

        /// <summary>
        /// Creates a new <see cref="HexDump"/> object.
        /// </summary>
        public HexDump() {
            Format = Formats.Plain;
            TextEncoding = Encoding.ASCII;
        }

        /// <summary>
        /// A string format for offset values.
        /// </summary>
        protected string FormatOffset;

        /// <summary>
        /// A string format for a single byte.
        /// </summary>
        protected string FormatByte;

        /// <summary>
        /// A string format for block separators.
        /// </summary>
        protected string FormatBlockSeparator;

        /// <summary>
        /// A string format for text separators.
        /// </summary>
        protected string FormatTextSeparator;

        /// <summary>
        /// A string format for text dump.
        /// </summary>
        protected string FormatText;

        /// <summary>
        /// A string format for null (empty) bytes.
        /// </summary>
        protected string FormatNull;

        /// <summary>
        /// This stream cannot be read.
        /// </summary>
        public override bool CanRead => false;

        /// <summary>
        /// This stream cannot be seeked.
        /// </summary>
        public override bool CanSeek => false;

        /// <summary>
        /// This stream can be written to.
        /// </summary>
        public override bool CanWrite => true;

        /// <summary>
        /// This stream does not return any length.
        /// </summary>
        public override long Length => -1;

        /// <summary>
        /// This stream doeasn't support setting position.
        /// </summary>
        public override long Position {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        /// <summary>
        /// This stream ignores <see cref="Flush"/> method.
        /// </summary>
        public override void Flush() { }

        /// <summary>
        /// This stream cannot be read.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();

        /// <summary>
        /// This stream cannot be seeked.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

        /// <summary>
        /// The lenght of this stream cannot be set.
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value) { }

        /// <summary>
        /// Outputs data as hexadecimal formatted dump.
        /// </summary>
        /// <param name="buffer">Buffer to dump.</param>
        /// <param name="offset">Optional offset (should be multiple of 16)</param>
        /// <param name="count">Number of bytes to dump.</param>
        public override void Write(byte[] buffer, int offset, int count) {
            if (buffer == null) return;
            for (int i = 0; i < count; i++, offset++) {
                if (offset % 16 == 0) {
                    if (i >= 16) {
                        Console.Write(FormatTextSeparator);
                        Console.Write(FormatText, TextDecode(buffer, offset - 16, 16));
                    }
                    if (i > 0) Console.WriteLine();
                    Console.Write(FormatOffset, offset);
                }
                else if (offset % 4 == 0) {
                    Console.Write(FormatBlockSeparator);
                }
                Console.Write(FormatByte, buffer[offset]);
            }
            var padding = 16 - (count % 16);
            if (padding == 16) padding = 0;
            for (
                int i = 0; i <= padding; i++, offset++) {
                if (offset % 16 == 0) {
                    Console.Write(FormatTextSeparator);
                    Console.Write(FormatText, TextDecode(buffer, offset - 16, 16 - padding));
                    Console.WriteLine();
                    if (i < padding) Console.Write(FormatOffset, offset);
                }
                else if (offset % 4 == 0) {
                    Console.Write(FormatBlockSeparator);
                }
                if (i < padding) Console.Write(FormatNull);
            }
        }

        /// <summary>
        /// Dumps entire buffer using colored output if available.
        /// </summary>
        /// <param name="buffer">Buffer with data to dump.</param>
        public static void WriteData(byte[] buffer) {
            using (var d = new HexDump() { Format = ConsoleEx.IsHexColorEnabled ? Formats.HexColor : Formats.Plain }) d.Write(buffer, 0, buffer?.Length ?? 0);
        }

        /// <summary>
        /// Dumps a specified buffer slice using colored output if available.
        /// </summary>
        /// <param name="buffer">Buffer with data to dump.</param>
        /// <param name="offset">First byte offset.</param>
        /// <param name="length">Dump length.</param>
        public static void WriteData(byte[] buffer, int offset, int length) {
            using (var d = new HexDump() { Format = ConsoleEx.IsHexColorEnabled ? Formats.HexColor : Formats.Plain }) d.Write(buffer, offset, length);
        }

        /// <summary>
        /// Dumps entire stream using colored output if available.
        /// </summary>
        /// <param name="stream">Stream to dump.</param>
        public static void WriteData(Stream stream) {
            using (var d = new HexDump() { Format = ConsoleEx.IsHexColorEnabled ? Formats.HexColor : Formats.Plain }) stream.CopyTo(d);
        }

        /// <summary>
        /// Dumps a specified portion of stream using colored output if available.
        /// </summary>
        /// <param name="stream">Stream to dump.</param>
        /// <param name="offset">Stream offset to seek.</param>
        /// <param name="length">Dump length.</param>
        public static void WriteData(Stream stream, int offset, int length) {
            using (var d = new HexDump() { Format = Formats.HexColor }) {
                var buffer = new byte[length];
                stream.Position = offset;
                stream.Read(buffer, 0, length);
                d.Write(buffer, 0, length);
            }
        }

        /// <summary>
        /// Decodes the text from bytes using current <see cref="TextEncoding"/>.
        /// </summary>
        /// <param name="buffer">Data buffer.</param>
        /// <param name="offset">Offset of the first character.</param>
        /// <param name="count">Number of bytes to decode.</param>
        /// <returns>Unicode string.</returns>
        private string TextDecode(byte[] buffer, int offset, int count) {
            var target = new byte[count];
            Buffer.BlockCopy(buffer, offset, target, 0, count);
            var result = TextEncoding.GetString(target);
            var filtered = "";
            for (int i = 0, n = result.Length; i < n; i++) {
                filtered += (result[i] < 0x20) ? '.' : result[i];
            }
            return Format == Formats.HexColor ? filtered.Replace("`", "``") : filtered;
        }

        /// <summary>
        /// Output format cache.
        /// </summary>
        Formats _Format;

    }

}