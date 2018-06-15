using System;
using System.Drawing;

namespace Woof.VectorMath {

    /// <summary>
    /// 2D vector class.
    /// </summary>
    public struct V2D {

        /// <summary>
        /// X coordinate.
        /// </summary>
        public double X;

        /// <summary>
        /// Y coordinate.
        /// </summary>
        public double Y;

        /// <summary>
        /// Gets a value indicating whether this vector is a zero vector.
        /// </summary>
        public bool IsZero => X == 0 && Y == 0;

        /// <summary>
        /// Gets the length of the vector.
        /// </summary>
        public double Length => Math.Sqrt(X * X + Y * Y);

        /// <summary>
        /// Creates a new 2D vector with coordinates.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        public V2D(double x, double y) { X = x; Y = y; }

        /// <summary>
        /// Returns hash code of the instance to speed up comparisons.
        /// </summary>
        /// <returns>A number that should be different for different vectors.</returns>
        public override int GetHashCode() => 13 * X.GetHashCode() + Y.GetHashCode();

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
        public double Dot(V2D a) => X * a.X + Y * a.Y;

        /// <summary>
        /// Returns cross product with another vector.
        /// </summary>
        /// <param name="a">Other vector.</param>
        /// <returns>Cross product.</returns>
        public double Cross(V2D a) => X * a.Y - Y - a.X;

        /// <summary>
        /// Interpolates a point between 2 points.
        /// </summary>
        /// <param name="a">Point A.</param>
        /// <param name="b">Point B.</param>
        /// <param name="t">Value between 0 and 1.</param>
        /// <returns></returns>
        public static V2D Interpolate(V2D a, V2D b, double t) => new V2D(a.X * (1 - t) + b.X * t, a.Y * (1 - t) + b.Y * t);

        /// <summary>
        /// Returns the distance between point C and AB segment.
        /// </summary>
        /// <param name="a">Point A.</param>
        /// <param name="b">Point B.</param>
        /// <param name="c">Point C.</param>
        /// <returns>Distance.</returns>
        public static double LineToPointDistance(V2D a, V2D b, V2D c) {
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

        public static bool operator ==(V2D a, V2D b) => a.X == b.X && a.Y == b.Y;

        public static bool operator !=(V2D a, V2D b) => a.X != b.X || a.Y != b.Y;

        public static bool operator >=(V2D a, V2D b) => a.Length >= b.Length;

        public static bool operator <=(V2D a, V2D b) => a.Length <= b.Length;

        public static bool operator >(V2D a, V2D b) => a.Length > b.Length;

        public static bool operator <(V2D a, V2D b) => a.Length < b.Length;

        public static V2D operator +(V2D a, V2D b) => new V2D(a.X + b.X, a.Y + b.Y);

        public static V2D operator -(V2D a, V2D b) => new V2D(a.X - b.X, a.Y - b.Y);

        public static V2D operator -(V2D a) => new V2D(-a.X, -a.Y);

        public static V2D operator +(V2D a) => new V2D(+a.X, +a.Y);

        public static V2D operator *(V2D a, double k) => new V2D(k * a.X, k * a.Y);

        public static V2D operator *(double k, V2D a) => new V2D(k * a.X, k * a.Y);

        public static V2D operator /(V2D a, double k) => new V2D(a.X / k, a.Y / k);

        public static V2D operator /(double k, V2D a) => new V2D(k / a.X, k / a.Y);

        public static implicit operator PointF(V2D v) => new PointF(-(float)v.X, -(float)v.Y); public static implicit operator V2D(PointF p) => new V2D(-p.X, -p.Y);

        #endregion

    }

}