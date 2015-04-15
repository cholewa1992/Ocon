using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OconExample
{
    class ComparableCollection<T> : ICollection<T>, IComparable<ComparableCollection<T>>
    {
        private readonly ICollection<T> _list;

        public ComparableCollection(ICollection<T> list)
        {
            _list = list;
        }

        public ComparableCollection(IEnumerable<T> list)
        {
            _list = list.ToList();
        } 

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return _list.IsReadOnly; }
        }

        public int CompareTo(ComparableCollection<T> other)
        {
            return 1;
        }
    }
}