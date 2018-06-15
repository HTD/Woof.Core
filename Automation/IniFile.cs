using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace Woof.Automation {

    /// <summary>
    /// A minimalistic ini / conf file class.
    /// Allows storing string type settings organized in sections.
    /// </summary>
    public class IniFile {

        /// <summary>
        /// Gets the path to the configuration file.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Gets the sections dictionary.
        /// </summary>
        public Dictionary<string, NameValueCollection> Sections => _Sections;

        /// <summary>
        /// Gets the sections dictionary keys.
        /// </summary>
        public IEnumerable<string> SectionKeys => _Sections.Select(s => s.Key);

        /// <summary>
        /// Creates and reads a configuration file.
        /// </summary>
        /// <param name="filePath">Path to the configuration file.</param>
        public IniFile(string filePath) {
            FilePath = filePath;
            Read();
        }

        /// <summary>
        /// Reads the configuration if exists.
        /// </summary>
        public void Read() {
            if (File.Exists(FilePath)) Read(File.ReadAllLines(FilePath));
        }

        /// <summary>
        /// Reads the configuration from line array.
        /// </summary>
        /// <param name="lines">Configuration lines.</param>
        private void Read(string[] lines) {
            string sectionKey = "main";
            foreach (var line in lines.Select(i => i.Trim())) {
                if (String.IsNullOrEmpty(line) || line[0] == '#') continue;
                if (line.Length > 1 && line[0] == '[' && line[line.Length - 1] == ']') {
                    sectionKey = line.Substring(1, line.Length - 2).Trim();
                    
                }
                else if (line.IndexOf('=') >= 0) {
                    var split = line.Split(new[] { '=' }, 2);
                    var settingKey = split[0].Trim();
                    var value = split[1].Trim();
                    if (!_Sections.ContainsKey(sectionKey)) _Sections.Add(sectionKey, new NameValueCollection());
                    _Sections[sectionKey][settingKey] = value;
                }
            }
        }

        /// <summary>
        /// Writes the configuration.
        /// </summary>
        public void Write() {
            var dir = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllLines(FilePath, this);
        }

        /// <summary>
        /// Gets or sets the configuration section specified with the section key. Set null to remove a key.
        /// </summary>
        /// <param name="key">Section key.</param>
        /// <returns><see cref="NameValueCollection"/></returns>
        public NameValueCollection this[string key] {
            get => _Sections.ContainsKey(key) ? _Sections[key] : null;
            set {
                if (_Sections.ContainsKey(key)) {
                    if (value is NameValueCollection) _Sections[key] = value;
                    else _Sections.Remove(key);
                }
                else {
                    if (value is NameValueCollection) _Sections.Add(key, value);
                }
            }
        }

        /// <summary>
        /// Converts the configuration file sections to array of strings.
        /// </summary>
        /// <param name="sections">Configuration file instance.</param>
        public static implicit operator string[](IniFile sections) {
            var lines = new List<string>();
            foreach (var section in sections._Sections) {
                lines.Add($"[{section.Key}]");
                if (section.Value != null) {
                    foreach (string key in section.Value) lines.Add($"{key}={section.Value[key]}");
                }
            }
            return lines.ToArray();
        }

        /// <summary>
        /// A dictionary containing sections.
        /// </summary>
        private readonly Dictionary<string, NameValueCollection> _Sections = new Dictionary<string, NameValueCollection>(StringComparer.OrdinalIgnoreCase);

    }

}