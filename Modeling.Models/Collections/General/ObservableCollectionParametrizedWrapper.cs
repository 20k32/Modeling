using Modeling.Core.Abstractions.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Modeling.Models.Collections.General
{
    sealed class ObservableCollectionParametrizedWrapper<T> : ObservableCollection<T>, IParametrizedCollection<T>
    {
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Items.Add(item);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void AddUnique(T item)
        {
            if (!Contains(item))
            {
                Add(item);
            }
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Items.Remove(item);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        void IParametrizedCollection<T>.Remove(T item) => _ = Remove(item);
    }
}
