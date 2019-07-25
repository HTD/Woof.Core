using System;
using System.IO;
using System.Linq;

namespace Woof.Core {

    /// <summary>
    /// Represents configured application directory.
    /// </summary>
    public class ApplicationDirectory {

        /// <summary>
        /// Creates configured application directory (object and the physical directory).
        /// </summary>
        /// <param name="directories">Absolute or relative path. Relative paths are always relative to exe file location.</param>
        public ApplicationDirectory(params string[] directories) {
            Directories = directories;
            if (!directories.Any()) {
                Absolute = Root;
            } else {
                if (Directories[0].Contains(':')) {
                    Absolute = Path.Combine(Directories);
                } else {
                    Absolute = Path.Combine(Directories.Prepend(Root).ToArray());
                }
            }
            var rootIndex = Absolute.IndexOf(Root, StringComparison.OrdinalIgnoreCase);
            if (rootIndex == 0) Relative = Absolute.Substring(Root.Length + 1);
            Directory.CreateDirectory(Absolute);
        }

        /// <summary>
        /// Gets the application root directory.
        /// </summary>
        private string Root { get; } = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// Gets the original string provided to the constructor.
        /// </summary>
        private string[] Directories { get; }

        /// <summary>
        /// Gets the absolute path to configured directory.
        /// </summary>
        public string Absolute { get; }

        /// <summary>
        /// Gets the relative path to configured directory.
        /// </summary>
        public string Relative { get; }

    }

}