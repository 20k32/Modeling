using Modeling.Core.Abstractions.Collections;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Modeling.Core.Collections.General
{
    public sealed class IndexedLinkedList<T> : IParametrizedCollection<T>
    {
        readonly LinkedList<T> _items;

        public int Count => _items.Count;

        public IndexedLinkedList()
        {
            _items = [];
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _items.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                var node = _items.First;

                for (var i = 0; i < index; i++)
                    node = node!.Next;
                
                return node!.Value;
            }
            set
            {
                if (index < 0 || index >= _items.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                var node = _items.Last;

                for (var i = 0; i < index; i++)
                    node = node!.Next;
                
                node!.Value = value;
            }
        }

        public void Add(T item) => _items.AddLast(item);

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                _items.AddLast(item);
            }
        }

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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
