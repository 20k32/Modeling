using Modeling.Core.Drawing;
using System;
using System.Collections.Generic;
using Windows.Graphics;
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
        public float GridDrawingThickness { get; set; }
        public float FigureDrawingThickness { get; set; }

        public float Scale { get; set; }

        public PointSingle CenterCanvasPosition { get; set; }
        public PointSingle RotatePointPosition { get; set; }

        public ICollection<PointSingle> Figure { get; set; }

        public bool DisplayMarkInCanvasCenter { get; set; }

        public bool DisplayGrid { get; set; }
        public bool DisplayAxis { get; set; }

        public TimeSpan RefreshRate { get; set; }

        public int PixelsPerCentimeter { get; set; }
        public SizeInt32 CanvasSize { get; set; }

        public Color HorizontalAxisColor { get; set; }
        public Color VerticalAxisColor { get; set; }

        public ICollection<PointSingle> HorizontalAxis { get; set; }
        public ICollection<PointSingle> VerticalAxis { get; set; }
        public Color VerticalAxisTicksColor { get; set; }
        public Color HorizontalAxisTicksColor { get; set; }

        public Color FigureBoundsColor { get; set; }
        public Color FigureCenterPointColor { get; set; }

        public float AxisTickLength { get; set; }
        public float AxisThickness { get; set; }
        public float AxisTickThickness { get; set; }
    }
}
