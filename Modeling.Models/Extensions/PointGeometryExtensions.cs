using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Drawing;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Drawing.Figures.PointGeometries;
using Modeling.Models.Drawing.Figures.PointGeometries.GeometryCreationFactory;
using Modeling.Models.Miscellaneous;
using System.Collections.Generic;

namespace Modeling.Models.Extensions
{
    public static class PointGeometryExtensions
    {
        public static IPointGeometry ToLinePointGeometry(this ICollection<PointSingle> points, SegmentDimensionParameter parameter)
        {
            var pointGeometry = Ioc.Default.GetRequiredService<IPointGeometryCreationFactory>()
                .CreateLinePointGeometry(parameter);

            pointGeometry.AddPointsRange(points);

            pointGeometry.CalculateBounds();
            pointGeometry.CalculateCenterPoint();

            return pointGeometry;
        }

        public static IPointGeometry ToCirclePointGeometry(this ICollection<PointSingle> points, PointSingle centerPoint, SegmentDimensionParameter parameter)
        {
            var pointGeometry = Ioc.Default.GetRequiredService<IPointGeometryCreationFactory>()
                .CreateCirclePointGeometry(centerPoint, parameter, 0, 360);

            pointGeometry.AddPointsRange(points);

            pointGeometry.CalculateBounds();
            pointGeometry.CalculateCenterPoint();

            return pointGeometry;
        }

        public static IPointGeometry ToCirclePointGeometry(this ICollection<PointSingle> points, PointSingle centerPoint, float startAngle, float endAngle, SegmentDimensionParameter parameter)
        {
            var pointGeometry = Ioc.Default.GetRequiredService<IPointGeometryCreationFactory>()
                .CreateCirclePointGeometry(centerPoint, parameter, startAngle, endAngle);

            pointGeometry.AddPointsRange(points);

            pointGeometry.CalculateBounds();
            pointGeometry.CalculateCenterPoint();

            return pointGeometry;
        }
    }
}
