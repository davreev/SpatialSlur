using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Rhino.Geometry;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

/*
 * Notes
 */

namespace SpatialSlur.SlurRhino
{ 
    using M = Rhino.Geometry.Mesh;

    /// <summary>
    /// 
    /// </summary>
    public static partial class RhinoFactory
    {
        /// <summary>
        /// Static creation methods for meshes.
        /// </summary>
        public static class Mesh
        {
            /// <summary>
            /// 
            /// </summary>
            public static M CreateExtrusion(Polyline polyline, Vector3d direction)
            {
                if (polyline.IsClosed)
                    return CreateExtrusionClosed(polyline, direction);
                else
                    return CreateExtrusionOpen(polyline, direction);
            }


            /// <summary>
            /// 
            /// </summary>
            private static M CreateExtrusionOpen(Polyline polyline, Vector3d direction)
            {
                M result = new M();
                var verts = result.Vertices;
                var faces = result.Faces;

                int n = polyline.Count;

                // add vertices
                for (int i = 0; i < n; i++)
                    verts.Add(polyline[i]);

                for (int i = 0; i < n; i++)
                    verts.Add(polyline[i] + direction);

                // add faces
                for (int i = 0; i < n - 1; i++)
                    faces.AddFace(i, i + 1, i + n + 1, i + n);

                return result;
            }


            /// <summary>
            /// 
            /// </summary>
            private static M CreateExtrusionClosed(Polyline polyline, Vector3d direction)
            {
                M result = new M();
                var verts = result.Vertices;
                var faces = result.Faces;

                int n = polyline.Count - 1;

                // add verts
                for (int i = 0; i < n; i++)
                    verts.Add(polyline[i]);

                for (int i = 0; i < n; i++)
                    verts.Add(polyline[i] + direction);

                // add faces
                for (int i = 0; i < n; i++)
                {
                    int j = (i + 1) % n;
                    faces.AddFace(i, j, j + n, i + n);
                }

                return result;
            }


            /// <summary>
            /// 
            /// </summary>
            public static M CreateLoft(Polyline polylineA, Polyline polylineB)
            {
                if (polylineA.IsClosed && polylineB.IsClosed)
                    return CreateLoftClosed(polylineA, polylineB);
                else
                    return CreateLoftOpen(polylineA, polylineB);
            }


            /// <summary>
            /// 
            /// </summary>
            private static M CreateLoftOpen(Polyline polylineA, Polyline polylineB)
            {
                M result = new M();
                var verts = result.Vertices;
                var faces = result.Faces;

                int n = polylineA.Count;

                // add vertices
                verts.AddVertices(polylineA);
                verts.AddVertices(polylineB);

                // add faces
                for (int i = 0; i < n - 1; i++)
                    faces.AddFace(i, i + 1, i + n + 1, i + n);

                return result;
            }


            /// <summary>
            /// 
            /// </summary>
            private static M CreateLoftClosed(Polyline polylineA, Polyline polylineB)
            {
                M result = new M();
                var verts = result.Vertices;
                var faces = result.Faces;

                int n = polylineA.Count - 1;

                // add verts
                for (int i = 0; i < n; i++)
                    verts.Add(polylineA[i]);

                for (int i = 0; i < n; i++)
                    verts.Add(polylineB[i]);

                // add faces
                for (int i = 0; i < n; i++)
                {
                    int j = (i + 1) % n;
                    faces.AddFace(i, j, j + n, i + n);
                }

                return result;
            }


            /// <summary>
            /// 
            /// </summary>
            public static M CreateLoft(IList<Polyline> polylines)
            {
                if (Enumerable.All(polylines, p => p.IsClosed))
                    return CreateLoftClosed(polylines);
                else
                    return CreateLoftOpen(polylines);
            }


            /// <summary>
            /// 
            /// </summary>
            private static M CreateLoftOpen(IList<Polyline> polylines)
            {
                M result = new M();
                var verts = result.Vertices;
                var faces = result.Faces;

                int ny = polylines.Count;
                int nx = Enumerable.Min(polylines, p => p.Count);
                int n;

                // add vertices
                for (int i = 0; i < ny; i++)
                {
                    var poly = polylines[i];

                    for (int j = 0; j < nx; j++)
                        verts.Add(poly[j]);
                }

                // add faces
                for (int i = 0; i < ny - 1; i++)
                {
                    n = i * nx;

                    for (int j = 0; j < nx - 1; j++)
                        faces.AddFace(n + j, n + j + 1, n + j + nx + 1, n + j + nx);
                }

                return result;
            }


