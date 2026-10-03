using Modeling.Models.Abstractions.Collections.Drawings;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Enums;
using System.Collections.Generic;

namespace Modeling.Models.Abstractions.Drawing
{
    public interface IAdjacentGeometriesProvider
    {
        IAdjacentPointGeometryCollection AdjacentGeometries { get; }

        void AddAdjacentGeometry(IPointGeometry geometry, AdjacentType adjacentType);
        void RemoveAdjacentGeometry(IPointGeometry geometry);
        void ClearAdjacentGeometries();
        void AddAdjacentGeometriesRange(IEnumerable<IPointGeometry> geometries, AdjacentType adjacentType);
    }
}
