namespace Modeling.Core.Drawing
{
    public readonly struct SizeSingle(float width = 0, float height = 0)
    {
        public static readonly SizeSingle Default = new SizeSingle(float.PositiveInfinity, float.PositiveInfinity);
        public readonly float Width = width;
        public readonly float Height = height;
    }
}
