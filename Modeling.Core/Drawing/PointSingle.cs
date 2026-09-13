using Modeling.Core.Constants;
using System;

namespace Modeling.Core.Drawing
{
    public readonly struct PointSingle
    {
        public readonly float X;
        public readonly float Y;

        public PointSingle(float x = DrawingConstants.DEFAULT_POINT_COORDINATE, float y = DrawingConstants.DEFAULT_POINT_COORDINATE)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj) =>
            obj is PointSingle single
            && Equals(single);

        public bool Equals(PointSingle single) =>
            X == single.X
            && Y == single.Y;

        public override int GetHashCode() => HashCode.Combine(X.GetHashCode(), Y.GetHashCode());

        public static bool operator ==(PointSingle left, PointSingle right) => left.Equals(right);

        public static bool operator !=(PointSingle left, PointSingle right) => !(left == right);

        public static PointSingle operator *(Matrix3x3Single matrix, PointSingle point)
        {
            float x = point.X * matrix.M11
                    + point.Y * matrix.M12
                    + matrix.M13;

            float y = point.X * matrix.M21
                    + point.Y * matrix.M22
                    + matrix.M23;

            return new PointSingle(x, y);
        }
    }
}
