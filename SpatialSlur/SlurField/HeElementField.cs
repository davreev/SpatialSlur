using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="T"></typeparam>
    public abstract class HeElementField<E, T> : IField<T>
        where E: HeElement, IHeElement
    {
        private const int MinCapacity = 4;
        
        private HeElementList<E> _elements;
        private T[] _values;
        private int _count;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        protected HeElementField(HeElementList<E> elements)
        {
            _elements = elements;
            _count = _elements.Count;
            _values = new T[Math.Max(_count, MinCapacity)];
        }


        /// <summary>
        /// 
        /// </summary>
        public T[] Values
        {
            get { return _values; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get { return _values[index]; }
            set { _values[index] = value; }
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
            get { return _values.Length; }
        }


        /// <summary>
        /// Removes values corresponding with unused elements.
        /// Should be called before compacting the element list.
        /// </summary>
        public void RemoveUnused()
        {
            _count = _elements.SwimAttributes(_values);
            TrimExcess();
        }


        /// <summary>
        /// Trims the field's array if capacity is greater than twice the count.
        /// </summary>
        public void TrimExcess()
        {
            int max = Math.Max(_count << 1, MinCapacity);

            if (_values.Length > max)
                Array.Resize(ref _values, max);
        }


        /// <summary>
        /// Syncs the field's array with the referenced element list, resizing it if necessary.
        /// </summary>
        public void Sync()
        {
            _count = _elements.Count;
            int min = _elements.Capacity;

            if (_values.Length < min)
                Array.Resize(ref _values, min);
        }
    }
}
