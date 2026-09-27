using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using Modeling.Core.Messages.Parameters.Canvas.Drawing;
using System.Collections.Generic;

namespace Modeling.Core.Extensions
{
    public static class CanvasMessageBuilder
    {
        public static PointListTransformMessageParameter With(this PointListTransformMessageParameter parameter,
            IReadOnlyList<PointSingle> points,
            DrawingColor? color = default,
            Matrix3x3Single? transformMatrix = default, bool? shouldFillGeometry = false,
            DrawingColor? fillColor = default, bool? clearBeforeRedraw = false,
            DrawingColor? backgroundColor = default, float? thickness = default)
                => new(
                    points,
                    color ?? parameter.Color,
                    transformMatrix ?? parameter.TransformMatrix,
                    shouldFillGeometry ?? parameter.ShouldFillGeometry,
                    fillColor ?? parameter.FillColor,
                    clearBeforeRedraw ?? parameter.ClearBeforeRedraw,
                    backgroundColor ?? parameter.BackgroundColor,
                    thickness ?? parameter.Thickness,
                    parent: parameter);

        public static PointListTransformMessageParameter WithNoParent(this PointListTransformMessageParameter parameter,
            IReadOnlyList<PointSingle> points,
            DrawingColor? color = default,
            Matrix3x3Single? transformMatrix = default, bool? shouldFillGeometry = false,
            DrawingColor? fillColor = default, bool? clearBeforeRedraw = false,
            DrawingColor? backgroundColor = default, float? thickness = DrawingConstants.GRID_DRAWING_THICKNESS)
                => new(
                    points,
                    color ?? parameter.Color,
                    transformMatrix ?? parameter.TransformMatrix,
                    shouldFillGeometry ?? parameter.ShouldFillGeometry,
                    fillColor ?? parameter.FillColor,
                    clearBeforeRedraw ?? parameter.ClearBeforeRedraw,
                    backgroundColor ?? parameter.BackgroundColor,
                    thickness ?? parameter.Thickness,
                    parent: default);
    }
}
