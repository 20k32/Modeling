using Modeling.Core.Drawing;
using System;

namespace Modeling.Models.Drawing.Curves
{
    /// <summary>
    /// Second order curve in the engineering (rational) form:
    /// r(u) = (rA·wA·(1 − u)² + 2·rB·wB·u·(1 − u) + rC·wC·u²) / (wA·(1 − u)² + 2·wB·u·(1 − u) + wC·u²), u ∈ [0; 1].
    /// A and C are the ends of the arc, the polygon sides AB and CB are the tangents at the ends.
    /// When all weights are equal to 1 the arc is a second order Bézier curve.
    /// </summary>
    public static class EngineeringQuadraticCurve
    {
        public const float PARAMETER_START = 0;
        public const float PARAMETER_END = 1;

        public const float PARABOLA_DISCRIMINANT = 0.5f;

        const float PARABOLA_TOLERANCE = 1e-3f;

        public static bool IsValidWeight(float weight) => float.IsFinite(weight) && weight > 0;

        public static PointSingle GetPoint(PointSingle a, PointSingle b, PointSingle c, float weightA, float weightB, float weightC, float u)
        {
            var v = 1 - u;

            var ka = weightA * v * v;
            var kb = 2 * weightB * u * v;
            var kc = weightC * u * u;

            var denominator = ka + kb + kc;

            return new PointSingle(
                (a.X * ka + b.X * kb + c.X * kc) / denominator,
                (a.Y * ka + b.Y * kb + c.Y * kc) / denominator);
        }

        /// <summary>
        /// The arc does not change when all weights are multiplied by the same number,
        /// so it is fully described by wB / √(wA·wC) (equal to wB when wA = wC = 1).
        /// </summary>
        public static float GetNormalizedWeight(float weightA, float weightB, float weightC)
            => weightB / MathF.Sqrt(weightA * weightC);

        /// <summary>
        /// Discriminant f = FE / BE, where E is the middle of AC and F = r(0.5) lies on the median BE.
        /// For wA = wC = 1 it gives f = wB / (1 + wB), and back wB = f / (1 − f).
        /// </summary>
        public static float GetDiscriminant(float weightA, float weightB, float weightC)
        {
            var weight = GetNormalizedWeight(weightA, weightB, weightC);

            return weight / (1 + weight);
        }

        public static float GetWeightFromDiscriminant(float discriminant) => discriminant / (1 - discriminant);

        /// <summary>f ∈ (0; 0.5) — arc of an ellipse, f = 0.5 — arc of a parabola, f ∈ (0.5; 1) — arc of a hyperbola.</summary>
        public static ConicArcType GetArcType(float weightA, float weightB, float weightC)
        {
            var discriminant = GetDiscriminant(weightA, weightB, weightC);

            if (MathF.Abs(discriminant - PARABOLA_DISCRIMINANT) < PARABOLA_TOLERANCE)
            {
                return ConicArcType.Parabola;
            }

            return discriminant < PARABOLA_DISCRIMINANT ? ConicArcType.Ellipse : ConicArcType.Hyperbola;
        }
    }
}
