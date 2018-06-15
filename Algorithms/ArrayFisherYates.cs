namespace Woof.Algorithms {

    /// <summary>
    /// Provides swapping and shuffling elements using Fisher-Yates algorithm.
    /// </summary>
    public static class ArrayFisherYates {

        /// <summary>
        /// Pseudo-random number generator.
        /// </summary>
        public static IPseudoRandomNumberGenerator PRNG = new XorShift32();

        /// <summary>
        /// Swaps array elements.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="x">Array to operate on.</param>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        public static void Swap<T>(this T[] x, int a, int b) { T y = x[a]; x[a] = x[b]; x[b] = y; }

        /// <summary>
        /// Shuffles the array using Fisher-Yates algorithm.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="x">Array to operate on.</param>
        /// <param name="s">Starting index. Default <c>0</c>.</param>
        public static void Shuffle<T>(this T[] x, int s = 0) {
            for (int i = s, n = x.Length; i < n - 1; i++) x.Swap(i, i + PRNG.Next(0, n - i));
        }

        /// <summary>
        /// <para>Returns array shuffled using Fisher-Yates algorithm.</para>
        /// <para>Original data is changed!</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x">Array to operate on.</param>
        /// <param name="s">Starting index. Default <c>0</c>.</param>
        /// <returns></returns>
        public static T[] Shuffled<T>(this T[] x, int s = 0) {
            x.Shuffle(s);
            return x;
        }

    }

}