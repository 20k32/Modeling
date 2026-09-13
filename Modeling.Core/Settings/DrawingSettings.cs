using Modeling.Core.Drawing;
using System;
using System.Collections.Generic;
using Windows.UI;

namespace Modeling.Core.Settings
{
    sealed class DrawingSettings : IDrawingSettings
    {
        public bool? Initialized { get; set; }

        public int DpiX { get; set; }
        public int DpiY { get; set; }

        public Color BackgroundColor { get; set; }

        public Color DrawingColor { get; set; }
        public float DrawingThickness { get; set; }

        public float Scale { get; set; }

        public PointSingle CenterCanvasPosition { get; set; }
        public PointSingle RotatePointPosition { get; set; }

        public ICollection<PointSingle> Figure { get; set; }

        public bool DisplayMarkInCanvasCenter { get; set; }

        public bool DisplayGrid { get; set; }
        public bool DisplayAxis { get; set; }

        public TimeSpan RefreshRate { get; set; }
    }
}
