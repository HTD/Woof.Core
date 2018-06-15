using System.IO;
using System.Linq;
using System.Reflection;
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
                using (var stream = AssemblyReference.GetManifestResourceStream(AssemblyPath))
                    return XDocument.Load(stream);
            }
        }

        /// <summary>
        /// Creates embedded resource reference.
        /// </summary>
        /// <param name="assembly">Assembly to load the resource from.</param>
        /// <param name="referencePath">Relative path to the project file.</param>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when embedded resource name could not be matched.</exception>
        /// <remarks>
        /// Only the last part of the path is matched, so specify the fullest patch which identifies the correct file.
        /// </remarks>
        public Resource(Assembly assembly, string referencePath) {
            AssemblyReference = assembly;
            ReferencePath = referencePath;
            var pattern = ReferencePath.Replace('/', '.').Replace('\\', '.');
            AssemblyPath = AssemblyReference.GetManifestResourceNames().FirstOrDefault(i => i.EndsWith(pattern));
            if (AssemblyPath == null) throw new FileNotFoundException("Embedded resource not found.", ReferencePath);
        }

        /// <summary>
        /// Creates embedded resource reference.
        /// </summary>
        /// <param name="referencePath">Relative path to the project file.</param>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when embedded resource name could not be matched.</exception>
        /// <remarks>
        /// Only the last part of the path is matched, so specify the fullest patch which identifies the correct file.
        /// </remarks>
        public Resource(string referencePath) : this(Assembly.GetEntryAssembly(), referencePath) { }

        #region Private data

        readonly Assembly AssemblyReference;
        readonly string AssemblyPath;
        readonly string ReferencePath;

        #endregion

    }

}