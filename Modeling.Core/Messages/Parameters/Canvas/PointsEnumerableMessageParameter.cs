using Modeling.Core.Abstractions;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Modeling.Core.Messages.Parameters.Canvas
{
    public sealed class PointsEnumerableMessageParameter : IDefaultCheck
    {
        public readonly ICollection<PointSingle> Points;

        public PointsEnumerableMessageParameter(ICollection<PointSingle> points)
        {
            Points = points;
        }

        public bool IsDefault() => Points is null && Points.All(point => point == DrawingConstants.DEFAULT_POINT);
    }
}
