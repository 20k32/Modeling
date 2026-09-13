using Modeling.Core.Drawing;
using System.Collections.Generic;
using System.Drawing;

namespace Modeling.Core.Settings
{
    sealed class DrawingSettings : IDrawingSettings
    {
        public int DpiX { get; set; }
        public int DpiY { get; set; }
        public Color DrawingColor { get; set; }
        public double DrawingThickness { get; set; }
        public double Scale { get; set; }
        public PointSingle CenterCanvasPosition { get; set; }
        public PointSingle RotatePointPosition { get; set; }
        public ICollection<PointSingle> Figure { get; set; }
        public bool DisplayMarkInCanvasCenter { get; set; }
        public bool DisplayGrid { get; set; }
        public bool DisplayAxis { get; set; }
    }
}
