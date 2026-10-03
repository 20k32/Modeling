using Modeling.Core.Abstractions.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Modeling.Core.Collections
{
    public class BlockingCollection<T, TCollection> : IBlockingCollection<T>
        where TCollection : ICollection<T>, new()
    {
        const int INVALID_THREAD_IDENTIFIER = -1;

        readonly TCollection _items;
        readonly object _syncRoot;
        int enumerationLockedThreadId;

        public object SyncRoot => _syncRoot;

        public BlockingCollection()
        {
            enumerationLockedThreadId = INVALID_THREAD_IDENTIFIER;

            _syncRoot = new object();
            _items = [];
        }

        public void Add(T item)
        {
            ThrowIfCurrentThreadBlockedEnumeration();

            lock (SyncRoot)
            {
                _items.Add(item);
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            ThrowIfCurrentThreadBlockedEnumeration();

            lock (SyncRoot)
            {
                foreach (var item in items)
                {
                    _items.Add(item);
                }
            }
        }

        public void Clear()
        {
            ThrowIfCurrentThreadBlockedEnumeration();

            lock (SyncRoot)
            {
                _items.Clear();
            }
        }

        public bool Contains(T item)
        {
            ThrowIfCurrentThreadBlockedEnumeration();

            lock (SyncRoot)
            {
                return _items.Contains(item);
            }
        }

        public void Remove(T item)
        {
            ThrowIfCurrentThreadBlockedEnumeration();

            lock (SyncRoot)
            {
                _items.Remove(item);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (SyncRoot)
            {
                try
                {
                    enumerationLockedThreadId = Environment.CurrentManagedThreadId;

                    foreach (var item in _items)
                    {
                        yield return item;
                    }
                }
                finally
                {
                    enumerationLockedThreadId = INVALID_THREAD_IDENTIFIER;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void ThrowIfCurrentThreadBlockedEnumeration([CallerMemberName] string methodName = "")
        {
            if (enumerationLockedThreadId != INVALID_THREAD_IDENTIFIER)
            {
                throw new Exception($"Deadlock will occur if {methodName} will continue execution for thread {enumerationLockedThreadId}");
            }
        }
    }
}
