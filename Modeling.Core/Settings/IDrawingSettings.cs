using Modeling.Core.Abstractions;
using Modeling.Core.Drawing;
using System.Collections.Generic;
using Windows.Graphics;
using Windows.UI;

namespace Modeling.Core.Settings
{
    public interface IDrawingSettings : ISettingsChanged
    {
        bool? Initialized { get; set; }

        int DpiX { get; set; }
        int DpiY { get; set; }

        Color BackgroundColor { get; set; }

        Color DrawingColor { get; set; }
        float GridDrawingThickness { get; set; }
        float FigureDrawingThickness { get; set; }

        float Scale { get; set; }

        float AxisTickLength { get; set; }

        PointSingle CenterCanvasPosition { get; set; }
        PointSingle RotatePointPosition { get; set; }

        ICollection<PointSingle> Figure { get; set; }
        ICollection<PointSingle> HorizontalAxis { get; set; }
        ICollection<PointSingle> VerticalAxis { get; set; }

        bool DisplayMarkInCanvasCenter { get; set; }
        bool DisplayGrid { get; set; }
        bool DisplayAxis { get; set; }

        public float PixelsPerCentimeter { get; set; }
        public SizeInt32 CanvasSize { get; set; }

        public Color HorizontalAxisColor { get; set; }
        public Color VerticalAxisColor { get; set; }
        public Color VerticalAxisTicksColor { get; set; }
        public Color HorizontalAxisTicksColor { get; set; }
        public Color FigureBoundsColor { get; set; }
        public Color FigureCenterPointColor { get; set; }

        float AxisThickness { get; set; }
        float AxisTickThickness { get; set; }

        bool AttachGridToFigure { get; set; }
        bool DrawGrid { get; set; }
        bool DrawAxis { get; set; }
        bool DrawAxisMarks { get; set; }
        bool DrawAxisArrows { get; set; }
        bool DrawFigureShapeBounds { get; set; }
        bool DrawFigureBounds { get; set; }

        public int CanvasRefreshRate { get; set; }
    }
}
