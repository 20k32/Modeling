using Microsoft.Graphics.Canvas;
using Modeling.Core.Enums;

namespace Modeling.Models.Extensions
{
    public static class DrawingAliasingExtensions
    {
        public static CanvasAntialiasing ToCanvasAntialiasing(this DrawingAliasing drawingAliasing) => drawingAliasing switch
        {
            DrawingAliasing.Aliased => CanvasAntialiasing.Aliased,
            DrawingAliasing.AntiAliased => CanvasAntialiasing.Antialiased,
            _ => CanvasAntialiasing.Antialiased, // default behavior for win2d canvas
        };
    }
}
