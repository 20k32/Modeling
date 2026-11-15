using System.Numerics;

namespace Modeling.Models.Drawing.Projection
{
    /// <summary>
    /// A point of the space is a row vector in homogeneous coordinates [x y z w], it is multiplied by a 4×4 transformation matrix from the right:
    /// [x' y' z' w'] = [x y z w] · M, every coordinate is the scalar product of the row with a column of the matrix.
    /// </summary>
    public static class HomogeneousTransforms
    {
        public static Vector4 Multiply(Vector4 point, Matrix4x4 matrix) => new(
            point.X * matrix.M11 + point.Y * matrix.M21 + point.Z * matrix.M31 + point.W * matrix.M41,
            point.X * matrix.M12 + point.Y * matrix.M22 + point.Z * matrix.M32 + point.W * matrix.M42,
            point.X * matrix.M13 + point.Y * matrix.M23 + point.Z * matrix.M33 + point.W * matrix.M43,
            point.X * matrix.M14 + point.Y * matrix.M24 + point.Z * matrix.M34 + point.W * matrix.M44);

        /// <summary>Euclidean transformation of a point: [x y z 1] · M, the result is divided by w (w = 1 for shifts and rotations).</summary>
        public static Vector3 TransformPoint(Vector3 point, Matrix4x4 matrix)
        {
            var result = Multiply(new Vector4(point, 1), matrix);

            return new Vector3(result.X, result.Y, result.Z) / result.W;
        }
    }
}
