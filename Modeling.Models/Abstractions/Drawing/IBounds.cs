using Modeling.Core.Drawing;

namespace Modeling.Models.Abstractions.Drawing
{
    public interface IBounds
    {
        RectangleSingle Bounds { get; }
        void SetBounds();
    }
}
