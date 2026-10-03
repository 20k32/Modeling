using Modeling.Core.Abstractions.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Modeling.Core.Collections.General
{
    public sealed class IndexedHashSet<T> : IParametrizedCollection<T>
    {
        readonly HashSet<T> _items;

        public int Count => _items.Count;

        public T this[int index] 
        { 
            get => _items.ElementAt(index); 
            set => throw new System.NotImplementedException(); 
        }

        public IndexedHashSet()
        {
            _items = [];
        }

        public void Add(T item) => _items.Add(item);

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                _items.Add(item);
            }
        }

        public void AddUnique(T item) => Add(item);

        public void Clear() => _items.Clear();

        public bool Contains(T item) => _items.Contains(item);

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        public void Remove(T item) => _items.Remove(item);

        public void RemoveRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Remove(item);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
