using Modeling.Core.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modeling.Core.Settings
{
    internal sealed class DrawingSettings : IDrawingSettings
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
