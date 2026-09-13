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
        public const float DEFAULT_SCALE = 1;

        public static readonly Color DEFAULT_COLOR = Colors.Black;
        public static readonly DrawingColor DEFAULT_DRAWING_COLOR = new(DEFAULT_COLOR);

        public static readonly Color DEFAULT_BACKGROUND_COLOR = Colors.White;
        public static readonly DrawingColor DEFAULT_DRAWING_BACKGROUND_COLOR = new(DEFAULT_BACKGROUND_COLOR);

        public static readonly DrawingColor TRANSPARENT_DRAWING_COLOR = new(Colors.Transparent);

        public static readonly PointSingle DEFAULT_CENTER_CANVAS_POSITION = INVALID_POINT;
        public static readonly PointSingle DEFAULT_ROTATE_POINT_POSITION = INVALID_POINT;

        public static bool DISPLAY_MARK_IN_CANVAS_CENTER_BY_DEFAULT = false;
        public static bool DISPLAY_AXIS_BY_DEFAULT = true;
        public static bool DISPLAY_GRID_BY_DEFAULT = true;



        public const int STANDART_DPI = 96;
    }
}