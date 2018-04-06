
/*
 * Notes
 */

#if USING_RHINO

using System.Collections.Generic;

using Rhino.Geometry;
using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;

namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class GridField3dExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="selection"></param>
        /// <returns></returns>
        public static Mesh MeshSelection(this Grid3d field, IEnumerable<int> selection)
        {
            var mesh = new Mesh();
            var verts = mesh.Vertices;
            var faces = mesh.Faces;

            (var dx, var dy) = ((Vec2d)field.Scale * 0.5);

            // add vertices
            foreach (int index in selection)
            {
                var p = field.CoordinateAt(index);
                verts.Add(p.X - dx, p.Y - dy, p.Z);
                verts.Add(p.X + dx, p.Y - dy, p.Z);
                verts.Add(p.X - dx, p.Y + dy, p.Z);
                verts.Add(p.X + dx, p.Y + dy, p.Z);
            }

            // add faces
            for (int i = 0; i < verts.Count; i += 4)
                faces.AddFace(i, i + 1, i + 3, i + 2);

            return mesh;
        }
    }
}

#endif