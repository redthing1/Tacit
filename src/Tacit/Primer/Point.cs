using System;

namespace Tacit.Primer;

public struct Point {
    public readonly int x;
    public readonly int y;

    public Point(int v) : this(v, v) {}

    public Point(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public static Point operator +(Point p1, Point p2) {
        return new Point(p1.x + p2.x, p1.y + p2.y);
    }

    public static Point operator -(Point p1, Point p2) {
        return new Point(p1.x - p2.x, p1.y - p2.y);
    }

    /// <summary>
    ///     Manhattan distance
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    public static int MhDist(Point p1, Point p2) {
        return Math.Abs(p1.x - p2.x) + Math.Abs(p1.y - p2.y);
    }

    /// <summary>
    ///     Chebyshev distance
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    public static int ChDist(Point p1, Point p2) {
        return Math.Max(Math.Abs(p1.x - p2.x), Math.Abs(p1.y - p2.y));
    }

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) {
            return false;
        }

        return EqualTo((Point)obj);
    }

    public override int GetHashCode() {
        unchecked// Overflow is fine, just wrap
        {
            var hash = 17;
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            return hash;
        }
    }

    public static bool operator ==(Point lhs, Point rhs) {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(Point lhs, Point rhs) {
        return !lhs.Equals(rhs);
    }

    private bool EqualTo(Point p) {
        return x == p.x && y == p.y;
    }

    public override string ToString() {
        return $"({x}, {y})";
    }
}