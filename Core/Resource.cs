using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Woof.Core {

    /// <summary>
    /// Embedded resource reader.
    /// </summary>
    public class Resource {

        /// <summary>
        /// Gets embedded resource as stream.
        /// </summary>
        public Stream Stream => AssemblyReference.GetManifestResourceStream(AssemblyPath);

        /// <summary>
        /// Gets embedded resource as text.
        /// </summary>
        public string Text {
            get {
                string value;
                using (var stream = AssemblyReference.GetManifestResourceStream(AssemblyPath))
                using (var reader = new StreamReader(stream)) value = reader.ReadToEnd();
                return value;
            }
        }

        /// <summary>
        /// Gets embedded resource as document.
        /// </summary>
        public XDocument Document {
            get {
                using var stream = AssemblyReference.GetManifestResourceStream(AssemblyPath);
                return XDocument.Load(stream);
            }
        }

        /// <summary>
        /// Creates embedded resource reference.
        /// </summary>
        /// <param name="assembly">Assembly to load the resource from.</param>
        /// <param name="path">Relative path to the project file.</param>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when embedded resource name could not be matched.</exception>
        /// <remarks>
        /// Only the last part of the path is matched, so specify the fullest patch which identifies the correct file.
        /// </remarks>
        public Resource(Assembly assembly, string path) {
            AssemblyReference = assembly;
            ReferencePath = path;
            AssemblyPath = GetAssemblyPath(AssemblyReference, ReferencePath);
            if (AssemblyPath == null) throw new FileNotFoundException("Embedded resource not found.", ReferencePath);
        }

        /// <summary>
        /// Creates embedded resource reference.
        /// </summary>
        /// <param name="path">Relative path to the project file.</param>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when embedded resource name could not be matched.</exception>
        /// <remarks>
        /// Only the last part of the path is matched, so specify the fullest patch which identifies the correct file.
        /// </remarks>
        public Resource(string path) : this(Assembly.GetEntryAssembly(), path) { }

        /// <summary>
        /// Enumerates all resource paths matching the specified pattern within given assembly.
        /// </summary>
        /// <param name="assembly">Assembly to test.</param>
        /// <param name="pattern">Wildcard pattern.</param>
        /// <returns>A collection of matching paths.</returns>
        public static IEnumerable<string> Enumerate(Assembly assembly, string pattern = "*") {
            if (pattern == "*") return assembly.GetManifestResourceNames();
            var regex = new Regex(Regex.Escape(pattern.Replace('/', '.').Replace('\\', '.')).Replace("\\?", ".").Replace("\\*", ".*") + "$");
            return assembly.GetManifestResourceNames().Where(i => regex.IsMatch(i));
        }

        /// <summary>
        /// Enumerates all resource paths matching the specified pattern within entry assembly (usually main exe).
        /// </summary>
        /// <param name="pattern">Wildcard pattern.</param>
        /// <returns>A collection of matching paths.</returns>
        public static IEnumerable<string> Enumerate(string pattern = "*") => Enumerate(Assembly.GetEntryAssembly(), pattern);

        /// <summary>
        /// Checks if a resource specified with the path exists within given assembly.
        /// </summary>
        /// <param name="assembly">Assembly to test.</param>
        /// <param name="path">Relative path to the project file.</param>
        /// <returns>True if the resource exists.</returns>
        public static bool Exists(Assembly assembly, string path) => GetAssemblyPath(assembly, path) != null;

        /// <summary>
        /// Checks if a resource specified with the path exists within entry assembly (usually main exe).
        /// </summary>
        /// <param name="path">Relative path to the project file.</param>
        /// <returns>True if the resource exists.</returns>
        public static bool Exists(string path) => Exists(Assembly.GetEntryAssembly(), path);

        /// <summary>
        /// Gets the path to the resource stream.
        /// </summary>
        /// <param name="assembly">Assembly to test.</param>
        /// <param name="path">Relative path to the project file.</param>
        /// <returns>Assembly path.</returns>
        private static string GetAssemblyPath(Assembly assembly, string path) {
            var pattern = path.Replace('/', '.').Replace('\\', '.');
            return assembly.GetManifestResourceNames().FirstOrDefault(i => i.EndsWith(pattern));
        }

        #region Private data

        readonly Assembly AssemblyReference;
        readonly string AssemblyPath;
        readonly string ReferencePath;

        #endregion

    }

}