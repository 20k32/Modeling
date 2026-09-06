using Modeling.Core.Constants;

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

        public static bool operator ==(PointSingle left, PointSingle right) => left.Equals(right);

        public static bool operator !=(PointSingle left, PointSingle right) => !(left == right);
    }
}
