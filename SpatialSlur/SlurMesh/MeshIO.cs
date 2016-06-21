using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// Various static methods for exporting and importing mesh data.
    /// </summary>
    public static class MeshIO
    {
        private static string _objHeader = "# This .obj file was created by SpatialSlur";
        private static char[] _objSeparators = new char[]{ ' ', '\t', '\0' };
        private static char[] _objFaceSeparator = new char[] { '/' };


        /// <summary>
        /// Writes the given HeMesh to file in .obj format.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mesh"></param>
        public static void WriteObj(HeMesh mesh, string path)
        {
            var verts = mesh.Vertices;
            var faces = mesh.Faces;

            using (var writer = new StreamWriter(path, false, Encoding.ASCII))
            {
                writer.WriteLine(_objHeader);
                writer.WriteLine();

                // write vertices
                for (int i = 0; i < verts.Count; i++)
                {
                    HeVertex v = verts[i];
                    if (v.IsUnused) continue;

                    Vec3d p = v.Position;
                    writer.WriteLine("v {0} {1} {2}", p.x, p.y, p.z);
                }

                // write faces
                for (int i = 0; i < faces.Count; i++)
                {
                    HeFace f = faces[i];
                    if (f.IsUnused) continue;

                    writer.Write("f");
                    foreach (HeVertex v in f.Vertices)
                        writer.Write(" {0}", v.Index + 1);

                    writer.WriteLine();
                }
            }
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mesh"></param>
        /// <param name="uvs"></param>
        public static void WriteObj(HeMesh mesh, IList<Vec2d> uvs, string path)
        {
            var verts = mesh.Vertices;
            var faces = mesh.Faces;
            verts.SizeCheck(uvs);

            using (var writer = new StreamWriter(path, false, Encoding.ASCII))
            {
                writer.WriteLine(_objHeader);
                writer.WriteLine();

                // write vertices
                for (int i = 0; i < verts.Count; i++)
                {
                    HeVertex v = verts[i];
                    if (v.IsUnused) continue;
                    Vec3d p = v.Position;
                    writer.WriteLine("v {0} {1} {2}", p.x, p.y, p.z);
                }

                // write uvs
                for (int i = 0; i < verts.Count; i++)
                {
                    if (verts[i].IsUnused) continue;
                    Vec2d uv = uvs[i];
                    writer.WriteLine("vt {0} {1}", uv.x, uv.y);
                }

                // write faces
                for (int i = 0; i < faces.Count; i++)
                {
                    HeFace f = faces[i];
                    if (f.IsUnused) continue;

                    writer.Write("f");
                    foreach (HeVertex v in f.Vertices)
                        writer.Write(" {0}/{0}", v.Index + 1);

                    writer.WriteLine();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mesh"></param>
        /// <param name="normals"></param>
        public static void WriteObj(HeMesh mesh, IList<Vec3d> normals, string path)
        {
            var verts = mesh.Vertices;
            var faces = mesh.Faces;
            verts.SizeCheck(normals);

            using (var writer = new StreamWriter(path, false, Encoding.ASCII))
            {
                writer.WriteLine(_objHeader);
                writer.WriteLine();

                // write vertices
                for (int i = 0; i < verts.Count; i++)
                {
                    HeVertex v = verts[i];
                    if (v.IsUnused) continue;
                    Vec3d p = v.Position;
                    writer.WriteLine("v {0} {1} {2}", p.x, p.y, p.z);
                }

                // write normals
                for (int i = 0; i < verts.Count; i++)
                {
                    if (verts[i].IsUnused) continue;
                    Vec3d n = normals[i];
                    writer.WriteLine("vn {0} {1} {2}", n.x, n.y, n.z);
                }

                // write faces
                for (int i = 0; i < faces.Count; i++)
                {
                    HeFace f = faces[i];
                    if (f.IsUnused) continue;

                    writer.Write("f");
                    foreach (HeVertex v in f.Vertices)
                        writer.Write(" {0}//{0}", v.Index + 1);

                    writer.WriteLine();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mesh"></param>
        /// <param name="uvs"></param>
        /// <param name="normals"></param>
        public static void WriteObj(HeMesh mesh, IList<Vec2d> uvs, IList<Vec3d> normals, string path)
        {
            var verts = mesh.Vertices;
            var faces = mesh.Faces;
            verts.SizeCheck(uvs);
            verts.SizeCheck(normals);

            using (var writer = new StreamWriter(path, false, Encoding.ASCII))
            {
                writer.WriteLine(_objHeader);
                writer.WriteLine();

                // write vertices
                for (int i = 0; i < verts.Count; i++)
                {
                    HeVertex v = verts[i];
                    if (v.IsUnused) continue;
                    Vec3d p = v.Position;
                    writer.WriteLine("v {0} {1} {2}", p.x, p.y, p.z);
                }

                // write uvs
                for (int i = 0; i < verts.Count; i++)
                {
                    if (verts[i].IsUnused) continue;
                    Vec2d uv = uvs[i];
                    writer.WriteLine("vt {0} {1}", uv.x, uv.y);
                }

                // write normals
                for (int i = 0; i < verts.Count; i++)
                {
                    if (verts[i].IsUnused) continue;
                    Vec3d n = normals[i];
                    writer.WriteLine("vn {0} {1} {2}", n.x, n.y, n.z);
                }

                // write faces
                for (int i = 0; i < faces.Count; i++)
                {
                    HeFace f = faces[i];
                    if (f.IsUnused) continue;

                    writer.Write("f");
                    foreach (HeVertex v in f.Vertices)
                        writer.Write(" {0}/{0}/{0}", v.Index + 1);

                    writer.WriteLine();
                }
            }
        }


        /// <summary>
        /// Creates an HeMesh instance from an .obj file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static HeMesh ReadObj(string path)
        {
            HeMesh mesh = new HeMesh();
            var verts = mesh.Vertices;
            var faces = mesh.Faces;
       
            using (var reader = new StreamReader(path, Encoding.ASCII))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    // skip empty lines and comments
                    if (line.Length == 0 || line[0] == '#') continue;

                    // check the first character
                    var segments = line.Split(_objSeparators, StringSplitOptions.RemoveEmptyEntries);
                    switch (segments[0])
                    {
                        case "v":
                            {
                                // parse vertex
                                double x = Double.Parse(segments[1]);
                                double y = Double.Parse(segments[2]);
                                double z = Double.Parse(segments[3]);

                                verts.Add(x, y, z);
                                break;
                            }
                        case "f":
                            {
                                // parse face
                                List<int> face = new List<int>();
                                for (int i = 1; i < segments.Length; i++)
                                {
                                    var ids = segments[i].Split(_objFaceSeparator);
                                    face.Add(Int32.Parse(ids[0]) - 1);
                                }

                                faces.Add(face);
                                break;
                            }
                    }
                }
            }

            return mesh;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="uvs"></param>
        /// <returns></returns>
        public static HeMesh ReadObj(string path, out IList<Vec2d> uvs)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="uvs"></param>
        /// <param name="normals"></param>
        /// <returns></returns>
        public static HeMesh ReadObj(string path, out IList<Vec2d> uvs, out IList<Vec3d> normals)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
