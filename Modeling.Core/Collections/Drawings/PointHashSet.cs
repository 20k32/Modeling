using Modeling.Core.Abstractions.Collections.Drawings;
using Modeling.Core.Collections.General;
using Modeling.Core.Drawing;
using System.Collections.Generic;

namespace Modeling.Core.Collections.Drawings
{
    sealed class PointHashSet : BlockingCollection<PointSingle, IndexedHashSet<PointSingle>>, IPointHashSetCollection
    { }
}
