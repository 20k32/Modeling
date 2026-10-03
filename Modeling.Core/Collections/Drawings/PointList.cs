using Modeling.Core.Abstractions.Collections.Drawings;
using Modeling.Core.Drawing;
using System.Collections.Generic;

namespace Modeling.Core.Collections.Drawings
{
    public sealed class PointList : BlockingCollection<PointSingle, List<PointSingle>>, IPointListCollection
    { }
}
