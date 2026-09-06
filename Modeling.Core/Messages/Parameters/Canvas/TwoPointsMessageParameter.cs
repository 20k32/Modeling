using Modeling.Core.Abstractions;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;

namespace Modeling.Core.Messages.Parameters.Canvas
{
    public sealed class TwoPointsMessageParameter : IDefaultCheck
    {
        public readonly PointSingle PointA;
        public readonly PointSingle PointB;

        public TwoPointsMessageParameter(PointSingle pointA, PointSingle pointB)
        {
            PointA = pointA;
            PointB = pointB;
        }

        public bool IsDefault() => PointA == DrawingConstants.DEFAULT_POINT && PointB == DrawingConstants.DEFAULT_POINT;
    }
}
