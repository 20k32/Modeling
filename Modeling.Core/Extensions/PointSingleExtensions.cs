using Modeling.Core.Drawing;
using System.Numerics;


namespace Modeling.Core.Extensions
{
    public static class PointSingleExtensions
    {
        public static Vector2 ToVector2(this PointSingle pointSingle)
        {
            return new(pointSingle.X, pointSingle.Y);
        }

        public static PointSingle FromVector2(this Vector2 vector2)
        {
            return new(vector2.X, vector2.Y);
        }

        public static Windows.Foundation.Point ToWindowsFoundationPoint(this PointSingle pointSingle)
        {
            return new(pointSingle.X, pointSingle.Y);
        }

        public static PointSingle FromWindowsFoundationPoint(this Windows.Foundation.Point point)
        {
            return new((float)point.X, (float)point.Y);
        }

        public static System.Drawing.Point ToSystemDrawingPoint(this PointSingle pointSingle)
        {
            return new((int)pointSingle.X, (int)pointSingle.Y);
        }

        public static PointSingle FromSystemDrawingPoint(this System.Drawing.Point point)
        {
            return new(point.X, point.Y);
        }
    }
}
