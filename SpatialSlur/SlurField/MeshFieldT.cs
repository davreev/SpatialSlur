using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using SpatialSlur.SlurMesh;
using SpatialSlur.SlurCore;

/*
* Notes
*/

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MeshField<T> : IField<T>
    {
        private readonly HeMesh _mesh;
        private T[] _values;
        private int _count;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        protected MeshField(HeMesh mesh)
        {
            _mesh = mesh;
            _count = _mesh.Vertices.Count;
            _values = new T[_count];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        protected MeshField(MeshField<T> other)
        {
            _mesh = other.Mesh;
            _values = new T[other.Capacity];
            _values.Set(other._values);
            _count = other._count;
        }


        /// <summary>
        /// Returns the HeMesh instance associated with this field.
        /// </summary>
        public HeMesh Mesh
        {
            get { return _mesh; }
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
        /// Compacts the field's array according to used vertices in the mesh.
        /// Should be called before compacting the mesh vertex list.
        /// </summary>
        public void Compact()
        {
            _count = _mesh.Vertices.CompactAttributes(_values);
            TrimExcess();
        }


        /// <summary>
        /// Trims the field's array if capacity is greater than twice the count.
        /// </summary>
        public void TrimExcess()
        {
            int maxCapacity = Math.Max(_count << 1, 2);

            if (Capacity > maxCapacity)
                Array.Resize(ref _values, maxCapacity);

            // prevent object loitering
            Array.Clear(_values, _count, Capacity - _count);
        }


        /// <summary>
        /// Ensures the capacity of the field's array is greater than or equal to the number of vertices in the mesh.
        /// </summary>
        public void EnsureCapacity()
        {
            _count = _mesh.Vertices.Count;

            if (_count > Capacity)
                Array.Resize(ref _values, _count << 1);
        }


        /// <summary>
        /// Assumes weights sum to 1.0.
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <param name="w0"></param>
        /// <param name="w1"></param>
        /// <param name="w2"></param>
        /// <returns></returns>
        public abstract T ValueAt(int i0, int i1, int i2, double w0, double w1, double w2);
    }
}
