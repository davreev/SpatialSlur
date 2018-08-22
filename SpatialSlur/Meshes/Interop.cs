
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;

using SpatialSlur.Meshes;
using SpatialSlur.Meshes.Impl;

using static SpatialSlur.Meshes.Delegates;

namespace SpatialSlur
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class Interop
    {
        /// <summary>
        /// Static methods for importing from and exporting to external formats.
        /// </summary>
        public static class Meshes
        {
            #region OBJ

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="TV"></typeparam>
            /// <typeparam name="TE"></typeparam>
            /// <typeparam name="TF"></typeparam>
            /// <param name="mesh"></param>
            /// <param name="path"></param>
            /// <param name="getTexture"></param>
            public static void WriteToObj<TV, TE, TF>(HeMesh<TV, TE, TF> mesh, string path, Func<TV, Vector2d> getTexture = null)
                where TV : HeMesh<TV, TE, TF>.Vertex, IPosition3d, INormal3d
                where TE : HeMesh<TV, TE, TF>.Halfedge
                where TF : HeMesh<TV, TE, TF>.Face
            {
                WriteToObj(mesh, path, Position3d<TV>.Get, Normal3d<TV>.Get, getTexture);
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
            public static void WriteToObj<TV, TE, TF>(HeMesh<TV, TE, TF> mesh, string path, Func<TV, Vector3d> getPosition, Func<TV, Vector3d> getNormal = null, Func<TV, Vector2d> getTexture = null)
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
            public static void ReadFromObj<TV, TE, TF>(string path, HeMesh<TV, TE, TF> mesh, Action<TV, Vector3d> setPosition)
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
                                    setPosition(v, new Vector3d(x, y, z));
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

            #endregion


            #region JSON

            #region Nested Types

            /// <summary>
            /// Json serializable representation of a halfedge graph.
            /// </summary>
            [DataContract]
            private class HeGraphJsonBuffer<VA, EA>
            {
                [DataMember(Name = "Vertices", Order = 0)]
                private int[] _vertexRefs;

                [DataMember(Name = "Halfedges", Order = 1)]
                private int[][] _hedgeRefs;

                [DataMember(Name = "VertexAttributes", Order = 2)]
                private VA[] _vertexAttributes = Array.Empty<VA>();

                [DataMember(Name = "HalfedgeAttributes", Order = 3)]
                private EA[] _hedgeAttributes = Array.Empty<EA>();


                /// <summary>
                /// Writes the given graph to this buffer.
                /// </summary>
                public void WriteFrom<V, E>(HeGraph<V, E> graph, Func<V, VA> getVertexAttributes = null, Func<E, EA> getHedgeAttributes = null)
                    where V : HeGraph<V, E>.Vertex
                    where E : HeGraph<V, E>.Halfedge
                {
                    var verts = graph.Vertices;
                    var hedges = graph.Halfedges;

                    _vertexRefs = new int[verts.Count];
                    _hedgeRefs = new int[hedges.Count][];

                    // write vertex topology
                    for (int i = 0; i < verts.Count; i++)
                    {
                        var v = verts[i];
                        _vertexRefs[i] = v.First ?? -1;
                    }

                    // write halfedge topology
                    for (int i = 0; i < hedges.Count; i++)
                    {
                        var he = hedges[i];

                        _hedgeRefs[i] = new int[]
                        {
                        he.Previous?? -1,
                        he.Next,
                        he.Start
                        };
                    }

                    // write vertex attributes
                    if (getVertexAttributes != null)
                    {
                        _vertexAttributes = new VA[verts.Count];

                        for (int i = 0; i < verts.Count; i++)
                            _vertexAttributes[i] = getVertexAttributes(verts[i]);
                    }

                    // write halfedge attributes
                    if (getHedgeAttributes != null)
                    {
                        _hedgeAttributes = new EA[hedges.Count];

                        for (int i = 0; i < hedges.Count; i++)
                            _hedgeAttributes[i] = getHedgeAttributes(hedges[i]);
                    }
                }


                /// <summary>
                /// Reads this buffer to the given graph.
                /// </summary>
                public void ReadTo<V, E>(HeGraph<V, E> graph, Action<V, VA> setVertexAttributes = null, Action<E, EA> setHedgeAttributes = null)
                    where V : HeGraph<V, E>.Vertex
                    where E : HeGraph<V, E>.Halfedge
                {
                    var verts = graph.Vertices;
                    var hedges = graph.Halfedges;

                    int nv = verts.Count;
                    int nhe = hedges.Count;

                    // add new vertices
                    for (int i = 0; i < _vertexRefs.Length; i++)
                        graph.AddVertex();

                    // add new halfedges
                    for (int i = 0; i < _hedgeRefs.Length; i += 2)
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

                        var prev = refs[0];
                        if (prev != -1) he.Previous = hedges[prev + nhe];

                        he.Next = hedges[refs[1] + nhe];

                        var start = refs[2];
                        if (start > -1) he.Start = verts[start + nhe];
                    }

                    // TODO validate topology?

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
            }


            /// <summary>
            /// JSON serializable representation of a halfedge mesh.
            /// </summary>
            [DataContract]
            private class HeMeshJsonBuffer<VA, EA, FA>
            {
                [DataMember(Name = "Vertices", Order = 0)]
                private int[] _vertexRefs;

                [DataMember(Name = "Halfedges", Order = 1)]
                private int[][] _hedgeRefs;

                [DataMember(Name = "Faces", Order = 2)]
                private int[] _faceRefs;

                [DataMember(Name = "VertexAttributes", Order = 3)]
                private VA[] _vertexAttributes = Array.Empty<VA>();

                [DataMember(Name = "HalfedgeAttributes", Order = 4)]
                private EA[] _hedgeAttributes = Array.Empty<EA>();

                [DataMember(Name = "FaceAttributes", Order = 5)]
                private FA[] _faceAttributes = Array.Empty<FA>();


                /// <summary>
                /// Writes the given mesh to this buffer.
                /// </summary>
                public void WriteFrom<V, E, F>(HeMesh<V, E, F> mesh, Func<V, VA> getVertexAttributes = null, Func<E, EA> getHedgeAttributes = null, Func<F, FA> getFaceAttributes = null)
                    where V : HeMesh<V, E, F>.Vertex
                    where E : HeMesh<V, E, F>.Halfedge
                    where F : HeMesh<V, E, F>.Face
                {
                    var verts = mesh.Vertices;
                    var hedges = mesh.Halfedges;
                    var faces = mesh.Faces;

                    _vertexRefs = new int[verts.Count];
                    _hedgeRefs = new int[hedges.Count][];
                    _faceRefs = new int[faces.Count];

                    // write halfedge topology
                    for (int i = 0; i < hedges.Count; i++)
                    {
                        var he = hedges[i];

                        _hedgeRefs[i] = new int[]
                        {
                         he.Previous?? -1,
                         he.Next,
                         he.Start,
                         he.Face?? -1
                        };
                    }

                    // write vertex topology
                    for (int i = 0; i < verts.Count; i++)
                    {
                        var v = verts[i];
                        _vertexRefs[i] = v.First ?? -1;
                    }

                    // write face topology
                    for (int i = 0; i < faces.Count; i++)
                    {
                        var f = faces[i];
                        _faceRefs[i] = f.First ?? -1;
                    }

                    // write vertex attributes
                    if (getVertexAttributes != null)
                    {
                        _vertexAttributes = new VA[verts.Count];

                        for (int i = 0; i < verts.Count; i++)
                            _vertexAttributes[i] = getVertexAttributes(verts[i]);
                    }

                    // write halfedge attributes
                    if (getHedgeAttributes != null)
                    {
                        _hedgeAttributes = new EA[hedges.Count];

                        for (int i = 0; i < hedges.Count; i++)
                            _hedgeAttributes[i] = getHedgeAttributes(hedges[i]);
                    }

                    // write face attributes
                    if (getFaceAttributes != null)
                    {
                        _faceAttributes = new FA[faces.Count];

                        for (int i = 0; i < faces.Count; i++)
                            _faceAttributes[i] = getFaceAttributes(faces[i]);
                    }
                }


                /// <summary>
                /// Reads this buffer to the given mesh.
                /// </summary>
                public void ReadTo<V, E, F>(HeMesh<V, E, F> mesh, Action<V, VA> setVertexAttributes = null, Action<E, EA> setHedgeAttributes = null, Action<F, FA> setFaceAttributes = null)
                    where V : HeMesh<V, E, F>.Vertex
                    where E : HeMesh<V, E, F>.Halfedge
                    where F : HeMesh<V, E, F>.Face
                {
                    var verts = mesh.Vertices;
                    int nv = verts.Count;

                    var hedges = mesh.Halfedges;
                    int nhe = hedges.Count;

                    var faces = mesh.Faces;
                    int nf = faces.Count;

                    // add new vertices
                    for (int i = 0; i < _vertexRefs.Length; i++)
                        mesh.AddVertex();

                    // add new halfedges
                    for (int i = 0; i < _hedgeRefs.Length; i += 2)
                        mesh.AddEdge();

                    // add new faces
                    for (int i = 0; i < _faceRefs.Length; i++)
                        mesh.AddFace();

                    // link up vertices
                    for (int i = 0; i < _vertexRefs.Length; i++)
                    {
                        var v = verts[i + nv];

                        var first = _vertexRefs[i];
                        if (first != -1) v.First = hedges[first + nhe];
                    }

                    // link up halfedges
                    for (int i = 0; i < _hedgeRefs.Length; i++)
                    {
                        var he = hedges[i + nhe];
                        var refs = _hedgeRefs[i];

                        var prev = refs[0];
                        if (prev != -1) he.Previous = hedges[prev + nhe];

                        he.Next = hedges[refs[1] + nhe];
                        he.Start = verts[refs[2] + nv];

                        var face = refs[3];
                        if (face != -1) he.Face = faces[face + nf];
                    }

                    // link up faces
                    for (int i = 0; i < _faceRefs.Length; i++)
                    {
                        var f = faces[i + nf];

                        var first = _faceRefs[i];
                        if (first != -1) f.First = hedges[first + nhe];
                    }

                    // TODO validate topology?

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
            /// <typeparam name="V"></typeparam>
            /// <typeparam name="E"></typeparam>
            /// <typeparam name="VA"></typeparam>
            /// <typeparam name="EA"></typeparam>
            /// <param name="graph"></param>
            /// <param name="path"></param>
            /// <param name="getVertexAttributes"></param>
            /// <param name="getHedgeAttributes"></param>
            public static void WriteToJson<V, E, VA, EA>(HeGraph<V, E> graph, string path, Func<V, VA> getVertexAttributes = null, Func<E, EA> getHedgeAttributes = null)
                where V : HeGraph<V, E>.Vertex
                where E : HeGraph<V, E>.Halfedge
            {
                var buffer = new HeGraphJsonBuffer<VA, EA>();
                buffer.WriteFrom(graph, getVertexAttributes, getHedgeAttributes);
                Interop.SerializeJson(buffer, path);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="path"></param>
            /// <param name="graph"></param>
            /// <param name="setVertexAttributes"></param>
            /// <param name="setHedgeAttributes"></param>
            public static void ReadFromJson<V, E, VA, EA>(string path, HeGraph<V, E> graph, Action<V, VA> setVertexAttributes = null, Action<E, EA> setHedgeAttributes = null)
                where V : HeGraph<V, E>.Vertex
                where E : HeGraph<V, E>.Halfedge
            {
                var buffer = Interop.DeserializeJson<HeGraphJsonBuffer<VA, EA>>(path);
                buffer.ReadTo(graph, setVertexAttributes, setHedgeAttributes);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="V"></typeparam>
            /// <typeparam name="E"></typeparam>
            /// <typeparam name="F"></typeparam>
            /// <typeparam name="VA"></typeparam>
            /// <typeparam name="EA"></typeparam>
            /// <typeparam name="FA"></typeparam>
            /// <param name="mesh"></param>
            /// <param name="path"></param>
            /// <param name="getVertexAttributes"></param>
            /// <param name="getHedgeAttributes"></param>
            /// <param name="getFaceAttributes"></param>
            public static void WriteToJson<V, E, F, VA, EA, FA>(HeMesh<V, E, F> mesh, string path, Func<V, VA> getVertexAttributes = null, Func<E, EA> getHedgeAttributes = null, Func<F, FA> getFaceAttributes = null)
                where V : HeMesh<V, E, F>.Vertex
                where E : HeMesh<V, E, F>.Halfedge
                where F : HeMesh<V, E, F>.Face
            {
                var buffer = new HeMeshJsonBuffer<VA, EA, FA>();
                buffer.WriteFrom(mesh, getVertexAttributes, getHedgeAttributes, getFaceAttributes);
                Interop.SerializeJson(buffer, path);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="V"></typeparam>
            /// <typeparam name="E"></typeparam>
            /// <typeparam name="F"></typeparam>
            /// <typeparam name="VA"></typeparam>
            /// <typeparam name="EA"></typeparam>
            /// <typeparam name="FA"></typeparam>
            /// <param name="path"></param>
            /// <param name="mesh"></param>
            /// <param name="setVertexAttributes"></param>
            /// <param name="setHedgeAttributes"></param>
            /// <param name="setFaceAttributes"></param>
            public static void ReadFromJson<V, E, F, VA, EA, FA>(string path, HeMesh<V, E, F> mesh, Action<V, VA> setVertexAttributes = null, Action<E, EA> setHedgeAttributes = null, Action<F, FA> setFaceAttributes = null)
                where V : HeMesh<V, E, F>.Vertex
                where E : HeMesh<V, E, F>.Halfedge
                where F : HeMesh<V, E, F>.Face
            {
                var buffer = Interop.DeserializeJson<HeMeshJsonBuffer<VA, EA, FA>>(path);
                buffer.ReadTo(mesh, setVertexAttributes, setHedgeAttributes, setFaceAttributes);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="graph"></param>
            /// <param name="path"></param>
            public static void WriteToJson(HeGraph3d graph, string path)
            {
                WriteToJson<HeGraph3d.Vertex, HeGraph3d.Halfedge, double[], double[]>(graph, path, ToJson);

                double[] ToJson(HeGraph3d.Vertex vertex)
                {
                    var p = vertex.Position;
                    var n = vertex.Normal;
                    return new double[] { p.X, p.Y, p.Z, n.X, n.Y, n.Z };
                }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="path"></param>
            /// <param name="graph"></param>
            public static void ReadFromJson(string path, HeGraph3d graph)
            {
                ReadFromJson<HeGraph3d.Vertex, HeGraph3d.Halfedge, double[], double[]>(path, graph, FromJson);

                void FromJson(HeGraph3d.Vertex vertex, double[] values)
                {
                    vertex.Position = new Vector3d(
                        values[0],
                        values[1],
                        values[2]);

                    vertex.Normal = new Vector3d(
                        values[3],
                        values[4],
                        values[5]);
                }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="mesh"></param>
            /// <param name="path"></param>
            public static void WriteToJson(HeMesh3d mesh, string path)
            {
                WriteToJson<HeMesh3d.Vertex, HeMesh3d.Halfedge, HeMesh3d.Face, double[], double[], double[]>(mesh, path, ToJson);

                double[] ToJson(HeMesh3d.Vertex vertex)
                {
                    var p = vertex.Position;
                    var n = vertex.Normal;
                    return new double[] { p.X, p.Y, p.Z, n.X, n.Y, n.Z };
                }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="path"></param>
            /// <param name="mesh"></param>
            public static void ReadFromJson(string path, HeMesh3d mesh)
            {
                ReadFromJson<HeMesh3d.Vertex, HeMesh3d.Halfedge, HeMesh3d.Face, double[], double[], double[]>(path, mesh, FromJson);

                void FromJson(HeMesh3d.Vertex vertex, double[] values)
                {
                    vertex.Position = new Vector3d(
                        values[0],
                        values[1],
                        values[2]);

                    vertex.Normal = new Vector3d(
                       values[3],
                       values[4],
                       values[5]);
                }
            }

            #endregion
        }
    }
}