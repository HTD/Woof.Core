using System;

namespace Woof.Algorithms {

    /// <summary>
    /// Box-Muller transform implementation on <see cref="System.Random"/> and <see cref="IPseudoRandomNumberGenerator"/>.
    /// </summary>
    public static class BoxMuller {

        /// <summary>
        /// Standard deviation for (0..1) range.
        /// </summary>
        const double nStdDev = 1 / (4 * Math.PI);

        /// <summary>
        /// Mean for (0..1) range.
        /// </summary>
        const double nMean = 0.5;

        /// <summary>
        /// Generates a random value from a standard Normal distribution.<br/>
        /// With 0 as mean and standard deviation as 1 it returns values from (-2π..2π).<br/>
        /// With default 0.5 as mean and standard deviation as 1/4π it returns values from (0..1).
        /// </summary>
        /// <param name="prng">Pseudo random number generator.</param>
        /// <param name="stdDev">Standard deviation (default 1/4π).</param>
        /// <param name="mean">Mean (default 0.5).</param>
        /// <returns>2π * (-1..1) * stdDev + mean.</returns>
        public static double NextNormal(this IPseudoRandomNumberGenerator prng, double stdDev = nStdDev, double mean = nMean)
            => NextNormal(prng.NextDouble, stdDev = nStdDev, mean = nMean);

        /// <summary>
        /// Generates a random value from a standard Normal distribution.<br/>
        /// With 0 as mean and standard deviation as 1 it returns values from (-2π..2π).<br/>
        /// With default 0.5 as mean and standard deviation as 1/4π it returns values from (0..1).
        /// </summary>
        /// <param name="prng">Pseudo random number generator.</param>
        /// <param name="stdDev">Standard deviation (default 1/4π).</param>
        /// <param name="mean">Mean (default 0.5).</param>
        /// <returns>2π * (-1..1) * stdDev + mean.</returns>
        public static double NextNormal(this Random prng, double stdDev = nStdDev, double mean = nMean)
            => NextNormal(prng.NextDouble, stdDev = nStdDev, mean = nMean);

        /// <summary>
        /// Generates a random value from a Normal distribution covering the range from [0..max).
        /// </summary>
        /// <param name="prng">Pseudo random number generator.</param>
        /// <param name="max">Exclusive upper limit.</param>
        /// <returns>Integer from [0..max).</returns>
        public static int NextNormal(this IPseudoRandomNumberGenerator prng, int max) => NFSI(prng.NextNormal(), max);

        /// <summary>
        /// Generates a random value from a Normal distribution covering the range from [0..max).
        /// </summary>
        /// <param name="prng">Pseudo random number generator.</param>
        /// <param name="max">Exclusive upper limit.</param>
        /// <returns>Integer from [0..max).</returns>
        public static int NextNormal(this Random prng, int max) => NFSI(prng.NextNormal(), max);

        /// <summary>
        /// Generates a random value from a Normal distribution covering the range from [min..max).
        /// </summary>
        /// <param name="prng">Pseudo random number generator.</param>
        /// <param name="min">Inclusive lower limit.</param>
        /// <param name="max">Exclusive upper limit.</param>
        /// <returns>Integer from [min..max).</returns>
        public static int NextNormal(this IPseudoRandomNumberGenerator prng, int min, int max) => NFSI(prng.NextNormal(), min, max);

        /// <summary>
        /// Generates a random value from a Normal distribution covering the range from [min..max).
        /// </summary>
        /// <param name="prng">Pseudo random number generator.</param>
        /// <param name="min">Inclusive lower limit.</param>
        /// <param name="max">Exclusive upper limit.</param>
        /// <returns>Integer from [min..max).</returns>
        public static int NextNormal(this Random prng, int min, int max) => NFSI(prng.NextNormal(), min, max);

        /// <summary>
        /// Generates a random value from a standard Normal distribution.<br/>
        /// With 0 as mean and standard deviation as 1 it returns values from (-2π..2π).<br/>
        /// </summary>
        /// <param name="prng">Fuction returning random <see cref="double"/> numbers from [0..1) with uniform distribution.</param>
        /// <param name="stdDev">Standard deviation.</param>
        /// <param name="mean">Mean.</param>
        /// <returns>2π * (-1..1) * stdDev + mean.</returns>
        private static double NextNormal(Func<Double> prng, double stdDev, double mean) {
            if (useSecond) {
                useSecond = false;
                return secondValue;
            }
            double x1, x2, w, firstValue;
            do {
                x1 = prng() * 2.0 - 1.0;
                x2 = prng() * 2.0 - 1.0;
                w = x1 * x1 + x2 * x2;
            }
            while (w >= 1.0);
            w = Math.Sqrt((-2.0 * Math.Log(w)) / w);
            firstValue = x1 * w * stdDev + mean;
            secondValue = x2 * w * stdDev + mean;
            useSecond = true;
            return firstValue;
        }

        /// <summary>
        /// Normal For Small Integers Calculation.
        /// </summary>
        /// <param name="normal">Normal random from [0..1).</param>
        /// <param name="limit">Exclusive limit.</param>
        /// <param name="sdc">Standard deviation coefficient. Default 2.0.</param>
        /// <param name="mc">Mean coefficient. Default -0.5.</param>
        /// <returns>Integer from [0..limit).</returns>
        private static int NFSI(double normal, int limit, double sdc = 2.0, double mc = -0.5) {
            var n = (int)Math.Floor(limit * (sdc * normal + mc));
            if (n < 0) return 0;
            if (n >= limit) return limit - 1;
            return n;
        }

        /// <summary>
        /// Normal For Small Integers Calculation.
        /// </summary>
        /// <param name="normal">Normal random from [0..1).</param>
        /// <param name="min">Minimal value (inclusive).</param>
        /// <param name="max">Maximal value (exclusive).</param>
        /// <param name="sdc">Standard deviation coefficient. Default 2.0.</param>
        /// <param name="mc">Mean coefficient. Default -0.5.</param>
        /// <returns>Integer from [min..max).</returns>
        private static int NFSI(double normal, int min, int max, double sdc = 2.0, double mc = -0.5) {
            if (min == max) return min;
            if (min > max) throw new ArgumentException("min > max", nameof(min));
            var limit = max - min;
            return min + NFSI(normal, limit, sdc, mc);
        }

        [ThreadStatic]
        static bool useSecond;

        [ThreadStatic]
        static double secondValue;

    }

}