using Modeling.Core.Drawing;
using Modeling.Models.Enums;
using System.Collections.Generic;

namespace Modeling.Models.Drawing.DrawingMessageValues
{
    public sealed class ConnectPointsMessageValue : DrawingMessageValue
    {
        public IList<PointSingle> Points { get; init; }
        public float Thickness { get; init; }

        public ConnectPointsMessageValue(DrawingColor color, float thickness, params PointSingle[] points) : base(color)
        {
            MessageType = DrawingMessageType.DrawPolygon;

            Points = points ?? [];
            Thickness = thickness;
        }

        public ConnectPointsMessageValue(DrawingColor color, float thickness, IList<PointSingle> points) : base(color)
        {
            MessageType = DrawingMessageType.DrawPolygon;

            Points = points ?? [];
            Thickness = thickness;
        }

        public override bool IsDefault() => base.IsDefault() || (Points?.Count ?? 0) == 0;
    }
}
