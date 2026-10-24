using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using System;

namespace Modeling.Models.Drawing.Curves
{
    /// <summary>
    /// Bernoulli lemniscate (x² + y²)² = 2a²(x² − y²) in the parametric form
    /// x(t) = a√2·cos t / (1 + sin²t), y(t) = a√2·cos t·sin t / (1 + sin²t).
    /// t ∈ [0; 2π) traces the whole curve: t ∈ (−π/2; π/2) is the right loop, t ∈ (π/2; 3π/2) is the left loop,
    /// t = π/2 and t = 3π/2 both map to the node at the origin.
    /// All lengths are in the same units as the scale parameter a.
    /// </summary>
    public static class BernoulliLemniscate
    {
        public const float PARAMETER_START = 0;
        public const float PARAMETER_END = 2 * MathF.PI;

        public const float RIGHT_LOOP_PARAMETER_START = -MathF.PI / 2;
        public const float RIGHT_LOOP_PARAMETER_END = MathF.PI / 2;

        /// <summary>Lemniscate constant ϖ = Γ(1/4)² / (2√(2π)).</summary>
        public const float LEMNISCATE_CONSTANT = 2.6220576f;

        public const bool HAS_ASYMPTOTES = false;

        const int INTEGRATION_INTERVALS_PER_PERIOD = 1024;
        const float ZERO_TOLERANCE = 1e-6f;

        static readonly float SQRT_TWO = MathF.Sqrt(2);

        /// <summary>Both inflection parameters correspond to the node at the origin, where curvature changes its sign.</summary>
        public static readonly float[] INFLECTION_POINT_PARAMETERS = [MathF.PI / 2, 3 * MathF.PI / 2];

        public static bool IsValidScale(float scale) => float.IsFinite(scale) && scale > 0;

        public static PointSingle GetPoint(float scale, float t)
        {
            var sin = MathF.Sin(t);
            var cos = MathF.Cos(t);

            var k = scale * SQRT_TWO / (1 + sin * sin);

            return new PointSingle(k * cos, k * cos * sin);
        }

        /// <summary>
        /// x'(t) = −a√2·sin t·(3 − sin²t) / (1 + sin²t)², y'(t) = a√2·(1 − 3sin²t) / (1 + sin²t)².
        /// The derivative never vanishes, so the curve is regular and the tangent exists at every point.
        /// </summary>
        public static PointSingle GetFirstDerivative(float scale, float t)
        {
            var sin = MathF.Sin(t);
            var sinSquared = sin * sin;
            var denominator = 1 + sinSquared;

            var k = scale * SQRT_TWO / (denominator * denominator);

            return new PointSingle(
                -k * sin * (3 - sinSquared),
                k * (1 - 3 * sinSquared));
        }

        /// <summary>
        /// x''(t) = −a√2·cos t·(3 − 12sin²t + sin⁴t) / (1 + sin²t)³, y''(t) = −2a√2·sin t·cos t·(5 − 3sin²t) / (1 + sin²t)³.
        /// </summary>
        public static PointSingle GetSecondDerivative(float scale, float t)
        {
            var sin = MathF.Sin(t);
            var cos = MathF.Cos(t);
            var sinSquared = sin * sin;
            var denominator = 1 + sinSquared;

            var k = scale * SQRT_TWO / (denominator * denominator * denominator);

            return new PointSingle(
                -k * cos * (3 - 12 * sinSquared + sinSquared * sinSquared),
                -2 * k * sin * cos * (5 - 3 * sinSquared));
        }

        /// <summary>|r'(t)| = a√2 / √(1 + sin²t).</summary>
        public static float GetSpeed(float scale, float t)
        {
            var sin = MathF.Sin(t);

            return scale * SQRT_TWO / MathF.Sqrt(1 + sin * sin);
        }

        /// <summary>
        /// Signed curvature κ = (x'y'' − y'x'') / |r'|^4 = 3cos t / (a√2·√(1 + sin^2(t))).
        /// Equivalent to the classic result |κ| = 3r / (2a^2), where r is the distance to the origin.
        /// </summary>
        public static float GetCurvature(float scale, float t)
        {
            var sin = MathF.Sin(t);

            return 3 * MathF.Cos(t) / (scale * SQRT_TWO * MathF.Sqrt(1 + sin * sin));
        }

