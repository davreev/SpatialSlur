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
    public class HeQuadStrip<V, E, F> : IEnumerable<E>
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
        internal HeQuadStrip(E first, E last)
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
                he = NextInStrip(he);
            } while (he != _last);

            yield return he; // return last
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<E> SkipFirst(int count)
        {
            return SkipFirstAndLast(count, 0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<E> SkipLast(int count)
        {
            return SkipFirstAndLast(0, count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstCount"></param>
        /// <param name="lastCount"></param>
        /// <returns></returns>
        public IEnumerable<E> SkipFirstAndLast(int firstCount, int lastCount)
        {
            var he0 = _first;
            var he1 = _last;

            // offset forward from first
            for (int i = 0; i < firstCount; i++)
            {
                he0 = NextInStrip(he0);
                if (he0 == he1) yield break; // break if crossed
            }

            // offset backwards from last
            for (int i = 0; i < lastCount; i++)
            {
                he1 = PrevInStrip(he1);
                if (he1 == he0) yield break; // break if crossed
            }

            // enumerate between
            do
            {
                yield return he0;
                he0 = NextInStrip(he0);
            } while (he0 != he1);

            yield return he0; // return last
        }


        /// <summary>
        /// Return false if the strip is not periodic.
        /// </summary>
        /// <returns></returns>
        public bool ShiftSeam(int count)
        {
            if (!IsPeriodic)
                return false;

            for(int i = 0; i < count; i++)
                _first = NextInStrip(_first);

            _last = _first;
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        private E NextInStrip(E hedge)
        {
            return hedge.NextInFace.NextInFace.Twin;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        private E PrevInStrip(E hedge)
        {
            return hedge.Twin.NextInFace.NextInFace;
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
}
