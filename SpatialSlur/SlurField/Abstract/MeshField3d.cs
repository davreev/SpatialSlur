
/*
 * Notes 
 */

#if USING_RHINO

using System;
using System.Collections.Generic;
using System.Linq;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;
using SpatialSlur.SlurRhino;
using Rhino.Geometry;

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class MeshField3d
    {
#region Static

        /// <summary></summary>
        public static readonly MeshField3dFactory<double> Double = new MeshField3dDouble.Factory();
        
        /// <summary></summary>
        public static readonly MeshField3dFactory<Vec3d> Vec3d = new MeshField3dVec3d.Factory();

#endregion


        private HeMesh3d _mesh;
        private Mesh _queryMesh; // triangulated query mesh


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
                RebuildQueryMesh();
            }
        }


        /// <inheritdoc />
        public int Count
        {
            get { return _mesh.Vertices.Count; }
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
            return _queryMesh.ClosestMeshPoint(point, 0.0);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MeshField3d<T> : MeshField3d, IField2d<T>, IField3d<T>, IDiscreteField3d<T>
        where T : struct
    {
#region Static
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        public static implicit operator T[](MeshField3d<T> field)
        {
            return field.Values;
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
        public T[] Values
        {
            get { return _values; }
        }


        /// <inheritdoc />
        public T this[int index]
        {
            get
            {
                if (index >= Count)
                    throw new IndexOutOfRangeException();

                return _values[index];
            }
            set
            {
                if (index >= Count)
                    throw new IndexOutOfRangeException();

                _values[index] = value;
            }
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
            var cap = Mesh.Vertices.Capacity;

            if (_values.Length != cap)
                Array.Resize(ref _values, cap);
        }


        #region Explicit interface implementations

        /// <inheritdoc />
        IDiscreteField<T> IDiscreteField<T>.Duplicate(bool setValues)
        {
            return Duplicate(setValues);
        }


        /// <inheritdoc />
        IDiscreteField3d<T> IDiscreteField3d<T>.Duplicate(bool setValues)
        {
            return Duplicate(setValues);
        }


        /// <inheritdoc />
        T IField2d<T>.ValueAt(Vec2d point)
        {
            return ValueAt(point);
        }


        /// <inheritdoc />
        IEnumerable<Vec3d> IDiscreteField3d<T>.Coordinates
        {
            get { return Mesh.Vertices.Select(v => v.Position); }
        }


        /// <inheritdoc />
        Vec3d IDiscreteField3d<T>.CoordinateAt(int index)
        {
            return Mesh.Vertices[index].Position;
        }

#endregion
    }
}

#endif