        /// <summary>R = 1 / |κ|; positive infinity at the inflection point.</summary>
        public static float GetRadiusOfCurvature(float scale, float t)
        {
            var curvature = MathF.Abs(GetCurvature(scale, t));

            return curvature < ZERO_TOLERANCE / scale ? float.PositiveInfinity : 1 / curvature;
        }

        /// <summary>dy/dx = y'(t) / x'(t); infinity for the vertical tangents at the vertices (t = 0, π).</summary>
        public static float GetTangentSlope(float scale, float t)
        {
            var derivative = GetFirstDerivative(scale, t);

            return MathF.Abs(derivative.X) < ZERO_TOLERANCE * scale
                ? float.PositiveInfinity
                : derivative.Y / derivative.X;
        }

        public static PointSingle GetUnitTangent(float scale, float t)
        {
            var derivative = GetFirstDerivative(scale, t);
            var length = MathF.Sqrt(derivative.X * derivative.X + derivative.Y * derivative.Y);

            return new PointSingle(derivative.X / length, derivative.Y / length);
        }

        /// <summary>Unit normal obtained by rotating the unit tangent by +90 deg.</summary>
        public static PointSingle GetUnitNormal(float scale, float t)
        {
            var tangent = GetUnitTangent(scale, t);

            return new PointSingle(-tangent.Y, tangent.X);
        }

        /// <summary>Arc length L(t0, t1) = ∫ a√2 / √(1 + sin^2(t)) dt, evaluated with the composite Simpson rule.</summary>
        public static float GetArcLength(float scale, float startParameter, float endParameter)
        {
            var integral = IntegrateSimpson(
                static t => 1 / MathF.Sqrt(1 + MathF.Sin(t) * MathF.Sin(t)),
                startParameter,
                endParameter);

            return scale * SQRT_TWO * integral;
        }

        public static float GetTotalArcLength(float scale) => GetArcLength(scale, PARAMETER_START, PARAMETER_END);

        public static float GetLoopArcLength(float scale) => GetArcLength(scale, RIGHT_LOOP_PARAMETER_START, RIGHT_LOOP_PARAMETER_END);

        public static float GetTotalArcLengthClosedForm(float scale) => 2 * SQRT_TWO * LEMNISCATE_CONSTANT * scale;

        /// <summary>
        /// Area of one loop by Green's theorem: S = 1/2∫(x·y' − y·x') dt = 1/2∫ 2a^2·cos^3(t) / (1 + sin^2(t))^2 dt over (−π/2; π/2).
        /// </summary>
        public static float GetLoopArea(float scale)
        {
            var integral = IntegrateSimpson(
                static t =>
                {
                    var cos = MathF.Cos(t);
                    var denominator = 1 + MathF.Sin(t) * MathF.Sin(t);

                    return cos * cos * cos / (denominator * denominator);
                },
                RIGHT_LOOP_PARAMETER_START,
                RIGHT_LOOP_PARAMETER_END);

            return scale * scale * integral;
        }

        public static float GetTotalArea(float scale) => 2 * GetLoopArea(scale);

        /// <summary>Closed form of the loop area: S = a^2.</summary>
        public static float GetLoopAreaClosedForm(float scale) => scale * scale;

        /// <summary>The curve is bounded: |x| ≤ a√2, |y| ≤ a/2 (reached at sin^2(t) = 1/3).</summary>
        public static PointSingle GetBoundingHalfSize(float scale) => new PointSingle(scale * SQRT_TWO, scale / 2);

        public static PointSingle GetInflectionPoint() => DrawingConstants.DEFAULT_POINT;

        /// <summary>Maps any parameter value into [0; 2π).</summary>
        public static float NormalizeParameter(float t)
        {
            var result = t % PARAMETER_END;

            return result < 0 ? result + PARAMETER_END : result;
        }

        static float IntegrateSimpson(Func<float, float> function, float start, float end)
        {
            if (start == end)
            {
                return 0;
            }

            var intervals = (int)MathF.Ceiling(INTEGRATION_INTERVALS_PER_PERIOD * MathF.Abs(end - start) / PARAMETER_END);
            intervals = Math.Max(2, intervals + intervals % 2);

            var step = (end - start) / intervals;
            var sum = function(start) + function(end);

            for (var i = 1; i < intervals; i++)
            {
                sum += function(start + i * step) * (i % 2 == 0 ? 2 : 4);
            }

            return sum * step / 3;
        }
    }
}
