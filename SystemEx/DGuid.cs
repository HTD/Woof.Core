using System;
using System.Security.Cryptography;
using System.Text;

namespace Woof.SystemEx {

    /// <summary>
    /// Deterministic (namespaced) GUID compliant with <a href="https://tools.ietf.org/html/rfc4122">RFC 4122</a>.
    /// </summary>
    public class DGuid {

        /// <summary>
        /// Default namespace for this class of GUIDs.
        /// </summary>
        private static Guid DefaultNamespace = new Guid("3ae7c92c-f7a4-4750-8971-4ec7aae78638");

        /// <summary>
        /// Gets or sets actual namespace GUID.
        /// Override to create different types of GUIDs.
        /// </summary>
        protected virtual Guid Namespace { get; } = DefaultNamespace;

        /// <summary>
        /// Deterministic guid value.
        /// </summary>
        private Guid DG;

        /// <summary>
        /// Creates deterministic UUID v5 from any byte buffer.
        /// </summary>
        /// <param name="id">Initialization data.</param>
        public DGuid(byte[] id) {
            if (Namespace == Guid.Empty) Namespace = DefaultNamespace;
            var ns = Namespace.ToByteArray(); // name space bytes
            var dg = new byte[16]; // deterministic guid bytes (output)
            byte[] hash; // id hash
            ns = new byte[] {
                ns[0x3], ns[0x2], ns[0x1], ns[0x0], ns[0x5], ns[0x4], ns[0x7], ns[0x6],
                ns[0x8], ns[0x9], ns[0xa], ns[0xb], ns[0xc], ns[0xd], ns[0xe], ns[0xf]
            }; // https://tools.ietf.org/html/rfc4122#section-4.1.2
            using (var sha1 = SHA1.Create()) {
                sha1.TransformBlock(ns, 0, ns.Length, null, 0);
                sha1.TransformFinalBlock(id, 0, id.Length);
                hash = sha1.Hash;
            }
            Buffer.BlockCopy(hash, 0, dg, 0, 16);
            dg[6] = (byte)((dg[6] & 0x0f) | 0x50);
            dg[8] = (byte)((dg[8] & 0x3f) | 0x80);
            dg = new byte[] {
                dg[0x3], dg[0x2], dg[0x1], dg[0x0], dg[0x5], dg[0x4], dg[0x7], dg[0x6],
                dg[0x8], dg[0x9], dg[0xa], dg[0xb], dg[0xc], dg[0xd], dg[0xe], dg[0xf]
            };
            DG = new Guid(dg);
        }

        /// <summary>
        /// Creates deterministic UUID v5 from string.
        /// </summary>
        /// <param name="id">Initialization string.</param>
        public DGuid(string id) : this(Encoding.UTF8.GetBytes(id)) { }

        /// <summary>
        /// Returns DGuid as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => DG.ToString();
        
        /// <summary>
        /// Implicit DGuid to Guid conversion.
        /// </summary>
        /// <param name="dg">Deterministic UUID.</param>
        public static implicit operator Guid(DGuid dg) => dg.DG;
        
        /// <summary>
        /// Implicit DGuid to byte[] conversion.
        /// </summary>
        /// <param name="dg">Deterministic UUID.</param>
        public static implicit operator byte[] (DGuid dg) => dg.DG.ToByteArray();
        
        /// <summary>
        /// Implicit DGuid to string conversion.
        /// </summary>
        /// <param name="dg">Deterministic UUID.</param>
        public static implicit operator string(DGuid dg) => dg.DG.ToString();

    }

}