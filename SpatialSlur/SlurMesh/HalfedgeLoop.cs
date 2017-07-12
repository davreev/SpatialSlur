using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    public class HalfedgeLoop<V, E> : IEnumerable<E>
        where V : HeVertex<V, E>
        where E : Halfedge<V, E>
    {
        private E _first;
        private E _last;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="last"></param>
        internal HalfedgeLoop(E first, E last)
        {
            _first = first;
            _last = last;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsPeriodic
        {
            get { return _first == _last; }
        }


        /// <summary>
        /// 
        /// /// </summary>
        /// <returns></returns>
        public IEnumerator<E> GetEnumerator()
        {
            var he = _first;

            do
            {
                yield return he;
                he = he.Twin.NextAtStart.NextAtStart;
            } while (he != _last);

            if (he != _first)
                yield return he;
        }


        #region Explicit Implementations

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }


    /// <summary>
    /// 
    /// </summary>
    internal static class HalfedgeLoop
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <returns></returns>
        internal static HalfedgeLoop<V, E> Create<V, E>(E first, E last)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
        {
            return new HalfedgeLoop<V, E>(first, last);
        }
    }
}
