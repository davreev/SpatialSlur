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
    public class HalfedgeStrip<V, E, F> : IEnumerable<E>
        where V : HeVertex<V, E, F>
        where E : Halfedge<V, E, F>
        where F : HeFace<V, E, F>
    {
        private E _first;
        private E _last;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="last"></param>
        internal HalfedgeStrip(E first, E last)
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
                he = he.NextInFace.NextInFace.Twin;
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
    internal static class HalfedgeStrip
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <returns></returns>
        internal static HalfedgeStrip<V, E, F> Create<V, E, F>(E first, E last)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return new HalfedgeStrip<V, E, F>(first, last);
        }
    }

}
