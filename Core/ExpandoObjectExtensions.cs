using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Woof.Core {

    /// <summary>
    /// ExpandoObject extra methods.
    /// </summary>
    public static class ExpandoObjectExtensions {

        /// <summary>
        /// Tests if ExpandoObject has specified property set without throwing exception.
        /// </summary>
        /// <param name="obj">ExpandoObject.</param>
        /// <param name="propertyName">Property name.</param>
        /// <returns>True if property exists.</returns>
        public static bool HasProperty(ExpandoObject obj, string propertyName)
            => ((IDictionary<String, object>)obj).ContainsKey(propertyName);
    }

}