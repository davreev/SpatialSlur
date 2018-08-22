
/*
 * Notes
 */

using System;
using SpatialSlur.Collections;

namespace SpatialSlur.Meshes
{
    /// <summary>
    /// Simple face-vertex representation of a triangle mesh.
    /// </summary>
    public abstract class TriMesh<TVector>
        where TVector : struct
    {
        #region Static Members

        /// <summary>
        /// Adds the given item to the given buffer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        /// <param name="item"></param>
        private static void Add<T>(ref T[] buffer, int count, T item)
        {
            const int minCapacity = 4;

            if (buffer.Length == count)
                Array.Resize(ref buffer, Math.Max(count << 1, minCapacity));

            buffer[count] = item;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        private static void TrimExcess<T>(ref T[] buffer, int count)
        {
            int max = count << 1;
            
            if (buffer.Length > max)
                Array.Resize(ref buffer, max);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="b0"></param>
        /// <param name="n0"></param>
        /// <param name="b1"></param>
        /// <param name="n1"></param>
        private static void Append<T>(ref T[] b0, int n0, T[] b1, int n1)
        {
            var n = n0 + n1;

            if(b0.Length <= n)
                Array.Resize(ref b0, b0.Length + n);

            b0.SetRange(b1, n0, 0, n1);
        }

        #endregion


        private TVector[] _positions = Array.Empty<TVector>();
        private TVector[] _normals = Array.Empty<TVector>();
        private Vector3i[] _faces = Array.Empty<Vector3i>();

        private int _positionCount;
        private int _normalCount;
        private int _faceCount;


        /// <summary>
        /// 
        /// </summary>
        public TriMesh()
        {
        }

        
        /// <summary>
        /// 
        /// </summary>
        public TriMesh(int positionCapacity, int normalCapacity, int faceCapacity)
        {
            _positions = new TVector[positionCapacity];
            _normals = new TVector[normalCapacity];
            _faces = new Vector3i[faceCapacity];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public TriMesh(TriMesh<TVector> other)
        {
            _positions = other._positions.ShallowCopy();
            _normals = other._normals.ShallowCopy();
            _faces = other._faces.ShallowCopy();

            _positionCount = other._positionCount;
            _normalCount = other._normalCount;
            _faceCount = other._faceCount;
        }


        /// <summary>
        /// Returns the array of vertex positions
        /// </summary>
        public ArrayView<TVector> Positions
        {
            get { return _positions.AsView(_positionCount); }
        }


        /// <summary>
        /// Returns the array of vertex normals
        /// </summary>
        public ArrayView<TVector> Normals
        {
            get { return _normals.AsView(_normalCount); }
        }


        /// <summary>
        /// Returns the array of faces.
        /// </summary>
        public ArrayView<Vector3i> Faces
        {
            get { return _faces.AsView(_faceCount); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        public void AddPosition(TVector position)
        {
            Add(ref _positions, _positionCount++, position);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="normal"></param>
        public void AddNormal(TVector normal)
        {
            Add(ref _normals, _normalCount++, normal);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="face"></param>
        public void AddFace(Vector3i face)
        {
            Add(ref _faces, _faceCount++, face);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Append(TriMesh<TVector> other)
        {
            Append(ref _positions, _positionCount, other._positions, other._positionCount);
            Append(ref _normals, _normalCount, other._normals, other._normalCount);
            Append(ref _faces, _faceCount, other._faces, other._faceCount);
        }


        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _positionCount = _normalCount = _faceCount = 0;
        }


        /// <summary>
        /// 
        /// </summary>
        public void TrimExcess()
        {
            TrimExcess(ref _positions, _positionCount);
            TrimExcess(ref _normals, _normalCount);
            TrimExcess(ref _faces, _faceCount);
        }
    }
}
