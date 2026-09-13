using Modeling.Core.Abstractions;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;

namespace Modeling.Core.Messages.Parameters.Canvas
{
    public sealed class OnePointMessageParameter(PointSingle point) : IDefaultCheck
    {
        public PointSingle Point { get; init; } = point;

        public bool IsDefault() => Point == DrawingConstants.DEFAULT_POINT;
    }
}
