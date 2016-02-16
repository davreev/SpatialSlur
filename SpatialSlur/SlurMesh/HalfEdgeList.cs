using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public class HalfEdgeList : HeElementList<HalfEdge>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        internal HalfEdgeList(HeMesh mesh)
            : base(mesh)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="capacity"></param>
        internal HalfEdgeList(HeMesh mesh, int capacity)
            : base(mesh, capacity)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributes"></param>
        internal void HalfSizeCheck<U>(IList<U> attributes)
        {
            if (attributes.Count != Count >> 1)
                throw new ArgumentException("The number of attributes provided does not match the number of edges in the mesh.");
        }


        /// <summary>
        /// Adds an edge and its twin to the list.
        /// </summary>
        /// <param name="edge"></param>
        internal void AddPair(HalfEdge edge)
        {
            Add(edge);
            Add(edge.Twin);
        }


        /// <summary>
        /// Creates a pair of halfedges between the given vertices and add them to the list.
        /// Note that the face references of the new halfedges are left unassigned.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        internal HalfEdge AddPair(HeVertex start, HeVertex end)
        {
            HalfEdge e0 = new HalfEdge();
            HalfEdge e1 = new HalfEdge();

            e0.Start = start;
            e1.Start = end;
            HalfEdge.MakeTwins(e0, e1);

            AddPair(e0);
            return e0;
        }


        #region Element Attributes

        /// <summary>
        /// Returns the length of each half-edge in the mesh.
        /// </summary>
        /// <returns></returns>
        public double[] GetHalfEdgeLengths()
        {
            double[] result = new double[Count];
            UpdateHalfEdgeLengths(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateHalfEdgeLengths(IList<double> result)
        {
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count >> 1), range =>
            {
                int i0 = range.Item1 << 1;
                int i1 = range.Item2 << 1;

                for (int i = i0; i < i1; i += 2)
                {
                    HalfEdge e = this[i];
                    if (e.IsUnused) continue;

                    double d = e.Span.Length;
                    result[i] = d;
                    result[i + 1] = d;
                }
            });
        }



        /// <summary>
        /// Returns the length of each edge in the mesh.
        /// </summary>
        /// <returns></returns>
        public double[] GetEdgeLengths()
        {
            double[] result = new double[Count >> 1];
            UpdateEdgeLengths(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateEdgeLengths(IList<double> result)
        {
            HalfSizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count >> 1), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HalfEdge e = this[i << 1];
                    if (e.IsUnused) continue;
                    result[i] = e.Span.Length;
                }
            });
        }


        /// <summary>
        /// Returns the angle between each half-edge and its previous.
        /// </summary>
        /// <returns></returns>
        public double[] GetHalfEdgeAngles()
        {
            double[] result = new double[Count];
            UpdateHalfEdgeAngles(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double[] GetHalfEdgeAngles(IList<double> edgeLengths)
        {
            double[] result = new double[Count];
            UpdateHalfEdgeAngles(edgeLengths, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void UpdateHalfEdgeAngles(IList<double> result)
        {
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HalfEdge e = this[i];
                    if (e.IsUnused) continue;
                    result[i] = Vec3d.Angle(e.Span, e.Previous.Twin.Span);
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void UpdateHalfEdgeAngles(IList<double> edgeLengths, IList<double> result)
        {
            SizeCheck(edgeLengths);
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HalfEdge e0 = this[i];
                    if (e0.IsUnused) continue;

                    HalfEdge e1 = e0.Previous.Twin;
                    result[i] = Math.Acos((e0.Span / edgeLengths[i]) * (e1.Span / edgeLengths[e1.Index]));
                }
            });
        }


        /// <summary>
        /// Returns a dihedral angle at each pair of half-edges in the mesh.
        /// </summary>
        /// <returns></returns>
        public double[] GetDihedralAngles(IList<Vec3d> faceNormals)
        {
            double[] result = new double[Count >> 1];
            UpdateDihedralAngles(faceNormals, result);
            return result;
        }


        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public void UpdateDihedralAngles(IList<Vec3d> faceNormals, IList<double> result)
        {
            SizeCheck(result);
            Mesh.Faces.SizeCheck(faceNormals);

            Parallel.ForEach(Partitioner.Create(0, Count >> 1), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HalfEdge e = this[i << 1];
                    if (e.IsUnused || e.IsBoundary) continue;

                    double angle = Vec3d.Angle(faceNormals[e.Face.Index], faceNormals[e.Twin.Face.Index]);
                    result[i] = angle;
                }
            });
        }


        /// <summary>
        /// Returns the area associated with each halfedge.
        /// This is calculated as W in the diagram below.
        /// http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf
        /// </summary>
        /// <param name="faceCenters"></param>
        /// <returns></returns>
        public double[] GetHalfEdgeAreas(IList<Vec3d> faceCenters)
        {
            double[] result = new double[Count];
            UpdateHalfEdgeAreas(faceCenters, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faceCenters"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public void UpdateHalfEdgeAreas(IList<Vec3d> faceCenters, IList<double> result)
        {
            SizeCheck(result);
            Mesh.Faces.SizeCheck(faceCenters);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HalfEdge e = this[i];
                    if (e.IsUnused || e.Face == null) continue;

                    Vec3d v0 = e.Span * 0.5;
                    Vec3d v1 = faceCenters[e.Face.Index] - e.Start.Position;
                    Vec3d v2 = e.Previous.Span * -0.5;

                    result[i] = (Vec3d.Cross(v0, v1).Length + Vec3d.Cross(v1, v2).Length) * 0.5;
                }
            });
        }


        /// <summary>
        /// Returns the cotangent of each half-edge.
        /// Intended for use on triangle meshes.
        /// http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf
        /// </summary>
        /// <returns></returns>
        public double[] GetHalfEdgeCotangents()
        {
            double[] result = new double[Count];
            UpdateHalfEdgeCotangents(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateHalfEdgeCotangents(IList<double> result)
        {
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HalfEdge e = this[i];
                    if (e.IsUnused) continue;

                    Vec3d v0 = e.Previous.Span;
                    Vec3d v1 = e.Next.Twin.Span;
                    result[i] = v0 * v1 / Vec3d.Cross(v0, v1).Length;
                }
            });
        }


        /// <summary>
        /// Returns a symmetric cotangent weight for each half-edge.
        /// Intended for use on triangle meshes.
        /// Note that this implementation doesn't consider vertex areas.
        /// http://reuter.mit.edu/papers/reuter-smi09.pdf
        /// http://libigl.github.io/libigl/tutorial/tutorial.html#normals
        /// </summary>
        /// <returns></returns>
        public double[] GetCotanWeights()
        {
            double[] result = new double[Count];
            UpdateCotanWeights(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateCotanWeights(IList<double> result)
        {
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count >> 1), range =>
            {
                int i0 = range.Item1 << 1;
                int i1 = range.Item2 << 1;

                for (int i = i0; i < i1; i += 2)
                {
                    HalfEdge e = this[i];
                    if (e.IsUnused) continue;
                    double w = 0.0;

                    if (e.Face != null)
                    {
                        Vec3d v0 = e.Previous.Span;
                        Vec3d v1 = e.Next.Twin.Span;
                        w += v0 * v1 / Vec3d.Cross(v0, v1).Length;
                    }

                    e = e.Twin;
                    if (e.Face != null)
                    {
                        Vec3d v0 = e.Previous.Span;
                        Vec3d v1 = e.Next.Twin.Span;
                        w += v0 * v1 / Vec3d.Cross(v0, v1).Length;
                    }

                    w *= 0.5;
                    result[i] = w;
                    result[i + 1] = w;
                }
            });
        }


        /// <summary>
        /// Returns the a symmetric area-dependent cotangent weight for each half-edge.
        /// Intended for use on triangle meshes.
        /// Symmetric derivation of the Laplace-Beltrami operator detailed in
        /// http://reuter.mit.edu/papers/reuter-smi09.pdf
        /// </summary>
        /// <returns></returns>
        public double[] GetCotanWeights2()
        {
            double[] result = new double[Count];
            UpdateCotanWeights2(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexAreas"></param>
        /// <returns></returns>
        public double[] GetCotanWeights2(out double[] vertexAreas)
        {
            double[] result = new double[Count];
            UpdateCotanWeights2(result, out vertexAreas);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateCotanWeights2(IList<double> result)
        {
            double[] vertexAreas;
            UpdateCotanWeights2(result, out vertexAreas);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="vertexAreas"></param>
        public void UpdateCotanWeights2(IList<double> result, out double[] vertexAreas)
        {
            SizeCheck(result);

            vertexAreas = new double[Mesh.Vertices.Count];
            HeFaceList faces = Mesh.Faces;
            double t = 1.0 / 6.0;

            // calculate cotangent weights and vertex areas
            for (int i = 0; i < Count; i += 2)
            {
                HalfEdge e = this[i];
                if (e.IsUnused) continue;
                double w = 0.0;

                if (e.Face != null)
                {
                    Vec3d v0 = e.Previous.Span;
                    Vec3d v1 = e.Next.Twin.Span;
                    double a = Vec3d.Cross(v0, v1).Length;
                    vertexAreas[e.Start.Index] += a * t; // 1/3rd the triangular area (or 1/6th the parallelgram area)
                    w += v0 * v1 / a;
                }

                e = e.Twin;
                if (e.Face != null)
                {
                    Vec3d v0 = e.Previous.Span;
                    Vec3d v1 = e.Next.Twin.Span;
                    double a = Vec3d.Cross(v0, v1).Length;
                    vertexAreas[e.Start.Index] += a * t; // 1/3rd the triangular area (or 1/6th the parallelgram area)
                    w += v0 * v1 / a;
                }

                result[i] = w * 0.5; // cache weight with first edge of each pair
            }

            // divide weights by vertex areas
            double[] areas = vertexAreas; // can't use out parameter within lambda statement
            Parallel.ForEach(Partitioner.Create(0, Count >> 1), range =>
            {
                int i0 = range.Item1 << 1;
                int i1 = range.Item2 << 1;

                for (int i = i0; i < i1; i += 2)
                {
                    HalfEdge e = this[i];
                    if (e.IsUnused) continue;

                    double w = result[i] / Math.Sqrt(areas[e.Start.Index] * areas[e.End.Index]); // symmetric area weighting
                    result[i] = w;
                    result[i + 1] = w;
                }
            });
        }


        /// <summary>
        /// Normalizes half-edge weights such that weights of outgoing edges around each vertex sum to 1.
        /// Note that this breaks weight symmetry between half-edge pairs.
        /// </summary>
        /// <param name="halfEdgeWeights"></param>
        public void NormalizeHalfEdgeWeights(IList<double> halfEdgeWeights)
        {
            SizeCheck(halfEdgeWeights);

            HeVertexList verts = Mesh.Vertices;
            Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = verts[i];
                    if (v.IsUnused) continue;

                    double sum = 0.0;

                    foreach (HalfEdge e in v.OutgoingHalfEdges)
                        sum += halfEdgeWeights[e.Index];

                    if (sum > 0.0)
                    {
                        sum = 1.0 / sum;
                        foreach (HalfEdge e in v.OutgoingHalfEdges)
                            halfEdgeWeights[e.Index] *= sum;
                    }
                }
            });
        }


        /// <summary>
        /// Returns the span vector for each half-edge in the mesh.
        /// </summary>
        /// <param name="unitize"></param>
        /// <returns></returns>
        public Vec3d[] GetHalfEdgeVectors(bool unitize)
        {
            Vec3d[] result = new Vec3d[Count];
            UpdateHalfEdgeVectors(unitize, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitize"></param>
        /// <param name="result"></param>
        public void UpdateHalfEdgeVectors(bool unitize, IList<Vec3d> result)
        {
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count >> 1), range =>
            {
                int i0 = range.Item1 << 1;
                int i1 = range.Item2 << 1;

                for (int i = i0; i < i1; i += 2)
                {
                    HalfEdge e = this[i];
                    if (e.IsUnused) continue;

                    Vec3d v = e.Span;
                    if (unitize) v.Unitize();
                    result[i] = v;
                    result[i + 1] = -v;
                }
            });
        }


        /// <summary>
        /// Returns a normal for each half-edge in the mesh.
        /// These are calculated as the cross product of each edge and its previous.
        /// Note that these are used in the calculation of both vertex and face normals.
        /// </summary>
        /// <param name="unitize"></param>
        /// <returns></returns>
        public Vec3d[] GetHalfEdgeNormals(bool unitize)
        {
            Vec3d[] result = new Vec3d[Count];
            UpdateHalfEdgeNormals(unitize, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitize"></param>
        /// <param name="result"></param>
        public void UpdateHalfEdgeNormals(bool unitize, IList<Vec3d> result)
        {
            SizeCheck(result);

            if (unitize)
                UpdateHalfEdgeUnitNormals(result);
            else
                UpdateHalfEdgeNormals(result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        private void UpdateHalfEdgeNormals(IList<Vec3d> result)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HalfEdge e = this[i];
                    if (e.IsUnused) continue;

                    result[i] = Vec3d.Cross(e.Previous.Span, e.Span);
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        private void UpdateHalfEdgeUnitNormals(IList<Vec3d> result)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HalfEdge e = this[i];
                    if (e.IsUnused) continue;

                    Vec3d v = Vec3d.Cross(e.Previous.Span, e.Span);
                    v.Unitize();
                    result[i] = v;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitize"></param>
        /// <returns></returns>
        public Vec3d[] GetHalfEdgeBisectors(bool unitize)
        {
            Vec3d[] result = new Vec3d[Count];
            UpdateHalfEdgeBisectors(unitize, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitize"></param>
        /// <param name="result"></param>
        public void UpdateHalfEdgeBisectors(bool unitize, IList<Vec3d> result)
        {
            SizeCheck(result);

            if (unitize)
                UpdateHalfEdgeUnitBisectors(result);
            else
                UpdateHalfEdgeBisectors(result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        private void UpdateHalfEdgeBisectors(IList<Vec3d> result)
        {

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HalfEdge e = this[i];
                    if (e.IsUnused) continue;

                    Vec3d v0 = e.Span;
                    Vec3d v1 = e.Previous.Span;

                    v0 = (v0 / v0.Length - v1 / v1.Length) * 0.5;
                    result[i] = v0;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        private void UpdateHalfEdgeUnitBisectors(IList<Vec3d> result)
        {

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HalfEdge e = this[i];
                    if (e.IsUnused) continue;

                    Vec3d v0 = e.Span;
                    Vec3d v1 = e.Previous.Span;

                    v0 = (v0 / v0.Length - v1 / v1.Length) * 0.5;
                    v0.Unitize();
                    result[i] = v0;
                }
            });
        }

        #endregion


        #region Euler Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        public void Remove(HalfEdge edge)
        {
            Mesh.Faces.MergeFaces(edge);
        }


        /// <summary>
        /// Removes a pair of edges from the mesh.
        /// Note that this method does not update any face-edge references.
        /// </summary>
        /// <param name="edge"></param>
        internal void RemovePair(HalfEdge edge)
        {
            HalfEdge e0 = edge;
            HalfEdge e1 = e0.Twin;

            if (e0.IsFromDegree1)
                e0.Start.MakeUnused(); // flag degree 1 vertex as unused
            else
            {
                HalfEdge.MakeConsecutive(e0.Previous, e1.Next); // update edge-edge refs
                if (e0.IsFirstFromStart) e1.Next.MakeFirstFromStart(); // update vertex-edge ref if necessary
            }

            if (e1.IsFromDegree1)
                e1.Start.MakeUnused(); // flag degree 1 vertex as unused
            else
            {
                HalfEdge.MakeConsecutive(e1.Previous, e0.Next); // update edge-edge refs
                if (e1.IsFirstFromStart) e0.Next.MakeFirstFromStart(); // update vertex-edge ref if necessary
            }

            // flag elements for removal
            e0.MakeUnused();
            e1.MakeUnused();
        }


        /// <summary>
        /// Splits the given edge by adding a new vertex in the middle.
        /// Returns the new edge outgoing from the new vertex.
        /// </summary>
        public HalfEdge SplitEdge(HalfEdge edge)
        {
            Validate(edge);

            HalfEdge e0 = edge;
            HalfEdge e1 = e0.Twin;

            HeVertex v0 = e0.Start;
            HeVertex v1 = e1.Start;
            //HeVertex v2 = Mesh.Vertices.Add(v0.Position);
            HeVertex v2 = Mesh.Vertices.Add((v0.Position + v1.Position) * 0.5);

            HalfEdge e2 = Mesh.HalfEdges.AddPair(v2, v1);
            HalfEdge e3 = e2.Twin;

            // update edge-vertex references
            e1.Start = v2;

            // update edge-face references
            e2.Face = e0.Face;
            e3.Face = e1.Face;

            // update vertex-edge references if necessary
            if (v1.First == e1)
            {
                v1.First = e3;
                v2.First = e1;
            }
            else
            {
                v2.First = e2;
            }

            // update edge-edge references
            HalfEdge.MakeConsecutive(e2, e0.Next);
            HalfEdge.MakeConsecutive(e1.Previous, e3);
            HalfEdge.MakeConsecutive(e0, e2);
            HalfEdge.MakeConsecutive(e3, e1);

            return e2;
        }


        /// <summary>
        /// Splits an edge by adding a new vertex in the middle. 
        /// Faces adjacent to the given edge are also split at the new vertex.
        /// Returns the new edge outgoing from the new vertex or null on failure.
        /// Assumes triangle mesh.
        /// </summary>
        public HalfEdge SplitEdgeFace(HalfEdge edge)
        {
            HalfEdge e0 = SplitEdge(edge);
            HalfEdge e1 = e0.Twin.Next;

            HeFaceList faces = Mesh.Faces;
            faces.SplitFace(e0, e0.Next.Next);
            faces.SplitFace(e1, e1.Next.Next);

            return e0;
        }


        /// <summary>
        /// Collapses the given half edge by merging the vertices at either end.
        /// The start vertex is retained and the end vertex is flagged as unused.
        /// Return true on success.
        /// </summary>
        /// <param name="edge"></param>
        public bool CollapseEdge(HalfEdge edge)
        {
            Validate(edge);

            HalfEdge e0 = edge;
            HalfEdge e1 = e0.Twin;

            HeVertex v0 = e0.Start; // to be removed
            HeVertex v1 = e1.Start;

            HeFace f0 = e0.Face;
            HeFace f1 = e1.Face;

            /*
            // avoids creation of non-manifold vertices
            if (!e0.IsBoundary && (v0.IsBoundary && v1.IsBoundary))
                return false;
            */

            // avoids creation of non-manifold edges
            int allow = 0; // the number of common neighbours allowed between v0 and v1
            if (f0 != null && f0.IsTri) allow++;
            if (f1 != null && f1.IsTri) allow++;
            if (Mesh.Vertices.CountCommonNeighbours(v0, v1) > allow)
                return false;

            // update edge-vertex refs of all edges emanating from the vertex which is being removed
            foreach (HalfEdge e in v0.OutgoingHalfEdges) e.Start = v1;

            // update vertex-edge ref for the remaining vertex if necessary
            if (e1.IsFirstFromStart) e1.Next.MakeFirstFromStart();

            // update edge-edge refs
            HalfEdge.MakeConsecutive(e0.Previous, e0.Next);
            HalfEdge.MakeConsecutive(e1.Previous, e1.Next);

            // update face-edge refs if necessary and deal with potential collapse by merging
            if (f0 != null)
            {
                if (e0.IsFirstInFace) e0.Next.MakeFirstInFace();
                if (!f0.IsValid) Mesh.Faces.MergeFaces(f0.First);
            }

            if (f1 != null)
            {
                if (e1.IsFirstInFace) e1.Next.MakeFirstInFace();
                if (!f1.IsValid) Mesh.Faces.MergeFaces(f1.First);
            }

            // flag elements for removal
            e0.MakeUnused();
            e1.MakeUnused();
            v0.MakeUnused();
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool SpinEdge(HalfEdge edge)
        {
            Validate(edge);

            // edge must be adjacent to 2 faces
            if (edge.IsBoundary) return false;

            HalfEdge e0 = edge;
            HalfEdge e1 = e0.Twin;
            HalfEdge e2 = e0.Next;
            HalfEdge e3 = e1.Next;

            // don't allow for the creation of valence 1 vertices
            if (e0.IsFromDegree2 || e1.IsFromDegree2) return false;

            // update vertex-edge refs if necessary
            if (e0.IsFirstFromStart) e0.Start.First = e3;
            if (e1.IsFirstFromStart) e1.Start.First = e2;

            // update edge-vertex refs
            e0.Start = e3.End;
            e1.Start = e2.End;

            HeFace f0 = e0.Face;
            HeFace f1 = e1.Face;

            // update face-edge refs if necessary
            if (e2.IsFirstInFace) f0.First = e2.Next;
            if (e3.IsFirstInFace) f1.First = e3.Next;

            // update edge-face refs
            e2.Face = f1;
            e3.Face = f0;

            // update edge-edge refs
            HalfEdge.MakeConsecutive(e0, e2.Next);
            HalfEdge.MakeConsecutive(e1, e3.Next);
            HalfEdge.MakeConsecutive(e1.Previous, e2);
            HalfEdge.MakeConsecutive(e0.Previous, e3);
            HalfEdge.MakeConsecutive(e2, e1);
            HalfEdge.MakeConsecutive(e3, e0);
            return true;
        }


        /*
        /// <summary>
        /// OBSOLETE degree 1 verts are now taken care of in HeEdgeList.RemovePair
        /// 
        /// removes an open chain of degree 1 vertices
        /// note that this method does not update face-edge references
        /// </summary>
        /// <param name="edge"></param>
        internal HeEdge Prune(HeEdge edge)
        {
            HeEdge e0 = edge;
            HeEdge e1 = e0.Twin;

            // advance until edge pair are no longer twins
            do
            {
                // stop if found an isolated segment
                if (e0.Next == e1)
                {
                    e0.Start.MakeUnused();
                    e1.Start.MakeUnused();
                    e0.MakeUnused();
                    e1.MakeUnused();
                    return null;
                }

                // flag elements for removal
                e0.Start.MakeUnused();
                e0.MakeUnused();
                e1.MakeUnused();

                // advance to next edge pair
                e0 = e0.Next;
                e1 = e1.Previous;
            } while (e0.Twin == e1);

            // update vertex-edge refs if necessary
            HeVertex v0 = e0.Start;
            if (v0.Outgoing.IsUnused) v0.Outgoing = e0;
    
            //update edge-edge refs
            HeEdge.MakeConsecutive(e1, e0);
            return e0;
        }
        */

        #endregion

    }
}
