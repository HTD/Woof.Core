using System;

namespace Woof.Algorithms {

    /// <summary>
    /// Box-Muller transform implementation on <see cref="System.Random"/> and <see cref="IPseudoRandomNumberGenerator"/>.
    /// </summary>
    public static class BoxMuller {

        /// <summary>
        /// Returns random numbers form (0,1) range with normal Gaussian distribution.
        /// </summary>
        /// <param name="prng">Pseudo random number generator.</param>
        /// <returns>Value from 0 to 1.</returns>
        public static double NextGaussian(this IPseudoRandomNumberGenerator prng) => Calculate(prng.NextDouble(), prng.NextDouble());

        /// <summary>
        /// Returns random numbers form (0,1) range with normal Gaussian distribution.
        /// </summary>
        /// <param name="prng">Pseudo random number generator.</param>
        /// <returns>Value from 0 to 1.</returns>
        public static double NextGaussian(this Random prng) => Calculate(prng.NextDouble(), prng.NextDouble());

        /// <summary>
        /// Calculates normal Gaussian distribution.
        /// </summary>
        /// <param name="u1">Uniform value 1.</param>
        /// <param name="u2">Uniform value 2.</param>
        /// <returns>Result.</returns>
        private static double Calculate(double u1, double u2)
            => Math.Sqrt(-2.0 * Math.Log(1.0 - u1)) * Math.Sin(2.0 * Math.PI * (1.0 - u2));

    }

}