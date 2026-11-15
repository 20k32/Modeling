using System;
using System.Collections.Generic;
using System.Numerics;

namespace Modeling.Models.Drawing.Surfaces
{
    /// <summary>
    /// Individual surface 14: the upper part is a concave ellipsoid, the lower part is a sphere.
    /// The sphere of radius a:   xш = a·cos u·cos v,   yш = a·sin u·cos v,   zш = a·sin v,   0° ≤ u ≤ 360°, −90° ≤ v ≤ 90°.
    /// The surface:              x = xш,   y = yш,   z = a − k·zш if zш > 0,   z = zш if zш ≤ 0   (k = 0.5 in the variant).
    /// At the equator the lower half ends at z = 0 and the upper one starts at z = a, the grid joins them with a vertical wall.
    /// </summary>
    public readonly record struct ConcaveTopEllipsoidSurface(float Radius, float DepthCoefficient)
    {
        public const float U_START_DEGREES = 0;
        public const float U_END_DEGREES = 360;
        public const float V_START_DEGREES = -90;
        public const float V_END_DEGREES = 90;

        static float ToRadians(float degrees) => degrees * MathF.PI / 180;

        public Vector3 GetSpherePoint(float uDegrees, float vDegrees)
        {
            var u = ToRadians(uDegrees);
            var v = ToRadians(vDegrees);

            return new Vector3(
                Radius * MathF.Cos(u) * MathF.Cos(v),
                Radius * MathF.Sin(u) * MathF.Cos(v),
                Radius * MathF.Sin(v));
        }

        public bool IsUpperPart(float uDegrees, float vDegrees) => GetSpherePoint(uDegrees, vDegrees).Z > 0;

        static float GetLowerPartZ(Vector3 spherePoint) => spherePoint.Z;

        float GetUpperPartZ(Vector3 spherePoint) => Radius - DepthCoefficient * spherePoint.Z;

        /// <summary>Equation of the surface: every point of the grid and of the contour is computed here.</summary>
        public Vector3 GetPoint(float uDegrees, float vDegrees)
        {
            var spherePoint = GetSpherePoint(uDegrees, vDegrees);

            var z = spherePoint.Z > 0
                ? GetUpperPartZ(spherePoint)
                : GetLowerPartZ(spherePoint);

            return spherePoint with { Z = z };
        }

        /// <summary>
        /// UV grid: rows go along v (from the lower pole to the equator for the sphere, then from the equator to the upper pole for the concave part),
        /// columns go along u. The equator is taken twice: z = 0 for the lower half and z = a for the upper one.
        /// </summary>
        public Vector3[,] CreateGrid(int uCount, int halfVCount)
        {
            var rows = CreateGridRows(halfVCount);
            var result = new Vector3[rows.Count, uCount];

            for (var row = 0; row < rows.Count; row++)
            {
                var (v, isUpperPart) = rows[row];

                for (var column = 0; column < uCount; column++)
                {
                    var u = U_START_DEGREES + (U_END_DEGREES - U_START_DEGREES) * column / uCount;
                    var spherePoint = GetSpherePoint(u, v);

                    result[row, column] = spherePoint with
                    {
                        Z = isUpperPart ? GetUpperPartZ(spherePoint) : GetLowerPartZ(spherePoint),
                    };
                }
            }

            return result;
        }

        public static List<(float V, bool IsUpperPart)> CreateGridRows(int halfVCount)
        {
            var result = new List<(float V, bool IsUpperPart)>(2 * halfVCount + 2);

            for (var i = 0; i <= halfVCount; i++)
            {
                result.Add((V_START_DEGREES * (halfVCount - i) / halfVCount, false));
            }

            for (var i = 0; i <= halfVCount; i++)
            {
                result.Add((V_END_DEGREES * i / halfVCount, true));
            }

            return result;
        }

        /// <summary>Amount of the grid cells along u and along a half of v for the wanted mesh step in degrees.</summary>
        public static int GetUCount(float uStepDegrees) => Math.Max(3, (int)MathF.Round((U_END_DEGREES - U_START_DEGREES) / uStepDegrees));

        public static int GetHalfVCount(float vStepDegrees) => Math.Max(1, (int)MathF.Round(V_END_DEGREES / vStepDegrees));
    }
}
