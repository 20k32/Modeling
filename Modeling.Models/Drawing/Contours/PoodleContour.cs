using Modeling.Core.Drawing;
using System;
using System.Collections.Generic;

namespace Modeling.Models.Drawing.Contours
{
    /// <summary>
    /// Individual contour 17 (poodle), coordinates in centimeters, the origin is in the middle of the picture.
    /// Every part is a closed smooth contour built from its control points (see CurvilinearContour.FromControlPoints).
    /// The morph target is a smiley: the silhouette becomes the face, the eye and the nose become the eyes, the ear becomes the smile.
    /// </summary>
    public static class PoodleContour
    {
        public static readonly string[] CONTOUR_NAMES = ["silhouette", "eye", "ear", "nose"];

        static readonly PointSingle[] SILHOUETTE_CONTROL_POINTS =
        [
            new(-4.64f, 1.54f), new(-3.9f, 1.97f), new(-3.1f, 2.35f), new(-3.36f, 2.68f), new(-3.52f, 3.02f), new(-3.1f, 3.12f),
            new(-2.93f, 3.48f), new(-2.56f, 3.52f), new(-2.22f, 3.39f), new(-1.82f, 3.42f), new(-1.55f, 3.26f), new(-1.73f, 2.99f),
            new(-1.49f, 2.85f), new(-1.29f, 2.61f), new(-1.09f, 1.41f), new(-0.75f, 0.87f), new(0.39f, 0.76f), new(1.73f, 0.82f),
            new(3.2f, 0.84f), new(3.54f, 1.14f), new(3.34f, 1.47f), new(3.23f, 1.97f), new(3.6f, 2.21f), new(3.87f, 2.4f),
            new(4.3f, 2.29f), new(4.48f, 1.97f), new(4.3f, 1.7f), new(4.52f, 1.41f), new(4.21f, 1.11f), new(3.87f, 1.14f),
            new(3.6f, 0.8f), new(3.44f, 0.34f), new(3.36f, -0.6f), new(3.44f, -1.54f), new(3.87f, -1.68f), new(4.3f, -1.7f),
            new(4.74f, -2.01f), new(4.78f, -2.32f), new(4.41f, -2.55f), new(4.3f, -2.91f), new(3.87f, -2.85f), new(3.6f, -3.08f),
            new(3.34f, -3.48f), new(3.18f, -3.28f), new(3.34f, -3.02f), new(3f, -2.75f), new(2.91f, -2.35f), new(3.04f, -1.94f),
            new(2.47f, -1.74f), new(1.86f, -1.27f), new(1.39f, -0.8f), new(0.99f, -1.21f), new(0.12f, -1.57f), new(-0.62f, -1.65f),
            new(-0.98f, -1.68f), new(-0.55f, -1.94f), new(-0.17f, -2.28f), new(-0.42f, -2.61f), new(-0.82f, -2.85f), new(-1.15f, -3.08f),
            new(-1.49f, -3.42f), new(-1.92f, -3.39f), new(-2.05f, -3.12f), new(-2.26f, -2.85f), new(-2.26f, -2.41f), new(-2.02f, -2.05f),
            new(-1.73f, -2.08f), new(-2.02f, -1.27f), new(-2.26f, -0.34f), new(-2.32f, 0.34f), new(-2.36f, 0.8f), new(-2.56f, 1.01f),
            new(-3.23f, 1.01f), new(-4.03f, 1.01f), new(-4.5f, 1.21f),
        ];

        static readonly PointSingle[] EYE_CONTROL_POINTS =
        [
            new(-3.1f, 2.08f), new(-2.88f, 1.97f), new(-2.96f, 1.7f), new(-3.12f, 1.81f),
        ];

        static readonly PointSingle[] EAR_CONTROL_POINTS =
        [
            new(-2.67f, 2.61f), new(-2.22f, 2.72f), new(-1.97f, 2.48f), new(-1.94f, 1.81f), new(-2.13f, 1.25f), new(-2.4f, 1.3f),
            new(-2.59f, 1.81f),
        ];

