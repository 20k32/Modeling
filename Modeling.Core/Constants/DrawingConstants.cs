using Microsoft.UI;
using Modeling.Core.Drawing;
using System;
using Windows.Graphics;
using Windows.UI;

namespace Modeling.Core.Constants
{
    public static class DrawingConstants
    {
        public const float INVALID_POINT_COORDIATE = float.NaN;
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

        public const bool DISPLAY_MARK_IN_CANVAS_CENTER_BY_DEFAULT = false;
        public const bool DISPLAY_AXIS_BY_DEFAULT = true;
        public const bool DISPLAY_GRID_BY_DEFAULT = true;

        public const int ONE_SECOND_MILISECONDS = 1000;
        public const int DEFAULT_FRAMES_PER_SECOND = 60;
        public static readonly TimeSpan DEFAULT_REFRESH_RATE = new(ONE_SECOND_MILISECONDS / DEFAULT_FRAMES_PER_SECOND);

        public const int STANDART_DPI = 96;

        public static readonly Matrix3x3Single NON_TRANSFORM_MATRIX = new()
        {
            M11 = 1,
            M12 = 0,
            M13 = 0,

            M21 = 0,
            M22 = 1,
            M23 = 0,

            M31 = 0,
            M32 = 0,
            M33 = 1
        };

        public const int PIXELS_PER_CENTIMETER = 35;
        public static readonly SizeInt32 CANVAS_SIZE = new(1366, 768);
        public const int START_POINT_DRAWING_COORDINATE_X_Y = 1;
        public static readonly PointSingle START_DRAWING_POINT = new(START_POINT_DRAWING_COORDINATE_X_Y, START_POINT_DRAWING_COORDINATE_X_Y);
    }
}