            /// <summary>
            /// 
            /// </summary>
            private static M CreateLoftClosed(IList<Polyline> polylines)
            {
                M result = new M();
                var verts = result.Vertices;
                var faces = result.Faces;

                int ny = polylines.Count;
                int nx = Enumerable.Min(polylines, p => p.Count) - 1;
                int n;

                // add vertices
                for (int i = 0; i < ny; i++)
                {
                    var poly = polylines[i];

                    for (int j = 0; j < nx; j++)
                        verts.Add(poly[j]);
                }

                // add faces
                for (int i = 0; i < ny - 1; i++)
                {
                    n = i * nx;

                    for (int j0 = 0; j0 < nx; j0++)
                    {
                        int j1 = (j0 + 1) % nx;
                        faces.AddFace(n + j0, n + j1, n + j1 + nx, n + j0 + nx);
                    }
                }

                return result;
            }

           
            /// <summary>
            /// 
            /// </summary>
            /// <param name="mesh"></param>
            /// <returns></returns>
            public static M CreatePolySoup(M mesh)
            {
                var verts = mesh.Vertices;
                var faces = mesh.Faces;

                var newMesh = new M();
                var newVerts = newMesh.Vertices;
                var newFaces = newMesh.Faces;

                // add verts and faces
                for (int i = 0; i < faces.Count; i++)
                {
                    var f = faces[i];
                    int nv = newVerts.Count;

                    if (f.IsTriangle)
                    {
                        for (int j = 0; j < 3; j++)
                            newVerts.Add(verts[f[j]]);

                        newFaces.AddFace(nv, nv + 1, nv + 2);
                    }
                    else
                    {
                        for (int j = 0; j < 4; j++)
                            newVerts.Add(verts[f[j]]);

                        newFaces.AddFace(nv, nv + 1, nv + 2, nv + 3);
                    }
                }

                return newMesh;
            }


            /// <summary>
            /// Quadrangulates n-gons.
            /// </summary>
            /// <typeparam name="V"></typeparam>
            /// <typeparam name="E"></typeparam>
            /// <typeparam name="F"></typeparam>
            /// <param name="mesh"></param>
            /// <param name="getPosition"></param>
            /// <returns></returns>
            public static M CreatePolySoup<V, E, F>(HeMeshBase<V, E, F> mesh, Func<V, Point3f> getPosition)
                where V : HeVertex<V, E, F>
                where E : Halfedge<V, E, F>
                where F : HeFace<V, E, F>
            {
                return CreatePolySoup(mesh, getPosition, FaceQuadrangulators.Strip.Create(mesh));
            }


