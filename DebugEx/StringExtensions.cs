namespace Woof.DebugEx {

    public static class StringExtensions {

        public static string WhitespaceVisible(this string text) => text.Replace('\r', '←').Replace('\n', '↓').Replace(' ', '·').Replace('\t', '→');

    }

}