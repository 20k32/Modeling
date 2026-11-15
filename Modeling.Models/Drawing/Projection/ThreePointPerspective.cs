using Modeling.Core.Drawing;
using System;
using System.Numerics;

namespace Modeling.Models.Drawing.Projection
{
    /// <summary>
    /// Three point perspective on the plane x = 0 (variant 14, method 18). A point is a row [x y z 1] multiplied by the matrix
    ///     | 0  0  0  −1/xs |
    ///     | 0  1  0  −1/ys |
    ///     | 0  0  1  −1/zs |
    ///     | 0  0  0    1   |
    /// so x' = 0, y' = y / w, z' = z / w, where w = 1 − x/xs − y/ys − z/zs.
    /// Lines parallel to X, Y and Z meet in three vanishing points: (0; 0), (−ys; 0) and (0; −zs) on the plane.
    /// The plane is drawn with y to the right and z up.
    /// </summary>
    public static class ThreePointPerspective
    {
        // points behind (or too close to) the centers of projection are not drawn
        public const float MINIMUM_HOMOGENEOUS_WEIGHT = 0.05f;

        public static bool IsValidCenterDistance(float distance) => float.IsFinite(distance) && MathF.Abs(distance) >= 1;

        public static Matrix4x4 CreateMatrix(float centerX, float centerY, float centerZ) => new(
            0, 0, 0, -1 / centerX,
            0, 1, 0, -1 / centerY,
            0, 0, 1, -1 / centerZ,
            0, 0, 0, 1);

        public static Vector4 Transform(Vector3 point, Matrix4x4 projection)
            => Vector4.Transform(new Vector4(point, 1), projection);

        public static bool IsVisible(Vector4 homogeneous) => homogeneous.W >= MINIMUM_HOMOGENEOUS_WEIGHT;

        /// <summary>Point on the plane x = 0 after the division by the homogeneous coordinate: (y; z).</summary>
        public static PointSingle ToPlane(Vector4 homogeneous) => new(homogeneous.Y / homogeneous.W, homogeneous.Z / homogeneous.W);

        public static bool TryProject(Vector3 point, Matrix4x4 projection, out PointSingle result)
        {
            var homogeneous = Transform(point, projection);

            result = IsVisible(homogeneous) ? ToPlane(homogeneous) : default;

            return IsVisible(homogeneous);
        }

        public static PointSingle GetVanishingPointX() => new(0, 0);

        public static PointSingle GetVanishingPointY(float centerY) => new(-centerY, 0);

        public static PointSingle GetVanishingPointZ(float centerZ) => new(0, -centerZ);
    }
}
