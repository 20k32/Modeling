using Modeling.Core.Drawing;

namespace Modeling.Core.Constants
{
    public static class DrawingConstants
    {
        public const float INVALID_POINT_COORDIATE = float.PositiveInfinity;
        public const float DEFAULT_POINT_COORDINATE = 0;

        public static readonly PointSingle DEFAULT_POINT = new();
        public static readonly PointSingle INVALID_POINT = new(INVALID_POINT_COORDIATE);
    }
}