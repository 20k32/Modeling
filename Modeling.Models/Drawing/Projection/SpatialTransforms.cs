using System;
using System.Numerics;

namespace Modeling.Models.Drawing.Projection
{
    /// <summary>
    /// Euclidean transforms of the solid in the space (rows [x y z 1] multiplied by 4×4 matrices, the same convention as the projection).
    /// The solid is described around its own center, so the rotations turn it around its own axes, the shift goes last.
    /// </summary>
    public static class SpatialTransforms
    {
        public static float DegreesToRadians(float degrees) => degrees * MathF.PI / 180;

        /// <summary>
        /// Rotation around X:      Rotation around Y:      Rotation around Z:       Shift:
        /// | 1    0     0   0 |    | cos  0  −sin  0 |    |  cos  sin  0  0 |     | 1   0   0   0 |
        /// | 0   cos   sin  0 |    |  0   1    0   0 |    | −sin  cos  0  0 |     | 0   1   0   0 |
        /// | 0  −sin   cos  0 |    | sin  0   cos  0 |    |   0    0   1  0 |     | 0   0   1   0 |
        /// | 0    0     0   1 |    |  0   0    0   1 |    |   0    0   0  1 |     | dx  dy  dz  1 |
        /// </summary>
        public static Matrix4x4 Create(float rotationXDegrees, float rotationYDegrees, float rotationZDegrees, Vector3 shift)
            => Matrix4x4.CreateRotationX(DegreesToRadians(rotationXDegrees))
            * Matrix4x4.CreateRotationY(DegreesToRadians(rotationYDegrees))
            * Matrix4x4.CreateRotationZ(DegreesToRadians(rotationZDegrees))
            * Matrix4x4.CreateTranslation(shift);
    }
}
