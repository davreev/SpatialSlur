using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public static class HeSubdivide
    {
        // Delegates for different SmoothBoundary types
        private static Action<HeVertexList, int>[] _ccSmooth = 
        { 
            CCSmoothFixed, 
            CCSmoothCornerFixed, 
            CCSmoothFree 
        };


        /// <summary>
        /// Applies a single iteration of TriSplit subdivision.
        /// This is equivalent to Loop subdiision without the smoothing.
        /// </summary>
        /// <param name="mesh"></param>
        public static void TriSplit(HeMesh mesh)
        {
            // TODO
        }


        /// <summary>
        /// Applies a single iteration of QuadSplit subdivision. 
        /// This is equivalent to CatmullClark subdivision without the smoothing.
        /// </summary>
        /// <param name="mesh"></param>
        public static void QuadSplit(HeMesh mesh)
        {
            var verts = mesh.Vertices;
            var edges = mesh.Halfedges;
            var faces = mesh.Faces;

            int nv = verts.Count;
            int ne = edges.Count;
            int nf = faces.Count;

            // create face vertices (1 new vertex per face)
            for (int i = 0; i < nf; i++)
            {
                HeFace f = faces[i];

                if (f.IsUnused)
                    verts.Add(new Vec3d()); // add dummy vertex for unused elements
                else
                    verts.Add(f.GetCenter());
            }

            // create edge vertices (1 new vertex per halfedge pair)
            for (int i = 0; i < ne; i += 2)
            {
                Halfedge he = edges[i];

                if (he.IsUnused)
                    verts.Add(new Vec3d()); // add dummy vertex for unused elements
                else
                    edges.SplitEdgeImpl(he); // split edge (adds a new vertex at edge center)
            }

            // create new edges and faces
            for (int i = 0; i < nf; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;

                HeVertex fv = verts[nv + i]; // face vertex
                Halfedge he0 = f.First;
                HeVertex v0 = he0.Start;

                // ensure that the first halfedge in the face starts from a new vertex
                if (v0.Index < nv)
                {
                    he0 = he0.Next;
                    v0 = he0.Start;
                    f.First = he0;
                }

                // create new halfedges to face vertex and link up with old halfedges
                Halfedge he1 = he0;
                do
                {
                    Halfedge he2 = edges.AddPair(he1.Start, fv);
                    Halfedge.MakeConsecutive(he1.Previous, he2);
                    Halfedge.MakeConsecutive(he2.Twin, he1);
                    he1 = he1.Next.Next;
                } while (he1.Start != v0);

                // set outgoing halfedge for the face vertex
                fv.First = he1.Twin;

                // connect new halfedges to eachother and create new faces where necessary
                he1 = he0;
                do
                {
                    Halfedge he2 = he1.Next;
                    Halfedge he3 = he2.Next;
                    Halfedge he4 = he1.Previous;
                    Halfedge.MakeConsecutive(he3, he4);

                    // create new face if necessary
                    if (f == null)
                    {
                        f = new HeFace();
                        faces.Add(f);
                        f.First = he1;
                        he1.Face = he2.Face = f; // update face refs for old halfedges
                    }

                    // set face refs for new halfedges
                    he3.Face = he4.Face = f;
                    f = null;

                    he1 = he3.Twin.Next;
                } while (he1.Start != v0);
            }
        }


        /// <summary>
        /// Applies a single iteration of Catmull-Clark subdivision to the given mesh.
        /// http://rosettacode.org/wiki/Catmull%E2%80%93Clark_subdivision_surface
        /// http://w3.impa.br/~lcruz/courses/cma/surfaces.html
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="boundaryType"></param>
        public static void CatmullClark(HeMesh mesh, SmoothBoundaryType boundaryType)
        {
            var verts = mesh.Vertices;
            int nv = verts.Count;
            int nf = mesh.Faces.Count;

            // apply topological changes
            QuadSplit(mesh);
     
            // update old vertex positions (this depends on boundary type)
            _ccSmooth[(int)boundaryType](verts, nv);

            // update halfedge vertex positions
            for (int i = nv + nf; i < verts.Count; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused || v.IsBoundary) continue;

                Halfedge he = v.First;
                HeVertex fv0 = he.Previous.Start;
                HeVertex fv1 = he.Twin.Next.End;
                v.Position = ((fv0.Position + fv1.Position) * 0.5 + v.Position) * 0.5;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void CCSmoothFixed(HeVertexList vertices, int count)
        {
            for (int i = 0; i < count; i++)
            {
                HeVertex v = vertices[i];
                if (v.IsUnused || v.IsBoundary) continue;

                Vec3d favg = new Vec3d();
                Vec3d eavg = new Vec3d();
                int n = 0;

                foreach (Halfedge he in v.IncomingHalfedges)
                {
                    eavg += he.Start.Position;
                    favg += he.Previous.Start.Position;
                    n++;
                }

                double t = 1.0 / n;
                v.Position = (t * favg + t * 2 * eavg + (n - 3) * v.Position) * t;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void CCSmoothCornerFixed(HeVertexList vertices, int count)
        {
            for (int i = 0; i < count; i++)
            {
                HeVertex v = vertices[i];
                if (v.IsUnused) continue;

                if (v.IsBoundary)
                {
                    Halfedge he = v.First;
                    if (!he.IsFromDegree2) v.Position = 0.25 * he.End.Position + 0.25 * he.Previous.Start.Position + 0.5 * v.Position;
                }
                else
                {
                    Vec3d favg = new Vec3d();
                    Vec3d eavg = new Vec3d();
                    int n = 0;

                    foreach (Halfedge he in v.IncomingHalfedges)
                    {
                        eavg += he.Start.Position;
                        favg += he.Previous.Start.Position;
                        n++;
                    }

                    double t = 1.0 / n;
                    v.Position = (t * favg + t * 2 * eavg + (n - 3) * v.Position) * t;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void CCSmoothFree(HeVertexList vertices, int count)
        {
            for (int i = 0; i < count; i++)
            {
                HeVertex v = vertices[i];
                if (v.IsUnused) continue;

                if (v.IsBoundary)
                {
                    Halfedge he = v.First;
                    v.Position = 0.25 * he.End.Position + 0.25 * he.Previous.Start.Position + 0.5 * v.Position;
                }
                else
                {
                    Vec3d favg = new Vec3d();
                    Vec3d eavg = new Vec3d();
                    int n = 0;

                    foreach (Halfedge he in v.IncomingHalfedges)
                    {
                        eavg += he.Start.Position;
                        favg += he.Previous.Start.Position;
                        n++;
                    }

                    double t = 1.0 / n;
                    v.Position = (t * favg + t * 2 * eavg + (n - 3) * v.Position) * t;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void CCSmoothCrease(HeVertexList vertices, int count, IList<int> edgeStatus)
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
        /// <param name="mesh"></param>
        public static void Stellate(HeMesh mesh)
        {
            HeFaceList faces = mesh.Faces;
            int nf = faces.Count;

            for (int i = 0; i < nf; i++)
            {
                HeFace f = faces[i];
                if (!f.IsUnused) faces.StellateImpl(f, f.GetCenter());
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="facePoints"></param>
        public static void Stellate(HeMesh mesh, IList<Vec3d> facePoints)
        {
            HeFaceList faces = mesh.Faces;
            faces.SizeCheck(facePoints);
            int nf = faces.Count;

            for (int i = 0; i < nf; i++)
            {
                HeFace f = faces[i];
                if (!f.IsUnused) faces.StellateImpl(f, facePoints[i]);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="halfedgePoints"></param>
        /// <param name="closeFaces"></param>
        /// <param name="closeVertices"></param>
        /// <returns></returns>
        public static HeMesh BevelEdges(HeMesh mesh, IList<Vec3d> halfedgePoints, bool closeVertices = true, bool closeFaces = true)
        {
            var hedges = mesh.Halfedges;
            hedges.SizeCheck(halfedgePoints);

            HeMesh result = new HeMesh();
            var newVerts = result.Vertices;

            // add one new vertex per halfedge
            for (int i = 0; i < hedges.Count; i++)
                newVerts.Add(halfedgePoints[i]);

            // add faces to result
            BevelEdgesAddFaces(mesh, result, closeVertices, closeFaces);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="faceCenters"></param>
        /// <param name="value"></param>
        /// <param name="closeVertices"></param>
        /// <param name="closeFaces"></param>
        /// <returns></returns>
        public static HeMesh BevelEdges(HeMesh mesh, IList<Vec3d> faceCenters, double value, bool closeVertices = true, bool closeFaces = true)
        {
            var hedges = mesh.Halfedges;
            mesh.Faces.SizeCheck(faceCenters);

            HeMesh result = new HeMesh();
            var newVerts = result.Vertices;

            // add one new vertex per halfedge
            for (int i = 0; i < hedges.Count; i++)
            {
                Halfedge he = hedges[i];
                HeFace f = he.Face;

                if (he.IsUnused || f == null)
                    newVerts.Add(new Vec3d()); // add dummy vertex for unused halfedges
                else
                {
                    int fi = f.Index;
                    newVerts.Add(Vec3d.Lerp(he.Start.Position, faceCenters[fi], value));
                }
            }

            // add faces to result
            BevelEdgesAddFaces(mesh, result, closeVertices, closeFaces);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="faceCenters"></param>
        /// <param name="faceValues"></param>
        /// <param name="closeVertices"></param>
        /// <param name="closeFaces"></param>
        /// <returns></returns>
        public static HeMesh BevelEdges(HeMesh mesh, IList<Vec3d> faceCenters, IList<double> faceValues, bool closeVertices = true, bool closeFaces = true)
        {
            var hedges = mesh.Halfedges;
            var faces = mesh.Faces;
            faces.SizeCheck(faceValues);
            faces.SizeCheck(faceCenters);

            HeMesh result = new HeMesh();
            var newVerts = result.Vertices;

            // add one new vertex per halfedge
            for (int i = 0; i < hedges.Count; i++)
            {
                Halfedge he = hedges[i];
                HeFace f = he.Face;
      
                if (he.IsUnused || f == null)
                    newVerts.Add(new Vec3d()); // add placeholder vertex to retain list order
                else
                {
                    int fi = f.Index;
                    newVerts.Add(Vec3d.Lerp(he.Start.Position, faceCenters[fi], faceValues[fi]));
                }
            }

            // add faces to result
            BevelEdgesAddFaces(mesh, result, closeVertices, closeFaces);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        private static void BevelEdgesAddFaces(HeMesh mesh, HeMesh newMesh, bool closeVertices, bool closeFaces)
        {
            var verts = mesh.Vertices;
            var hedges = mesh.Halfedges;
            var faces = mesh.Faces;

            var newVerts = newMesh.Vertices;
            var newFaces = newMesh.Faces;

            var fv = new List<HeVertex>(4);
            fv.Fill();  

            // add halfedge faces (1 per halfedge in the original mesh)
            for (int i = 0; i < hedges.Count; i += 2)
            {
                Halfedge he0 = hedges[i];
                if (he0.IsUnused || he0.IsBoundary) continue;
                Halfedge he1 = he0.Twin;

                fv[0] = newVerts[he0.Next.Index];
                fv[1] = newVerts[he0.Index];
                fv[2] = newVerts[he1.Next.Index];
                fv[3] = newVerts[he1.Index];
                newFaces.AddImpl(fv);
            }

            // add vertex faces (1 per vertex in the original mesh)
            if (closeVertices)
            {
                for (int i = 0; i < verts.Count; i++)
                {
                    HeVertex v = verts[i];
                    if (v.IsUnused || v.IsBoundary) continue;
               
                    // circulate vertex in reverse for consistent face windings
                    fv.Clear();
                    Halfedge he = v.First;
                    do
                    {
                        fv.Add(newVerts[he.Index]);
                        he = he.Previous.Twin;
                    } while (he != v.First);

                    newFaces.AddImpl(fv);
                }
            }

            // add face faces (1 per face in the original mesh)
            if (closeFaces)
            {
                for (int i = 0; i < faces.Count; i++)
                {
                    HeFace f = faces[i];
                    if (f.IsUnused) continue;
              
                    // collect indices of face halfedges
                    fv.Clear();
                    foreach (Halfedge he in f.Halfedges)
                        fv.Add(newVerts[he.Index]);

                    newFaces.AddImpl(fv);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="facePoints"></param>
        /// <param name="halfedgePoints"></param>
        /// <param name="closeVertices"></param>
        /// <returns></returns>
        public static HeMesh FrameDual(HeMesh mesh, IList<Vec3d> facePoints, IList<Vec3d> halfedgePoints, bool closeVertices = true)
        {
            var hedges = mesh.Halfedges;
            var faces = mesh.Faces;
            hedges.SizeCheck(halfedgePoints);
            faces.SizeCheck(facePoints);

            HeMesh result = new HeMesh();
            var newVerts = result.Vertices;

            // add one new vertex per face
            for (int i = 0; i < faces.Count; i++)
                newVerts.Add(facePoints[i]);

            // add one new vertex per edge
            for (int i = 0; i < hedges.Count; i++)
                newVerts.Add(halfedgePoints[i]);

            // add faces to result
            FrameDualAddFaces(mesh, result, closeVertices);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="facePoints"></param>
        /// <param name="value"></param>
        /// <param name="closeVertices"></param>
        /// <returns></returns>
        public static HeMesh FrameDual(HeMesh mesh, IList<Vec3d> facePoints, double value, bool closeVertices = true)
        {
            var hedges = mesh.Halfedges;
            var faces = mesh.Faces;
            faces.SizeCheck(facePoints);

            HeMesh result = new HeMesh();
            var newVerts = result.Vertices;

            // add one new vertex per face
            for (int i = 0; i < faces.Count; i++)
                newVerts.Add(facePoints[i]);

            // add one new vertex per edge
            for (int i = 0; i < hedges.Count; i++)
            {
                Halfedge he = hedges[i];
                HeFace f = he.Face;

                if (he.IsUnused || f == null)
                    result.Vertices.Add(new Vec3d()); // add dummy vertex for unused halfedges
                else
                {
                    int fi = f.Index;
                    result.Vertices.Add(Vec3d.Lerp(he.Start.Position, facePoints[fi], value));
                }
            }

            // add faces to result
            FrameDualAddFaces(mesh, result, closeVertices);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="facePoints"></param>
        /// <param name="faceValues"></param>
        /// <param name="closeVertices"></param>
        /// <returns></returns>
        public static HeMesh FrameDual(HeMesh mesh, IList<Vec3d> facePoints, IList<double> faceValues, bool closeVertices = true)
        {
            var hedges = mesh.Halfedges;
            var faces = mesh.Faces;
            faces.SizeCheck(facePoints);
            faces.SizeCheck(faceValues);

            HeMesh result = new HeMesh();
            var newVerts = result.Vertices;

            // add one new vertex per face
            for (int i = 0; i < faces.Count; i++)
                newVerts.Add(facePoints[i]);

            // add one new vertex per edge
            for (int i = 0; i < hedges.Count; i++)
            {
                Halfedge he = hedges[i];
                HeFace f = he.Face;

                if (he.IsUnused || f == null)
                    result.Vertices.Add(new Vec3d()); // add dummy vertex for unused halfedges
                else
                {
                    int fi = f.Index;
                    result.Vertices.Add(Vec3d.Lerp(he.Start.Position, facePoints[fi], faceValues[fi]));
                }
            }

            // add faces to result
            FrameDualAddFaces(mesh, result, closeVertices);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        private static void FrameDualAddFaces(HeMesh mesh, HeMesh newMesh, bool closeVertices)
        {
            var verts = mesh.Vertices;
            var hedges = mesh.Halfedges;
            var faces = mesh.Faces;

            var newVerts = newMesh.Vertices;
            var newFaces = newMesh.Faces;
       
            var fv = new List<HeVertex>(4);
            fv.Fill();
            int nf = faces.Count;

            // add faces
            for (int i = 0; i < hedges.Count; i += 2)
            {
                Halfedge he0 = hedges[i];
                if (he0.IsUnused || he0.IsBoundary) continue;
                Halfedge he1 = he0.Twin;

                HeFace f0 = he0.Face;
                HeFace f1 = he1.Face;

                fv[0] = newVerts[f0.Index];
                fv[1] = newVerts[he0.Index + nf];
                fv[2] = newVerts[he1.Next.Index + nf];
                fv[3] = newVerts[f1.Index];
                newFaces.AddImpl(fv);

                fv[0] = newVerts[f1.Index];
                fv[1] = newVerts[he1.Index + nf];
                fv[2] = newVerts[he0.Next.Index + nf];
                fv[3] = newVerts[f0.Index];
                newFaces.AddImpl(fv);
            }

            // add vertex faces (1 per vertex in the original mesh)
            if (closeVertices)
            {
                for (int i = 0; i < verts.Count; i++)
                {
                    HeVertex v = verts[i];
                    if (v.IsUnused || v.IsBoundary) continue;
       
                    // circulate vertex in reverse for consistent face windings
                    fv.Clear();
                    Halfedge he = v.First;
                    do
                    {
                        fv.Add(newVerts[he.Index + nf]);
                        he = he.Previous.Twin;
                    } while (he != v.First);

                    newFaces.AddImpl(fv);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="faceCenters"></param>
        /// <param name="faceNormals"></param>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static HeMesh Weave(HeMesh mesh, IList<Vec3d> faceCenters, IList<Vec3d> faceNormals, double value, double offset)
        {
            var hedges = mesh.Halfedges;
            var faces = mesh.Faces;
            faces.SizeCheck(faceCenters);
            faces.SizeCheck(faceNormals);

            HeMesh result = new HeMesh();
            var newVerts = result.Vertices;
            var newFaces = result.Faces;

            // add vertices (8 per halfedge pair in m0)
            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he0 = hedges[i];

                // add dummy verts for unused edges
                if (he0.IsUnused)
                {
                    for (int j = 0; j < 8; j++)
                        newVerts.Add(new HeVertex());

                    continue;
                }

                var he1 = hedges[i + 1];
                var f0 = he0.Face;
                var f1 = he1.Face;

                // get new verts
                Vec3d cen = he0.PointAt(0.5);
                Vec3d p0 = Vec3d.Lerp(he0.Start.Position, cen, value);
                Vec3d p1 = (f0 == null) ? new Vec3d() : Vec3d.Lerp(faceCenters[f0.Index], cen, value);
                Vec3d p2 = Vec3d.Lerp(he1.Start.Position, cen, value);
                Vec3d p3 = (f1 == null) ? new Vec3d() : Vec3d.Lerp(faceCenters[f1.Index], cen, value); 

                // add verts
                if (he0.IsBoundary)
                {
                    newVerts.Add(p0);
                    newVerts.Add(p1);
                    newVerts.Add(p0);
                    newVerts.Add(p1);

                    newVerts.Add(p2);
                    newVerts.Add(p3);
                    newVerts.Add(p2);
                    newVerts.Add(p3);
                }
                else
                {
                    Vec3d d = faceNormals[f0.Index] + faceNormals[f1.Index];
                    d *= offset / d.Length;

                    newVerts.Add(p0 - d);
                    newVerts.Add(p1 - d);
                    newVerts.Add(p0 + d);
                    newVerts.Add(p1 + d);

                    newVerts.Add(p2 - d);
                    newVerts.Add(p3 - d);
                    newVerts.Add(p2 + d);
                    newVerts.Add(p3 + d);
                }
            }

            // add faces
            WeaveAddFaces(mesh, result);
            return result;
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
        public static HeMesh Weave(HeMesh mesh, IList<Vec3d> faceCenters, IList<Vec3d> vertexNormals, IList<double> edgeValues, double offset)
        {
            // TODO
            throw new NotImplementedException();

            // add faces
            //WeaveAddFaces(mesh, result);
            //return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="faceCenters"></param>
        /// <param name="vertexNormals"></param>
        /// <param name="edgeValues"></param>
        /// <param name="vertexOffsets"></param>
        /// <returns></returns>
        public static HeMesh Weave(HeMesh mesh, IList<Vec3d> faceCenters, IList<Vec3d> vertexNormals, IList<double> edgeValues, IList<double> vertexOffsets)
        {
            // TODO
            throw new NotImplementedException();

            // add faces
            //WeaveAddFaces(mesh, result);
            //return result;
        }


        /// <summary>
        /// 
        /// </summary>
        private static void WeaveAddFaces(HeMesh mesh, HeMesh newMesh)
        {
            var hedges = mesh.Halfedges;
            var newFaces = newMesh.Faces;

            // add node faces
            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he0 = hedges[i];
                var he1 = hedges[i + 1];
                int i0 = i << 2;

                if (he0.Face == null)
                {
                    newFaces.Add(i0, i0 + 5, i0 + 4);
                }
                else if (he1.Face == null)
                {
                    newFaces.Add(i0 + 4, i0 + 1, i0);
                }
                else
                {
                    newFaces.Add(i0 + 5, i0 + 4, i0 + 1, i0);
                    newFaces.Add(i0 + 7, i0 + 6, i0 + 3, i0 + 2);
                }
            }

            // add edge faces
            for (int i = 0; i < hedges.Count; i++)
            {
                var he0 = hedges[i];
                if (he0.Face == null) continue;

                int i0 = i << 2;
                int i1 = he0.Next.Index << 2;

                if (he0.IsBoundary)
                {
                    if ((i & 1) == 0)
                        newFaces.Add(i0 + 1, i0 + 4, i1, i1 + 1); // even edges
                    else
                        newFaces.Add(i0 + 1, i0 - 4, i1, i1 + 1); // odd edges
                }
                else
                {
                    if ((i & 1) == 0)
                        newFaces.Add(i0 + 3, i0 + 6, i1, i1 + 1);  // even edges
                    else
                        newFaces.Add(i0 + 3, i0 - 2, i1, i1 + 1); // odd edges
                }
            }
        }

    }
}
