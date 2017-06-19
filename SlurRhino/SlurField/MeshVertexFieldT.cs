using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;

using Rhino.Geometry;

/*
 * Notes
 */

namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MeshVertexField<T> : IField<T>, IField3d<T>
        where T : struct
    {
        private const int MinCapacity = 4;

        private Mesh _mesh;
        private T[] _values;
        private int _count;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public MeshVertexField(Mesh mesh)
        {
            _mesh = mesh;
            _count = _mesh.Vertices.Count;
            _values = new T[Math.Max(_count << 1, MinCapacity)];
        }


        /// <summary>
        /// 
        /// </summary>
        public Mesh Mesh
        {
            get { return _mesh; }
        }


        /// <inheritdoc/>
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


        /// <inheritdoc/>
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
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T ValueAt(Vec3d point)
        {
            var mp = _mesh.ClosestMeshPoint(point.ToPoint3d(), 0.0);
            return ValueAt(mp);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract T ValueAt(MeshPoint point);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if (_count == _values.Length)
                Array.Resize(ref _values, _values.Length << 1);

            _values[_count++] = item;
        }


        /// <summary>
        /// Removes all values corresponding with unused vertices in the mesh.
        /// </summary>
        /// <returns></returns>
        public int RemoveUnused()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Syncs the number of values in this field with the number of vertices in the referenced mesh.
        /// Resizes the internal array if necessary.
        /// </summary>
        public void Sync()
        {
            _count = _mesh.Vertices.Count;

            if (_count > _values.Length)
                Array.Resize(ref _values, _count << 1);
        }


        /// <summary>
        /// Shrinks the capacity of the internal array to twice the number of values.
        /// </summary>
        public void TrimExcess()
        {
            int max = Math.Max(_count << 1, MinCapacity);

            if (_values.Length > max)
                Array.Resize(ref _values, max); 
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public void PaintMesh(Func<T, Color> mapper, bool parallel = false)
        {
            _mesh.PaintByVertexValue(_values, mapper, parallel);
        }
    }
}
