using Modeling.Core.Drawing;
using System.Collections.Generic;
using Windows.UI;

namespace Modeling.Core.Settings
{
    public interface IDrawingSettings
    {
        bool? Initialized { get; set; }

        int DpiX { get; set; }
        int DpiY { get; set; }
        
        Color BackgroundColor { get; set; }

        Color DrawingColor { get; set; }
        double DrawingThickness { get; set; }
        
        double Scale { get; set; }
        
        PointSingle CenterCanvasPosition { get; set; }
        PointSingle RotatePointPosition { get; set;}
        ICollection<PointSingle> Figure { get; set; }

        bool DisplayMarkInCanvasCenter { get; set; }
        bool DisplayGrid { get; set; }
        bool DisplayAxis { get; set; }
    }
}
