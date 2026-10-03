using Modeling.Core.Abstractions.Collections.Drawings;
using Modeling.Core.Drawing;
using System.Collections.Generic;

namespace Modeling.Core.Collections.Drawings
{
    sealed class PointHashSet : BlockingCollection<PointSingle, HashSet<PointSingle>>, IPointHashSetCollection
    { }
}
