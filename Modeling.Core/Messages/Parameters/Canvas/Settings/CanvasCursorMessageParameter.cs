using Modeling.Core.Abstractions;
using Modeling.Core.Enums;

namespace Modeling.Core.Messages.Parameters.Canvas.Settings
{
    public sealed class CanvasCursorMessageParameter(MouseCursor cursor) : IDefaultCheck
    {
        public MouseCursor MouseCursor { get; init; } = cursor;
        public bool IsDefault() => MouseCursor == MouseCursor.None;
    }
}
