using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Woof.TextEx {

    public static class StringExtensions {

        /// <summary>
        /// Returns the byte buffer as formatted string.
        /// </summary>
        /// <param name="data">Byte buffer.</param>
        /// <returns>
        /// <list type="table">
        /// <term>empty</term><description>[]</description>
        /// <term>all printable</term><description>"text"</description>
        /// <term>contains non-printable</term><description>[hex]</description>
        /// </list>
        /// </returns>
        public static string AsHexOrASCII(this byte[] data) {
            if (data == null || data.Length < 1) return "[]";
            int printable = 0;
            for (int i = 0, n = data.Length; i < n; i++) if (data[i] >= 32 && data[i] <= 126) printable++;
            if (printable == data.Length) return $"\"{Encoding.ASCII.GetString(data)}\"";
            return $"[0x{String.Join("", data.Select(i => i.ToString("x2")))}]";
        }

        /// <summary>
        /// Returns true if a wildcard character defined in <see cref="TextPattern"/> class exists in this string.
        /// </summary>
        /// <param name="text">Source text.</param>
        /// <returns>True if wildcard character found.</returns>
        public static bool ContainsWildcard(this string text) => text.IndexOf(TextPattern.Wildcard) >= 0;

        /// <summary>
        /// Returns true if the text matches the wildcard pattern specified.
        /// </summary>
        /// <param name="text">Source text.</param>
        /// <param name="pattern">Wildcard pattern.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>True if specified pattern is found.</returns>
        public static bool MatchesPattern(this string text, string pattern, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
            => pattern.ContainsWildcard() ? new TextPattern(pattern, comparisonType).IsMatch(text) : text.Equals(pattern, comparisonType);

        /// <summary>
        /// Replaces special XML / HTML characters with entities, with optional HTML replacements.
        /// </summary>
        /// <param name="text">Text containing special HTML characters or meaningful whitespace.</param>
        /// <returns>XML / HTML text.</returns>
        public static string ParseXml(this string text, HtmlParsingFlags options = HtmlParsingFlags.None) {
            if (text == null) return null;
            var result = text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
            if (options.HasFlag(HtmlParsingFlags.ConvertLineEndsToBrTags)) result = RxLineEnds.Replace(result, "<br />\r\n");
            if (options.HasFlag(HtmlParsingFlags.ConvertWhitespace)) result = result.Replace("  ", " &nbsp;").Replace("\t", " &nbsp; &nbsp;");
            return result;
        }

        /// <summary>
        /// Matches a pattern withing the string using <see cref="TextPattern"/> class.
        /// </summary>
        /// <param name="text">Source text.</param>
        /// <param name="pattern">Search pattern with wildcards.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>Matched fragment.</returns>
        public static string PatternMatch(this string text, string pattern, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
            => new TextPattern(pattern, comparisonType).Match(text);

        /// <summary>
        /// Matches pattern containing '%' character as wildcard.
        /// </summary>
        /// <param name="text">Source text.</param>
        /// <param name="searchPattern">Search pattern using '%' wildcard.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>Replaced text.</returns>
        public static string PatternReplace(this string text, string searchPattern, string replacePattern, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
            => new TextPattern(searchPattern, comparisonType).Replace(text, replacePattern);

        /// <summary>
        /// Returns a value indicating whether a specified substring occurs within this string.
        /// </summary>
        /// <param name="text">Source text.</param>
        /// <param name="substring">The string to seek.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns></returns>
        public static bool ContainsFragment(this string text, string substring, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
            => text.IndexOf(substring, comparisonType) >= 0;

        /// <summary>
        /// Returns a new string in which all occurrences of a specified string in the current
        /// instance are replaced with another specified string.
        /// </summary>
        /// <param name="text">This string.</param>
        /// <param name="search">The string to be replaced.</param>
        /// <param name="replacement">The string to replace all occurrences of search.</param>
        /// <param name="comparison">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>
        /// A string that is equivalent to the current string except that all instances of
        /// oldValue are replaced with newValue. If oldValue is not found in the current
        /// instance, the method returns the current instance unchanged.
        /// </returns>
        public static string Replace(this string text, string search, string replacement, StringComparison comparison) {
            int index = 0, length1 = search.Length, length2 = replacement.Length;
            while ((index = text.IndexOf(search, index, comparison)) >= 0) {
                text = text.Remove(index, length1).Insert(index, replacement);
                index += length2;
            }
            return text;
        }

        private static readonly Regex RxLineEnds = new Regex(@"\r?\n", RegexOptions.Compiled | RegexOptions.Singleline);

    }

    /// <summary>
    /// HTML parsing flags.
    /// </summary>
    [Flags]
    public enum HtmlParsingFlags {
        /// <summary>
        /// Just entity conversion, no whitespace conversion.
        /// </summary>
        None = 0,
        /// <summary>
        /// All line ends will be converted to BR tags.
        /// </summary>
        ConvertLineEndsToBrTags = 1,
        /// <summary>
        /// All horizontal whitespace will be converted to contain non-breaking space every second character.
        /// </summary>
        ConvertWhitespace = 2
    }

}