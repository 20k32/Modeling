using System;

namespace Modeling.Core.Drawing
{
    public readonly struct RectangleSingle(float top, float left, float right, float bottom)
    {
        public readonly float Top = top;
        public readonly float Left = left;
        public readonly float Right = right;
        public readonly float Bottom = bottom;

        public readonly float Width = MathF.Abs(right - left);
        public readonly float Height = MathF.Abs(bottom - top);
    }
}
