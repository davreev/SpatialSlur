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
        /// Constructs a new mesh field using topology information from an existing mesh
        /// </summary>
        /// <param name="mesh"></param>
        protected MeshField(HeMesh mesh)
        {
            if (mesh == null)
                throw new ArgumentNullException("mesh");

            _mesh = mesh;
            _n = mesh.Vertices.Count;
            RebuildDisplayMesh();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="duplicateMesh"></param>
        protected MeshField(MeshField field, bool duplicateMesh = false)
        {
            _mesh = (duplicateMesh) ? field._mesh.Duplicate() : field._mesh;
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
        /// Returns true if the number of vertices in the associated mesh no longer matches the size of the field
        /// </summary>
        public bool IsExpired
        {
            get { return _n != _mesh.Vertices.Count; }
        }


        /// <summary>
        /// returns the mesh associated with this field
        /// any geometrical modifications made to this mesh should be followed by a call to RebuildDisplayMesh()
        /// any topological modifications made to this mesh should be followed by a call to Resize()
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
        /// returns a shallow copy of the field
        /// the new field will reference the same underlying mesh
        /// </summary>
        /// <returns></returns>
        public abstract MeshField Duplicate();


        /// <summary>
        /// returns a deep copy of the field
        /// the new field will reference a copy of the underlying mesh
        /// </summary>
        /// <returns></returns>
        public abstract MeshField DuplicateDeep();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        protected void SizeCheck(MeshField other)
        {
            if (Count != other.Count)
                throw new ArgumentException("The two fields must have the same number of values.");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        protected void SizeCheck<T>(IList<T> list)
        {
            if (list.Count != _n)
                throw new ArgumentException("The number of items in the given list must match the number of values in the field.");
        }

        
        /*
        /// <summary>
        /// 
        /// </summary>
        public void Resize()
        {
            if (!IsExpired) return;

            // resize _values by compacting or expanding
            // reset _n
            throw new NotImplementedException();
        }
        */


        /// <summary>
        /// this must be called after any changes to the mesh used by this field
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
    }
}
