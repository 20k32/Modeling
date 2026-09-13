using Modeling.Core.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modeling.Core.Settings
{
    public interface IDrawingSettings
    {
        int DpiX { get; set; }
        int DpiY { get; set; }
        
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
