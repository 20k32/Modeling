using System;
using System.Collections.Generic;
using System.Text;

namespace Modeling.Core.Abstractions.Collections
{
    public interface IBlockingCollection<T> : IParametrizedCollection<T>
    {
        object SyncRoot { get; }
    }
}
