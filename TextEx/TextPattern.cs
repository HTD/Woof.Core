using System;
using System.Collections.Generic;
using System.Linq;

namespace Woof.TextEx {

    /// <summary>
    /// Simple text match and replace class behaving similar to SQL LIKE.
    /// </summary>
    public class TextPattern {

        /// <summary>
        /// Wildcard character.
        /// </summary>
        public static char Wildcard = '%';

        /// <summary>
        /// Gets the pattern string.
        /// </summary>
        public string Pattern { get; }

        /// <summary>
        /// Gets the one of the enumeration values that determines how this string and value are compared.
        /// </summary>
        public StringComparison ComparisonType { get; }

        /// <summary>
        /// Creates text matching pattern similar to SQL LIKE.
        /// </summary>
        /// <param name="pattern">Pattern string.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how this string and value are compared.</param>
        public TextPattern(string pattern, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase) {
            if (String.IsNullOrEmpty(pattern)) throw new ArgumentException("Pattern cannot be empty.");
            Pattern = pattern;
            ComparisonType = comparisonType;
            Compiled = Compile(Pattern).ToArray();
            FixedParts = Compiled.Where(i => i.IsFixed).Select(i => i.Value).ToArray();
        }

        /// <summary>
        /// Returns true if the text matches the pattern.
        /// </summary>
        /// <param name="text">Text to be matched with the pattern.</param>
        /// <returns>True if the text matches the pattern.</returns>
        public bool IsMatch(string text) {
            Positions = FixedParts.Select(e => text.IndexOf(e, ComparisonType)).ToArray();
            if (Positions.Any(i => i < 0)) return false;
            if (Compiled.First().IsFixed && !text.StartsWith(FixedParts[0], ComparisonType)) return false;
            if (Compiled.Last().IsFixed && !text.EndsWith(FixedParts[FixedParts.Length - 1], ComparisonType)) return false;
            for (int i = 0, l = -1, n = Positions.Length; i < n; i++) {
                if (Positions[i] <= l) return false;
                l = Positions[i];
            }
            return true;
        }

        /// <summary>
        /// Matches the text against the pattern and returns <see cref="PatternMatch"/> elements.
        /// </summary>
        /// <param name="text">Text to be matched with the pattern.</param>
        /// <returns>A collection of <see cref="PatternMatch"/> elements.</returns>
        public IEnumerable<PatternMatch> Matches(string text) {
            if (!IsMatch(text)) yield break;
            var markers = new List<int> { 0 };
            for (int i = 0, n = Positions.Length; i < n; i++) {
                if (Positions[i] > 0)
                    markers.Add(Positions[i]);
                if (Positions[i] + FixedParts[i].Length < text.Length)
                    markers.Add(Positions[i] + FixedParts[i].Length);
            }
            markers.Add(text.Length);
            var isFixed = Positions[0] == 0 ? 1 : 0;
            for (int i = 1, n = markers.Count; i < n; i++, isFixed ^= 1) {
                var start = markers[i - 1];
                var end = markers[i];
                yield return new PatternMatch(text.Substring(start, end - start), isFixed > 0);
            }
        }

        /// <summary>
        /// Returns matched text between fixed elements.
        /// </summary>
        /// <param name="text">Text to be matched with the pattern.</param>
        /// <returns>Text matched or null if not matched.</returns>
        public string Match(string text) {
            var matches = Matches(text).ToArray();
            if (matches.Length < 1) return null;
            var elements = new List<string>(matches.Length);
            for (int i = 0, s = 0, n = matches.Length; i < n; i++) {
                if (matches[i].IsFixed) s = 1;
                if (s > 0 && (i < n - 1 || matches[i].IsFixed)) elements.Add(matches[i].Value);
            }
            return String.Join("", elements);
        }

        /// <summary>
        /// Replaces the text using this pattern and replacement pattern.
        /// </summary>
        /// <param name="text">Text to make replacements in.</param>
        /// <param name="pattern">Replacements pattern.</param>
        /// <returns>Text with the replacements made.</returns>
        public string Replace(string text, string pattern) {
            if (!IsMatch(text)) return text;
            var replacements = Compile(pattern).Where(e => e.IsFixed).Select(e => e.Value).ToArray();
            if (replacements.Length != FixedParts.Length) return text;
            var i = 0;
            return String.Join("", Matches(text).Select(e => e.IsFixed ? replacements[i++] : e.Value));
        }

        /// <summary>
        /// Compile the pattern into <see cref="PatternMatch"/> collection.
        /// </summary>
        /// <param name="pattern">Text pattern to compile.</param>
        /// <returns>Compiled collection.</returns>
        private IEnumerable<PatternMatch> Compile(string pattern) {
            int i = 0, s = 0, n = pattern.Length;
            for (; i < n; i++) {
                if (pattern[i] == Wildcard) {
                    if (i > s) yield return new PatternMatch(pattern.Substring(s, i - s), true);
                    s = i + 1;
                    yield return new PatternMatch(Wildcard);
                }
            }
            if (i > s) yield return new PatternMatch(pattern.Substring(s, i - s), true);
        }

        /// <summary>
        /// Compiled pattern matches.
        /// </summary>
        private PatternMatch[] Compiled;

        /// <summary>
        /// Extracted pattern exact literals.
        /// </summary>
        private string[] FixedParts;

        /// <summary>
        /// Fixed parts positions in the pattern.
        /// </summary>
        private int[] Positions;

    }

    /// <summary>
    /// Represents a text pattern match.
    /// </summary>
    public struct PatternMatch {

        /// <summary>
        /// The part is contained explicitely in the pattern.
        /// </summary>
        public readonly bool IsFixed;

        /// <summary>
        /// Match text value.
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// Creates a wildcard type dummy match.
        /// </summary>
        /// <param name="wildcard">Wildcard character.</param>
        public PatternMatch(char wildcard) {
            IsFixed = false;
            Value = wildcard.ToString();
        }

        /// <summary>
        /// Creates a pattern match from a substring.
        /// </summary>
        /// <param name="value">Substring matched.</param>
        /// <param name="isFixed">Is explicit match with the pattern.</param>
        public PatternMatch(string value, bool isFixed) {
            IsFixed = isFixed;
            Value = value;
        }

    }

}