using System;

namespace Woof.Algorithms {

    /// <summary>
    /// Fast 64-bit Xorshift Star generatr.
    /// </summary>
    public class XorShiftStar64 {

        /// <summary>
        /// Gets or sets generator seed. Assign 0 for random seed.
        /// </summary>
        public long Seed {
            get => State; set => State = value > 0 ? value : DateTime.Now.Ticks;
        }

        /// <summary>
        /// Creates a new instance of <see cref="XorShiftStar64"/> PRNG.
        /// </summary>
        /// <param name="seed">Seed value, leave zero for random seed.</param>
        public XorShiftStar64(long seed = 0) => Seed = seed;
        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="buffer">An array of bytes to contain random numbers.</param>
        unsafe public void NextBytes(byte[] buffer) {
            int n = buffer.Length;
            int m = n >> 3;
            fixed (void* p = buffer) {
                long* i64 = (long*)p;
                long* n64 = i64 + m;
                while (i64 < n64) {
                    State ^= State >> 12;
                    State ^= State << 25;
                    State ^= State >> 27;
                    *(i64++) = State *= 2685821657736338717;
                }
            }
            for (int i = m << 3; i < n; i++) {
                BCS = (i % 8) << 3;
                if (BCS != 0) { buffer[i] = (byte)(Seed >> BCS & 0xff); continue; }
                State ^= State >> 12;
                State ^= State << 25;
                State ^= State >> 27;
                State *= 2685821657736338717;
                buffer[i] = (byte)(Seed & 0xff);
            }

        }

        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to 0.0 and less than 1.0.
        /// </summary>
        /// <returns>A double-precision floating point number that is greater than or equal to 0.0 and less than 1.0.</returns>
        public double NextDouble() {
            State ^= State >> 12;
            State ^= State << 25;
            State ^= State >> 27;
            return (State *= 2685821657736338717) / Max;
        }

        #region Private data

        private const double Max = Int64.MaxValue;

        /// <summary>
        /// Internal state.
        /// </summary>
        private long State;

        /// <summary>
        /// Byte counter shift state.
        /// </summary>
        int BCS;

        #endregion

    }

}