using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Woof.SystemEx {

    /// <summary>
    /// A class for storing command line arguments collection and converting them to a valid Windows command line string.
    /// </summary>
    public sealed class CommandLineArguments : IEnumerable<string> {

        /// <summary>
        /// Returs arguments collection length.
        /// </summary>
        public int Length => Items?.Length ?? 0;

        /// <summary>
        /// Returns argument value specified with its index.
        /// </summary>
        /// <param name="i">Zero based collection index.</param>
        /// <returns>Argument value.</returns>
        public string this[int i] => i < Length ? Items[i] : null;

        /// <summary>
        /// Creates new command line arguments collection.
        /// </summary>
        /// <param name="arguments">Unquoted arguments.</param>
        public CommandLineArguments(params string[] arguments) => Items = arguments;

        /// <summary>
        /// Serializes command line arguments collection with necessary character quoting.
        /// </summary>
        /// <returns>Serialized command line arguments string.</returns>
        public override string ToString() {
            if (Items == null) return null;
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < Items.Length; i++) {
                if (i > 0) b.Append(' ');
                AppendArgument(b, Items[i]);
            }
            return b.ToString();
        }

        /// <summary>
        /// Performs implict <see cref="ToString"/> conversion.
        /// </summary>
        /// <param name="args">Arguments object.</param>
        public static implicit operator string(CommandLineArguments args) => args.ToString();

        /// <summary>
        /// Quotes argument string and appends it to specified <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="b"><see cref="StringBuilder"/> object the quoted argument will be appended to.</param>
        /// <param name="arg">Unquoted argument value.</param>
        private void AppendArgument(StringBuilder b, string arg) {
            if (arg.Length > 0 && arg.IndexOfAny(ArgQuoteChars) < 0) {
                b.Append(arg);
            }
            else {
                b.Append('"');
                for (int j = 0; ; j++) {
                    int backslashCount = 0;
                    while (j < arg.Length && arg[j] == '\\') {
                        backslashCount++;
                        j++;
                    }
                    if (j == arg.Length) {
                        b.Append('\\', backslashCount * 2);
                        break;
                    }
                    else if (arg[j] == '"') {
                        b.Append('\\', backslashCount * 2 + 1);
                        b.Append('"');
                    }
                    else {
                        b.Append('\\', backslashCount);
                        b.Append(arg[j]);
                    }
                }
                b.Append('"');
            }
        }

        /// <summary>
        /// Enumerates items.
        /// </summary>
        /// <returns>Generic enumerator.</returns>
        public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)Items).GetEnumerator();

        /// <summary>
        /// Enumerates items.
        /// </summary>
        /// <returns>Non-generic enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<string>)Items).GetEnumerator();

        /// <summary>
        /// Characters which must be quoted in argument strings.
        /// </summary>
        private readonly char[] ArgQuoteChars = { ' ', '\t', '\n', '\v', '"' };

        /// <summary>
        /// Argument values.
        /// </summary>
        private readonly string[] Items;

    }

}