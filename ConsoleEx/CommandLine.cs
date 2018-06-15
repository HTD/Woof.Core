using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

namespace Woof.ConsoleEx {

    /// <summary>
    /// Command line arguments processing class.
    /// </summary>
    public class CommandLine {
        
        /// <summary>
        /// Gets a value indicating whether the command line is empty.
        /// </summary>
        public bool IsEmpty { get; }

        /// <summary>
        /// Gets a value indicating whether the command line contains any unprefixed parameters.
        /// </summary>
        public bool HasParameters { get; }

        /// <summary>
        /// Gets a value indicating whether the command lines contains prefixed options.
        /// </summary>
        public bool HasOptions { get; }

        /// <summary>
        /// Gets all parameters (arguments not prefixed with a switch).
        /// </summary>
        public string[] Parameters => ParameterCollection;

        /// <summary>
        /// Gets all options (arguments prefixed with a switch).
        /// </summary>
        public CommandLineArgumentCollection Options => OptionCollection;

        /// <summary>
        /// Parses command line arguments into parameters and options.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public CommandLine(string[] args) {
            IsEmpty = true;
            List<string> parameterList = null;
            foreach (var arg in args) {
                var match = RxElement.Match(arg);
                if (match.Success) {
                    IsEmpty = false;
                    var @switch = match.Groups[1].Value;
                    var bareText = match.Groups[2].Value;
                    var split = bareText.Split(new[] { '=' }, 2);
                    var key = split[0];
                    var value = split.Length > 1 ? split[1] : null;
                    if (@switch.Length < 1) { // parameters are arguments prefixed
                        if (parameterList == null) parameterList = new List<string>();
                        parameterList.Add(arg);
                        HasParameters = true;
                    }
                    else { // options are prefixed
                        OptionCollection[key] = value;
                        HasOptions = true;
                    }
                }
            }
            if (parameterList != null) ParameterCollection = parameterList.ToArray();

        }

        ///// <summary>
        ///// Returns true if command line contains specified parameter.
        ///// </summary>
        ///// <param name="value">Parameter name or names separated with "|".</param>
        ///// <returns></returns>
        //public bool HasParameter(string value) => ParameterCollection.ContainsKey(value);

        /// <summary>
        /// Returns true if command line contains specified option.
        /// </summary>
        /// <param name="value">Option name or names separated with "|".</param>
        /// <returns>True if command line contains the option specified.</returns>
        public bool HasOption(string value) => OptionCollection.ContainsKey(value);

        #region Private data

        /// <summary>
        /// Matches element with or without optional switch.
        /// </summary>
        private static readonly Regex RxElement = new Regex(@"^(/|-{0,2})(.*)$", RegexOptions.Compiled);

        /// <summary>
        /// Internal parameter collection.
        /// </summary>
        private readonly string[] ParameterCollection;

        /// <summary>
        /// Internal option collection.
        /// </summary>
        private readonly CommandLineArgumentCollection OptionCollection = new CommandLineArgumentCollection();

        #endregion

    }

    /// <summary>
    /// A command line argument collection with alias search capability.
    /// </summary>
    public class CommandLineArgumentCollection : NameValueCollection {

        /// <summary>
        /// Gets the key collection.
        /// </summary>
        public new IEnumerable<string> Keys => base.Keys.OfType<string>();

        /// <summary>
        /// Gets the value collection.
        /// </summary>
        public IEnumerable<string> Values => Keys.Select(k => base[k]);

        /// <summary>
        /// Gets or sets the entry with the specified key in the <see cref="CommandLineArgumentCollection"/>.
        /// </summary>
        /// <param name="name">Name of the key, or name aliases separated with '|' character.</param>
        /// <returns></returns>
        new string this[string name] {
            get {
                if (name.Contains('|')) {
                    var aliases = name.Split('|');
                    foreach (var alias in aliases) if (Keys.Contains(alias)) return base[alias];
                    return null;
                }
                return base[name];
            }
            set => base[name] = value;
        }

        /// <summary>
        /// Returns true if the collection contains specified key or one of its aliases.
        /// </summary>
        /// <param name="key">Name of the key, or name aliases separated with '|' character.</param>
        /// <returns></returns>
        public bool ContainsKey(string key) {
            if (key.Contains('|')) {
                var aliases = key.Split('|');
                foreach (var alias in aliases) if (Keys.Contains(alias)) return true;
                return false;
            }
            return Keys.Contains(key);
        }

    }

}