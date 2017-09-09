using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;
using SpatialSlur.SlurMesh;

using Rhino.Geometry;

/*
 * Notes 
 */
 
namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MeshVertexField : IMeshField
    {
        private HeMesh3d _mesh;
        private Mesh _queryMesh; // triangulated query mesh


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public MeshVertexField(HeMesh3d mesh)
        {
            _mesh = mesh;
            RebuildQueryMesh();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public MeshVertexField(MeshVertexField other)
        {
            _mesh = other._mesh;
            _queryMesh = other._queryMesh;
        }


        /// <summary>
        /// Returns the mesh referenced by this field.
        /// </summary>
        public HeMesh3d Mesh
        {
            get { return _mesh; }
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
        public MeshPoint ClosestMeshPoint(Vec3d point)
        {
            return _queryMesh.ClosestMeshPoint(point.ToPoint3d(), 0.0);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MeshVertexField<T> : MeshVertexField, IMeshField<T>, IField2d<T>, IField3d<T>, IDiscreteField3d<T>
        where T : struct
    {
        private const int _minCapacity = 4;
        private T[] _values;
        private int _count;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public MeshVertexField(HeMesh3d mesh)
            : base(mesh)
        {
            Init();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public MeshVertexField(MeshVertexField other)
            : base(other)
        {
            Init();
        }


        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            var verts = Mesh.Vertices;
            _values = new T[verts.Capacity];
            _count = verts.Count;
        }


        /// <summary>
        /// Returns a reference to the under
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
            get
            {
                if (index >= _count)
                    throw new IndexOutOfRangeException();

                return _values[index];
            }
            set
            {
                if (index >= _count)
                    throw new IndexOutOfRangeException();

                _values[index] = value;
            }
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
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract MeshVertexField<T> DuplicateBase(bool copyValues);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T ValueAt(Vec3d point)
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
            var w = point.T;

            return ValueAt(f.A, f.B, f.C, w[0], w[1], w[2]);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <param name="w0"></param>
        /// <param name="w1"></param>
        /// <param name="w2"></param>
        /// <returns></returns>
        public abstract T ValueAt(int i0, int i1, int i2, double w0, double w1, double w2);


        /// <summary>
        /// Resizes the internal array to match the capacity of the vertex list in the referenced mesh.
        /// </summary>
        public void Resize()
        {
            var verts = Mesh.Vertices;
            _count = verts.Count;

            if (_values.Length != verts.Capacity)
                Array.Resize(ref _values, verts.Capacity);
        }


        #region Explicit interface implementations

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDiscreteField<T> IDiscreteField<T>.Duplicate(bool copyValues)
        {
            return DuplicateBase(copyValues);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDiscreteField3d<T> IDiscreteField3d<T>.Duplicate(bool copyValues)
        {
            return DuplicateBase(copyValues);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        T IField2d<T>.ValueAt(Vec2d point)
        {
            return ValueAt(point);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Vec3d> IDiscreteField3d<T>.Coordinates
        {
            get { return Mesh.Vertices.Select(v => v.Position); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Vec3d IDiscreteField3d<T>.CoordinateAt(int index)
        {
            return Mesh.Vertices[index].Position;
        }

        #endregion
    }
}
