using System;
using System.Collections.Generic;
using System.Text;

namespace Modeling.Core.Abstractions.Collections
{
    public interface IParametrizedCollection<T> : IEnumerable<T>
    {
        void AddRange(IEnumerable<T> items);
        void Add(T item);
        void Remove(T item);
        void Clear();
        bool Contains(T item);
    }
}
