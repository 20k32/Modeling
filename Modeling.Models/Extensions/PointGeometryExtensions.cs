using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Drawing;
using Modeling.Models.Abstractions.Drawing.Figure;
using System.Collections;
using System.Collections.Generic;

namespace Modeling.Models.Extensions
{
    public static class PointGeometryExtensions
    {
        public static IPointGeometry ToPointGeometry(this ICollection<PointSingle> points)
        {
            var pointGeometry = Ioc.Default.GetRequiredService<IPointGeometry>();

            pointGeometry.AddPointsRange(points);

            return pointGeometry;
        }
    }
}
