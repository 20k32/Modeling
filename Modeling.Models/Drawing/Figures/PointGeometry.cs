using Modeling.Core.Drawing;
using Modeling.Models.Abstractions.Drawing.Figure;
using System;
using System.Collections.Generic;

namespace Modeling.Models.Drawing.Figures
{
    public sealed class PointGeometry : IPointGeometry
    {
        RectangleSingle _bounds;

        public RectangleSingle Bounds => _bounds;
        public HashSet<PointSingle> Points { get; } = [];

        public bool TryAddPoint(PointSingle point) => Points.Add(point);
        public void AddPointsRange(IEnumerable<PointSingle> points)
        {
            foreach (var point in points)
            {
                _ = TryAddPoint(point);
            }
        }

        public void SetBounds()
        {
            var left = float.MaxValue;
            var top = float.MaxValue;
            var width = float.MinValue;
            var height = float.MinValue;

            foreach (var point in Points)
            {
                left = MathF.Min(left, point.X);
                top = MathF.Min(top, point.Y);
                width = MathF.Max(width, point.X);
                height = MathF.Max(height, point.Y);
            }

            _bounds = new(top, left, width, height);
        }
    }
}
