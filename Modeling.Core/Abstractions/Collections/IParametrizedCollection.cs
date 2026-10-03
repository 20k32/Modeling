using System;
using System.Collections.Generic;
using System.Text;

namespace Modeling.Core.Abstractions.Collections
{
    public interface IParametrizedCollection<T> : IEnumerable<T>
    {
        T this[int index] { get; set; }

        int Count { get; }

        void AddRange(IEnumerable<T> items);
        void Add(T item);
        void AddUnique(T item);
        void Remove(T item);
        void RemoveRange(IEnumerable<T> items);
        void Clear();
        bool Contains(T item);
    }
}
