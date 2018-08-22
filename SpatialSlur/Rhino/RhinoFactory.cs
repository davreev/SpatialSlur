
/*
 * Notes
 * 
 * Factory class is split up for consistent type aliasing where T always refers to the type created by the factory method.
 */

#if USING_RHINO

using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using Rhino.Geometry;
using SpatialSlur;
using SpatialSlur.Meshes;
using SpatialSlur.Meshes.Impl;

using Vec3d = Rhino.Geometry.Vector3d;
using Vec3f = Rhino.Geometry.Vector3f;

#region Mesh

namespace SpatialSlur.Rhino
{
    using T = Mesh;

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
            public static T CreateExtrusion(Polyline polyline, Vec3d direction)
            {
                if (polyline.IsClosed)
                    return CreateExtrusionClosed(polyline, direction);
                else
                    return CreateExtrusionOpen(polyline, direction);
            }


            /// <summary>
            /// 
            /// </summary>
            private static T CreateExtrusionOpen(Polyline polyline, Vec3d direction)
            {
                T result = new T();
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
            private static T CreateExtrusionClosed(Polyline polyline, Vec3d direction)
            {
                T result = new T();
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
            public static T CreateLoft(Polyline polylineA, Polyline polylineB)
            {
                if (polylineA.IsClosed && polylineB.IsClosed)
                    return CreateLoftClosed(polylineA, polylineB);
                else
                    return CreateLoftOpen(polylineA, polylineB);
            }


            /// <summary>
            /// 
            /// </summary>
            private static T CreateLoftOpen(Polyline polylineA, Polyline polylineB)
            {
                T result = new T();
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
            private static T CreateLoftClosed(Polyline polylineA, Polyline polylineB)
            {
                T result = new T();
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
            public static T CreateLoft(IList<Polyline> polylines)
            {
                if (Enumerable.All(polylines, p => p.IsClosed))
                    return CreateLoftClosed(polylines);
                else
                    return CreateLoftOpen(polylines);
            }


            /// <summary>
            /// 
            /// </summary>
            private static T CreateLoftOpen(IList<Polyline> polylines)
            {
                T result = new T();
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
            private static T CreateLoftClosed(IList<Polyline> polylines)
            {
                T result = new T();
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
            public static T CreatePolySoup(T mesh)
            {
                var verts = mesh.Vertices;
                var faces = mesh.Faces;

                var newMesh = new T();
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
            /// 
            /// </summary>
            /// <typeparam name="V"></typeparam>
            /// <typeparam name="E"></typeparam>
            /// <typeparam name="F"></typeparam>
            /// <param name="mesh"></param>
            /// <param name="getPosition"></param>
            /// <param name="getColor"></param>
            /// <param name="quadrangulator"></param>
            /// <returns></returns>
            public static T CreatePolySoup<V, E, F>(HeMesh<V, E, F> mesh, Func<V, Point3f> getPosition, Func<F, Color> getColor = null, IFaceQuadrangulator quadrangulator = null)
                where V : HeMesh<V, E, F>.Vertex
                where E : HeMesh<V, E, F>.Halfedge
                where F : HeMesh<V, E, F>.Face
            {
                var result = new T();
                var newVerts = result.Vertices;
                var newColors = result.VertexColors;
                var newFaces = result.Faces;

                // default quadrangulator
                if (quadrangulator == null)
                    quadrangulator = FaceQuadrangulator.CreateStrip();

                // add vertices per face
                foreach (var f in mesh.Faces)
                {
                    if (f.IsUnused) continue;
                    int degree = f.GetDegree();
                    int nv = newVerts.Count;

                    // add colors
                    if (getColor != null)
                    {
                        var c = getColor(f);
                        for (int i = 0; i < degree; i++) newColors.Add(c);
                    }

                    // handle n-gons
                    if (degree > 4)
                    {
                        var quads = quadrangulator.GetQuads(f.First);
               
                        // add first 2 vertices
                        var first = quads.First();
                        newVerts.Add(getPosition(first.Item1));
                        newVerts.Add(getPosition(first.Item2));

                        // add remaining vertices and faces
                        foreach (var quad in quads)
                        {
                            var v0 = quad.Item3;
                            var v1 = quad.Item4;
                            
                            if (v1 == null)
                            {
                                newVerts.Add(getPosition(v0));
                                newFaces.AddFace(nv, nv + 1, nv + 2);
                                break;
                            }

                            newVerts.Add(getPosition(v1));
                            newVerts.Add(getPosition(v0));
                            newFaces.AddFace(nv, nv + 1, nv + 3, nv + 2);

                            nv += 2;
                        }
                    }
                    else
                    {
                        // add face vertices
                        foreach (var v in f.Vertices)
                            newVerts.Add(getPosition(v));

                        // add face
                        if (degree == 3)
                            newFaces.AddFace(nv, nv + 1, nv + 2);
                        else
                            newFaces.AddFace(nv, nv + 1, nv + 2, nv + 3);
                    }
                }

                return result;
            }


            /*
            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="V"></typeparam>
            /// <typeparam name="E"></typeparam>
            /// <typeparam name="F"></typeparam>
            /// <param name="mesh"></param>
            /// <param name="getPosition"></param>
            /// <param name="quadrangulator"></param>
            /// <returns></returns>
            public static M CreatePolySoup<V, E, F>(HeMesh<V, E, F> mesh, Func<V, Point3f> getPosition, Func<F, Color> getColor = null, IFaceQuadrangulator<V, E, F> quadrangulator = null)
                where V : HeMeshBase<V, E, F>.Vertex
                where E : HeMeshBase<V, E, F>.Halfedge
                where F : HeMeshBase<V, E, F>.Face
            {
                var result = new M();
                var newVerts = result.Vertices;
                var newColors = result.VertexColors;
                var newFaces = result.Faces;

                // default quadrangulator
                if (quadrangulator == null)
                    quadrangulator = FaceQuadrangulators.Strip.Create(mesh);

                // add vertices per face
                foreach (var f in mesh.Faces)
                {
                    if (f.IsUnused) continue;
                    int nv = newVerts.Count;
                    int degree = 0;

                    // add face vertices
                    foreach (var v in f.Vertices)
                    {
                        newVerts.Add(getPosition(v));
                        degree++;
                    }

                    // add colors
                    if (getColor != null)
                    {
                        var c = getColor(f);
                        for (int i = 0; i < degree; i++) newColors.Add(c);
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
            */


            /// <summary>
            /// 
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
            public static T CreateFromHeMesh<V, E, F>(HeMesh<V, E, F> mesh, Func<V, Point3f> getPosition, Func<V, Vec3f> getNormal = null, Func<V, Point2f> getTexture = null, Func<V, Color> getColor = null, IFaceQuadrangulator quadrangulator = null)
                where V : HeMesh<V, E, F>.Vertex
                where E : HeMesh<V, E, F>.Halfedge
                where F : HeMesh<V, E, F>.Face
            {
                var verts = mesh.Vertices;

                var result = new T();
                var newVerts = result.Vertices;
                var newFaces = result.Faces;

                var newNorms = result.Normals;
                var newCoords = result.TextureCoordinates;
                var newColors = result.VertexColors;

                // default quadrangulator
                if (quadrangulator == null)
                    quadrangulator = FaceQuadrangulator.CreateStrip();

                // add vertices
                for (int i = 0; i < verts.Count; i++)
                {
                    var v = verts[i];
                    newVerts.Add(getPosition(v));

                    if (getNormal != null)
                        newNorms.Add(getNormal(v));

                    if (getTexture != null)
                        newCoords.Add(getTexture(v));

                    if (getColor != null)
                        newColors.Add(getColor(v));
                }

                // add faces
                foreach (var f in mesh.Faces)
                {
                    if (f.IsUnused) continue;
                    var he = f.First;
                    int degree = f.GetDegree();

                    if (degree == 3)
                    {
                        newFaces.AddFace(
                            he.Start.Index,
                            he.Next.Start.Index,
                            he.Previous.Start.Index
                            );
                    }
                    else if (degree == 4)
                    {
                        newFaces.AddFace(
                            he.Start.Index,
                            he.Next.Start.Index,
                            he.Next.Next.Start.Index,
                            he.Previous.Start.Index
                            );
                    }
                    else
                    {
                        foreach (var quad in quadrangulator.GetQuads(he))
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
            public static T CreateFromQuadStrip<V, E, F>(QuadStrip<V, E, F> strip, Func<V, Point3f> getPosition, Func<V, Vec3f> getNormal = null, Func<V, Point2f> getTexture = null, Func<V, Color> getColor = null)
                where V : HeMesh<V, E, F>.Vertex
                where E : HeMesh<V, E, F>.Halfedge
                where F : HeMesh<V, E, F>.Face
            {
                var mesh = new T();

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
                if (strip.IsPeriodic)
                    faces.AddFace(n, n + 1, 1, 0);

                return mesh;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="mesh"></param>
            /// <param name="vertexValues"></param>
            /// <param name="interval"></param>
            /// <returns></returns>
            public static T CreateIsoTrim(T mesh, IReadOnlyList<double> vertexValues, Intervald interval)
            {
                // TODO implement
                throw new NotImplementedException();
            }
        }
    }
}

#endregion


#region NurbsSurface

namespace SpatialSlur.Rhino
{
    using T = NurbsSurface;
    using V = HeMesh3d.Vertex;
    using E = HeMesh3d.Halfedge;

    /// <summary>
    /// 
    /// </summary>
    public static partial class RhinoFactory
    {
        /// <summary>
        /// 
        /// </summary>
        public static class NurbsSurface
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="mesh"></param>
            /// <param name="degreeU"></param>
            /// <param name="degreeV"></param>
            /// <returns></returns>
            public static List<T> CreateFromQuadMesh(HeMesh3d mesh, int degreeU, int degreeV)
            {
                if (!IsQuadMesh(mesh))
                    throw new ArgumentException("The given mesh must have exclusively quadrilateral faces.");

                var verts = mesh.Vertices;
                var corners = verts.Where(v => IsSingular(v)).ToList();

                // if no singular verts, add first boundary vertex (cylinder)
                if (corners.Count == 0)
                {
                    var vc = verts.FirstOrDefault(v => !v.IsUnused && v.IsBoundary);

                    // if no boundary add first valid vertex (torus)
                    if (vc == null)
                        vc = verts.FirstOrDefault(v => !v.IsUnused);

                    corners.Add(vc);
                }

                var labels = GetVertexLabels(mesh, corners);
                return GetPatchSurfaces(mesh, corners, labels, degreeU, degreeV).ToList();
            }


            /// <summary>
            /// 
            /// </summary>
            private static bool IsQuadMesh(HeMesh3d mesh)
            {
                foreach (var f in mesh.Faces)
                {
                    if (f.IsUnused || f.IsDegree(4)) continue;
                    return false;
                }

                return true;
            }


            /// <summary>
            ///
            /// </summary>
            private static bool IsSingular(V vertex)
            {
                if (vertex.IsUnused)
                    return false;

                if (vertex.IsBoundary)
                {
                    if (!vertex.IsDegree3)
                        return true;
                }
                else
                {
                    if (!vertex.IsDegree(4))
                        return true;
                }

                return false;
            }


            /// <summary>
            /// Labels vertices as 0 = interior, 1 = seam, 2 = corner
            /// </summary>
            private static int[] GetVertexLabels(HeMesh3d mesh, List<V> corners)
            {
                var verts = mesh.Vertices;
                var labels = new int[verts.Count];
                int currTag = TagSeams(mesh, corners);

                // label known corners
                foreach (var v in corners)
                    labels[v] = 2;

                // set vertex labels based on the number of incident seam edges
                for (int i = 0; i < verts.Count; i++)
                {
                    var v = verts[i];
                    if (v.IsUnused || labels[i] == 2) continue;

                    // count number of tagged edges
                    int ne = 0;
                    foreach (var he in v.OutgoingHalfedges)
                    {
                        if (he.Tag == currTag)
                            if (++ne > 2) break;
                    }

                    // assign vertex label
                    if (v.IsBoundary)
                    {
                        if (ne > 0)
                        {
                            labels[i] = 2;
                            corners.Add(v);
                        }
                    }
                    else
                    {
                        if (ne == 2)
                        {
                            labels[i] = 1;
                        }
                        else if (ne > 2)
                        {
                            labels[i] = 2;
                            corners.Add(v);
                        }
                    }
                }

                return labels;
            }


            /// <summary>
            /// Tags seam edges by marching outwards from corner vertices
            /// </summary>
            private static int TagSeams(HeMesh3d mesh, List<V> corners)
            {
                int currTag = mesh.Halfedges.NextTag;

                foreach (var v in corners)
                {
                    foreach (var he0 in v.OutgoingHalfedges)
                    {
                        if (he0.IsBoundary) continue;
                        var he1 = he0;

                        while (he1.Tag != currTag && !he1.IsBoundary)
                        {
                            he1.Tag = he1.Twin.Tag = currTag;
                            he1 = he1.Next.Twin.Next;
                        }
                    }
                }

                return currTag;
            }


            /// <summary>
            /// 
            /// </summary>
            private static IEnumerable<T> GetPatchSurfaces(HeMesh3d mesh, List<V> corners, int[] labels, int degreeU, int degreeV)
            {
                var currTag = mesh.Faces.NextTag;

                foreach (var v in corners)
                {
                    foreach (var he0 in v.OutgoingHalfedges)
                    {
                        if (he0.IsHole || he0.Face.Tag == currTag) continue;

                        GetPatchDimensions(he0, labels, out int nu, out int nv);
                        var cps = GetPatchHedges(he0, nu, nv, currTag).Select(he => (Point3d)he.Start.Position);

                        yield return T.CreateFromPoints(cps, nv, nu, degreeU, degreeV); // u and v count flipped
                    }
                }
            }


            /// <summary>
            /// 
            /// </summary>
            private static void GetPatchDimensions(E hedge, int[] labels, out int countU, out int countV)
            {
                var heU = hedge;
                countU = 1;

                do
                {
                    countU++;
                    heU = heU.Next.Twin.Next;
                } while (labels[heU.Start] != 2);

                var heV = hedge;
                countV = 1;

                do
                {
                    countV++;
                    heV = heV.Next.Next.Twin;
                } while (labels[heV.Start] != 2);
            }


            /// <summary>
            /// 
            /// </summary>
            private static IEnumerable<E> GetPatchHedges(E fromCorner, int countU, int countV, int faceTag)
            {
                var heV = fromCorner;

                for (int i = 1; i < countV; i++)
                {
                    var heU = heV;

                    for (int j = 1; j < countU; j++)
                    {
                        yield return heU;
                        heU.Face.Tag = faceTag;
                        heU = heU.Next.Twin.Next; // advance in u direction
                    }

                    yield return heU; // last in row
                    heV = heV.Next.Next.Twin; // advance in v direction
                }

                // last row (don't tag faces)
                {
                    if (heV.IsHole)
                    {
                        var heU = heV;

                        for (int j = 0; j < countU; j++)
                        {
                            yield return heU;
                            heU = heU.Next; // advance in u direction
                        }
                    }
                    else
                    {
                        var heU = heV;

                        for (int j = 0; j < countU; j++)
                        {
                            yield return heU;
                            heU = heU.Next.Twin.Next; // advance in u direction
                        }
                    }
                }
            }
        }
    }
}

#endregion


#region Transform

namespace SpatialSlur.Rhino
{
    using T = Transform;

    /// <summary>
    /// 
    /// </summary>
    public static partial class RhinoFactory
    {
        /// <summary>
        /// 
        /// </summary>
        public static class Transform
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="plane"></param>
            public static T CreateFromPlane(Plane plane)
            {
                return Create(plane.Origin, plane.XAxis, plane.YAxis, plane.ZAxis);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="plane"></param>
            public static T CreateInverseFromPlane(Plane plane)
            {
                return CreateOrthoInverse(plane.Origin, plane.XAxis, plane.YAxis, plane.ZAxis);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="origin"></param>
            /// <param name="xAxis"></param>
            /// <param name="xyVector"></param>
            /// <returns></returns>
            public static T CreateProperRigid(Vector3d origin, Vector3d xAxis, Vector3d xyVector)
            {
                if (Geometry.Orthonormalize(ref xAxis, ref xyVector, out Vector3d z))
                    return Create(origin, xAxis, xyVector, z);

                return T.Unset;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="origin"></param>
            /// <param name="xAxis"></param>
            /// <param name="xyVector"></param>
            /// <returns></returns>
            public static T CreateProperRigidInverse(Vector3d origin, Vector3d xAxis, Vector3d xyVector)
            {
                if (Geometry.Orthonormalize(ref xAxis, ref xyVector, out Vector3d z))
                    return CreateOrthoInverse(origin, xAxis, xyVector, z);

                return T.Unset;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="origin"></param>
            /// <param name="basisX"></param>
            /// <param name="basisY"></param>
            /// <param name="basisZ"></param>
            /// <returns></returns>
            public static T Create(Vector3d origin, Vector3d basisX, Vector3d basisY, Vector3d basisZ)
            {
                T m = T.Identity;

                m.M00 = basisX.X;
                m.M01 = basisY.X;
                m.M02 = basisZ.X;
                m.M03 = origin.X;

                m.M10 = basisX.Y;
                m.M11 = basisY.Y;
                m.M12 = basisZ.Y;
                m.M13 = origin.Y;

                m.M20 = basisX.Z;
                m.M21 = basisY.Z;
                m.M22 = basisZ.Z;
                m.M23 = origin.Z;

                return m;
            }


            /// <summary>
            /// Assumes the given axes are orthonormal.
            /// </summary>
            /// <param name="origin"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="z"></param>
            /// <returns></returns>
            private static T CreateOrthoInverse(Vector3d origin, Vector3d x, Vector3d y, Vector3d z)
            {
                T m = T.Identity;

                m.M00 = x.X;
                m.M01 = x.Y;
                m.M02 = x.Z;
                m.M03 = -Vector3d.Dot(origin, x);

                m.M10 = y.X;
                m.M11 = y.Y;
                m.M12 = y.Z;
                m.M13 = -Vector3d.Dot(origin, y);

                m.M20 = z.X;
                m.M21= z.Y;
                m.M22 = z.Z;
                m.M23 = -Vector3d.Dot(origin, z);

                return m;
            }
        }
    }
}

#endregion

#endif