using Modeling.Core.Abstractions.Collections;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Modeling.Core.Collections.General
{
    public sealed class IndexedList<T> : IParametrizedCollection<T>
    {
        readonly List<T> _items;

        public int Count => _items.Count;

        public T this[int index]
        {
            get => _items[index];
            set
            {
                _items[index] = value;
            }
        }

        public IndexedList()
        {
            _items = [];
        }

        public void Add(T item) => _items.Add(item);

        public void AddRange(IEnumerable<T> items) => _items.AddRange(items);

        public void AddUnique(T item)
        {
            if (!Contains(item))
            {
                Add(item);
            }
        }

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

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
