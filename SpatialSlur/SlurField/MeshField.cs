using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Rhino.Geometry;
using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MeshField
    {
        private readonly HeMesh _mesh;
        private Mesh _displayMesh; // underlying rhino mesh used for display and closest point queries
        private readonly int _n;


        /// <summary>
        /// Constructs a new mesh field from a given HeMesh instance.
        /// </summary>
        /// <param name="mesh"></param>
        protected MeshField(HeMesh mesh)
        {
            if (mesh == null)
                throw new ArgumentNullException("mesh");

            _mesh = mesh;
            _n = _mesh.Vertices.Count;
            RebuildDisplayMesh();
        }


        /// <summary>
        /// Constructs a new mesh field from a given Rhino mesh.
        /// </summary>
        /// <param name="mesh"></param>
        protected MeshField(Mesh mesh)
        {
            if (mesh == null)
                throw new ArgumentNullException("mesh");

            _mesh = RhinoFactory.CreateHeMesh(mesh);
            _n = _mesh.Vertices.Count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="duplicateMesh"></param>
        protected MeshField(MeshField field, bool duplicateMesh = false)
        {
            _mesh = (duplicateMesh) ? new HeMesh(field._mesh) : field._mesh;
            _n = field._n;
            RebuildDisplayMesh();
        }


        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return _n; }
        }


        /// <summary>
        /// Returns true if the number of vertices in the associated mesh no longer matches the number of values in the field.
        /// </summary>
        public bool IsExpired
        {
            get { return _n != _mesh.Vertices.Count; }
        }


        /// <summary>
        /// Returns the HeMesh instance associated with this field.
        /// Topological modifications made to this mesh should be followed by a call to Resize.
        /// Geometrical modifications made to this mesh should be followed by a call to RebuildDisplayMesh.
        /// </summary>
        public HeMesh Mesh
        {
            get { return _mesh; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Mesh DisplayMesh
        {
            get { return _displayMesh; }
        }

        
        /// <summary>
        /// This should be called after making any topological operations to the base mesh that change the number of vertices.
        /// </summary>
        public void Resize()
        {
            if (!IsExpired) return;

            // TODO
            throw new NotImplementedException();
        }
       

        /// <summary>
        /// This should be called before painting the display mesh if changes have been made to the underlying HeMesh instance.
        /// </summary>
        public void RebuildDisplayMesh()
        {
            _displayMesh = _mesh.ToRhinoMesh();
            _displayMesh.VertexColors.CreateMonotoneMesh(Color.Empty);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public MeshPoint ClosestMeshPoint(Vec3d point)
        {
            return _displayMesh.ClosestMeshPoint(point.ToPoint3d(), 0.0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="maxDistance"></param>
        public MeshPoint ClosestMeshPoint(Vec3d point, double maxDistance)
        {
            return _displayMesh.ClosestMeshPoint(point.ToPoint3d(), maxDistance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        internal void SizeCheck(MeshField other)
        {
            if (Count != other.Count)
                throw new ArgumentException("The two fields must have the same number of values.");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        internal void SizeCheck<T>(IList<T> list)
        {
            if (list.Count < _n)
                throw new ArgumentException("The number of items in the given list cannot be less than the number of values in the field.");
        }
  
    }
}
