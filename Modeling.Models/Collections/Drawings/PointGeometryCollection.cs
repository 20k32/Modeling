using Modeling.Core.Collections;
using Modeling.Models.Abstractions.Collections.Drawings;
using Modeling.Models.Drawing.Figures.PointGeometries;
using System.Collections.Generic;

namespace Modeling.Models.Collections.Drawings
{
    sealed class PointGeometryCollection : BlockingCollection<IPointGeometry, LinkedList<IPointGeometry>>, IPointGeometryCollection
    { }
}
