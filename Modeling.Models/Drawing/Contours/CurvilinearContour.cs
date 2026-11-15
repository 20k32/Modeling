using Modeling.Core.Drawing;
using Modeling.Models.Drawing.Curves;
using System;
using System.Collections.Generic;

namespace Modeling.Models.Drawing.Contours
{
    /// <summary>
    /// Curvilinear contour made of second order arcs in the engineering form.
    /// Reference points go as A0, B0, A1, B1, … : an arc i is (A_i, B_i, A_i+1), so the end of one arc is the start of the next one.
    /// A closed contour has an even number of points (the last arc ends at A0), an open one has an odd number.
    /// Every reference point has its own weight.
    /// </summary>
    public sealed partial class CurvilinearContour
    {
        const float DEFAULT_WEIGHT = 1;

        readonly PointSingle[] _points;
        readonly float[] _weights;
        readonly bool[] _breakPoints;

        public bool IsClosed { get; }

        public int PointsCount => _points.Length;

        public int SegmentsCount => IsClosed ? _points.Length / 2 : (_points.Length - 1) / 2;

        public CurvilinearContour(IReadOnlyList<PointSingle> referencePoints, bool isClosed)
        {
            var isValidCount = isClosed
                ? referencePoints.Count >= 4 && referencePoints.Count % 2 == 0
                : referencePoints.Count >= 3 && referencePoints.Count % 2 == 1;

            if (!isValidCount)
            {
                throw new ArgumentException("Invalid amount of reference points for the contour.", nameof(referencePoints));
            }

            IsClosed = isClosed;

            _points = new PointSingle[referencePoints.Count];
            _weights = new float[referencePoints.Count];
            _breakPoints = new bool[referencePoints.Count];

            for (var i = 0; i < referencePoints.Count; i++)
            {
                _points[i] = referencePoints[i];
                _weights[i] = DEFAULT_WEIGHT;
            }
        }

        /// <summary>
        /// Builds a closed smooth contour: every given point becomes the middle point B of an arc,
        /// the ends of the arcs are the middles between neighbour given points, so neighbour arcs share the tangent.
        /// </summary>
        public static CurvilinearContour FromControlPoints(IReadOnlyList<PointSingle> controlPoints)
            => new(CreateReferencePoints(controlPoints), isClosed: true);

        public static PointSingle[] CreateReferencePoints(IReadOnlyList<PointSingle> controlPoints)
        {
            var count = controlPoints.Count;
            var result = new PointSingle[count * 2];

            for (var i = 0; i < count; i++)
            {
                var previous = controlPoints[(i - 1 + count) % count];
                var current = controlPoints[i];

                result[2 * i] = new PointSingle((previous.X + current.X) / 2, (previous.Y + current.Y) / 2);
                result[2 * i + 1] = current;
            }

            return result;
        }

        public static bool IsControlPoint(int index) => index % 2 == 1;

        public PointSingle GetPoint(int index) => _points[index];

        public void SetPoint(int index, PointSingle point) => _points[index] = point;

        public float GetWeight(int index) => _weights[index];

        public void SetWeight(int index, float weight)
        {
            if (EngineeringQuadraticCurve.IsValidWeight(weight))
            {
                _weights[index] = weight;
            }
        }

        public PointSingle[] CopyPoints() => (PointSingle[])_points.Clone();

        public void SetPoints(IReadOnlyList<PointSingle> points)
        {
            for (var i = 0; i < _points.Length && i < points.Count; i++)
            {
                _points[i] = points[i];
            }
        }

        public int GetSegmentStartIndex(int segment) => 2 * segment;

        public int GetSegmentEndIndex(int segment) => (2 * segment + 2) % _points.Length;

        /// <summary>Arc the point belongs to: a control point B_i and its start A_i belong to the arc i.</summary>
        public int GetSegmentOfPoint(int index) => Math.Min(index / 2, SegmentsCount - 1);

        public PointSingle GetSegmentPoint(int segment, float u)
        {
            var a = GetSegmentStartIndex(segment);
            var b = a + 1;
            var c = GetSegmentEndIndex(segment);

            return EngineeringQuadraticCurve.GetPoint(
                _points[a], _points[b], _points[c],
                _weights[a], _weights[b], _weights[c],
                u);
        }

        public ConicArcType GetSegmentArcType(int segment)
        {
            var a = GetSegmentStartIndex(segment);

            return EngineeringQuadraticCurve.GetArcType(_weights[a], _weights[a + 1], _weights[GetSegmentEndIndex(segment)]);
        }

        public float GetSegmentDiscriminant(int segment)
        {
            var a = GetSegmentStartIndex(segment);

            return EngineeringQuadraticCurve.GetDiscriminant(_weights[a], _weights[a + 1], _weights[GetSegmentEndIndex(segment)]);
        }
    }
}
