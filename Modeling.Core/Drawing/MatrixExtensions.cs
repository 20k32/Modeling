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
                M13 = -centerPoint.X * (cos - 1) - centerPoint.Y * sin,

                M21 = -sin,
                M22 = cos,
                M23 = centerPoint.X * sin - centerPoint.Y * (cos - 1),

                M31 = 0,
                M32 = 0,
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

        public static Matrix3x3Single Inverse(this Matrix3x3Single matrix)
        {
            var a = matrix.M11;
            var b = matrix.M12;
            var c = matrix.M13;
            var d = matrix.M21;
            var e = matrix.M22;
            var f = matrix.M23;
            var g = matrix.M31;
            var h = matrix.M32;
            var i = matrix.M33;

            var cofactor11 = e * i - f * h;
            var cofactor12 = f * g - d * i;
            var cofactor13 = d * h - e * g;
            var determinant = a * cofactor11 + b * cofactor12 + c * cofactor13;

            if (MathF.Abs(determinant) < 1e-6f)
            {
                throw new InvalidOperationException(
                    "The matrix cannot be inverted because its determinant is zero or too small.");
            }

            float inverseDeterminant = 1f / determinant;

            return new Matrix3x3Single
            {
                M11 = cofactor11 * inverseDeterminant,
                M12 = (c * h - b * i) * inverseDeterminant,
                M13 = (b * f - c * e) * inverseDeterminant,

                M21 = cofactor12 * inverseDeterminant,
                M22 = (a * i - c * g) * inverseDeterminant,
                M23 = (c * d - a * f) * inverseDeterminant,

                M31 = cofactor13 * inverseDeterminant,
                M32 = (b * g - a * h) * inverseDeterminant,
                M33 = (a * e - b * d) * inverseDeterminant
            };
        }

        public static Matrix3x3Single CreateAffineTransform(PointSingle origin,
            PointSingle xAxisPoint,
            PointSingle yAxisPoint)
        {
            return new Matrix3x3Single
            {
                M11 = xAxisPoint.X,
                M12 = yAxisPoint.X,
                M13 = origin.X,

                M21 = xAxisPoint.Y,
                M22 = yAxisPoint.Y,
                M23 = origin.Y,

                M31 = 0,
                M32 = 0,
                M33 = 1
            };
        }

        public static Matrix3x3Single CreatePointSymmetryTransform(PointSingle center)
        {
            return new Matrix3x3Single
            {
                M11 = -1,
                M12 = 0,
                M13 = 2 * center.X,

                M21 = 0,
                M22 = -1,
                M23 = 2 * center.Y,

                M31 = 0,
                M32 = 0,
                M33 = 1
            };
        }

        public static Matrix3x3Single CreateProjectiveTransform2(PointSingle origin,
                                        PointSingle xAxisPoint,
                                        PointSingle yAxisPoint,
                                        float wx,
                                        float wy,
                                        float w0)
        {
            return new Matrix3x3Single
            {
                M11 = xAxisPoint.X * wx,
                M12 = yAxisPoint.X * wy,
                M13 = origin.X * w0,

                M21 = xAxisPoint.Y * wx,
                M22 = yAxisPoint.Y * wy,
                M23 = origin.Y * w0,

                M31 = wx,
                M32 = wy,
                M33 = w0
            };
        }
    }
}
