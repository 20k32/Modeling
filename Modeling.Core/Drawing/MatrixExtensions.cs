using System;

namespace Modeling.Core.Drawing
{
    public static class MatrixExtensions
    {
        public static Matrix3x3Single CreateRotationTransform(PointSingle centerPoint, float angleRadian)
        {
            var cos = MathF.Cos(angleRadian);
            var sin = MathF.Sin(angleRadian);

            return new()
            {
                M11 = cos,
                M12 = sin,
                M13 = 0,

                M21 = -sin,
                M22 = cos,
                M23 = 0,

                M31 = -centerPoint.X * (cos - 1) + centerPoint.Y * sin,
                M32 = -centerPoint.X * sin - centerPoint.Y * (cos - 1),
                M33 = 1
            };
        }

        public static Matrix3x3Single CreateRotationTransform(float angleRadian)
        {
            var centerPoint = new PointSingle(0, 0);

            var cos = MathF.Cos(angleRadian);
            var sin = MathF.Sin(angleRadian);

            return new()
            {
                M11 = cos,
                M12 = sin,
                M13 = 0,

                M21 = -sin,
                M22 = cos,
                M23 = 0,

                M31 = -centerPoint.X * (cos - 1) + centerPoint.Y * sin,
                M32 = -centerPoint.X * sin - centerPoint.Y * (cos - 1),
                M33 = 1
            };
        }

        public static Matrix3x3Single CreateTranslationTransform(float offsetX, float offsetY) => new()
        {
            M11 = 1,
            M12 = 0,
            M13 = offsetX,

            M21 = 0,
            M22 = 1,
            M23 = offsetY,

            M31 = 0,
            M32 = 0,
            M33 = 1
        };
    }
}
