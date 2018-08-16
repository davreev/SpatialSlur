
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;

using SpatialSlur;
using SpatialSlur.Collections;
using SpatialSlur.Meshes.Impl;

namespace SpatialSlur.Meshes
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="E"></typeparam>
    [Serializable]
    public class HalfedgeList<E> : NodeList<E>
        where E : Halfedge<E>
    {
        /// <summary>
        /// 
        /// </summary>
        public HalfedgeList()
            :base()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public HalfedgeList(int capacity)
            : base(capacity)
        {
        }


        /// <inheritdoc/>
        public override int CountUnused()
        {
            var hedges = AsView();
            int result = 0;

            for (int i = 0; i < hedges.Count; i += 2)
                if (hedges[i].IsUnused) result += 2;

            return result;
        }


        /// <inheritdoc/>
        protected override int Swim()
        {
            var hedges = AsView();
            int index = 0;

            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he = hedges[i];
                if (he.IsUnused) continue; // skip unused halfedge pairs

                he.Index = index;
                hedges[index++] = he;

                he = he.Twin;

                he.Index = index;
                hedges[index++] = he;
            }

            hedges.Subview(index, hedges.Count - index).Clear();
            return index;
        }


        /// <inheritdoc/>
        public override int SwimAttributes<A>(IList<A> attributes)
        {
            if (attributes is A[] arr)
                return SwimAttributes(arr);
            
            var hedges = AsView();
            int index = 0;

            for (int i = 0; i < hedges.Count; i += 2)
            {
                if (hedges[i].IsUnused) continue; // skip unused halfedge pairs
                attributes[index++] = attributes[i];
                attributes[index++] = attributes[i + 1];
            }

            return index;
        }


        /// <inheritdoc/>
        public override int SwimAttributes<A>(A[] attributes)
        {
            var hedges = AsView();
            int index = 0;

            for (int i = 0; i < hedges.Count; i += 2)
            {
                if (hedges[i].IsUnused) continue; // skip unused halfedge pairs
                attributes[index++] = attributes[i];
                attributes[index++] = attributes[i + 1];
            }

            return index;
        }


        /// <summary>
        /// Reorders halfedge pairs based on the given key.
        /// Note that the key is taken from the first halfedge in each pair.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="getKey"></param>
        public override void Sort<K>(Func<E, K> getKey)
        {
            var hedges = AsView();
            int index = 0;

            // sort in pairs
            foreach (var he0 in Edges().OrderBy(getKey))
            {
                hedges[index++] = he0;
                hedges[index++] = he0.Twin;
            }

            // re-index after since indices may be used to fetch sort keys
            for (int i = 0; i < hedges.Count; i++)
                hedges[i].Index = i;

            // returns the first halfedge from each pair
            IEnumerable<E> Edges()
            {
                for (int i = 0; i < hedges.Count; i += 2)
                    yield return hedges[i];
            }
        }


        /// <inheritdoc/>
        public override void Action(Action<E> action, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, Count);

            void Body(int from, int to)
            {
                var hedges = AsView();

                for (int i = from; i < to; i++)
                {
                    var he = hedges[i];
                    if (!he.IsUnused) action(he);
                }
            }
        }
    }
}
