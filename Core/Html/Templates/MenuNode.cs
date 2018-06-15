using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Woof.Core.Html.Templates {

    /// <summary>
    /// Universal menu node for HTML controls.
    /// </summary>
    public class MenuNode {

        /// <summary>
        /// CSS class of the element.
        /// </summary>
        public string Class;

        /// <summary>
        /// HTML link or span for dropdowns.
        /// </summary>
        public XLink Value;

        /// <summary>
        /// Parent node.
        /// </summary>
        public MenuNode Parent;

        /// <summary>
        /// Subitems.
        /// </summary>
        public List<MenuNode> Children;

        /// <summary>
        /// Adds a new item with specified text and optional link.
        /// </summary>
        /// <param name="text">Item text.</param>
        /// <param name="href">Item URL.</param>
        /// <returns>New item.</returns>
        public MenuNode Add(string text, string href = null) {
            if (Children == null) Children = new List<MenuNode>();
            var node = new MenuNode() { Value = new XLink(text, href), Parent = this };
            Children.Add(node);
            return node;
        }

        /// <summary>
        /// Adds a new item with a link.
        /// </summary>
        /// <param name="link">HTML link.</param>
        /// <returns>New item.</returns>
        public MenuNode Add(XLink link) {
            if (Children == null) Children = new List<MenuNode>();
            var node = new MenuNode() { Value = link, Parent = this };
            Children.Add(node);
            return node;
        }

        /// <summary>
        /// Adds a new item with a new menu.
        /// </summary>
        /// <param name="node">Menu.</param>
        /// <returns>New menu.</returns>
        private MenuNode Add(MenuNode node) {
            if (Children == null) Children = new List<MenuNode>();
            node.Parent = this;
            Children.Add(node);
            return node;
        }

        /// <summary>
        /// Creates menu node from markdown list representation.
        /// </summary>
        /// <param name="markdown">Valid markdown for menu.</param>
        /// <returns>Menu.</returns>
        /// <remarks>
        /// "#" can be used as list header to indicate menu title.
        /// Only "-" list items are allowed.
        /// Elements outside allowed range or syntactically invalid will cause an exception.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown on syntax errors in markdown.</exception>
        public static MenuNode FromMarkdown(string markdown) {
            var items = MarkdownLine.Parse(markdown);
            var root = new MenuNode { Children = new List<MenuNode>() };
            MenuNode target = root;
            MenuNode current = null;
            MenuNode last = null;
            int l = items.First().Indentation, // current indentation level.
                s = 0; // indentation size.
            foreach (var lineItem in items) {
                current = new MenuNode { Value = new XLink(lineItem.Text, lineItem.Href) };
                if (lineItem.Indentation > l) {
                    if (s < 1) s = lineItem.Indentation - l;
                    l = lineItem.Indentation;
                    target = last;
                    target.Children = new List<MenuNode>();
                }
                else if (lineItem.Indentation < l) {
                    var sd = (l - lineItem.Indentation) / s;
                    l = lineItem.Indentation;
                    for (int j = 0; j < sd; j++) target = target.Parent;
                }
                if (lineItem.Type == '#') root.Value = current.Value;
                else {
                    current.Parent = target;
                    target.Children.Add(last = current);
                }
            }
            return root;
        }

        /// <summary>Flat markdown line lexem.</summary>
        private struct MarkdownLine {

            /// <summary>Indentation level.</summary>
            public int Indentation;

            /// <summary>Lexem type of 2 allowed: '#' and '-'.</summary>
            public char Type;

            /// <summary>Text content: all elements.</summary>
            public string Text;

            /// <summary>Href content: link elements.</summary>
            public string Href;

            /// <summary>Creates a lexem from valid markdown line.</summary>
            /// <param name="line">Valid markdown line.</param>
            /// <exception cref="ArgumentException">Thrown when line is not supported or valid markdown type.</exception>
            private MarkdownLine(string line) {
                Indentation = 0;
                Type = '\0';
                Text = null;
                Href = null;
                char c;
                for (int i = 0, g = 1, m = 0, n = line.Length; i < n; i++) {
                    // i: character index, g: ignore whitespace, m: state, n: character count.
                    c = line[i]; // current character.
                    switch (m) { // simple FSM parser.
                        case 0: // detect type.
                            if (c == ' ' || c == '\t') continue;
                            Indentation = i;
                            if (c == '#' || c == '-') {
                                Type = c;
                                Text = "";
                                m = 1;
                            }
                            else throw new ArgumentException("Unsupported markdown.", "line");
                            break;
                        case 1: // detect link text start.
                            if (g == 1 && (c == ' ' || c == '\t')) continue;
                            if (c == '[') { m = 2; continue; }
                            else { g = 0; Text += c; }
                            break;
                        case 2: // detect link text end.
                            if (c == ']') { m = 3; continue; }
                            else Text += c;
                            break;
                        case 3: // detect link href start.
                            if (c == ' ' || c == '\t') continue;
                            if (c == '(') { Href = ""; m = 4; continue; }
                            else throw new ArgumentException("Unsupported markdown.", "line");
                        case 4: // detect link href end.
                            if (c == ')') break;
                            else Href += c;
                            break;
                    }
                }
                if (Text != null) Text = Text.Trim();
                if (Href != null) Href = Href.Trim();
            }

            /// <summary>Parses a markdown fragment into lexeme collection.</summary>
            /// <param name="markdown">Valid markdown fragment.</param>
            /// <returns>Lexeme collection.</returns>
            /// <exception cref="ArgumentException">Thrown when line is not supported or valid markdown type.</exception>
            public static IEnumerable<MarkdownLine> Parse(string markdown) => Parse(RxCRLF.Split(markdown));

            /// <summary>Parses markdown lines into lexeme collection.</summary>
            /// <param name="lines">Lines containing markdown items.</param>
            /// <returns>Lexeme collection.</returns>
            /// <exception cref="ArgumentException">Thrown when line is not supported or valid markdown type.</exception>
            private static IEnumerable<MarkdownLine> Parse(IEnumerable<string> lines) {
                foreach (var line in lines) if (!String.IsNullOrWhiteSpace(line)) yield return new MarkdownLine(line);
            }

            /// <summary>Regular expression for splitting mixed line end type text into lines.</summary>
            static readonly Regex RxCRLF = new Regex(@"\r?\n", RegexOptions.Compiled);

        }

    }

}