using System;
using System.Collections.Generic;
using System.Text;

namespace Modeling.Core.Abstractions
{
    public interface IObjectTree
    {
        IObjectTree Parent { get; }
    }
}
