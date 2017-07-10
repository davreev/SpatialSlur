using System;
using System.Collections.Generic;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    using M = HeMesh<HeMesh3d.V, HeMesh3d.E, HeMesh3d.F>;

    /// <summary>
    /// 
    /// </summary>
    public static class HeMeshSubdivide
    {
        /// <summary>
        /// Delegates for different SmoothBoundary types
        /// </summary>
        private static Action<HeMesh, List<Vec3d>>[] _catmullClarkSmooth =
        {
            CatmullClarkSmoothFixed,
            CatmullClarkSmoothCornerFixed,
            CatmullClarkSmoothFree
        };


        /// <summary>
        ///
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        public static void TriSplit(HeMesh mesh, List<Vec3d> vertexPositions)
        {
            // TODO
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public static void Loop(HeMesh mesh)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="TF"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="getPosition"></param>
        /// <param name="setPosition"></param>
        public static void QuadSplit<TV, TE, TF>(HeMesh<TV, TE, TF> mesh, Func<TV, Vec3d> getPosition, Action<TV, Vec3d> setPosition)
            where TV : HeVertex<TV, TE, TF>
            where TE : Halfedge<TV, TE, TF>
            where TF : HeFace<TV, TE, TF>
        {
            int nv = mesh.Vertices.Count;
            int nhe = mesh.Halfedges.Count;
            int nf = mesh.Faces.Count;

            // TODO create elements before assigning attributes

            QuadSplitTopo(mesh);
            QuadSplitGeom(mesh, getPosition, setPosition, nv, nhe, nf);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        private static void QuadSplitTopo<TV, TE, TF>(HeMesh<TV, TE, TF> mesh)
            where TV : HeVertex<TV, TE, TF>
            where TE : Halfedge<TV, TE, TF>
            where TF : HeFace<TV, TE, TF>
        {
            var hedges = mesh.Halfedges;
            var verts = mesh.Vertices;
            var faces = mesh.Faces;

            int nhe = hedges.Count;
            int nv = verts.Count;
            int nf = faces.Count;

            // create face vertices (1 new vertex per face)
            mesh.AddVertices(nf);

            // create edge vertices (1 new vertex per edge)
            for (int i = 0; i < nhe; i += 2)
            {
                var he = hedges[i];

                if (he.IsRemoved)
                    mesh.AddVertex(); // add placeholder
                else
                    mesh.SplitEdgeImpl(he); // split edge (adds a new vertex)
            }

            // create new edges and faces
            for (int i = 0; i < nf; i++)
            {
                var f = faces[i];
                if (f.IsRemoved) continue;

                var fv = verts[i + nv]; // face vertex
                var he0 = f.First;
                var v0 = he0.Start;

                // advance to first old vertex in face
                if (v0.Index >= nv)
                {
                    he0 = he0.NextInFace;
                    v0 = he0.Start;
                }

                // create new halfedges to face vertex and link up with old halfedges
                var he1 = he0;
                do
                {
                    var he2 = he1.NextInFace;
                    var he3 = mesh.AddEdge(he2.Start, fv);

                    he1.MakeConsecutive(he3);
                    he3.Twin.MakeConsecutive(he2);

                    he1 = he2.NextInFace;
                } while (he1 != he0);

                // connect new halfedges and create new faces
                {
                    fv.FirstOut = he1.NextInFace.Twin;
                    var he2 = he1.PrevInFace;

                    do
                    {
                        var he3 = he1.NextInFace;
                        var he4 = he2.PrevInFace;
                        he3.MakeConsecutive(he4);

                        if (f == null)
                        {
                            f = mesh.AddFace();
                            he1.Face = he2.Face = f;
                        }

                        he3.Face = he4.Face = f; // set face refs for new halfedges
                        f.First = he1; // set first halfedge in face

                        f = null;
                        he2 = he3.Twin.NextInFace;
                        he1 = he2.NextInFace;
                    } while (he1 != he0);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="TF"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="getPosition"></param>
        /// <param name="setPosition"></param>
        private static void QuadSplitGeom<TV, TE, TF>(HeMesh<TV, TE, TF> mesh, Func<TV, Vec3d> getPosition, Action<TV, Vec3d> setPosition, int nv, int nhe, int nf)
            where TV : HeVertex<TV, TE, TF>
            where TE : Halfedge<TV, TE, TF>
            where TF : HeFace<TV, TE, TF>
        {
            var verts = mesh.Vertices;
            var hedges = mesh.Halfedges;
            var faces = mesh.Faces;

            // loop from old verts to new verts
            for (int i = 0; i < nf; i++)
            {
                var f = faces[i];
                if (f.IsRemoved) continue;

                var v = verts[i + nv];
                setPosition(v, f.Vertices.Mean(getPosition));
            }

            // create edge vertices (1 new vertex per halfedge pair)
            for (int i = 0; i < nhe; i+=2)
            {
                var he = hedges[i];
                if (he.IsRemoved) continue;

                var v = verts[(i >> 1) + nf + nv];
                setPosition(v, he.Lerp(getPosition, 0.5));
            }
        }


        /// <summary>
        /// Applies a single iteration of Catmull-Clark subdivision to the given mesh.
        /// http://rosettacode.org/wiki/Catmull%E2%80%93Clark_subdivision_surface
        /// http://w3.impa.br/~lcruz/courses/cma/surfaces.html
        /// 
        /// TODO add support for any number of scalar/vector vertex attributes (double, Vec2d, Vec3d, Vecd, double[])
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="boundaryType"></param>
        public static void CatmullClark(HeMesh mesh, List<Vec3d> vertexPositions, SmoothBoundaryType boundaryType)
        {
            CatmullClarkGeom(mesh, vertexPositions, boundaryType);
            QuadSplitTopo(mesh);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="boundaryType"></param>
        private static void CatmullClarkGeom(HeMesh mesh, List<Vec3d> vertexPositions, SmoothBoundaryType boundaryType)
        {
            var hedges = mesh.Halfedges;
            var verts = mesh.Vertices;
            var faces = mesh.Faces;

            int nhe = hedges.Count;
            int nv = verts.Count;
            int nf = faces.Count;

            // add face verts (1 per face)
            for (int i = 0; i < nf; i++)
            {
                var f = faces[i];

                if (f.IsRemoved)
                    vertexPositions.Add(new Vec3d());
                else
                    vertexPositions.Add(f.Vertices.Mean(vertexPositions));
            }

            // add edge verts (1 per edge)
            // requires face points to compute
            for (int i = 0; i < nhe; i += 2)
            {
                var he0 = hedges[i];

                if (he0.IsRemoved)
                {
                    vertexPositions.Add(new Vec3d()); // add placeholder vertex for unused elements
                }
                else 
                {
                    var he1 = hedges[i + 1];
                    var p0 = vertexPositions[he0.Start.Index];
                    var p1 = vertexPositions[he1.Start.Index];

                    if (he0.IsBoundary)
                    {
                        vertexPositions.Add((p0 + p1) * 0.5);
                    }
                    else
                    {
                        var p2 = vertexPositions[he0.Face.Index + nv];
                        var p3 = vertexPositions[he1.Face.Index + nv];
                        vertexPositions.Add((p0 + p1 + p2 + p3) * 0.25);
                    }
                }
            }

            // update old vertex positions (this depends on boundary type)
            _catmullClarkSmooth[(int)boundaryType](mesh, vertexPositions);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void CatmullClarkSmoothFixed(HeMesh mesh, List<Vec3d> vertexPositions)
        {
            var verts = mesh.Vertices;

            int nv = verts.Count; // index of first face vertex
            int nvf = nv + mesh.Faces.Count; // index of first edge vertex

            for (int i = 0; i < nv; i++)
            {
                var v = verts[i];
                if (v.IsRemoved || v.IsBoundary) continue; // skip boundary verts

                Vec3d fsum = new Vec3d();
                Vec3d esum = new Vec3d();
                int n = 0;

                foreach (var he in v.OutgoingHalfedges)
                {
                    fsum += vertexPositions[he.Face.Index + nv];
                    esum += vertexPositions[(he.Index >> 1) + nvf];
                    n++;
                }

                double t = 1.0 / n;
                vertexPositions[i] = (vertexPositions[i] * (n - 3) + fsum * t + 2 * esum * t) * t;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void CatmullClarkSmoothCornerFixed(HeMesh mesh, List<Vec3d> vertexPositions)
        {
            var verts = mesh.Vertices;

            int nv = verts.Count; // index of first face vertex
            int nvf = nv + mesh.Faces.Count; // index of first edge vertex

            for (int i = 0; i < nv; i++)
            {
                var v = verts[i];
                if (v.IsRemoved) continue;

                if (v.IsBoundary)
                {
                    var he = v.FirstOut;
                    if (!he.IsAtDegree2)
                    {
                        var p0 = vertexPositions[(he.Index >> 1) + nvf];
                        var p1 = vertexPositions[(he.PrevInFace.Index >> 1) + nvf];
                        vertexPositions[i] = vertexPositions[i] * 0.5 + (p0 + p1) * 0.25;
                    }
                }
                else
                {
                    Vec3d fsum = new Vec3d();
                    Vec3d esum = new Vec3d();
                    int n = 0;

                    foreach (var he in v.OutgoingHalfedges)
                    {
                        fsum += vertexPositions[he.Face.Index + nv];
                        esum += vertexPositions[(he.Index >> 1) + nvf];
                        n++;
                    }

                    double t = 1.0 / n;
                    vertexPositions[i] = (vertexPositions[i] * (n - 3) + fsum * t + 2 * esum * t) * t;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void CatmullClarkSmoothFree(HeMesh mesh, List<Vec3d> vertexPositions)
        {
            var verts = mesh.Vertices;

            int nv = verts.Count; // index of first face vertex
            int nvf = nv + mesh.Faces.Count; // index of first edge vertex

            for (int i = 0; i < nv; i++)
            {
                var v = verts[i];
                if (v.IsRemoved) continue;

                if (v.IsBoundary)
                {
                    var he = v.FirstOut;
                    var p0 = vertexPositions[(he.Index >> 1) + nvf];
                    var p1 = vertexPositions[(he.PrevInFace.Index >> 1) + nvf];
                    vertexPositions[i] = vertexPositions[i] * 0.5 + (p0 + p1) * 0.25;
                }
                else
                {
                    Vec3d fsum = new Vec3d();
                    Vec3d esum = new Vec3d();
                    int n = 0;

                    foreach (var he in v.OutgoingHalfedges)
                    {
                        fsum += vertexPositions[he.Face.Index + nv];
                        esum += vertexPositions[(he.Index >> 1) + nvf];
                        n++;
                    }

                    double t = 1.0 / n;
                    vertexPositions[i] = (vertexPositions[i] * (n - 3) + fsum * t + 2 * esum * t) * t;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="closeFaces"></param>
        /// <param name="closeVertices"></param>
        /// <returns></returns>
        public static HeMesh BevelEdges(HeMesh mesh, bool closeVertices = true, bool closeFaces = true)
        {
            return BevelEdgesTopo(mesh, closeVertices, closeFaces);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="faceCenters"></param>
        /// <param name="scaleFactor"></param>
        /// <param name="newVertexPositions"></param>
        /// <param name="closeVertices"></param>
        /// <param name="closeFaces"></param>
        /// <returns></returns>
        public static HeMesh BevelEdges(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<Vec3d> faceCenters, double scaleFactor, out Vec3d[] newVertexPositions, bool closeVertices = true, bool closeFaces = true)
        {
            BevelEdgesGeom(mesh, vertexPositions, faceCenters, scaleFactor, out newVertexPositions);
            return BevelEdgesTopo(mesh, closeVertices, closeFaces);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="faceCenters"></param>
        /// <param name="faceFactors"></param>
        /// <param name="newVertexPositions"></param>
        /// <param name="closeVertices"></param>
        /// <param name="closeFaces"></param>
        /// <returns></returns>
        public static HeMesh BevelEdges(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<Vec3d> faceCenters, IReadOnlyList<double> faceFactors, out Vec3d[] newVertexPositions, bool closeVertices = true, bool closeFaces = true)
        {
            BevelEdgesGeom(mesh, vertexPositions, faceCenters, faceFactors, out newVertexPositions);
            return BevelEdgesTopo(mesh, closeVertices, closeFaces);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="faceCenters"></param>
        /// <param name="scaleFactor"></param>
        /// <param name="newVertexPositions"></param>
        private static void BevelEdgesGeom(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<Vec3d> faceCenters, double scaleFactor, out Vec3d[] newVertexPositions)
        {
            var hedges = mesh.Halfedges;
            int nhe = hedges.Count;

            newVertexPositions = new Vec3d[nhe];

            // add one new vertex per halfedge
            for (int i = 0; i < nhe; i++)
            {
                var he = hedges[i];
                var f = he.Face;
                if (he.IsRemoved || f == null) continue;

                int fi = f.Index;
                var p = vertexPositions[he.Start.Index];
                newVertexPositions[i] = Vec3d.Lerp(p, faceCenters[fi], scaleFactor);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="faceCenters"></param>
        /// <param name="faceFactors"></param>
        /// <param name="newVertexPositions"></param>
        private static void BevelEdgesGeom(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<Vec3d> faceCenters, IReadOnlyList<double> faceFactors, out Vec3d[] newVertexPositions)
        {
            var hedges = mesh.Halfedges;
            int nhe = hedges.Count;

            newVertexPositions = new Vec3d[nhe];

            // add one new vertex per halfedge
            for (int i = 0; i < nhe; i++)
            {
                var he = hedges[i];
                var f = he.Face;
                if (he.IsRemoved || f == null) continue;

                int fi = f.Index;
                var p = vertexPositions[he.Start.Index];
                newVertexPositions[i] = Vec3d.Lerp(p, faceCenters[fi], faceFactors[fi]);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static HeMesh BevelEdgesTopo(HeMesh mesh, bool closeVertices, bool closeFaces)
        {
            var verts = mesh.Vertices;
            var hedges = mesh.Halfedges;
            var faces = mesh.Faces;

            int nhe = hedges.Count;
            int nv = verts.Count;
            int nf = faces.Count;

            var newMesh = new HeMesh(nhe, nhe << 2, nhe + nv + nf);
            var newVerts = newMesh.Vertices;
            var newFaces = newMesh.Faces;

            var fv = new List<Vertex>(4);
            fv.Fill();

            // add halfedge vertices (1 per halfedge in the original mesh)
            newVerts.Add(nhe);

            // add halfedge faces (1 per edge in the original mesh)
            for (int i = 0; i < nhe; i += 2)
            {
                var he0 = hedges[i];
                if (he0.IsRemoved || he0.IsBoundary) continue;
                var he1 = he0.Twin;

                fv[0] = newVerts[he0.NextInFace.Index];
                fv[1] = newVerts[he0.Index];
                fv[2] = newVerts[he1.NextInFace.Index];
                fv[3] = newVerts[he1.Index];
                newFaces.AddImpl(fv);
            }

            // add vertex faces (1 per vertex in the original mesh)
            if (closeVertices)
            {
                for (int i = 0; i < nv; i++)
                {
                    var v = verts[i];
                    if (v.IsRemoved || v.IsBoundary) continue;
               
                    // circulate vertex in reverse for consistent face windings
                    fv.Clear();
                    var he = v.FirstOut;
                    do
                    {
                        fv.Add(newVerts[he.Index]);
                        he = he.PrevInFace.Twin;
                    } while (he != v.FirstOut);

                    newFaces.AddImpl(fv);
                }
            }

            // add face faces (1 per face in the original mesh)
            if (closeFaces)
            {
                for (int i = 0; i < nf; i++)
                {
                    var f = faces[i];
                    if (f.IsRemoved) continue;
              
                    // collect indices of face halfedges
                    fv.Clear();
                    foreach (var he in f.Halfedges)
                        fv.Add(newVerts[he.Index]);

                    newFaces.AddImpl(fv);
                }
            }

            return newMesh;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="facePoints"></param>
        /// <param name="scaleFactor"></param>
        /// <param name="newVertexPositions"></param>
        /// <param name="closeVertices"></param>
        /// <returns></returns>
        public static HeMesh FrameDual(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<Vec3d> facePoints, double scaleFactor, out Vec3d[] newVertexPositions, bool closeVertices = true)
        {
            FrameDualGeom(mesh, vertexPositions, facePoints, scaleFactor, out newVertexPositions);
            return FrameDualTopo(mesh, closeVertices);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="facePoints"></param>
        /// <param name="faceFactors"></param>
        /// <param name="newVertexPositions"></param>
        /// <param name="closeVertices"></param>
        /// <returns></returns>
        public static HeMesh FrameDual(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<Vec3d> facePoints, IReadOnlyList<double> faceFactors, out Vec3d[] newVertexPositions, bool closeVertices = true)
        {
            FrameDualGeom(mesh, vertexPositions, facePoints, faceFactors, out newVertexPositions);
            return FrameDualTopo(mesh, closeVertices);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="facePoints"></param>
        /// <param name="scaleFactor"></param>
        /// <param name="newVertexPositions"></param>
        private static void FrameDualGeom(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<Vec3d> facePoints, double scaleFactor, out Vec3d[] newVertexPositions)
        {
            var hedges = mesh.Halfedges;
            int nhe = hedges.Count;
            int nf = mesh.Faces.Count;

            newVertexPositions = new Vec3d[nf + nhe];

            // add one new vertex per face
            for (int i = 0; i < nf; i++)
                newVertexPositions[i] = facePoints[i];

            // add one new vertex per edge
            for (int i = 0; i < nhe; i++)
            {
                var he = hedges[i];
                var f = he.Face;
                if (he.IsRemoved || f == null) continue;

                int fi = f.Index;
                var p = vertexPositions[he.Start.Index];
                newVertexPositions[i + nf] = Vec3d.Lerp(p, facePoints[fi], scaleFactor);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="facePoints"></param>
        /// <param name="faceFactors"></param>
        /// <param name="newVertexPositions"></param>
        private static void FrameDualGeom(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<Vec3d> facePoints, IReadOnlyList<double> faceFactors, out Vec3d[] newVertexPositions)
        {
            var hedges = mesh.Halfedges;
            int nhe = hedges.Count;
            int nf = mesh.Faces.Count;

            newVertexPositions = new Vec3d[nf + nhe];

            // add one new vertex per face
            for (int i = 0; i < nf; i++)
                newVertexPositions[i] = facePoints[i];

            // add one new vertex per edge
            for (int i = 0; i < nhe; i++)
            {
                var he = hedges[i];
                var f = he.Face;
                if (he.IsRemoved || f == null) continue;

                int fi = f.Index;
                var p = vertexPositions[he.Start.Index];
                newVertexPositions[i + nf] = Vec3d.Lerp(p, facePoints[fi], faceFactors[fi]);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static HeMesh FrameDualTopo(HeMesh mesh, bool closeVertices)
        {
            var verts = mesh.Vertices;
            var hedges = mesh.Halfedges;
            var faces = mesh.Faces;

            int nhe = hedges.Count;
            int nv = verts.Count;
            int nf = faces.Count;

            var newMesh = new HeMesh(nhe, nhe * 5, nhe + nf);
            var newVerts = newMesh.Vertices;
            var newFaces = newMesh.Faces;

            var fv = new List<Vertex>(4);
            fv.Fill();

            // add face vertices (1 per face)
            newVerts.Add(nf);

            // add halfedge vertices (1 per halfedge)
            newVerts.Add(nhe);

            // add faces
            for (int i = 0; i < nhe; i += 2)
            {
                var he0 = hedges[i];
                if (he0.IsRemoved || he0.IsBoundary) continue;
                var he1 = he0.Twin;

                var f0 = he0.Face;
                var f1 = he1.Face;

                fv[0] = newVerts[f0.Index];
                fv[1] = newVerts[he0.Index + nf];
                fv[2] = newVerts[he1.NextInFace.Index + nf];
                fv[3] = newVerts[f1.Index];
                newFaces.AddImpl(fv);

                fv[0] = newVerts[f1.Index];
                fv[1] = newVerts[he1.Index + nf];
                fv[2] = newVerts[he0.NextInFace.Index + nf];
                fv[3] = newVerts[f0.Index];
                newFaces.AddImpl(fv);
            }

            // add vertex faces (1 per vertex in the original mesh)
            if (closeVertices)
            {
                for (int i = 0; i < nv; i++)
                {
                    var v = verts[i];
                    if (v.IsRemoved || v.IsBoundary) continue;
       
                    // circulate vertex in reverse for consistent face windings
                    fv.Clear();
                    var he = v.FirstOut;
                    do
                    {
                        fv.Add(newVerts[he.Index + nf]);
                        he = he.PrevInFace.Twin;
                    } while (he != v.FirstOut);

                    newFaces.AddImpl(fv);
                }
            }

            return newMesh;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="faceCenters"></param>
        /// <param name="vertexNormals"></param>
        /// <param name="edgeValues"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static HeMesh Weave(HeMesh mesh, IReadOnlyList<Vec3d> faceCenters, IReadOnlyList<Vec3d> vertexNormals, IReadOnlyList<double> edgeValues, double offset)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="faceCenters"></param>
        /// <param name="faceNormals"></param>
        /// <param name="scaleFactor"></param>
        /// <param name="offset"></param>
        /// <param name="newVertexPositions"></param>
        /// <returns></returns>
        public static HeMesh Weave(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<Vec3d> faceCenters, IReadOnlyList<Vec3d> faceNormals, double scaleFactor, double offset, out Vec3d[] newVertexPositions)
        {
            WeaveGeom(mesh, vertexPositions, faceCenters, faceNormals, scaleFactor, offset, out newVertexPositions);
            return WeaveTopo(mesh);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="faceCenters"></param>
        /// <param name="faceNormals"></param>
        /// <param name="scaleFactor"></param>
        /// <param name="offset"></param>
        /// <param name="newVertexPositions"></param>
        private static void WeaveGeom(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<Vec3d> faceCenters, IReadOnlyList<Vec3d> faceNormals, double scaleFactor, double offset, out Vec3d[] newVertexPositions)
        {
            var hedges = mesh.Halfedges;
            int nhe = hedges.Count;

            newVertexPositions = new Vec3d[nhe << 2];

            // add vertices (8 per halfedge pair in m0)
            for (int i = 0; i < nhe; i += 2)
            {
                var he0 = hedges[i];
                var he1 = hedges[i + 1];
                var f0 = he0.Face;
                var f1 = he1.Face;

                Vec3d p00 = vertexPositions[he0.Start.Index];
                Vec3d p01 = vertexPositions[he1.Start.Index];
                Vec3d cen = Vec3d.Lerp(p00, p01, 0.5);

                Vec3d p0 = Vec3d.Lerp(cen, p00, scaleFactor);
                Vec3d p1 = (f0 == null) ? new Vec3d() : Vec3d.Lerp(cen, faceCenters[f0.Index], scaleFactor);
                Vec3d p2 = Vec3d.Lerp(cen, p01, scaleFactor);
                Vec3d p3 = (f1 == null) ? new Vec3d() : Vec3d.Lerp(cen, faceCenters[f1.Index], scaleFactor);

                int j = i << 2;
       
                // add verts
                if (he0.IsBoundary)
                {
                    newVertexPositions[j] = p0;
                    newVertexPositions[j + 1] = p1;
                    newVertexPositions[j + 2] = p0;
                    newVertexPositions[j + 3] = p1;

                    newVertexPositions[j + 4] = p2;
                    newVertexPositions[j + 5] = p3;
                    newVertexPositions[j + 6] = p2;
                    newVertexPositions[j + 7] = p3;
                }
                else
                {
                    Vec3d d = faceNormals[f0.Index] + faceNormals[f1.Index];
                    d *= offset / d.Length;

                    newVertexPositions[j] = p0 - d;
                    newVertexPositions[j + 1] = p1 - d;
                    newVertexPositions[j + 2] = p0 + d;
                    newVertexPositions[j + 3] = p1 + d;

                    newVertexPositions[j + 4] = p2 - d;
                    newVertexPositions[j + 5] = p3 - d;
                    newVertexPositions[j + 6] = p2 + d;
                    newVertexPositions[j + 7] = p3 + d;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static HeMesh WeaveTopo(HeMesh mesh)
        {
            var hedges = mesh.Halfedges;
            int nhe = hedges.Count;

            var newMesh = new HeMesh(nhe << 2, nhe << 3, nhe << 2);
            var newVerts = newMesh.Vertices;
            var newFaces = newMesh.Faces;

            // add vertices (8 per edge in initial mesh)
            for (int i = 0; i < nhe; i += 2)
                newVerts.Add(8);

            // add node faces
            for (int i = 0; i < nhe; i += 2)
            {
                var he0 = hedges[i];
                var he1 = hedges[i + 1];
                int j = i << 2;

                if (he0.Face == null)
                {
                    newFaces.Add(j, j + 5, j + 4);
                }
                else if (he1.Face == null)
                {
                    newFaces.Add(j + 4, j + 1, j);
                }
                else
                {
                    newFaces.Add(j + 5, j + 4, j + 1, j);
                    newFaces.Add(j + 7, j + 6, j + 3, j + 2);
                }
            }

            // add edge faces
            for (int i = 0; i < nhe; i++)
            {
                var he0 = hedges[i];
                if (he0.Face == null) continue;

                int j0 = i << 2;
                int j1 = he0.NextInFace.Index << 2;

                if (he0.IsBoundary)
                {
                    if ((i & 1) == 0)
                        newFaces.Add(j0 + 1, j0 + 4, j1, j1 + 1); // even edges
                    else
                        newFaces.Add(j0 + 1, j0 - 4, j1, j1 + 1); // odd edges
                }
                else
                {
                    if ((i & 1) == 0)
                        newFaces.Add(j0 + 3, j0 + 6, j1, j1 + 1);  // even edges
                    else
                        newFaces.Add(j0 + 3, j0 - 2, j1, j1 + 1); // odd edges
                }
            }

            return newMesh;
        }
    }
}
