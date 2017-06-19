using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class MeshVertexVectorField : MeshVertexField<Vec3d>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public MeshVertexVectorField(Mesh mesh)
            : base(mesh)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MeshVertexVectorField Duplicate()
        {
            var copy = new MeshVertexVectorField(Mesh);
            copy.Set(this);
            return copy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override Vec3d ValueAt(MeshPoint point)
        {
            var f = point.Mesh.Faces[point.FaceIndex];
            var w = point.T;
            return this[f[0]] * w[0] + this[f[1]] * w[1] + this[f[2]] * w[2];
        }
    }
}
