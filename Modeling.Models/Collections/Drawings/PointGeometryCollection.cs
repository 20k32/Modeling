using Modeling.Models.Abstractions.Collections.Drawings;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Core.Collections.General;
using Modeling.Core.Collections;
using Modeling.Models.Drawing.Figures.PointGeometries;

namespace Modeling.Models.Collections.Drawings
{
    sealed class PointGeometryCollection : BlockingCollection<IPointGeometry, IndexedLinkedList<IPointGeometry>>, IPointGeometryCollection
    { }
}