            /// <summary>
            /// Quadrangulates n-gons based on the given quadrangulator.
            /// </summary>
            /// <typeparam name="V"></typeparam>
            /// <typeparam name="E"></typeparam>
            /// <typeparam name="F"></typeparam>
            /// <param name="mesh"></param>
            /// <param name="getPosition"></param>
            /// <param name="quadrangulator"></param>
            /// <returns></returns>
            public static M CreatePolySoup<V, E, F>(HeMeshBase<V, E, F> mesh, Func<V, Point3f> getPosition, IFaceQuadrangulator<V,E,F> quadrangulator)
                where V : HeVertex<V, E, F>
                where E : Halfedge<V, E, F>
                where F : HeFace<V, E, F>
            {
                var result = new M();
                var newVerts = result.Vertices;
                var newFaces = result.Faces;

                // add vertices per face
                foreach (var f in mesh.Faces)
                {
                    if (f.IsRemoved) continue;
                    int nv = newVerts.Count;
                    int degree = 0;

                    // add face vertices
                    foreach (var v in f.Vertices)
                    {
                        newVerts.Add(getPosition(v));
                        degree++;
                    }

                    // add face(s)
                    if (degree == 3)
                    {
                        newFaces.AddFace(nv, nv + 1, nv + 2);
                    }
                    else if (degree == 4)
                    {
                        newFaces.AddFace(nv, nv + 1, nv + 2, nv + 3);
                    }
                    else
                    {
                        foreach (var quad in quadrangulator.GetQuads(f))
                        {
                            if (quad.Item4 == null)
                                newFaces.AddFace(nv + quad.Item1, nv + quad.Item2, nv + quad.Item3);
                            else
                                newFaces.AddFace(nv + quad.Item1, nv + quad.Item2, nv + quad.Item3, nv + quad.Item4);
                        }
                    }
                }

                return result;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="V"></typeparam>
            /// <typeparam name="E"></typeparam>
            /// <typeparam name="F"></typeparam>
            /// <param name="mesh"></param>
            /// <param name="getPosition"></param>
            /// <param name="getNormal"></param>
            /// <param name="getTexture"></param>
            /// <param name="getColor"></param>
            /// <returns></returns>
            public static M CreateFromHeMesh<V, E, F>(HeMeshBase<V, E, F> mesh, Func<V, Point3f> getPosition, Func<V, Vector3f> getNormal = null, Func<V, Point2f> getTexture = null, Func<V, Color> getColor = null)
                where V : HeVertex<V, E, F>
                where E : Halfedge<V, E, F>
                where F : HeFace<V, E, F>
            {
                return CreateFromHeMesh(mesh, getPosition, FaceQuadrangulators.Strip.Create(mesh), getNormal, getTexture, getColor);   
            }


            /// <summary>
            /// Quadrangulates n-gons based on the given quadrangulator.
            /// </summary>
            /// <typeparam name="V"></typeparam>
            /// <typeparam name="E"></typeparam>
            /// <typeparam name="F"></typeparam>
            /// <param name="mesh"></param>
            /// <param name="getPosition"></param>
            /// <param name="quadrangulator"></param>
            /// <param name="getNormal"></param>
            /// <param name="getTexture"></param>
            /// <param name="getColor"></param>
            /// <returns></returns>
            public static M CreateFromHeMesh<V, E, F>(HeMeshBase<V, E, F> mesh, Func<V, Point3f> getPosition, IFaceQuadrangulator<V, E, F> quadrangulator, Func<V, Vector3f> getNormal = null, Func<V, Point2f> getTexture = null, Func<V, Color> getColor = null)
                where V : HeVertex<V, E, F>
                where E : Halfedge<V, E, F>
                where F : HeFace<V, E, F>
            {
                var verts = mesh.Vertices;
                var faces = mesh.Faces;

                var result = new M();
                var newVerts = result.Vertices;
                var newFaces = result.Faces;

                var newNorms = result.Normals;
                var newTexCoords = result.TextureCoordinates;
                var newColors = result.VertexColors;

                // add vertices
                for (int i = 0; i < verts.Count; i++)
                {
                    var v = verts[i];
                    newVerts.Add(getPosition(v));

                    if (getNormal != null) newNorms.Add(getNormal(v));
                    if (getTexture != null) newTexCoords.Add(getTexture(v));
                    if (getColor != null) newColors.Add(getColor(v));
                }

                // add faces
                foreach (var f in mesh.Faces)
                {
                    if (f.IsRemoved) continue;
                    var he = f.First;
                    int degree = f.Degree;

                    if (degree == 3)
                    {
                        newFaces.AddFace(
                            he.Start.Index,
                            he.NextInFace.Start.Index,
                            he.PrevInFace.Start.Index
                            );
                    }
                    else if (degree == 4)
                    {
                        newFaces.AddFace(
                            he.Start.Index,
                            he.NextInFace.Start.Index,
                            he.NextInFace.NextInFace.Start.Index,
                            he.PrevInFace.Start.Index
                            );
                    }
                    else
                    {
                        foreach (var quad in quadrangulator.GetQuads(f))
                        {
                            if (quad.Item4 == null)
                                newFaces.AddFace(quad.Item1, quad.Item2, quad.Item3);
                            else
                                newFaces.AddFace(quad.Item1, quad.Item2, quad.Item3, quad.Item4);
                        }
                    }
                }

                return result;
            }
            

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="V"></typeparam>
            /// <typeparam name="E"></typeparam>
            /// <typeparam name="F"></typeparam>
            /// <param name="strip"></param>
            /// <param name="getPosition"></param>
            /// <param name="getNormal"></param>
            /// <param name="getTexture"></param>
            /// <param name="getColor"></param>
            /// <returns></returns>
            public static M CreateFromQuadStrip<V, E, F>(HeQuadStrip<V, E, F> strip, Func<V, Point3f> getPosition, Func<V, Vector3f> getNormal = null, Func<V, Point2f> getTexture = null, Func<V, Color> getColor = null)
                where V : HeVertex<V, E, F>
                where E : Halfedge<V, E, F>
                where F : HeFace<V, E, F>
            {
                var mesh = new M();

                var verts = mesh.Vertices;
                var faces = mesh.Faces;

                var norms = mesh.Normals;
                var texCoords = mesh.TextureCoordinates;
                var colors = mesh.VertexColors;

                int skip = strip.IsPeriodic ? 1 : 0;

                // add verts
                foreach (var he in strip.SkipLast(skip))
                {
                    var v0 = he.Start;
                    var v1 = he.End;

                    verts.Add(getPosition(v0));
                    verts.Add(getPosition(v1));

                    if (getNormal != null)
                    {
                        norms.Add(getNormal(v0));
                        norms.Add(getNormal(v1));
                    }

                    if (getTexture != null)
                    {
                        texCoords.Add(getTexture(v0));
                        texCoords.Add(getTexture(v1));
                    }

                    if (getColor != null)
                    {
                        colors.Add(getColor(v0));
                        colors.Add(getColor(v1));
                    }
                }

                // add faces
                var n = verts.Count - 2;
                for (int i = 0; i < n; i += 2)
                    faces.AddFace(i, i + 1, i + 3, i + 2);

                // add last face
                if (!strip.IsPeriodic)
                    faces.AddFace(n, n + 1, 1, 0);

                return mesh;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="mesh"></param>
            /// <param name="vertexValues"></param>
            /// <param name="domain"></param>
            /// <returns></returns>
            public static M CreateIsoTrim(M mesh, IReadOnlyList<double> vertexValues, Domain domain)
            {
                // TODO
                throw new NotImplementedException();
            }
        }
    }
}
