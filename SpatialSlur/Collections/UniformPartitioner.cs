
/*
 * Notes
 * 
 * TODO: Implement struct enumerator
 */

using System;
using System.Collections;
using System.Collections.Generic;
using static SpatialSlur.SlurMath;
using static SpatialSlur.Utilities;

namespace SpatialSlur.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct UniformPartitioner : IEnumerable<(int From, int To)>
    {
        #region Static Members

        private static readonly int _defaultCount = Environment.ProcessorCount << 3; // TODO: Test other values

        #endregion


        private readonly int _from;
        private readonly int _to;
        private readonly int _count;
        private readonly int _stride;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
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
        public (int From, int To) this[int index]
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
        public IEnumerator<(int From, int To)> GetEnumerator()
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


        #region Explicit Interface Implementations
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
