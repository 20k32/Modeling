namespace Modeling.Core.Drawing
{
    public readonly struct RectangleSingle(float top, float left, float width, float height)
    {
        public readonly float Top = top;
        public readonly float Left = left;
        public readonly float Width = width;
        public readonly float Height = height;
    }
}
