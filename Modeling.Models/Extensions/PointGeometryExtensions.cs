using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Drawing;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Drawing.Figures.PointGeometries.GeometryCreationFactory;
using System.Collections.Generic;

namespace Modeling.Models.Extensions
{
    public static class PointGeometryExtensions
    {
        public static IPointGeometry ToLinePointGeometry(this ICollection<PointSingle> points)
        {
            var pointGeometry = Ioc.Default.GetRequiredService<IPointGeometryCreationFactory>()
                .CreateLinePointGeometry();

            pointGeometry.AddPointsRange(points);

            pointGeometry.CalculateBounds();
            pointGeometry.CalculateCenterPoint();

            return pointGeometry;
        }

        public static IPointGeometry ToCirclePointGeometry(this ICollection<PointSingle> points, PointSingle centerPoint, float startAngle = 0, float endAngle = 360)
        {
            var pointGeometry = Ioc.Default.GetRequiredService<IPointGeometryCreationFactory>()
                .CreateCirclePointGeometry(centerPoint, startAngle, endAngle);

            pointGeometry.AddPointsRange(points);

            pointGeometry.CalculateBounds();
            pointGeometry.CalculateCenterPoint();

            return pointGeometry;
        }
    }
}
