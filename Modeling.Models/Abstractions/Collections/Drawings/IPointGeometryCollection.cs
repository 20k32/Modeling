using Modeling.Core.Abstractions.Collections;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Drawing.Figures.PointGeometries;

namespace Modeling.Models.Abstractions.Collections.Drawings
{
    public interface IPointGeometryCollection : IBlockingCollection<IPointGeometry>
    { }
}
