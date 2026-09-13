using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Modeling.Core.Drawing
{
    public readonly struct Matrix3x3Single
    {
        /// <summary>The first element of the first row.</summary>
        public readonly float M11 { get; init; }

        /// <summary>The second element of the first row.</summary>
        public readonly float M12 { get; init; }

        /// <summary>The third element of the first row.</summary>
        public readonly float M13 { get; init; }

        /// <summary>The first element of the second row.</summary>
        public readonly float M21 { get; init; }

        /// <summary>The second element of the second row.</summary>
        public readonly float M22 { get; init; }

        /// <summary>The third element of the second row.</summary>
        public readonly float M23 { get; init; }

        /// <summary>The first element of the third row.</summary>
        public readonly float M31 { get; init; }

        /// <summary>The second element of the third row.</summary>
        public readonly float M32 { get; init; }

        /// <summary>The third element of the third row.</summary>
        public readonly float M33 { get; init; }

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            var equals = false;

            if (obj is Matrix3x3Single matrix)
            {
                equals = M11 == matrix.M11 &&
                    M12 == matrix.M12 &&
                    M13 == matrix.M13 &&
                    M21 == matrix.M21 &&
                    M22 == matrix.M22 &&
                    M23 == matrix.M23 &&
                    M31 == matrix.M31 &&
                    M32 == matrix.M32 &&
                    M33 == matrix.M33;
            }

            return equals;
        }

        public override int GetHashCode() => HashCode.Combine(
                HashCode.Combine(M11, M12, M13, M21),
                HashCode.Combine(M22, M23, M31, M32),
                M33);

        public static bool operator ==(Matrix3x3Single matrixA, Matrix3x3Single matrixB) => matrixA.Equals(matrixB);
        public static bool operator !=(Matrix3x3Single matrixA, Matrix3x3Single matrixB) => !(matrixA == matrixB);

        public static Matrix3x3Single operator *(Matrix3x3Single left, Matrix3x3Single right)
            => new Matrix3x3Single
            {
                M11 = left.M11 * right.M11
                 + left.M12 * right.M21
                 + left.M13 * right.M31,

                M12 = left.M11 * right.M12
                 + left.M12 * right.M22
                 + left.M13 * right.M32,

                M13 = left.M11 * right.M13
                 + left.M12 * right.M23
                 + left.M13 * right.M33,


                M21 = left.M21 * right.M11
                 + left.M22 * right.M21
                 + left.M23 * right.M31,

                M22 = left.M21 * right.M12
                 + left.M22 * right.M22
                 + left.M23 * right.M32,

                M23 = left.M21 * right.M13
                 + left.M22 * right.M23
                 + left.M23 * right.M33,


                M31 = left.M31 * right.M11
                 + left.M32 * right.M21
                 + left.M33 * right.M31,

                M32 = left.M31 * right.M12
                 + left.M32 * right.M22
                 + left.M33 * right.M32,

                M33 = left.M31 * right.M13
                 + left.M32 * right.M23
                 + left.M33 * right.M33
            };
    }
}
