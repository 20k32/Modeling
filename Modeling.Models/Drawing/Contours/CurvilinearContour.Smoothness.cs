using Modeling.Core.Drawing;
using System;

namespace Modeling.Models.Drawing.Contours
{
    /// <summary>
    /// Lab 4: first order smoothness (G1) between neighbour arcs.
    /// A node is an arc end A shared by two arcs, reference points are the middle points B, a break point is a node without smoothness.
    /// For the rational arcs r'(0) ∥ (rB − rA) and r'(1) ∥ (rC − rB) whatever the weights are,
    /// so the arcs join smoothly when the node lies on the straight line between its two reference points.
    /// </summary>
    public sealed partial class CurvilinearContour
    {
        const float SMOOTHNESS_TOLERANCE = 1e-6f;

        public static bool IsNode(int index) => index % 2 == 0;

        public bool IsBreakPoint(int index) => IsNode(index) && _breakPoints[index];

        public void SetBreakPoint(int index, bool isBreakPoint)
        {
            if (IsNode(index))
            {
                _breakPoints[index] = isBreakPoint;
            }
        }

        /// <summary>A node joins two arcs smoothly unless it is a break point or an end of an open contour.</summary>
        public bool IsSmoothNode(int index) => IsNode(index) && !_breakPoints[index] && IsInnerPoint(index);

        bool IsInnerPoint(int index) => IsClosed || (index > 0 && index < _points.Length - 1);

        bool HasIndex(int index) => IsClosed || (index >= 0 && index < _points.Length);

        int Wrap(int index) => IsClosed ? (index % _points.Length + _points.Length) % _points.Length : index;

        /// <summary>
        /// Angle between the tangents of the two arcs at the node, in degrees: 0 means the arcs join smoothly.
        /// </summary>
        public float GetTangentBreakAngle(int node)
        {
            if (!IsNode(node) || !IsInnerPoint(node))
            {
                return 0;
            }

            var point = _points[node];
            var incoming = point - _points[Wrap(node - 1)];
            var outgoing = _points[Wrap(node + 1)] - point;

            var lengths = Length(incoming) * Length(outgoing);

            if (lengths < SMOOTHNESS_TOLERANCE)
            {
                return 0;
            }

            var cos = Math.Clamp((incoming.X * outgoing.X + incoming.Y * outgoing.Y) / lengths, -1f, 1f);

            return MathF.Acos(cos) * 180 / MathF.PI;
        }

        /// <summary>
        /// Moves a point keeping the smooth nodes smooth:
        /// a reference point B turns the opposite reference point of the neighbour arc around every smooth node next to it
        /// (its distance to the node, the constant λ2, stays the same), a smooth node takes its two reference points along.
        /// A break point does not affect its neighbours.
        /// </summary>
        public void MovePointWithSmoothness(int index, PointSingle point)
        {
            if (IsNode(index))
            {
                var offset = point - _points[index];
                _points[index] = point;

                if (IsSmoothNode(index))
                {
                    MoveReferencePoint(Wrap(index - 1), index, _points[Wrap(index - 1)] + offset);
                    MoveReferencePoint(Wrap(index + 1), index, _points[Wrap(index + 1)] + offset);
                }

                return;
            }

            _points[index] = point;

            AlignOppositeReferencePoint(Wrap(index - 1), index);
            AlignOppositeReferencePoint(Wrap(index + 1), index);
        }

        /// <summary>Makes the node smooth right away: the reference point after it is turned to the line of the one before it.</summary>
        public void EnforceSmoothness(int node)
        {
            if (IsSmoothNode(node))
            {
                AlignOppositeReferencePoint(node, Wrap(node - 1));
            }
        }

        /// <summary>Puts every smooth node back on the line of its reference points (keeping its position along the line).</summary>
        public void ProjectSmoothNodes()
        {
            for (var node = 0; node < _points.Length; node += 2)
            {
                if (IsSmoothNode(node))
                {
                    var previous = _points[Wrap(node - 1)];
                    var next = _points[Wrap(node + 1)];

                    _points[node] = Lerp(previous, next, GetLineRatio(_points[node], previous, next));
                }
            }
        }

        // the reference point on the other side of the smooth node goes to the continuation of the line (dragged point → node)
        void AlignOppositeReferencePoint(int node, int draggedPoint)
        {
            if (!HasIndex(node) || !IsSmoothNode(node))
            {
                return;
            }

            var opposite = Wrap(node + (node - draggedPoint));

            if (!HasIndex(opposite) || opposite == draggedPoint)
            {
                return;
            }

            var direction = _points[node] - _points[draggedPoint];
            var directionLength = Length(direction);
            var oppositeLength = Length(_points[opposite] - _points[node]);

            if (directionLength < SMOOTHNESS_TOLERANCE || oppositeLength < SMOOTHNESS_TOLERANCE)
            {
                return;
            }

            var scale = oppositeLength / directionLength;

            MoveReferencePoint(opposite, node, new PointSingle(
                _points[node].X + direction.X * scale,
                _points[node].Y + direction.Y * scale));
        }

        // a moved reference point also belongs to the arc on its other side: if the node there is smooth,
        // the node slides to the new line between its reference points, so the change does not run around the whole contour
        void MoveReferencePoint(int referencePoint, int nearNode, PointSingle point)
        {
            if (!HasIndex(referencePoint))
            {
                return;
            }

            var farNode = Wrap(referencePoint + (referencePoint - nearNode));

            if (HasIndex(farNode) && farNode != nearNode && IsSmoothNode(farNode))
            {
                var otherReferencePoint = Wrap(farNode + (farNode - referencePoint));
                var ratio = GetLineRatio(_points[farNode], _points[referencePoint], _points[otherReferencePoint]);

                _points[referencePoint] = point;
                _points[farNode] = Lerp(_points[referencePoint], _points[otherReferencePoint], ratio);

                return;
            }

            _points[referencePoint] = point;
        }

        static float Length(PointSingle vector) => MathF.Sqrt(vector.X * vector.X + vector.Y * vector.Y);

        static PointSingle Lerp(PointSingle start, PointSingle end, float ratio)
            => new(start.X + (end.X - start.X) * ratio, start.Y + (end.Y - start.Y) * ratio);

        // position of the point projected on the segment start-end: 0 at the start, 1 at the end
        static float GetLineRatio(PointSingle point, PointSingle start, PointSingle end)
        {
            var line = end - start;
            var lengthSquared = line.X * line.X + line.Y * line.Y;

            if (lengthSquared < SMOOTHNESS_TOLERANCE)
            {
                return 0.5f;
            }

            var ratio = ((point.X - start.X) * line.X + (point.Y - start.Y) * line.Y) / lengthSquared;

            return Math.Clamp(ratio, 0f, 1f);
        }
    }
}
