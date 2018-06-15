﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;

namespace Woof.Automation {

    /// <summary>
    /// Tools related to system environment paths.
    /// </summary>
    public static class PathTools {

        #region Current process environment

        /// <summary>
        /// Gets the automatic <see cref="EnvironmentVariableTarget"/> depending on whether current user has administrative privileges.
        /// </summary>
        public static EnvironmentVariableTarget AutoTarget {
            get {
                using (var identity = WindowsIdentity.GetCurrent())
                    return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator)
                        ? EnvironmentVariableTarget.Machine
                        : EnvironmentVariableTarget.User;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the "Program Files" folder will be x86 folder.
        /// (True for programs compiled with "Prefer 32-bit" option set).
        /// </summary>
        public static bool IsX86Target => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles).Contains("x86");

        /// <summary>
        /// Gets program files folder depending on target type (machine / user).
        /// </summary>
        /// <param name="target">Environment location.</param>
        /// <returns>Hopefully writeable directory to store new programs in.</returns>
        public static string GetProgramFilesDirectory(EnvironmentVariableTarget target) =>
            target == EnvironmentVariableTarget.Machine
                ? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs");

        #endregion

        #region Environment PATH manipulation

        /// <summary>
        /// Gets current environment PATH as string collection.
        /// </summary>
        /// <param name="target">Environment location.</param>
        /// <returns>Directory string collection.</returns>
        public static IEnumerable<string> GetPath(EnvironmentVariableTarget target = EnvironmentVariableTarget.User)
            => Environment.GetEnvironmentVariable(PATH, target).Split(PATH_SEPARATOR).Select(i => i.Trim());

        /// <summary>
        /// Sets specified environment PATH to specified directory string collection.
        /// </summary>
        /// <param name="collection">Directory path collection.</param>
        /// <param name="target">Environment location.</param>
        public static void SetPath(IEnumerable<string> collection, EnvironmentVariableTarget target = EnvironmentVariableTarget.User)
            => Environment.SetEnvironmentVariable(PATH, String.Join(PATH_SEPARATOR.ToString(), collection), target);

        /// <summary>
        /// Checks whether specified path exists in current environment PATH.
        /// </summary>
        /// <param name="path">Path to test.</param>
        /// <param name="target">Environment location.</param>
        /// <returns>True if exits.</returns>
        public static bool IsInPath(string path, EnvironmentVariableTarget target = EnvironmentVariableTarget.User)
            => GetPath(target).Contains(path, StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Adds specified path to the specified environment PATH if not exists.
        /// </summary>
        /// <param name="path">Path to add.</param>
        /// <param name="target">Environment location.</param>
        public static void AddToPath(string path, EnvironmentVariableTarget target = EnvironmentVariableTarget.User) {
            var currentPath = new List<string>(GetPath(target));
            if (currentPath.Contains(path, ICCR)) return;
            currentPath.Add(path);
            SetPath(currentPath, target);
        }

        /// <summary>
        /// Removes specified path from the specified environment PATH if exists.
        /// </summary>
        /// <param name="path">Path to remove.</param>
        /// <param name="target">Environment location.</param>
        public static void RemoveFromPath(string path, EnvironmentVariableTarget target = EnvironmentVariableTarget.User) {
            var currentPath = new List<string>(GetPath(target));
            if (!currentPath.Contains(path, ICCR)) return;
            var index =
                currentPath
                .Select((s, i) => new { Path = s, Index = i })
                .FirstOrDefault(i => i.Path.Equals(path, ICCN))
                ?.Index ?? -1;
            if (index >= 0) currentPath.RemoveAt(index);
            SetPath(currentPath, target);
        }

        /// <summary>
        /// Returns a full path to the file if it's accessible from the user or system environment PATH variable.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <returns>Full path to the file or null if the file doesn't exist.</returns>
        public static string GetFullPath(string fileName) =>
            fileName == null ? null : (
                GetPath(EnvironmentVariableTarget.User).Select(i => Path.Combine(i, fileName)).FirstOrDefault(i => File.Exists(i))
                    ??
                GetPath(EnvironmentVariableTarget.Machine).Select(i => Path.Combine(i, fileName)).FirstOrDefault(i => File.Exists(i))
                    ??
                GetPath(EnvironmentVariableTarget.User).Select(i => Path.Combine(i, $"{fileName}.exe")).FirstOrDefault(i => File.Exists(i))
                    ??
                GetPath(EnvironmentVariableTarget.Machine).Select(i => Path.Combine(i, $"{fileName}.exe")).FirstOrDefault(i => File.Exists(i))
            );

        /// <summary>
        /// Returns a value indicating whether the specified file is accessible from the user or system environment PATH variable.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <returns>True if the file is accessible.</returns>
        public static bool IsFileAcessibleInPath(string fileName) =>
            fileName == null ? false : (
                GetPath(EnvironmentVariableTarget.User).Select(i => Path.Combine(i, fileName)).Any(i => File.Exists(i))
                    ||
                GetPath(EnvironmentVariableTarget.Machine).Select(i => Path.Combine(i, fileName)).Any(i => File.Exists(i))
                    ||
                GetPath(EnvironmentVariableTarget.User).Select(i => Path.Combine(i, $"{fileName}.exe")).Any(i => File.Exists(i))
                    ||
                GetPath(EnvironmentVariableTarget.Machine).Select(i => Path.Combine(i, $"{fileName}.exe")).Any(i => File.Exists(i))
            );

        #endregion

        #region Configuration

        private const string PATH = "PATH";
        private const StringComparison ICCN = StringComparison.OrdinalIgnoreCase;
        private static readonly StringComparer ICCR = StringComparer.OrdinalIgnoreCase;
        private static readonly char PATH_SEPARATOR = System.IO.Path.PathSeparator;

        #endregion

    }

}