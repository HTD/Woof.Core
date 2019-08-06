using System;

namespace Woof.Algorithms {

    /// <summary>
    /// System Random generator wrapped in <see cref="IPseudoRandomNumberGenerator"/> interface.
    /// </summary>
    public class SysRandom : Random, IPseudoRandomNumberGenerator {

        /// <summary>
        /// Returns a random boolean value.
        /// </summary>
        /// <returns>A random boolean value.</returns>
        public bool NextBool() => Next(0, 2) > 0;

        /// <summary>
        /// Returns next random byte.
        /// </summary>
        /// <returns>Random byte.</returns>
        public byte NextByte() => (byte)(Next() & 0xff);

    }

}