using Modeling.Core.Enums;
using Modeling.Models.Abstractions.Collections.Drawings;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Drawing.Figures.PointGeometries;
using System.Collections.Generic;

namespace Modeling.Models.Abstractions.Drawing
{
    public interface IAdjacentGeometriesProvider
    {
        IAdjacentPointGeometryCollection AdjacentGeometries { get; }

        void AddAdjacentGeometry(IPointGeometry geometry, AdjacentType adjacentType);
        void RemoveAdjacentGeometry(IPointGeometry geometry);
        void ClearAdjacentGeometries();
    }
}
