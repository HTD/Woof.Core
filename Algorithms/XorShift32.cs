﻿using System;

namespace Woof.Algorithms {

    /// <summary>
    /// Fast 32-bit pseudo-random number generator.
    /// </summary>
    /// <remarks>
    /// Optimized for speed on x64 and x86 systems.
    /// </remarks>
    public class XorShift32 : IPseudoRandomNumberGenerator {

        /// <summary>
        /// Creates and seeds PRNG.
        /// </summary>
        /// <param name="seed">32-bit seed value, leave 0 for random seed.</param>
        public XorShift32(int seed = 0) {
            Seed = unchecked((uint)seed);
            while (Seed == 0) Seed = unchecked((uint)DateTime.UtcNow.Ticks);
        }

        /// <summary>
        /// Returns a non-negative random integer.
        /// </summary>
        /// <returns>A 32-bit signed integer that is greater than or equal to 0 and less than System.Int32.MaxValue.</returns>
        public int Next() {
            Seed ^= Seed << S1;
            Seed ^= Seed >> S2;
            Seed ^= Seed << S3;
            return (int)(Seed & Int32.MaxValue);
        }

        /// <summary>
        /// Returns a non-negative random integer that is less than the specified maximum.
        /// </summary>
        /// <param name="max">The exclusive upper bound of the random number to be generated. maxValue must be greater than or equal to 0.</param>
        /// <returns>A 32-bit signed integer greater than or equal to minValue and less than maxValue; that is, the range of return values includes minValue but not maxValue. If minValue equals maxValue, minValue is returned.</returns>
        public int Next(int max) {
            if (max < 0) throw new ArgumentException();
            if (max < 1) return 0;
            Seed ^= Seed << S1;
            Seed ^= Seed >> S2;
            Seed ^= Seed << S3;
            return (int)(Seed / Max * max);
        }

        /// <summary>
        /// Returns a random integer that is within a specified range.
        /// </summary>
        /// <param name="min">The inclusive lower bound of the random number returned.</param>
        /// <param name="max">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        /// <returns>A 32-bit signed integer greater than or equal to minValue and less than maxValue; that is, the range of return values includes minValue but not maxValue. If minValue equals maxValue, minValue is returned.</returns>
        public int Next(int min, int max) {
            if (min == max) return min;
            Seed ^= Seed << S1;
            Seed ^= Seed >> S2;
            Seed ^= Seed << S3;
            if (min == int.MinValue && max == int.MaxValue) return unchecked((int)Seed);
            return min + (int)(Seed / Max * ((double)max - (double)min));
        }

        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to 0.0 and less than 1.0.
        /// </summary>
        /// <returns>A double-precision floating point number that is greater than or equal to 0.0 and less than 1.0.</returns>
        public double NextDouble() {
            Seed ^= Seed << S1;
            Seed ^= Seed >> S2;
            Seed ^= Seed << S3;
            var seed52 = ((long)Seed << 21) & DM | D1; // we use 32 of 52 bits of double fraction part
            return BitConverter.Int64BitsToDouble(seed52) - 1.0;
        }

        /// <summary>
        /// Returns a random boolean value.
        /// </summary>
        /// <returns>A random boolean value.</returns>
        public bool NextBool() {
            Seed ^= Seed << S1;
            Seed ^= Seed >> S2;
            Seed ^= Seed << S3;
            return (Seed & 1) != 0;
        }

        /// <summary>
        /// Returns next random byte.
        /// </summary>
        /// <returns>Random byte.</returns>
        public byte NextByte() {
            BCS = (BC++ % 4) << 3;
            if (BCS != 0) return (byte)(Seed >> BCS & 0xff);
            Seed ^= Seed << S1;
            Seed ^= Seed >> S2;
            Seed ^= Seed << S3;
            return (byte)(Seed & 0xff);
        }

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="buffer">An array of bytes to contain random numbers.</param>
        public void NextBytes(byte[] buffer) {
            int i = 0;
            int n = buffer.Length;
            int m = n & 0x7ffffffc;
            while (i < m) {
                Seed ^= Seed << S1;
                Seed ^= Seed >> S2;
                Seed ^= Seed << S3;
                buffer[i++] = (byte)(Seed >> 24 & 0xff);
                buffer[i++] = (byte)(Seed >> 16 & 0xff);
                buffer[i++] = (byte)(Seed >> 8 & 0xff);
                buffer[i++] = (byte)(Seed & 0xff);
            }
            for (; i < n; i++) {
                BCS = (i % 4) << 3;
                if (BCS != 0) { buffer[i] = (byte)(Seed >> BCS & 0xff); continue; }
                Seed ^= Seed << S1;
                Seed ^= Seed >> S2;
                Seed ^= Seed << S3;
                buffer[i] = (byte)(Seed & 0xff);
            }
        }

        #region Constants

        const int
            S1 = 13,
            S2 = 17,
            S3 = 5;
        const long
            D1 = 0b0_01111111111_0000000000000000000000000000000000000000000000000000, // binary for 1.0 (2 ** 0)
            DM = 0b0_00000000000_1111111111111111111111111111111111111111111111111111; // binary mask for fraction part (0.(9))
        const double Max = uint.MaxValue;

        #endregion

        #region Private data

        uint Seed;
        int BC;
        int BCS;

        #endregion

    }

}