using System;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;
using SpatialSlur.SlurMesh;

/*
 * Notes
 */

namespace SpatialSlur.Examples
{
    /// <summary>
    /// 
    /// </summary>
    static class HeMeshFromFile
    {
        const string _fileIn = "face.obj";
        const string _fileOut = "face_mapped.obj";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Start()
        {
            var mesh = HeMesh3d.Factory.CreateFromOBJ(Paths.Resources + _fileIn);
            var verts = mesh.Vertices;

            var texCoords = new Vec2d[verts.Count];
            double scale = 0.5;
            double offset = scale * Math.PI * 0.25;

            // create texture coordinates from implicit function
            for (int i = 0; i < verts.Count; i++)
            {
                var p = verts[i].Position * scale;
                var u = ImplicitSurfaces.Gyroid(p.X, p.Y, p.Z);
                var v = ImplicitSurfaces.Gyroid(p.X + offset, p.Y + offset, p.Z + offset);
                texCoords[i] = new Vec2d(u, v);
            }

            // compute vertex normals & write to file
            mesh.UpdateVertexNormals();
            MeshIO.WriteToOBJ(mesh, Paths.Resources + _fileOut, v => texCoords[v]);

            Console.WriteLine("File written successfully. Press return to exit.");
            Console.ReadLine();
        }
    }
}
