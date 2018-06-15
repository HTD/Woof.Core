namespace Woof.Algorithms {

    /// <summary>
    /// Provides pseudo-random number generator methods.
    /// </summary>
    public interface IPseudoRandomNumberGenerator {

        /// <summary>
        /// Returns a non-negative random integer.
        /// </summary>
        /// <returns>A 32-bit signed integer that is greater than or equal to 0 and less than System.Int32.MaxValue.</returns>
        int Next();

        /// <summary>
        /// Returns a non-negative random integer that is less than the specified maximum.
        /// </summary>
        /// <param name="max">The exclusive upper bound of the random number to be generated. maxValue must be greater than or equal to 0.</param>
        /// <returns>A 32-bit signed integer greater than or equal to minValue and less than maxValue; that is, the range of return values includes minValue but not maxValue. If minValue equals maxValue, minValue is returned.</returns>
        int Next(int max);

        /// <summary>
        /// Returns a random integer that is within a specified range.
        /// </summary>
        /// <param name="min">The inclusive lower bound of the random number returned.</param>
        /// <param name="max">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        /// <returns>A 32-bit signed integer greater than or equal to minValue and less than maxValue; that is, the range of return values includes minValue but not maxValue. If minValue equals maxValue, minValue is returned.</returns>
        int Next(int min, int max);

        /// <summary>
        /// Returns a random boolean value.
        /// </summary>
        /// <returns>A random boolean value.</returns>
        bool NextBool();

        /// <summary>
        /// Returns next random byte.
        /// </summary>
        /// <returns>Random byte.</returns>
        byte NextByte();

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="buffer">An array of bytes to contain random numbers.</param>
        void NextBytes(byte[] buffer);

        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to 0.0 and less than 1.0.
        /// </summary>
        /// <returns>A double-precision floating point number that is greater than or equal to 0.0 and less than 1.0.</returns>
        double NextDouble();

    }

}