
/*
 * Notes
 */

#if USING_RHINO

using System.Collections.Generic;

using Rhino.Geometry;
using SpatialSlur;
using SpatialSlur.Fields;

namespace SpatialSlur.Rhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class GridField3dExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="selection"></param>
        /// <returns></returns>
        public static Mesh ToPolySoup(this Grid3d field, IEnumerable<int> selection)
        {
            var mesh = new Mesh();
            var verts = mesh.Vertices;
            var faces = mesh.Faces;

            (var dx, var dy) = (field.Scale.XY * 0.5);

            // add vertices
            foreach (int index in selection)
            {
                (var x, var y, var z) = field.ToWorldSpace(index);
                verts.Add(x - dx, y - dy, z);
                verts.Add(x + dx, y - dy, z);
                verts.Add(x - dx, y + dy, z);
                verts.Add(x + dx, y + dy, z);
            }

            // add faces
            for (int i = 0; i < verts.Count; i += 4)
                faces.AddFace(i, i + 1, i + 3, i + 2);

            return mesh;
        }
    }
}

#endif