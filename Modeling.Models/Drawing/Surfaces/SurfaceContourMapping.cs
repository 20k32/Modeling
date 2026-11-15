using Modeling.Core.Drawing;
using System;

namespace Modeling.Models.Drawing.Surfaces
{
    /// <summary>
    /// Curvilinear contour of the lab 3 on the UV grid of a surface. A point (x; y) of the contour (cm, the pivot point is its origin) is
    /// turned around the pivot by the angle φ, scaled into degrees of the grid and shifted to the place of the pivot (u0 + Δu; v0 + Δv):
    ///     u = u0 + Δu + s·(x·cos φ − y·sin φ),
    ///     v = v0 + Δv + s·(x·sin φ + y·cos φ).
    /// After that the equation of the surface gives the point in the space, so the contour always lies on the surface.
    /// </summary>
    public readonly record struct SurfaceContourMapping(float BaseU, float BaseV, float ShiftU, float ShiftV, float AngleDegrees, float Scale)
    {
        public PointSingle Pivot => new(BaseU + ShiftU, BaseV + ShiftV);

        public PointSingle ToUv(PointSingle contourPoint)
        {
            var angle = AngleDegrees * MathF.PI / 180;
            var cos = MathF.Cos(angle);
            var sin = MathF.Sin(angle);

            return new PointSingle(
                Pivot.X + Scale * (contourPoint.X * cos - contourPoint.Y * sin),
                Pivot.Y + Scale * (contourPoint.X * sin + contourPoint.Y * cos));
        }
    }
}
