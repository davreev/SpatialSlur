using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// Static methods for importing and exporting SlurMesh types.
    /// </summary>
    public static class MeshIO
    {
        #region Nested types

        /// <summary>
        /// Json serializable representation of a halfedge graph
        /// </summary>
        [DataContract]
        private class HeJsonBuffer
        {
            [DataMember(Name = "Vertices", Order = 0)]
            private int[] _vertexRefs;

            [DataMember(Name = "Halfedges", Order = 1)]
            private int[][] _hedgeRefs;

            [DataMember(Name = "Faces", Order = 2)]
            private int[] _faceRefs;

            [DataMember(Name = "VertexAttributes", Order = 3)]
            private object[][] _vertexAttributes = Array.Empty<object[]>();

            [DataMember(Name = "HalfedgeAttributes", Order = 4)]
            private object[][] _hedgeAttributes = Array.Empty<object[]>();

            [DataMember(Name = "FaceAttributes", Order = 5)]
            private object[][] _faceAttributes = Array.Empty<object[]>();


            /// <summary>
            /// Writes the given graph to this buffer.
            /// </summary>
            /// <param name="graph"></param>
            public void WriteFrom<TV, TE>(HeGraph<TV, TE> graph, Func<TV, IEnumerable<object>> getVertexAttributes = null, Func<TE, IEnumerable<object>> getHedgeAttributes = null)
                where TV : HeGraph<TV, TE>.Vertex
                where TE : HeGraph<TV, TE>.Halfedge
            {
                var verts = graph.Vertices;
                var hedges = graph.Halfedges;

                _vertexRefs = new int[verts.Count];
                _hedgeRefs = new int[hedges.Count][];

                // write vertex topology
                for (int i = 0; i < verts.Count; i++)
                {
                    var v = verts[i];
                    _vertexRefs[i] = v.IsUnused ? -1 : v.First;
                }

                // write halfedge topology
                for (int i = 0; i < hedges.Count; i++)
                {
                    var he = hedges[i];

                    _hedgeRefs[i] = new int[]
                    {
                        he.PreviousAtStart,
                        he.NextAtStart,
                        he.IsUnused ? -1 : he.Start
                    };
                }

                // write vertex attributes
                if (getVertexAttributes != null)
                {
                    _vertexAttributes = new object[verts.Count][];

                    for (int i = 0; i < verts.Count; i++)
                        _vertexAttributes[i] = getVertexAttributes(verts[i]).ToArray();
                }

                // write halfedge attributes
                if (getHedgeAttributes != null)
                {
                    _hedgeAttributes = new object[hedges.Count][];

                    for (int i = 0; i < hedges.Count; i++)
                        _hedgeAttributes[i] = getHedgeAttributes(hedges[i]).ToArray();
                }
            }


            /// <summary>
            /// Writes the given mesh to this buffer.
            /// </summary>
            /// <param name="graph"></param>
            public void WriteFrom<TV, TE, TF>(HeMesh<TV, TE, TF> mesh, Func<TV, IEnumerable<object>> getVertexAttributes = null, Func<TE, IEnumerable<object>> getHedgeAttributes = null, Func<TF, IEnumerable<object>> getFaceAttributes = null)
                where TV : HeMesh<TV, TE, TF>.Vertex
                where TE : HeMesh<TV, TE, TF>.Halfedge
                where TF : HeMesh<TV, TE, TF>.Face
            {
                var verts = mesh.Vertices;
                var hedges = mesh.Halfedges;
                var faces = mesh.Faces;

                _vertexRefs = new int[verts.Count];
                _hedgeRefs = new int[hedges.Count][];
                _faceRefs = new int[faces.Count];

                // write vertex topology
                for (int i = 0; i < verts.Count; i++)
                {
                    var v = verts[i];
                    _vertexRefs[i] = v.IsUnused ? -1 : v.First;
                }

                // write halfedge topology
                for (int i = 0; i < hedges.Count; i++)
                {
                    var he = hedges[i];

                    _hedgeRefs[i] = new int[]
                    {
                         he.PreviousAtStart,
                         he.NextAtStart,
                         he.IsUnused ? -1 : he.Start,
                         he.IsHole ? -1 : he.Face
                    };
                }

                // write face topology
                for (int i = 0; i < faces.Count; i++)
                {
                    var f = faces[i];
                    _faceRefs[i] = f.IsUnused ? -1 : f.First;
                }

                // write vertex attributes
                if (getVertexAttributes != null)
                {
                    _vertexAttributes = new object[verts.Count][];

                    for (int i = 0; i < verts.Count; i++)
                        _vertexAttributes[i] = getVertexAttributes(verts[i]).ToArray();
                }

                // write halfedge attributes
                if (getHedgeAttributes != null)
                {
                    _hedgeAttributes = new object[hedges.Count][];

                    for (int i = 0; i < hedges.Count; i++)
                        _hedgeAttributes[i] = getHedgeAttributes(hedges[i]).ToArray();
                }

                // write face attributes
                if (getFaceAttributes != null)
                {
                    _faceAttributes = new object[faces.Count][];

                    for (int i = 0; i < faces.Count; i++)
                        _faceAttributes[i] = getFaceAttributes(faces[i]).ToArray();
                }
            }


            /// <summary>
            /// Reads this buffer to the given graph.
            /// </summary>
            /// <param name="graph"></param>
            public void ReadTo<TV, TE>(HeGraph<TV, TE> graph, Action<TV, object[]> setVertexAttributes = null, Action<TE, object[]> setHedgeAttributes = null)
                where TV : HeGraph<TV, TE>.Vertex
                where TE : HeGraph<TV, TE>.Halfedge
            {
                var verts = graph.Vertices;
                var hedges = graph.Halfedges;

                int nv = verts.Count;
                int nhe = hedges.Count;

                // add new vertices
                for (int i = 0; i < _vertexRefs.Length; i++)
                    graph.AddVertex();

                // add new halfedges
                for (int i = 0; i < _hedgeRefs.Length; i++)
                    graph.AddEdge();

                // link up vertices
                for (int i = 0; i < _vertexRefs.Length; i++)
                {
                    var v = verts[i + nv];

                    var first = _vertexRefs[i];
                    if (first > -1) v.First = hedges[first + nhe];
                }

                // link up halfedges
                for (int i = 0; i < _hedgeRefs.Length; i++)
                {
                    var he = hedges[i + nhe];
                    var refs = _hedgeRefs[i];

                    he.Previous = hedges[refs[0] + nhe];
                    he.Next = hedges[refs[1] + nhe];

                    var start = refs[2];
                    if (start > -1) he.Start = verts[start + nhe];
                }

                // TODO 
                // validate topology?

                // set vertex attributes
                if (setVertexAttributes != null)
                {
                    for (int i = 0; i < _vertexAttributes.Length; i++)
                        setVertexAttributes(verts[i + nv], _vertexAttributes[i]);
                }

                // set vertex attributes
                if (setHedgeAttributes != null)
                {
                    for (int i = 0; i < _hedgeAttributes.Length; i++)
                        setHedgeAttributes(hedges[i + nhe], _hedgeAttributes[i]);
                }
            }


            /// <summary>
            /// Reads this buffer to the given graph.
            /// </summary>
            /// <param name="mesh"></param>
            public void ReadTo<TV, TE, TF>(HeMesh<TV, TE, TF> mesh, Action<TV, object[]> setVertexAttributes = null, Action<TE, object[]> setHedgeAttributes = null, Action<TF, object[]> setFaceAttributes = null)
                where TV : HeMesh<TV, TE, TF>.Vertex
                where TE : HeMesh<TV, TE, TF>.Halfedge
                where TF : HeMesh<TV, TE, TF>.Face
            {
                var verts = mesh.Vertices;
                var hedges = mesh.Halfedges;
                var faces = mesh.Faces;

                int nv = verts.Count;
                int nhe = hedges.Count;
                int nf = faces.Count;

                // add new vertices
                for (int i = 0; i < _vertexRefs.Length; i++)
                    mesh.AddVertex();

                // add new halfedges
                for (int i = 0; i < _hedgeRefs.Length; i++)
                    mesh.AddEdge();

                // add new faces
                for (int i = 0; i < _faceRefs.Length; i++)
                    mesh.AddFace();

                // link up vertices
                for (int i = 0; i < _vertexRefs.Length; i++)
                {
                    var v = verts[i + nv];

                    var first = _vertexRefs[i];
                    if (first > -1) v.First = hedges[first + nhe];
                }

                // link up halfedges
                for (int i = 0; i < _hedgeRefs.Length; i++)
                {
                    var he = hedges[i + nhe];
                    var refs = _hedgeRefs[i];

                    he.Previous = hedges[refs[0] + nhe];
                    he.Next = hedges[refs[1] + nhe];

                    var start = refs[2];
                    if (start > -1) he.Start = verts[start + nhe];

                    var face = refs[3];
                    if (face > -1) he.Face = faces[face + nf];
                }

                // link up faces
                for (int i = 0; i < _faceRefs.Length; i++)
                {
                    var f = faces[i + nf];

                    var first = _faceRefs[i];
                    if (first > -1) f.First = hedges[first + nhe];
                }

                // TODO 
                // validate topology?

                // set vertex attributes
                if (setVertexAttributes != null)
                {
                    for (int i = 0; i < _vertexAttributes.Length; i++)
                        setVertexAttributes(verts[i + nv], _vertexAttributes[i]);
                }

                // set vertex attributes
                if (setHedgeAttributes != null)
                {
                    for (int i = 0; i < _hedgeAttributes.Length; i++)
                        setHedgeAttributes(hedges[i + nhe], _hedgeAttributes[i]);
                }

                // set vertex attributes
                if (setFaceAttributes != null)
                {
                    for (int i = 0; i < _faceAttributes.Length; i++)
                        setFaceAttributes(faces[i + nf], _faceAttributes[i]);
                }
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public static void WriteToOBJ<TV, TE, TF>(HeMesh<TV, TE, TF> mesh, string path, Func<TV, Vec2d> getTexture = null)
            where TV : HeMesh<TV, TE, TF>.Vertex, IPosition3d, INormal3d
            where TE : HeMesh<TV, TE, TF>.Halfedge
            where TF : HeMesh<TV, TE, TF>.Face
        {
            WriteToObj(mesh, path, IPosition3d<TV>.Get, INormal3d<TV>.Get, getTexture);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="TF"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="path"></param>
        /// <param name="getPosition"></param>
        /// <param name="getNormal"></param>
        /// <param name="getTexture"></param>
        public static void WriteToObj<TV, TE, TF>(HeMesh<TV, TE, TF> mesh, string path, Func<TV, Vec3d> getPosition, Func<TV, Vec3d> getNormal = null, Func<TV, Vec2d> getTexture = null)
            where TV : HeMesh<TV, TE, TF>.Vertex
            where TE : HeMesh<TV, TE, TF>.Halfedge
            where TF : HeMesh<TV, TE, TF>.Face
        {
            var verts = mesh.Vertices;
            var faces = mesh.Faces;
            int formatIndex = 0;

            using (var writer = new StreamWriter(path, false))
            {
                writer.WriteLine(ObjUtil.Header);
                writer.WriteLine();

                // write vertex positions
                for (int i = 0; i < verts.Count; i++)
                {
                    var v = getPosition(verts[i]);
                    writer.WriteLine("v {0} {1} {2}", v.X, v.Y, v.Z);
                }

                // write vertex normals
                if (getNormal != null)
                {
                    for (int i = 0; i < verts.Count; i++)
                    {
                        var vn = getNormal(verts[i]);
                        writer.WriteLine("vn {0} {1} {2}", vn.X, vn.Y, vn.Z);
                    }

                    formatIndex |= 2;
                }

                // write vertex texCoords
                if (getTexture != null)
                {
                    for (int i = 0; i < verts.Count; i++)
                    {
                        var vt = getTexture(verts[i]);
                        writer.WriteLine("vt {0} {1}", vt.X, vt.Y);
                    }

                    formatIndex |= 1;
                }

                // write faces
                string format = ObjUtil.FaceFormats[formatIndex];

                for (int i = 0; i < faces.Count; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;

                    writer.Write("f");
                    foreach (var v in f.Vertices)
                        writer.Write(format, v.Index + 1);

                    writer.WriteLine();
                }
            }
        }


        /// <summary>
        /// Implementation currently ignores texture coordinates and normals.
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="TF"></typeparam>
        /// <param name="path"></param>
        /// <param name="mesh"></param>
        /// <param name="setPosition"></param>
        public static void ReadFromObj<TV, TE, TF>(string path, HeMesh<TV, TE, TF> mesh, Action<TV, Vec3d> setPosition)
            where TV : HeMesh<TV, TE, TF>.Vertex
            where TE : HeMesh<TV, TE, TF>.Halfedge
            where TF : HeMesh<TV, TE, TF>.Face
        {
            var verts = mesh.Vertices;
            var faces = mesh.Faces;
            var face = new List<int>();

            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // skip empty lines and comments
                    if (line.Length == 0 || line[0] == '#') continue;

                    // check the first character
                    var segments = line.Split(ObjUtil.Separators, StringSplitOptions.RemoveEmptyEntries);
                    switch (segments[0])
                    {
                        case "v":
                            {
                                // parse vertex
                                double x = double.Parse(segments[1]);
                                double y = double.Parse(segments[2]);
                                double z = double.Parse(segments[3]);

                                var v = mesh.AddVertex();
                                setPosition(v, new Vec3d(x, y, z));
                                break;
                            }
                        case "f":
                            {
                                // parse face
                                for (int i = 1; i < segments.Length; i++)
                                {
                                    var ids = segments[i].Split(ObjUtil.FaceSeparators);
                                    face.Add(int.Parse(ids[0]) - 1);
                                }

                                mesh.AddFace(face);
                                face.Clear();
                                break;
                            }
                    }
                }
            }
        }


        /// <summary>
        /// Consts used in reading and writing .obj files
        /// </summary>
        private static class ObjUtil
        {
            public const string Header = "# Generated by SpatialSlur https://github.com/daveReeves/SpatialSlur";
            public static readonly string[] FaceFormats = new string[] { " {0}", " {0}/{0}", " {0}//{0}", " {0}/{0}/{0}" };
            public static readonly char[] Separators = new char[] { ' ', '\t', '\0' };
            public static readonly char[] FaceSeparators = new char[] { '/' };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <typeparam name="TE"></typeparam>
        public static void WriteToJson(HeGraph3d graph, string path)
        {
            WriteToJson(graph, path, ToJson);

            IEnumerable<object> ToJson(HeGraph3d.Vertex vertex)
            {
                var p = vertex.Position;
                yield return p.X;
                yield return p.Y;
                yield return p.Z;

                var n = vertex.Normal;
                yield return n.X;
                yield return n.Y;
                yield return n.Z;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="graph"></param>
        /// <param name="setVertexAttributes"></param>
        /// <param name="setHedgeAttributes"></param>
        public static void ReadFromJson(string path, HeGraph3d graph)
        {
            ReadFromJson(path, graph, FromJson);

            void FromJson(HeGraph3d.Vertex vertex, object[] attributes)
            {
                vertex.Position = new Vec3d(
                    Convert.ToDouble(attributes[0]),
                    Convert.ToDouble(attributes[1]),
                    Convert.ToDouble(attributes[2])
                    );

                // parse optional attributes
                if (attributes.Length == 6)
                {
                    vertex.Normal = new Vec3d(
                        Convert.ToDouble(attributes[3]),
                        Convert.ToDouble(attributes[4]),
                        Convert.ToDouble(attributes[5])
                        );
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <typeparam name="TE"></typeparam>
        public static void WriteToJson<TV, TE>(HeGraph<TV, TE> graph, string path, Func<TV, IEnumerable<object>> getVertexAttributes = null, Func<TE, IEnumerable<object>> getHedgeAttributes = null)
            where TV : HeGraph<TV, TE>.Vertex
            where TE : HeGraph<TV, TE>.Halfedge
        {
            var buffer = new HeJsonBuffer();
            buffer.WriteFrom(graph, getVertexAttributes, getHedgeAttributes);
            CoreIO.SerializeJson(buffer, path);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="graph"></param>
        /// <param name="setVertexAttributes"></param>
        /// <param name="setHedgeAttributes"></param>
        public static void ReadFromJson<TV, TE>(string path, HeGraph<TV, TE> graph, Action<TV, object[]> setVertexAttributes = null, Action<TE, object[]> setHedgeAttributes = null)
            where TV : HeGraph<TV, TE>.Vertex
            where TE : HeGraph<TV, TE>.Halfedge
        {
            var buffer = CoreIO.DeserializeJson<HeJsonBuffer>(path);
            buffer.ReadTo(graph, setVertexAttributes, setHedgeAttributes);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <typeparam name="TE"></typeparam>
        public static void WriteToJson(HeMesh3d mesh, string path)
        {
            WriteToJson(mesh, path, ToJson);

            IEnumerable<object> ToJson(HeMesh3d.Vertex vertex)
            {
                var p = vertex.Position;
                yield return p.X;
                yield return p.Y;
                yield return p.Z;

                var n = vertex.Normal;
                yield return n.X;
                yield return n.Y;
                yield return n.Z;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mesh"></param>
        /// <param name="setVertexAttributes"></param>
        /// <param name="setHedgeAttributes"></param>
        public static void ReadFromJson(string path, HeMesh3d mesh)
        {
            ReadFromJson(path, mesh, FromJson);

            void FromJson(HeMesh3d.Vertex vertex, object[] attributes)
            {
                vertex.Position = new Vec3d(
                    Convert.ToDouble(attributes[0]),
                    Convert.ToDouble(attributes[1]),
                    Convert.ToDouble(attributes[2])
                    );

                if(attributes.Length == 6)
                {
                    vertex.Normal = new Vec3d(
                       Convert.ToDouble(attributes[3]),
                       Convert.ToDouble(attributes[4]),
                       Convert.ToDouble(attributes[5])
                       );
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <typeparam name="TE"></typeparam>
        public static void WriteToJson<TV, TE, TF>(HeMesh<TV, TE, TF> mesh, string path, Func<TV, IEnumerable<object>> getVertexAttributes = null, Func<TE, IEnumerable<object>> getHedgeAttributes = null, Func<TF, IEnumerable<object>> getFaceAttributes = null)
            where TV : HeMesh<TV, TE, TF>.Vertex
            where TE : HeMesh<TV, TE, TF>.Halfedge
            where TF : HeMesh<TV, TE, TF>.Face
        {
            var buffer = new HeJsonBuffer();
            buffer.WriteFrom(mesh, getVertexAttributes, getHedgeAttributes, getFaceAttributes);
            CoreIO.SerializeJson(buffer, path);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="graph"></param>
        /// <param name="setVertexAttributes"></param>
        /// <param name="setHedgeAttributes"></param>
        public static void ReadFromJson<TV, TE, TF>(string path, HeMesh<TV, TE, TF> mesh, Action<TV, object[]> setVertexAttributes = null, Action<TE, object[]> setHedgeAttributes = null, Action<TF, object[]> setFaceAttributes = null)
            where TV : HeMesh<TV, TE, TF>.Vertex
            where TE : HeMesh<TV, TE, TF>.Halfedge
            where TF : HeMesh<TV, TE, TF>.Face
        {
            //var buffer = CoreIO.DeserializeJson<HeMeshJsonBuffer>(path);
            var buffer = CoreIO.DeserializeJson<HeJsonBuffer>(path);
            buffer.ReadTo(mesh, setVertexAttributes, setHedgeAttributes, setFaceAttributes);
        }
    }
}
