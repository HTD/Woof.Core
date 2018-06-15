namespace Woof.Algorithms {

    /// <summary>
    /// Simple 8-bit Xorshift random bytes generator toy.
    /// </summary>
    public class XorShift8 {

        byte X = 0x15;
        byte Y = 0xe5;
        byte Z = 0xb5;
        byte S = 0x33;
        byte T;

        /// <summary>
        /// Returns a random byte.
        /// </summary>
        public byte Next {
            get {
                T = (byte)(X ^ (X << 3) & 0xff);
                X = Y;
                Y = Z;
                Z = S;
                return S = (byte)(S ^ (S >> 5) ^ (T ^ (T >> 2)));
            }
        }

    }

}