        static readonly PointSingle[] NOSE_CONTROL_POINTS =
        [
            new(-4.78f, 1.7f), new(-4.54f, 1.7f), new(-4.52f, 1.43f), new(-4.76f, 1.41f),
        ];

        const float FACE_RADIUS = 3.6f;
        const float EYE_RADIUS = 0.55f;
        const float SMILE_INNER_RADIUS = 1.75f;
        const float SMILE_OUTER_RADIUS = 2.3f;
        const float SMILE_START_ANGLE = 200 * MathF.PI / 180;
        const float SMILE_END_ANGLE = 340 * MathF.PI / 180;

        static readonly PointSingle LEFT_EYE_CENTER = new(-1.3f, 1.1f);
        static readonly PointSingle RIGHT_EYE_CENTER = new(1.3f, 1.1f);
        static readonly PointSingle SMILE_CENTER = new(0, 0.5f);

        public static List<CurvilinearContour> Create() =>
        [
            CurvilinearContour.FromControlPoints(SILHOUETTE_CONTROL_POINTS),
            CurvilinearContour.FromControlPoints(EYE_CONTROL_POINTS),
            CurvilinearContour.FromControlPoints(EAR_CONTROL_POINTS),
            CurvilinearContour.FromControlPoints(NOSE_CONTROL_POINTS),
        ];

        /// <summary>Final reference points for the animation, one array per contour in the same order as Create().</summary>
        public static List<PointSingle[]> CreateMorphTargets() =>
        [
            CreateTargetPoints(SILHOUETTE_CONTROL_POINTS.Length, s => GetCirclePoint(new PointSingle(), FACE_RADIUS, MathF.PI, s)),
            CreateTargetPoints(EYE_CONTROL_POINTS.Length, s => GetCirclePoint(LEFT_EYE_CENTER, EYE_RADIUS, MathF.PI / 2, s)),
            CreateTargetPoints(EAR_CONTROL_POINTS.Length, GetSmilePoint),
            CreateTargetPoints(NOSE_CONTROL_POINTS.Length, s => GetCirclePoint(RIGHT_EYE_CENTER, EYE_RADIUS, 3 * MathF.PI / 4, s)),
        ];

        // the control points are placed on the target curve, the arc ends are the middles between them (as in the source contour)
        static PointSingle[] CreateTargetPoints(int controlPointsCount, Func<float, PointSingle> curve)
        {
            var controlPoints = new PointSingle[controlPointsCount];

            for (var i = 0; i < controlPointsCount; i++)
            {
                controlPoints[i] = curve((float)i / controlPointsCount);
            }

            return CurvilinearContour.CreateReferencePoints(controlPoints);
        }

        // clockwise, like the traced parts of the poodle
        static PointSingle GetCirclePoint(PointSingle center, float radius, float startAngle, float s)
        {
            var angle = startAngle - 2 * MathF.PI * s;

            return new PointSingle(center.X + radius * MathF.Cos(angle), center.Y + radius * MathF.Sin(angle));
        }

        // a crescent: the upper (inner) arc from left to right, then the lower (outer) arc back
        static PointSingle GetSmilePoint(float s)
        {
            var isInnerArc = s < 0.5f;
            var t = isInnerArc ? s / 0.5f : (s - 0.5f) / 0.5f;

            var angle = isInnerArc
                ? SMILE_START_ANGLE + (SMILE_END_ANGLE - SMILE_START_ANGLE) * t
                : SMILE_END_ANGLE - (SMILE_END_ANGLE - SMILE_START_ANGLE) * t;
            var radius = isInnerArc ? SMILE_INNER_RADIUS : SMILE_OUTER_RADIUS;

            return new PointSingle(SMILE_CENTER.X + radius * MathF.Cos(angle), SMILE_CENTER.Y + radius * MathF.Sin(angle));
        }
    }
}
