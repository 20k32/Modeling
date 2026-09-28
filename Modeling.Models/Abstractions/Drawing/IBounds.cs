using Modeling.Core.Drawing;

namespace Modeling.Models.Abstractions.Drawing
{
    public interface IBounds
    {
        PointSingle CenterPoint { get; }
        RectangleSingle Bounds { get; }

        void CalculateCenterPoint();
        void CalculateBounds();
    }
}
