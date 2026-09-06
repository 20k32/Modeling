using Modeling.Core.Abstractions;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;

namespace Modeling.Core.Messages.Parameters.Canvas
{
    public sealed class OnePointMessageParameter : IDefaultCheck
    {
        public readonly PointSingle Point;
        public OnePointMessageParameter(PointSingle point)
        {
            Point = point;
        }

        public bool IsDefault() => Point == DrawingConstants.DEFAULT_POINT;
    }
}
