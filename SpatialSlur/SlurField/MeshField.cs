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
    public abstract class MeshField
    {
        private readonly HeMesh _mesh;
        private readonly int _n;


        /// <summary>
        /// Constructs a new mesh field from a given HeMesh instance.
        /// </summary>
        /// <param name="mesh"></param>
        protected MeshField(HeMesh mesh)
        {
            _mesh = mesh;
            _n = _mesh.Vertices.Count;
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
        /// </summary>
        public HeMesh Mesh
        {
            get { return _mesh; }
        }

        
        /// <summary>
        /// This should be called after making any topological operations to the base mesh that change the number of vertices.
        /// </summary>
        public void Resize()
        {
            // TODO
            throw new NotImplementedException();

            // if (!IsExpired) return;
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
