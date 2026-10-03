using Modeling.Core.Collections;
using Modeling.Models.Abstractions.Collections.Drawings;
using Modeling.Models.Abstractions.Drawing.Figure;
using System.Collections.Generic;

namespace Modeling.Models.Collections.Drawings
{
    sealed class AdjacentPointGeometriesCollection : BlockingCollection<IAdjacentPointGeometry, LinkedList<IAdjacentPointGeometry>>, IAdjacentPointGeometryCollection
    { }
}
