using System;

namespace Tacit.Primer;

public struct PointF {
    public readonly float x;
    public readonly float y;

    public PointF(float x, float y) {
        this.x = x;
        this.y = y;
    }

    public static PointF operator +(PointF p1, PointF p2) {
        return new PointF(p1.x + p2.x, p1.y + p2.y);
    }

    public static PointF operator -(PointF p1, PointF p2) {
        return new PointF(p1.x - p2.x, p1.y - p2.y);
    }

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) {
            return false;
        }

        return EqualTo((PointF)obj);
    }

    public Point Round() {
        return new Point(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
    }

    public static implicit operator PointF(Point point) {
        return new PointF(point.x, point.y);
    }

    public static float Dist(PointF p1, PointF p2) {
        return (float)Math.Sqrt(Math.Pow(p1.x - p2.x, y: 2) + Math.Pow(p1.y - p2.y, y: 2));
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

    public bool EqualTo(PointF p) {
        return Math.Abs(x - p.x) < float.Epsilon && Math.Abs(y - p.y) < float.Epsilon;
    }

    public override string ToString() {
        return $"({x}, {y})";
    }
}