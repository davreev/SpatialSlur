using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;
using SpatialSlur.SlurMesh;

using static SpatialSlur.SlurMesh.HeMesh3d;

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
        const string FileIn = "face.obj";
        const string FileOut = "face_mapped.obj";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main()
        {
            var mesh = CreateFromOBJ(Paths.Resources + FileIn);
            
            double scale = 0.5;
            double offset = scale * Math.PI * 0.25;

            // generates texture coordinates from an implicit surface function
            foreach (var v in mesh.Vertices)
            {
                var p = v.Position * scale;

                v.TexCoord = new Vec2d(
                    ImplicitSurfaces.Gyroid(p.X, p.Y, p.Z),
                    ImplicitSurfaces.Gyroid(p.X + offset, p.Y + offset, p.Z + offset)
                    );
            }
            
            // compute vertex normals & write to file
            mesh.GetVertexNormals(v => v.Position, (v, n) => v.Normal = n);
            mesh.WriteToOBJ(Paths.Resources + FileOut);

            Console.WriteLine("File written successfully. Press return to exit.");
            Console.ReadLine();
        }
    }
}
