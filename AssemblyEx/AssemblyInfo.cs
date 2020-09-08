using System;
using System.IO;
using System.Reflection;

namespace Woof.AssemblyEx {

    /// <summary>
    /// Quick access to current project's assebly information
    /// </summary>
    public class AssemblyInfo {

        /// <summary>
        /// Creates new <see cref="AssemblyInfo"/> instance.
        /// </summary>
        /// <param name="assembly">Assembly, entry assembly will be used if not specified.</param>
        public AssemblyInfo(Assembly assembly = null) => A = assembly ?? Assembly.GetEntryAssembly();
        #region Properties

        /// <summary>
        /// Gets the <see cref="AssemblyInfo"/> instance for the main executable (EXE, not DLL).
        /// </summary>
        public static AssemblyInfo Entry => new AssemblyInfo(Assembly.GetEntryAssembly());

        /// <summary>
        /// Gets the <see cref="AssemblyInfo"/> instance for the current module (DLL if called from a library or EXE if called from main).
        /// </summary>
        public static AssemblyInfo Executing => new AssemblyInfo(Assembly.GetExecutingAssembly());

        /// <summary>
        /// Gets the assembly path.
        /// </summary>
        public string Path => A.Location; // System.IO.Path.GetFullPath(Uri.UnescapeDataString(new UriBuilder(A.CodeBase).Path));

        /// <summary>
        /// Gets the assembly directory.
        /// </summary>
        public string Directory => System.IO.Path.GetDirectoryName(A.Location); //System.IO.Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(A.CodeBase).Path));

        /// <summary>
        /// Name (suitable for event source)
        /// </summary>
        public string Name => _Name ??= A.GetName().Name;

        /// <summary>
        /// Title (suitable for displayed service name)
        /// </summary>
        public string Title => _Title ??= (A.GetCustomAttribute<AssemblyTitleAttribute>()?.Title);

        /// <summary>
        /// Description (suitable for service description)
        /// </summary>
        public string Description => _Description ??= (A.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description);

        /// <summary>
        /// Company name
        /// </summary>
        public string Company => _Company ??= (A.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company);

        /// <summary>
        /// Internal product name (suitable as service name identifier)
        /// </summary>
        public string Product => _Product ??= (A.GetCustomAttribute<AssemblyProductAttribute>()?.Product);

        /// <summary>
        /// Copyright information
        /// </summary>
        public string Copyright => _Copyright ??= (A.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright);

        /// <summary>
        /// Trademark
        /// </summary>
        public string Trademark => _Trademark ??= (A.GetCustomAttribute<AssemblyTrademarkAttribute>()?.Trademark);

        /// <summary>
        /// Version
        /// </summary>
        public Version Version => _Version ??= A.GetName().Version;

        /// <summary>
        /// Main program namespace
        /// </summary>
        public string Namespace => _Namespace ??= A.EntryPoint.ReflectedType.Namespace;

        #endregion

        /// <summary>
        /// Returns a stream from embedded resource
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Stream Stream(string fileName) => A.GetManifestResourceStream($"{Namespace}.{fileName}");

        /// <summary>
        /// Returns a text read from embedded resource
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public string Text(string fileName) {
            using (var s = A.GetManifestResourceStream($"{Namespace}.{fileName}"))
                if (s != null) using (var r = new StreamReader(s)) return r.ReadToEnd();
            return null;
        }

        #region Private data

        /// <summary>
        /// Target assembly
        /// </summary>
        private readonly Assembly A;

        private string _Name;
        private string _Title;
        private string _Description;
        private string _Company;
        private string _Product;
        private string _Copyright;
        private string _Trademark;
        private Version _Version;
        private string _Namespace;

        #endregion

    }

}