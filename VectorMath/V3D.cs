using System;

namespace Woof.VectorMath {

    /// <summary>
    /// 3D vector structure.
    /// </summary>
    public struct V3D {

        /// <summary>
        /// X coordinate.
        /// </summary>
        public double X;

        /// <summary>
        /// Y coordinate.
        /// </summary>
        public double Y;

        /// <summary>
        /// Z coordinate.
        /// </summary>
        public double Z;

        /// <summary>
        /// Gets a value indicating whether this vector is a zero vector.
        /// </summary>
        public bool IsZero => X == 0 && Y == 0 && Z == 0;

        /// <summary>
        /// Gets the length of the vector.
        /// </summary>
        public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

        /// <summary>
        /// Creates a new 3D vector with coordinates.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        public V3D(double x, double y, double z) { X = x; Y = y; Z = z; }

        /// <summary>
        /// Returns hash code of the instance to speed up comparisons.
        /// </summary>
        /// <returns>A number that should be different for different vectors.</returns>
        public override int GetHashCode() => 13 * (13 * X.GetHashCode() + Y.GetHashCode()) + Z.GetHashCode();

        /// <summary>
        /// Tests for equality with another object.
        /// </summary>
        /// <param name="obj">Other object.</param>
        /// <returns>True if the same instance.</returns>
        public override bool Equals(object obj) => base.Equals(obj);

        /// <summary>
        /// Returns dot product with another vector.
        /// </summary>
        /// <param name="a">Other vector.</param>
        /// <returns>Dot product.</returns>
        public double Dot(V3D a) => X * a.X + Y * a.Y + Z * a.Z;

        /// <summary>
        /// Returns cross product with another vector.
        /// </summary>
        /// <param name="a">Other vector.</param>
        /// <returns>Cross product.</returns>
        public V3D Cross(V3D a) => new V3D(Y * a.Z - Z * a.Y, Z * a.X - X * a.Z, X * a.Y - Y * a.X);

        /// <summary>
        /// Interpolates a point between 2 points.
        /// </summary>
        /// <param name="a">Point A.</param>
        /// <param name="b">Point B.</param>
        /// <param name="t">Value between 0 and 1.</param>
        /// <returns>A point on a line between A and B.</returns>
        public static V3D Interpolate(V3D a, V3D b, double t)
            => new V3D(
                a.X * (1 - t) + b.X * t,
                a.Y * (1 - t) + b.Y * t,
                a.Z * (1 - t) + b.Z * t
            );

        /// <summary>
        /// Returns the distance between point C and AB segment.
        /// </summary>
        /// <param name="a">Point A.</param>
        /// <param name="b">Point B.</param>
        /// <param name="c">Point C.</param>
        /// <returns>Distance.</returns>
        public static double LineToPointDistance(V3D a, V3D b, V3D c) {
            var d = (b - a).Length;
            var d2 = d * d;
            if (d2 == 0d) return (c - a).Length;
            var t = ((c - a).Dot(b - a)) / d2;
            if (t < 0d) return (c - a).Length;
            else if (t > 1d) return (c - b).Length;
            var e = a + t * (b - a);
            return (e - c).Length;
        }

        #region Operators

        public static bool operator ==(V3D a, V3D b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z;

        public static bool operator !=(V3D a, V3D b) => a.X != b.X || a.Y != b.Y || a.Z != b.Z;

        public static bool operator >=(V3D a, V3D b) => a.Length >= b.Length;

        public static bool operator <=(V3D a, V3D b) => a.Length <= b.Length;

        public static bool operator >(V3D a, V3D b) => a.Length > b.Length;

        public static bool operator <(V3D a, V3D b) => a.Length < b.Length;

        public static V3D operator +(V3D a, V3D b) => new V3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static V3D operator -(V3D a, V3D b) => new V3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static V3D operator -(V3D a) => new V3D(-a.X, -a.Y, -a.Z);

        public static V3D operator +(V3D a) => new V3D(+a.X, +a.Y, +a.Z);

        public static V3D operator *(V3D a, double k) => new V3D(k * a.X, k * a.Y, k * a.Z);

        public static V3D operator *(double k, V3D a) => new V3D(k * a.X, k * a.Y, k * a.Z);

        public static V3D operator /(V3D a, double k) => new V3D(a.X / k, a.Y / k, a.Z / k);

        public static V3D operator /(double k, V3D a) => new V3D(k / a.X, k / a.Y, k / a.Z);

        #endregion

    }

}