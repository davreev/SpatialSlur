
/*
 * Notes
 */ 

using System;
using System.Collections;
using System.Collections.Generic;

using static SpatialSlur.SlurCore.SlurMath;
using static SpatialSlur.SlurCore.CoreUtil;

namespace SpatialSlur.SlurData
{

    /// <summary>
    /// 
    /// </summary>
    public struct UniformPartitioner : IEnumerable<(int, int)>
    {
        #region Static

        private static readonly int _defaultCount = Environment.ProcessorCount;

        #endregion


        private int _from;
        private int _to;
        private int _count;
        private int _stride;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="count"></param>
        public UniformPartitioner(int from, int to)
            : this(from, to, _defaultCount)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="count"></param>
        public UniformPartitioner(int from, int to, int count)
        {
            if (from < 0 || to <= from || count < 0)
                throw new ArgumentOutOfRangeException();

            _from = from;
            _to = to;
            _count = count;
            _stride = DivideCeil(to - from, count);
        }


        /// <summary>
        /// Returns the partition at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public (int, int) this[int index]
        {
            get
            {
                BoundsCheck(index, _count);
                index = _from + index * _stride;
                return (index, Math.Min(index + _stride, _to));
            }
        }


        /// <summary>
        /// Returns the number of partitions.
        /// </summary>
        public int Count
        {
            get => _count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<(int, int)> GetEnumerator()
        {
            int i0 = _from;
            int i1 = _from + _stride;

            while (i1 < _to)
            {
                yield return (i0, i1);

                i0 = i1;
                i1 += _stride;
            }

            yield return (i0, _to);
        }


        #region Explicit interface implementations

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
