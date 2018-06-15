using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Woof.TextEx {

    /// <summary>
    /// RFC 4180 CSV splitter.
    /// </summary>
    public class CsvSplitter {

        /// <summary>
        /// Gets or sets cell delimiter.
        /// </summary>
        public char Delimiter { get; set; } = ',';

        /// <summary>
        /// Gets or sets cell delimiter quoting character.
        /// </summary>
        public char? Quote { get; set; } = '"';

        /// <summary>
        /// Splits the CSV row to separate cells.
        /// </summary>
        /// <param name="text">Row text.</param>
        /// <returns>Field text collection.</returns>
        public IEnumerable<string> GetCells(string text) {
            char[] trimChars = null;
            if (Quote == '\'') trimChars = new char[] { ' ', '\t', '\r', '\n', '"' };
            else if (Quote == '"') trimChars = new char[] { ' ', '\t', '\r', '\n', '\'' };
            else trimChars = new char[] { ' ', '\t', '\r', '\n', '\'', '"' };
            text = text.Trim();
            if (String.IsNullOrEmpty(text)) yield break;
            var cell = String.Empty;
            var isQuote = false;
            for (int i = 0, n = text.Length; i < n; i++) {
                var current = text[i];
                if (isQuote) {
                    if (current == Quote) {
                        char next = i + 1 < n ? text[i + 1] : '\0';
                        if (next == current) {
                            cell += current;
                            i++;
                        }
                        else isQuote = false;
                    }
                    else cell += current;
                }
                else {
                    if (Quote.HasValue && current == Quote) {
                        char next = i + 1 < n ? text[i + 1] : '\0';
                        if (next == current) {
                            cell += current;
                            i++;
                        }
                        else isQuote = true;
                    }
                    else if (current == Delimiter) {
                        yield return cell.Trim(trimChars);
                        cell = String.Empty;
                    }
                    else cell += current;
                }
            }
            if (cell.Length > 0) yield return cell.Trim(trimChars);
        }

        /// <summary>
        /// Splits the CSV string into separate lines considering line end characters can be quoted.
        /// </summary>
        /// <param name="text">CSV file content.</param>
        /// <returns>Row text collection.</returns>
        public IEnumerable<string> GetLines(string text) {
            text = text.Trim();
            if (String.IsNullOrEmpty(text)) yield break;
            var row = String.Empty;
            var isQuote = false;
            for (int i = 0, n = text.Length; i < n; i++) {
                var current = text[i];
                if (isQuote) {
                    if (current == Quote) {
                        char next = i + 1 < n ? text[i + 1] : '\0';
                        row += current;
                        if (next == current) i++;
                        else isQuote = false;
                    }
                    else row += current;
                }
                else {
                    if (Quote.HasValue && current == Quote) {
                        char next = i + 1 < n ? text[i + 1] : '\0';
                        row += current;
                        if (next == current) i++;
                        else isQuote = true;
                    }
                    else if (current == '\r') {
                        char next = i + 1 < n ? text[i + 1] : '\0';
                        if (next == '\n') i++;
                        yield return row.Trim();
                        row = String.Empty;
                    }
                    else if (current == '\n') {
                        yield return row.Trim();
                        row = String.Empty;
                    }
                    else row += current;
                }
            }
            if (row.Length > 0) yield return row.Trim();
        }

        /// <summary>
        /// Splits the CSV text into rows and fields.
        /// </summary>
        /// <param name="text">CSV file content.</param>
        /// <returns>A collection of field collections.</returns>
        public IEnumerable<string[]> Split(string text) => GetLines(text).Select(row => GetCells(row).ToArray());

    }

}