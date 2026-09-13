using Microsoft.UI;
using Modeling.Core.Drawing;
using Windows.UI;

namespace Modeling.Core.Constants
{
    public static class DrawingConstants
    {
        public const float INVALID_POINT_COORDIATE = float.PositiveInfinity;
        public const float DEFAULT_POINT_COORDINATE = 0;

        public static readonly PointSingle DEFAULT_POINT = new();
        public static readonly PointSingle INVALID_POINT = new(INVALID_POINT_COORDIATE);

        public const float DEFAULT_DRAWING_THICKNESS = 1;

        public static readonly Color DEFAULT_COLOR = Colors.Black;
        public static readonly DrawingColor DEFAULT_DRAWING_COLOR = new(DEFAULT_COLOR);

        public static readonly DrawingColor TRANSPARENT_DRAWING_COLOR = new(Colors.Transparent);

        public const int STANDART_DPI = 96;
    }
}