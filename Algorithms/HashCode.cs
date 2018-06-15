using System.Collections;

namespace Woof.Algorithms {

    /// <summary>
    /// Hash code calculation object for custom comparers and such.
    /// </summary>
    public sealed class HashCode {

        /// <summary>
        /// Calculated hash code value.
        /// </summary>
        private readonly int Value;

        /// <summary>
        /// Creates a hash code from an object.
        /// </summary>
        /// <param name="x">Any object.</param>
        public HashCode(object x) => Value = x.GetHashCode();

        /// <summary>
        /// Creates a hash code from a collection.
        /// </summary>
        /// <param name="collection">Any collection.</param>
        public HashCode(IEnumerable collection) {
            foreach (int item in collection) Value = unchecked(Value * 17 + item.GetHashCode());
        }

        /// <summary>
        /// Creates a hash code from 2 objects.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public HashCode(object a, object b)
            => Value = 31 * a.GetHashCode() + b.GetHashCode();

        /// <summary>
        /// Creates a hash code from 3 objects.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public HashCode(object a, object b, object c)
            => Value = 31 * (31 * a.GetHashCode() + b.GetHashCode()) + c.GetHashCode();

        /// <summary>
        /// Creates a hash code from 4 objects.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        public HashCode(object a, object b, object c, object d)
            => Value = 31 * (31 * (31 * a.GetHashCode() + b.GetHashCode()) + c.GetHashCode()) + d.GetHashCode();

        /// <summary>
        /// Creates a hash code from 5 objects.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public HashCode(object a, object b, object c, object d, object e)
            => Value = 31 * (31 * (31 * (31 * a.GetHashCode() + b.GetHashCode()) + c.GetHashCode()) + d.GetHashCode()) + e.GetHashCode();

        /// <summary>
        /// Creates a hash code from 6 objects.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        public HashCode(object a, object b, object c, object d, object e, object f)
            => Value = 31 * (31 * (31 * (31 * (31 * a.GetHashCode() + b.GetHashCode()) + c.GetHashCode()) + d.GetHashCode()) + e.GetHashCode()) + f.GetHashCode();

        /// <summary>
        /// Creates a hash code from 7 objects.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        public HashCode(object a, object b, object c, object d, object e, object f, object g)
            => Value = 31 * (31 * (31 * (31 * (31 * (31 * a.GetHashCode() + b.GetHashCode()) + c.GetHashCode()) + d.GetHashCode()) + e.GetHashCode()) + f.GetHashCode()) + g.GetHashCode();

        /// <summary>
        /// Creates a hash code from 8 objects.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <param name="h"></param>
        public HashCode(object a, object b, object c, object d, object e, object f, object g, object h)
            => Value = 31 * (31 * (31 * (31 * (31 * (31 * (31 * a.GetHashCode() + b.GetHashCode()) + c.GetHashCode()) + d.GetHashCode()) + e.GetHashCode()) + f.GetHashCode()) + g.GetHashCode()) + h.GetHashCode();

        /// <summary>
        /// Imlicitly converts <see cref="HashCode"/> to <see cref="int"/>.
        /// </summary>
        /// <param name="hashCode"></param>
        public static implicit operator int(HashCode hashCode) => hashCode.Value;

    }

}