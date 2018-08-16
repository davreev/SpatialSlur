
/*
 * Notes 
 */

#if USING_RHINO

using System;
using System.Collections.Generic;
using System.Linq;

using SpatialSlur.Collections;
using SpatialSlur.Fields;
using SpatialSlur.Meshes;
using SpatialSlur.Rhino;
using Rhino.Geometry;

namespace SpatialSlur.Fields
{
    using V = HeMesh3d.Vertex;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class MeshField3d
    {
        #region Static members

        /// <summary></summary>
        public static readonly MeshField3dFactory<double> Double = new MeshField3dDouble.Factory();

        /// <summary></summary>
        public static readonly MeshField3dFactory<Vector3d> Vector3d = new MeshField3dVector3d.Factory();

        #endregion


        private HeMesh3d _mesh;
        private NodeList<V> _vertices;
        private Mesh _queryMesh; // triangulated mesh for closest point queries and raycasts


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public MeshField3d(HeMesh3d mesh)
        {
            Mesh = mesh;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public MeshField3d(MeshField3d other)
        {
            _mesh = other._mesh;
            _vertices = other._vertices;
            _queryMesh = other._queryMesh;
        }


        /// <summary>
        /// Returns the mesh referenced by this field.
        /// </summary>
        public HeMesh3d Mesh
        {
            get { return _mesh; }
            set
            {
                _mesh = value ?? throw new ArgumentNullException();
                _vertices = _mesh.Vertices;
                RebuildQueryMesh();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected NodeList<V> Vertices
        {
            get { return _vertices; }
        }


        /// <inheritdoc />
        public int Count
        {
            get { return _vertices.Count; }
        }

        
        /// <summary>
        /// This must be called after making changes to the referenced mesh
        /// </summary>
        public void RebuildQueryMesh()
        {
            _queryMesh = _mesh.ToMesh();
            _queryMesh.Faces.ConvertQuadsToTriangles();
        }


        /// <summary>
        /// Returns the closest mesh point
        /// </summary>
        /// <param name="point"></param>
        public MeshPoint ClosestMeshPoint(Vector3d point)
        {
            return _queryMesh.ClosestMeshPoint(point, 0.0);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class MeshField3d<T> : MeshField3d, IField2d<T>, IField3d<T>, ISampledField3d<T>
        where T : struct
    {
        #region Static members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        public static implicit operator ArrayView<T>(MeshField3d<T> field)
        {
            return field._values.AsView(field.Count);
        }

        #endregion


        private T[] _values;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public MeshField3d(HeMesh3d mesh)
            : base(mesh)
        {
            _values = new T[Count];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public MeshField3d(MeshField3d other)
            : base(other)
        {
            _values = new T[Count];
        }


        /// <inheritdoc />
        public ArrayView<T> Values
        {
            get { return _values.AsView(Count); }
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
        /// <returns></returns>
        public MeshField3d<T> Duplicate()
        {
            return Duplicate(true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract MeshField3d<T> Duplicate(bool setValues);


        /// <inheritdoc />
        public T ValueAt(Vector3d point)
        {
            return ValueAt(ClosestMeshPoint(point));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T ValueAt(MeshPoint point)
        {
            var f = point.Mesh.Faces[point.FaceIndex];
            var t = point.T;

            return ValueAt(
                new Vector3d(t[0], t[1], t[2]),
                new Vector3i(f.A, f.B, f.C));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="indices"></param>
        /// <returns></returns>
        public abstract T ValueAt(Vector3d weights, Vector3i indices);


        /// <summary>
        /// 
        /// </summary>
        public void EnsureCapacity()
        {
            if (_values.Length < Vertices.Count)
                Array.Resize(ref _values, Vertices.Capacity);
        }


        /// <summary>
        /// 
        /// </summary>
        public void TrimExcess()
        {
            int max = Count << 1;

            if (_values.Length > max)
                Array.Resize(ref _values, max);
        }


        #region Explicit interface implementations
        
        ISampledField<T> ISampledField<T>.Duplicate(bool setValues)
        {
            return Duplicate(setValues);
        }

        
        ISampledField3d<T> ISampledField3d<T>.Duplicate(bool setValues)
        {
            return Duplicate(setValues);
        }


        T IField2d<T>.ValueAt(Vector2d point)
        {
            return ValueAt(point.As3d);
        }


        IEnumerable<Vector3d> ISampledField3d<T>.Points
        {
            get { return Mesh.Vertices.Select(v => v.Position); }
        }


        Vector3d ISampledField3d<T>.PointAt(int index)
        {
            return Mesh.Vertices[index].Position;
        }

        #endregion
    }
}

#endif