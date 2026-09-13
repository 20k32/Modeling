using Modeling.Core.Abstractions;
using Modeling.Core.Constants;
using Windows.UI;

namespace Modeling.Core.Drawing
{
    public class DrawingColor : IDefaultCheck
    {
        public Color WindowsUIColor { get; init; }
        public string ColorHex => WindowsUIColor.ToString();

        public DrawingColor(Color color)
        {
            var uiColor = color == default ? DrawingConstants.DEFAULT_COLOR : color;
            WindowsUIColor = uiColor;
        }

        public bool IsDefault() => WindowsUIColor == default;
    }
}
