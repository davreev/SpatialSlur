
/*
 * Notes
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;

using SpatialSlur.Collections;
using SpatialSlur.Meshes.Impl;

namespace SpatialSlur.Meshes
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class HeNodeList<T> : IReadOnlyList<T>
        where T : HeNode
    {
        #region Static Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator ReadOnlyArrayView<T>(HeNodeList<T> source)
        {
            return source.AsView();
        }

        #endregion


        private T[] _items;
        private int _count;
        private int _currTag = int.MinValue;


        /// <summary>
        /// 
        /// </summary>
        public HeNodeList()
        {
            _items = Array.Empty<T>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public HeNodeList(int capacity)
        {
            _items = new T[capacity];
        }


        /// <summary>
        /// 
        /// </summary>
        internal int NextTag
        {
            get
            {
                if (_currTag == int.MaxValue) ResetTags();
                return ++_currTag;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return _count; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int Capacity
        {
            get { return _items.Length; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                if (index >= _count)
                    throw new IndexOutOfRangeException();

                return _items[index];
            }
            internal set
            {
                if (index >= _count)
                    throw new IndexOutOfRangeException();

                _items[index] = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private protected ArrayView<T> Items
        {
            get => _items.First(_count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void ResetTags()
        {
            _currTag = int.MinValue;

            for (int i = 0; i < _count; i++)
                _items[i].Tag = int.MinValue;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ReadOnlyArrayView<T> AsView()
        {
            return new ReadOnlyArrayView<T>(_items, 0, _count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ReadOnlyArrayView<T> AsView(int count)
        {
            return new ReadOnlyArrayView<T>(_items, 0, count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ReadOnlyArrayView<T> AsView(int start, int count)
        {
            return new ReadOnlyArrayView<T>(_items, start, count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ArrayView<T>.Enumerator GetEnumerator()
        {
            return _items.First(_count).GetEnumerator();
        }

        
        /// <summary>
        /// Adds the given node to the list.
        /// </summary>
        /// <param name="node"></param>
        internal void Add(T node)
        {
            node.Index = _count;
            node.Tag = _currTag;
            DynamicArray.Append(ref _items, _count++, node);
        }


        /// <summary>
        /// Reduces the capacity to twice the count.
        /// If the capacity is already less than twice the count, then this function does nothing.
        /// </summary>
        public void TrimExcess()
        {
            DynamicArray.ShrinkToFit(ref _items, _count << 1);
        }


        /// <summary>
        /// Returns the number of unused nodes in the list.
        /// </summary>
        public abstract int CountUnused();


        /// <summary>
        /// Removes all unused nodes in the list and re-indexes the remaining.
        /// Does not change the capacity of the list.
        /// If the list has any associated attributes, be sure to compact those first.
        /// </summary>
        public void Compact()
        {
            var n = Swim();
            _items.ClearRange(n, _count - n);
            _count = n;
        }


        /// <summary>
        /// 
        /// </summary>
        protected abstract int Swim();


        /// <summary>
        /// Removes all attributes corresponding with unused nodes.
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="attributes"></param>
        public void CompactAttributes<A>(List<A> attributes)
        {
            int marker = SwimAttributes(attributes);
            attributes.RemoveRange(marker, attributes.Count - marker);
        }


        /// <summary>
        /// Moves attributes corresponding with used nodes to the front of the given list.
        /// </summary>
        /// <param name="attributes"></param>
        public abstract int SwimAttributes<A>(IList<A> attributes);


        /// <summary>
        /// Moves attributes corresponding with used nodes to the front of the given array.
        /// </summary>
        /// <param name="attributes"></param>
        public abstract int SwimAttributes<A>(A[] attributes);


        /// <summary>
        /// Reorders the nodes in the list based on the given key.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="getKey"></param>
        public virtual void Sort<K>(Func<T, K> getKey)
            where K : IComparable<K>
        {
            var items = Items;
            int index = 0;

            // sort first
            foreach (var t in this.OrderBy(getKey))
                items[index++] = t;

            // re-index after since indices may be used to fetch keys
            for (int i = 0; i < items.Count; i++)
                items[i].Index = i;
        }


        /// <summary>
        /// Returns true if the given node belongs to this list.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool Owns(T node)
        {
            return node == _items[node.Index];
        }

        
        /// <summary>
        /// Throws an exception for nodes that don't belong to this list.
        /// </summary>
        /// <param name="node"></param>
        internal void OwnsCheck(T node)
        {
            const string errorMessage = "The given node must belong to this mesh.";

            if (!Owns(node))
                throw new ArgumentException(errorMessage);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        private void OwnsCheck(IEnumerable<T> nodes)
        {
            foreach (var e in nodes)
                OwnsCheck(e);
        }


        /// <summary>
        /// Performs the given action on all nodes in the list.
        /// Note that unused nodes are skipped.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="parallel"></param>
        public abstract void Action(Action<T> action, bool parallel = false);


        /// <summary>
        /// Returns unique nodes from the given collection (no duplicates).
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public IEnumerable<T> GetDistinct(IEnumerable<T> nodes)
        {
            OwnsCheck(nodes);
            return GetDistinctImpl(nodes);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        internal IEnumerable<T> GetDistinctImpl(IEnumerable<T> nodes)
        {
            int currTag = NextTag;

            foreach (var e in nodes)
            {
                if (e.Tag == currTag) continue;
                e.Tag = currTag;
                yield return e;
            }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodesA"></param>
        /// <param name="nodesB"></param>
        /// <returns></returns>
        public IEnumerable<T> GetUnion(IEnumerable<T> nodesA, IEnumerable<T> nodesB)
        {
            return GetDistinct(nodesA.Concat(nodesB));
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodesA"></param>
        /// <param name="nodesB"></param>
        /// <returns></returns>
        public IEnumerable<T> GetDifference(IEnumerable<T> nodesA, IEnumerable<T> nodesB)
        {
            OwnsCheck(nodesA);
            OwnsCheck(nodesB);
            return GetDifferenceImpl(nodesA, nodesB);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodesA"></param>
        /// <param name="nodesB"></param>
        /// <returns></returns>
        internal IEnumerable<T> GetDifferenceImpl(IEnumerable<T> nodesA, IEnumerable<T> nodesB)
        {
            int currTag = NextTag;

            // tag nodes in A
            foreach (var eB in nodesB)
                eB.Tag = currTag;

            // return tagged nodes in B
            foreach (var eA in nodesA)
                if (eA.Tag != currTag) yield return eA;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodesA"></param>
        /// <param name="nodesB"></param>
        /// <returns></returns>
        public IEnumerable<T> GetIntersection(IEnumerable<T> nodesA, IEnumerable<T> nodesB)
        {
            OwnsCheck(nodesA);
            OwnsCheck(nodesB);
            return GetIntersectionImpl(nodesA, nodesB);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodesA"></param>
        /// <param name="nodesB"></param>
        /// <returns></returns>
        internal IEnumerable<T> GetIntersectionImpl(IEnumerable<T> nodesA, IEnumerable<T> nodesB)
        {
            int currTag = NextTag;

            // tag nodes in A
            foreach (var eA in nodesA)
                eA.Tag = currTag;

            // return tagged nodes in B
            foreach (var eB in nodesB)
                if (eB.Tag == currTag) yield return eB;
        }


        #region Explicit Interface Implementations
        
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="E"></typeparam>
    [Serializable]
    public class NodeList<T, E> : HeNodeList<T>
        where T : HeNode<T, E>
        where E : Halfedge<E>
    {
        /// <summary>
        /// 
        /// </summary>
        public NodeList()
            :base()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public NodeList(int capacity)
            : base(capacity)
        {
        }


        /// <inheritdoc/>
        public override int CountUnused()
        {
            var items = Items;
            int result = 0;

            for (int i = 0; i < items.Count; i++)
                if (items[i].IsUnused) result++;

            return result;
        }


        /// <inheritdoc/>
        protected override int Swim()
        {
            var items = Items;
            int index = 0;

            for (int i = 0; i < items.Count; i++)
            {
                T node = items[i];
                if (node.IsUnused) continue; // skip unused nodes

                node.Index = index;
                items[index++] = node;
            }

            return index;
        }


        /// <inheritdoc/>
        public override int SwimAttributes<A>(IList<A> attributes)
        {
            if (attributes is A[] arr)
                return SwimAttributes(arr);

            var items = Items;
            int marker = 0;

            for (int i = 0; i < items.Count; i++)
            {
                if (!items[i].IsUnused)
                    attributes[marker++] = attributes[i];
            }

            return marker;
        }


        /// <inheritdoc/>
        public override int SwimAttributes<A>(A[] attributes)
        {
            var items = Items;
            int index = 0;

            for (int i = 0; i < items.Count; i++)
            {
                if (!items[i].IsUnused)
                    attributes[index++] = attributes[i];
            }

            return index;
        }


        /// <inheritdoc/>
        public override void Action(Action<T> action, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, Count);

            void Body(int from, int to)
            {
                var items = Items;
                
                for (int i = from; i < to; i++)
                {
                    var node = items[i];
                    if (!node.IsUnused) action(node);
                }
            }
        }
    }
}
