using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Woof.Core {

    /// <summary>
    /// Extensions for <see cref="SecureString"/> and some other classes making <see cref="SecureString"/> type way more usable, yet still secure.
    /// </summary>
    public static class SecureStringExtensions {

        /// <summary>
        /// Tests whether the secure strings are set to the same value. Case sensitive.
        /// </summary>
        /// <param name="thisSecureString">This secure string.</param>
        /// <param name="theOtherSecureString">The other secure string.</param>
        /// <returns>True if the strings are set to the same value.</returns>
        public static unsafe bool Equals(this SecureString thisSecureString, SecureString theOtherSecureString) {
            if (theOtherSecureString is null || thisSecureString.Length != theOtherSecureString.Length) return false;
            char* p1 = null, p2 = null;
            try {
                p1 = (char*)Marshal.SecureStringToGlobalAllocUnicode(thisSecureString);
                p2 = (char*)Marshal.SecureStringToGlobalAllocUnicode(theOtherSecureString);
                for (int i = 0, n = thisSecureString.Length; i < n; i++) {
                    if (*(p1 + i) != *(p2 + i)) return false;
                }
                return true;
            } finally {
                Marshal.ZeroFreeGlobalAllocUnicode((IntPtr)p1);
                Marshal.ZeroFreeGlobalAllocUnicode((IntPtr)p2);
            }
        }
        

    }

}