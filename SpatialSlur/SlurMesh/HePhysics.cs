using Rhino.Geometry;
using SpatialSlur.SlurCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// Haphazard collection of static methods for mesh relaxation and optimization.
    /// Note that these methods only handle the calculation of forces. Integration of mesh vertices is left up to the specific implementation.
    /// </summary>
    public static class HePhysics
    {
        #region Laplacians

        /// <summary>
        /// Delegates for boundary dependant methods
        /// </summary>
        private static Action<HeMesh, double, IList<Vec3d>, int, int>[] _lapSmooth = 
        { 
            LaplacianSmoothFixed, 
            LaplacianSmoothCornerFixed, 
            LaplacianSmoothFree 
        };

    
        private static Action<HeMesh, double, IList<double>, IList<Vec3d>, int, int>[] _lapSmooth2 = 
        { 
            LaplacianSmoothFixed, 
            LaplacianSmoothCornerFixed, 
            LaplacianSmoothFree 
        };

   
        private static Action<HeMesh, double, IList<Vec3d>>[] _lapFair =
        {
            LaplacianFairFixed,
            LaplacianFairCornerFixed,
            LaplacianFairFree
        };


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        /// <param name="boundaryType"></param>
        /// <param name="parallel"></param>
        public static void LaplacianSmooth(HeMesh mesh, double strength, IList<Vec3d> forceSums, SmoothBoundaryType boundaryType = SmoothBoundaryType.Fixed, bool parallel = false)
        {
            var verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            if (parallel)
            {
                var lapSmooth = _lapSmooth[(int)boundaryType];
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                    lapSmooth(mesh, strength, forceSums, range.Item1, range.Item2));
            }
            else
            {
                _lapSmooth[(int)boundaryType](mesh, strength, forceSums, 0, verts.Count);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        private static void LaplacianSmoothFixed(HeMesh mesh, double strength, IList<Vec3d> forceSums, int i0, int i1)
        {
            var verts = mesh.Vertices;

            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused || v.IsBoundary) continue;

                // get average position of neighbouring vertices
                Vec3d sum = new Vec3d();
                int n = 0;

                foreach (HeVertex vn in v.ConnectedVertices)
                {
                    sum += vn.Position;
                    n++;
                }

                // add force vector
                forceSums[i] += (sum / n - v.Position) * strength;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        private static void LaplacianSmoothCornerFixed(HeMesh mesh, double strength, IList<Vec3d> forceSums, int i0, int i1)
        {
            var verts = mesh.Vertices;

            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                // if boundary vertex, only consider neighbours which are also on the boundary
                if (v.IsBoundary)
                {
                    if (v.IsDegree2) continue; // skip corners

                    Halfedge2 he = v.First;
                    HeVertex v0 = he.Twin.Start;
                    HeVertex v1 = he.Previous.Start;

                    // add force vector
                    forceSums[i] += ((v0.Position + v1.Position) * 0.5 - v.Position) * strength;
                }
                else
                {
                    Vec3d sum = new Vec3d();
                    int n = 0;

                    foreach (HeVertex vn in v.ConnectedVertices)
                    {
                        sum += vn.Position;
                        n++;
                    }

                    // add force vector
                    forceSums[i] += (sum / n - v.Position) * strength;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        private static void LaplacianSmoothFree(HeMesh mesh, double strength, IList<Vec3d> forceSums, int i0, int i1)
        {
            var verts = mesh.Vertices;

            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                // if boundary vertex, only consider neighbours which are also on the boundary
                if (v.IsBoundary)
                {
                    Halfedge2 he = v.First;
                    HeVertex v0 = he.Twin.Start;
                    HeVertex v1 = he.Previous.Start;

                    // add force vector
                    forceSums[i] += ((v0.Position + v1.Position) * 0.5 - v.Position) * strength;
                }
                else
                {
                    Vec3d sum = new Vec3d();
                    int n = 0;

                    foreach (HeVertex vn in v.ConnectedVertices)
                    {
                        sum += vn.Position;
                        n++;
                    }

                    // add force vector
                    forceSums[i] += (sum / n - v.Position) * strength;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="forceSums"></param>
        /// <param name="boundaryType"></param>
        /// <param name="parallel"></param>
        public static void LaplacianSmooth(HeMesh mesh, double strength, IList<double> halfedgeWeights, IList<Vec3d> forceSums, SmoothBoundaryType boundaryType = SmoothBoundaryType.Fixed, bool parallel = false)
        {
            mesh.Halfedges.SizeCheck(halfedgeWeights);

            var verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            if (parallel)
            {
                var lapSmooth = _lapSmooth2[(int)boundaryType];
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                    lapSmooth(mesh, strength, halfedgeWeights, forceSums, range.Item1, range.Item2));
            }
            else
                _lapSmooth2[(int)boundaryType](mesh, strength, halfedgeWeights, forceSums, 0, verts.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void LaplacianSmoothFixed(HeMesh mesh, double strength, IList<double> halfedgeWeights, IList<Vec3d> forceSums, int i0, int i1)
        {
            var verts = mesh.Vertices;

            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused || v.IsBoundary) continue;

                Vec3d sum = new Vec3d();

                foreach (Halfedge2 he in v.OutgoingHalfedges)
                    sum += he.Span * halfedgeWeights[he.Index];

                // add force vector
                forceSums[i] += sum * strength;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void LaplacianSmoothCornerFixed(HeMesh mesh, double strength, IList<double> halfedgeWeights, IList<Vec3d> forceSums, int i0, int i1)
        {
            var verts = mesh.Vertices;

            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                Vec3d sum = new Vec3d();

                // if boundary vertex, only consider neighbours which are also on the boundary
                if (v.IsBoundary)
                {
                    if (v.IsDegree2) continue; // skip corners

                    Halfedge2 he = v.First;
                    sum += he.Span * halfedgeWeights[he.Index];
                    he = he.Previous.Twin;
                    sum += he.Span * halfedgeWeights[he.Index];
                }
                else
                {
                    foreach (Halfedge2 he in v.OutgoingHalfedges)
                        sum += he.Span * halfedgeWeights[he.Index];
                }

                // add force vector
                forceSums[i] += sum * strength;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void LaplacianSmoothFree(HeMesh mesh, double strength, IList<double> halfedgeWeights, IList<Vec3d> forceSums, int i0, int i1)
        {
            var verts = mesh.Vertices;

            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                Vec3d sum = new Vec3d();

                // if boundary vertex, only consider neighbours which are also on the boundary
                if (v.IsBoundary)
                {
                    Halfedge2 he = v.First;
                    sum += he.Span * halfedgeWeights[he.Index];
                    he = he.Previous.Twin;
                    sum += he.Span * halfedgeWeights[he.Index];
                }
                else
                {
                    foreach (Halfedge2 he in v.OutgoingHalfedges)
                        sum += he.Span * halfedgeWeights[he.Index];
                }

                // add force vector
                forceSums[i] += sum * strength;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        /// <param name="boundaryType"></param>
        public static void LaplacianFair(HeMesh mesh, double strength, IList<Vec3d> forceSums, SmoothBoundaryType boundaryType = SmoothBoundaryType.Fixed)
        {
            _lapFair[(int)boundaryType](mesh, strength, forceSums);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        private static void LaplacianFairFixed(HeMesh mesh, double strength, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            for (int i = 0; i < verts.Count; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused || v.IsBoundary) continue;

                // get average position of neighbouring vertices
                Vec3d sum = new Vec3d();
                int n = 0;

                foreach (HeVertex vn in v.ConnectedVertices)
                {
                    sum += vn.Position;
                    n++;
                }

                // compute force vector
                Vec3d frc = (sum / n - v.Position) * strength;
                forceSums[i] += frc;

                // distribute opposite force amongst neighbours
                frc /= n;
                foreach (HeVertex vn in v.ConnectedVertices)
                    forceSums[vn.Index] -= frc;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        private static void LaplacianFairCornerFixed(HeMesh mesh, double strength, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            for (int i = 0; i < verts.Count; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                // if boundary vertex, only consider neighbours which are also on the boundary
                if (v.IsBoundary)
                {
                    if (v.IsDegree2) continue; // skip corners

                    Halfedge2 he = v.First;
                    HeVertex v0 = he.Twin.Start;
                    HeVertex v1 = he.Previous.Start;

                    // compute force vector
                    Vec3d frc = ((v0.Position + v1.Position) * 0.5 - v.Position) * strength;
                    forceSums[i] += frc;

                    // distribute opposite force amongst neighbours
                    frc *= 0.5;
                    forceSums[v0.Index] -= frc;
                    forceSums[v1.Index] -= frc;
                }
                else
                {
                    // get average position of neighbouring vertices
                    Vec3d sum = new Vec3d();
                    int n = 0;

                    foreach (HeVertex vn in v.ConnectedVertices)
                    {
                        sum += vn.Position;
                        n++;
                    }

                    // compute force vector
                    Vec3d frc = (sum / n - v.Position) * strength;
                    forceSums[i] += frc;

                    // distribute opposite force amongst neighbours
                    frc /= n;
                    foreach (HeVertex vn in v.ConnectedVertices)
                        forceSums[vn.Index] -= frc;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        private static void LaplacianFairFree(HeMesh mesh, double strength, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            for (int i = 0; i < verts.Count; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                // if boundary vertex, only consider neighbours which are also on the boundary
                if (v.IsBoundary)
                {
                    Halfedge2 he = v.First;
                    HeVertex v0 = he.Twin.Start;
                    HeVertex v1 = he.Previous.Start;

                    // compute force vector
                    Vec3d frc = ((v0.Position + v1.Position) * 0.5 - v.Position) * strength;
                    forceSums[i] += frc;

                    // reverse and distribute amongst neighbours
                    frc *= 0.5;
                    forceSums[v0.Index] -= frc;
                    forceSums[v1.Index] -= frc;
                }
                else
                {
                    // get average position of neighbouring vertices
                    Vec3d sum = new Vec3d();
                    int n = 0;

                    foreach (HeVertex vn in v.ConnectedVertices)
                    {
                        sum += vn.Position;
                        n++;
                    }

                    // compute force vector
                    Vec3d frc = (sum / n - v.Position) * strength;
                    forceSums[i] += frc;

                    // distribute opposite force amongst neighbours
                    frc /= n;
                    foreach (HeVertex vn in v.ConnectedVertices)
                        forceSums[vn.Index] -= frc;
                }
            }
        }

        #endregion


        #region Distance-based Constraints

        /// <summary>
        /// Assumes rest length of 0.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainEdgeLengths(HeMesh mesh, double strength, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);
            var hedges = mesh.Halfedges;

            for (int i = 0; i < hedges.Count; i += 2)
            {
                Halfedge2 he = hedges[i];
                if (he.IsUnused) continue;

                Vec3d frc = he.Span * strength;
                forceSums[he.Start.Index] += frc;
                forceSums[he.End.Index] -= frc;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="restLength"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainEdgeLengths(HeMesh mesh, double strength, IList<double> edgeLengths, double restLength, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            var hedges = mesh.Halfedges;
            hedges.HalfSizeCheck(edgeLengths);

            for (int i = 0; i < hedges.Count; i += 2)
            {
                Halfedge2 he = hedges[i];
                if (he.IsUnused) continue;
         
                Vec3d frc = he.Span * ((1.0 - restLength / edgeLengths[i >> 1]) * strength);
                forceSums[he.Start.Index] += frc;
                forceSums[he.End.Index] -= frc;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="restLengths"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainEdgeLengths(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> restLengths, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            var hedges = mesh.Halfedges;
            hedges.HalfSizeCheck(edgeLengths);
            hedges.HalfSizeCheck(restLengths);

            for (int i = 0; i < hedges.Count; i += 2)
            {
                Halfedge2 he = hedges[i];
                if (he.IsUnused) continue;

                Vec3d frc = he.Span * ((1.0 - restLengths[i >> 1] / edgeLengths[i >> 1]) * strength);
                forceSums[he.Start.Index] += frc;
                forceSums[he.End.Index] -= frc;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainEdgeLengths(HeMesh mesh, double strength, IList<double> edgeLengths, double minLength, double maxLength, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            var hedges = mesh.Halfedges;
            hedges.HalfSizeCheck(edgeLengths);

            if (minLength > maxLength)
                throw new ArgumentException("The given minimum length cannot be greater than the given maximum length.");

            for (int i = 0; i < hedges.Count; i += 2)
            {
                Halfedge2 he = hedges[i];
                if (he.IsUnused) continue;

                Vec3d frc = he.Span;
                double mag = edgeLengths[i >> 1];

                if (mag > maxLength)
                    frc *= (1.0 - maxLength / mag) * strength;
                else if (mag < minLength)
                    frc *= (1.0 - minLength / mag) * strength;
                else
                    continue;

                forceSums[he.Start.Index] += frc;
                forceSums[he.End.Index] -= frc;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="minLengths"></param>
        /// <param name="maxLengths"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainEdgeLengths(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> minLengths, IList<double> maxLengths, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            var hedges = mesh.Halfedges;
            hedges.HalfSizeCheck(edgeLengths);
            hedges.HalfSizeCheck(minLengths);
            hedges.HalfSizeCheck(maxLengths);

            for (int i = 0; i < hedges.Count; i += 2)
            {
                Halfedge2 he = hedges[i];
                if (he.IsUnused) continue;

                int j = i >> 1;
                double minLength = minLengths[j];
                double maxLength = maxLengths[j];

                if (minLength > maxLength)
                    throw new ArgumentException("The given minimum length cannot be greater than the given maximum length.");

                Vec3d frc = he.Span;
                double mag = edgeLengths[j];

                if (mag > maxLength)
                    frc *= (1.0 - maxLength / mag) * strength;
                else if (mag < minLength)
                    frc *= (1.0 - minLength / mag) * strength;
                else
                    continue;

                forceSums[he.Start.Index] += frc;
                forceSums[he.End.Index] -= frc;
            }
        }


        /// <summary>
        /// Calculates forces which adjust edge lengths to make adjacent faces have tangent incircles.
        /// Note that this method assumes triangular faces.
        /// http://www.geometrie.tuwien.ac.at/hoebinger/mhoebinger_files/circlepackings.pdf
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="forceSums"></param>
        public static void CirclePack(HeMesh mesh, double strength, IList<double> edgeLengths, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HalfedgeList hedges = mesh.Halfedges;
            hedges.HalfSizeCheck(edgeLengths);

            for (int i = 0; i < hedges.Count; i += 2)
            {
                Halfedge2 he = hedges[i];
                if (he.IsUnused || he.IsBoundary) continue; // skip unused, boundary, or edges

                // collect relevant edges
                Halfedge2 he0 = he.Next;
                Halfedge2 he1 = he.Previous;
                Halfedge2 he2 = he.Twin.Next;
                Halfedge2 he3 = he.Twin.Previous;
          
                // collect edge lengths
                double d0 = edgeLengths[he0.Index >> 1];
                double d1 = edgeLengths[he1.Index >> 1];
                double d2 = edgeLengths[he2.Index >> 1];
                double d3 = edgeLengths[he3.Index >> 1];

                // get sums of opposite edges
                double mag0 = d0 + d2;
                double mag1 = d1 + d3;

                // compute force magnitude as deviation from mean sum
                double mean = (mag0 + mag1) * 0.5;
                mag0 = (mag0 - mean) * strength;
                mag1 = (mag1 - mean) * strength;

                // get force vectors
                Vec3d f0 = he0.Span * (mag0 / d0);
                Vec3d f1 = he1.Span * (mag1 / d1);
                Vec3d f2 = he2.Span * (mag0 / d2);
                Vec3d f3 = he3.Span * (mag1 / d3);

                forceSums[he0.Start.Index] += f0 - f3;
                forceSums[he1.Start.Index] += f1 - f0;
                forceSums[he2.Start.Index] += f2 - f1;
                forceSums[he3.Start.Index] += f3 - f2;
            }
        }


        /// <summary>
        /// Calculates forces which adjust the length of each edge to match a pair of radii associated with its end vertices.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="vertexRadii"></param>
        /// <param name="forceSums"></param>
        public static void CirclePack(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> vertexRadii, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);
            verts.SizeCheck(vertexRadii);

            HalfedgeList edges = mesh.Halfedges;
            edges.HalfSizeCheck(edgeLengths);

            for (int i = 0; i < edges.Count; i += 2)
            {
                Halfedge2 he = edges[i];
                if (he.IsUnused) continue; // skip unused edges

                // compute force vectors
                HeVertex v0 = he.Start;
                HeVertex v1 = he.End;

                double mag = edgeLengths[i >> 1];
                double rest = vertexRadii[v0.Index] + vertexRadii[v1.Index];
                //if (mag > rest) continue; // for compression only

                Vec3d frc = v0.VectorTo(v1);
                frc *= (mag - rest) * strength / mag;
                forceSums[v0.Index] += frc;
                forceSums[v1.Index] -= frc;
            }
        }


        /// <summary>
        /// Calculates forces which adjust edge lengths towards the average around their vertex.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="forceSums"></param>
        public static void EqualizeVertexEdgeLengths(HeMesh mesh, double strength, IList<double> edgeLengths, IList<Vec3d> forceSums)
        {
            var verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            mesh.Halfedges.HalfSizeCheck(edgeLengths);

            for (int i = 0; i < verts.Count; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue; // skip unused vertices

                // get average edge length around vertex
                double mean = 0.0;
                int n = 0;

                foreach (Halfedge2 he in v.OutgoingHalfedges)
                {
                    mean += edgeLengths[he.Index >> 1];
                    n++;
                }
                mean /= n;

                // compute force vectors
                foreach (Halfedge2 he in v.OutgoingHalfedges)
                {
                    HeVertex v0 = he.Start;
                    HeVertex v1 = he.End;

                    Vec3d frc = v0.VectorTo(v1);
                    double mag = edgeLengths[he.Index >> 1];

                    frc *= (mag - mean) * strength / mag;
                    forceSums[v0.Index] += frc;
                    forceSums[v1.Index] -= frc;
                }
            }
        }


        /// <summary>
        /// Calculates forces which adjust edge lengths towards the average within their face.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="forceSums"></param>
        public static void EqualizeFaceEdgeLengths(HeMesh mesh, double strength, IList<double> edgeLengths, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);
            mesh.Halfedges.HalfSizeCheck(edgeLengths);

            foreach (HeFace f in mesh.Faces)
            {
                // skip unused faces
                if (f.IsUnused) continue;

                // get average edge length within the face
                double mean = 0.0;
                int n = 0;

                foreach (Halfedge2 he in f.Halfedges)
                {
                    mean += edgeLengths[he.Index >> 1];
                    n++;
                }
                mean /= n;

                // compute force vectors
                foreach (Halfedge2 he in f.Halfedges)
                {
                    HeVertex v0 = he.Start;
                    HeVertex v1 = he.End;

                    Vec3d frc = v0.VectorTo(v1);
                    double mag = edgeLengths[he.Index >> 1];

                    frc *= (mag - mean) * strength / mag;
                    forceSums[v0.Index] += frc;
                    forceSums[v1.Index] -= frc;
                }
            }
        }

        #endregion


        #region Angle-based Constraints


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="halfedgeAngles"></param>
        /// <param name="restAngle"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainHalfedgeAngles(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> halfedgeAngles, double restAngle, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            var hedges = mesh.Halfedges;
            hedges.HalfSizeCheck(edgeLengths);
            hedges.SizeCheck(halfedgeAngles);

            for (int i = 0; i < hedges.Count; i++)
            {
                Halfedge2 he0 = hedges[i];
                if (he0.IsUnused || he0.Face == null) continue; // skip unused or boundary edges
                Halfedge2 he1 = he0.Next;

                double err = (halfedgeAngles[he1.Index] - restAngle) * 0.25; // angle error in range [-PI/4, PI/4]
                double m = Math.Tan(err) * (edgeLengths[i >> 1] + edgeLengths[he1.Index >> 1]) * 0.5 * strength;

                Vec3d d0 = he0.Span;
                Vec3d d1 = he1.Span;
   
                Vec3d f0 = Vec3d.Perp(d1, d0);
                Vec3d f1 = Vec3d.Perp(d0, d1);

                f0 *= m / f0.Length;
                f1 *= m / f1.Length;

                forceSums[he0.Start.Index] -= f0;
                forceSums[he1.Start.Index] += f0 - f1;
                forceSums[he1.End.Index] += f1;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="halfedgeAngles"></param>
        /// <param name="restAngles"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainHalfedgeAngles(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> halfedgeAngles, IList<double> restAngles, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            var hedges = mesh.Halfedges;
            hedges.HalfSizeCheck(edgeLengths);
            hedges.SizeCheck(halfedgeAngles);
            hedges.SizeCheck(restAngles);

            for (int i = 0; i < hedges.Count; i++)
            {
                Halfedge2 he0 = hedges[i];
                if (he0.IsUnused || he0.Face == null) continue; // skip unused or boundary edges
                Halfedge2 he1 = he0.Next;

                int i1 = he1.Index;
                double err = (halfedgeAngles[i1] - restAngles[i1]) * 0.25; // angle error in range [-PI/4, PI/4]
                double m = Math.Tan(err) * (edgeLengths[i >> 1] + edgeLengths[he1.Index >> 1]) * 0.5 * strength;

                Vec3d d0 = he0.Span;
                Vec3d d1 = he1.Span;

                Vec3d f0 = Vec3d.Perp(d1, d0);
                Vec3d f1 = Vec3d.Perp(d0, d1);

                f0 *= m / f0.Length;
                f1 *= m / f1.Length;

                forceSums[he0.Start.Index] -= f0;
                forceSums[he1.Start.Index] += f0 - f1;
                forceSums[he1.End.Index] += f1;
            }
        }


        /// <summary>
        /// Assumes triangular faces.
        /// http://www.tsg.ne.jp/TT/cg/ElasticOrigami_Tachi_IASS2013.pdf
        /// http://www.miralab.ch/repository/papers/165.pdf
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="restAngles"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainDihedralAngles(HeMesh mesh, double strength, IList<double> restAngles, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            var hedges = mesh.Halfedges;
            hedges.HalfSizeCheck(restAngles);

            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he0 = hedges[i];
                if (he0.IsUnused || he0.IsBoundary) continue;
                var he1 = hedges[i + 1];

                var v0 = he0.Start;
                var v1 = he1.Start;
                var v2 = he0.Previous.Start;
                var v3 = he1.Previous.Start;

                Vec3d d01 = v0.VectorTo(v1);
                Vec3d d02 = v0.VectorTo(v2);
                Vec3d d03 = v0.VectorTo(v3);
                Vec3d d21 = v2.VectorTo(v1);
                Vec3d d31 = v3.VectorTo(v1);

                // get heights
                double h0 = Vec3d.Perp(d02, d01).Length;
                double h1 = Vec3d.Perp(d03, d01).Length;
                double h = 0.5 / (h0 + h1); // inv mean height

                // get projection directions (face normals)
                Vec3d n0 = Vec3d.Cross(d02, d01);
                Vec3d n1 = Vec3d.Cross(d01, d03);

                // cache length and unitize
                double m0 = 1.0 / n0.Length;
                double m1 = 1.0 / n1.Length;

                /*
                // angle error
                double angle = Math.Acos(SlurMath.Clamp(n0 * n1 * m0 * m1, -1.0, 1.0)); // clamp to remove noise

                // get projection magnitude
                double m = (angle - restAngles[i >> 1]) * h * strength;
                if (n0 * d03 < 0.0) m *= -1.0; // flip if convex
                */

                // angle error
                double angle = Math.Acos(SlurMath.Clamp(n0 * n1 * m0 * m1, -1.0, 1.0)); // clamp to remove noise
                angle = (n1 * d02 < 0.0) ? Math.PI - angle : angle + Math.PI; // flip if convex

                // projection magnitude
                double m = (angle - restAngles[i >> 1]) * h * strength;

                // get relevant cotangents
                double c0 = d02 * d01 * m0;
                double c1 = d21 * d01 * m0;
                double c2 = d03 * d01 * m1;
                double c3 = d31 * d01 * m1;

                // apply projections
                forceSums[v0.Index] += n0 * (m * c1) + n1 * (m * c3);
                forceSums[v1.Index] += n0 * (m * c0) + n1 * (m * c2);
                forceSums[v2.Index] -= n0 * (m * (c0 + c1));
                forceSums[v3.Index] -= n1 * (m * (c2 + c3));
            }
        }


        /// <summary>
        /// Adjusts edge angles towards the average around their vertex.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="halfedgeAngles"></param>
        /// <param name="forceSums"></param>
        public static void EqualizeVertexHalfedgeAngles(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> halfedgeAngles, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            HalfedgeList hedges = mesh.Halfedges;
            hedges.HalfSizeCheck(edgeLengths);
            hedges.SizeCheck(halfedgeAngles);

            for (int i = 0; i < verts.Count; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                // get average angle at vertex
                double mean = 0.0;
                int n = 0;
                foreach (Halfedge2 he in v.OutgoingHalfedges)
                {
                    if (he.Face == null) continue;
                    mean += halfedgeAngles[he.Index];
                    n++;
                }
                mean /= n;

                // calculate forces
                foreach (Halfedge2 he0 in v.OutgoingHalfedges)
                {
                    if (he0.Face == null) continue; // boundary edges
                    Halfedge2 he1 = he0.Next;

                    double err = (halfedgeAngles[he1.Index] - mean) * 0.25; // angle error in range [-PI/4, PI/4]
                    double m = Math.Tan(err) * (edgeLengths[he0.Index >> 1] + edgeLengths[he1.Index >> 1]) * 0.5 * strength;

                    Vec3d d0 = he0.Span;
                    Vec3d d1 = he1.Span;

                    Vec3d f0 = Vec3d.Perp(d1, d0);
                    Vec3d f1 = Vec3d.Perp(d0, d1);

                    f0 *= m / f0.Length;
                    f1 *= m / f1.Length;

                    forceSums[he0.Start.Index] -= f0;
                    forceSums[he1.Start.Index] += f0 - f1;
                    forceSums[he1.End.Index] += f1;
                }
            }
        }


        /// <summary>
        /// Adjusts edge angles towards the average within their face.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="halfedgeAngles"></param>
        /// <param name="forceSums"></param>
        public static void EqualizeFaceHalfedgeAngles(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> halfedgeAngles, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            HalfedgeList hedges = mesh.Halfedges;
            hedges.HalfSizeCheck(edgeLengths);
            hedges.SizeCheck(halfedgeAngles);

            HeFaceList faces = mesh.Faces;
            for (int i = 0; i < faces.Count; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;

                // get average angle at vertex
                double mean = 0.0;
                int n = 0;
                foreach (Halfedge2 he in f.Halfedges)
                {
                    mean += halfedgeAngles[he.Index];
                    n++;
                }
                mean /= n;

                // calculate forces
                foreach (Halfedge2 he0 in f.Halfedges)
                {
                    Halfedge2 he1 = he0.Next;

                    double err = (halfedgeAngles[he1.Index] - mean) * 0.25; // angle error in range [-PI/4, PI/4]
                    double m = Math.Tan(err) * (edgeLengths[he0.Index >> 1] + edgeLengths[he1.Index >> 1]) * 0.5 * strength;

                    Vec3d d0 = he0.Span;
                    Vec3d d1 = he1.Span;
      
                    Vec3d f0 = Vec3d.Perp(d1, d0);
                    Vec3d f1 = Vec3d.Perp(d0, d1);

                    f0 *= m / f0.Length;
                    f1 *= m / f1.Length;

                    forceSums[he0.Start.Index] -= f0;
                    forceSums[he1.Start.Index] += f0 - f1;
                    forceSums[he1.End.Index] += f1;
                }
            }
        }

        #endregion


        /// <summary>
        /// Calculates forces which align the edges of a mesh with a given set of vectors.
        /// http://www.eecs.berkeley.edu/~sequin/CS285/PAPERS/Pottmann_FrFrm_arch.pdf
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="vectors"></param>
        /// <param name="forceSums"></param>
        public static void AlignEdges(HeMesh mesh, double strength, IList<Vec3d> vectors, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HalfedgeList hedges = mesh.Halfedges;
            hedges.HalfSizeCheck(vectors);

            for (int i = 0; i < hedges.Count; i += 2)
            {
                Halfedge2 he = hedges[i];
                if (he.IsUnused) continue; // skip unused edges

                // project edge vector onto target vector and take difference
                Vec3d frc = he.Span;
                Vec3d v = vectors[i >> 1];
                frc = (frc - Vec3d.Project(frc, v)) * strength;
            
                forceSums[he.Start.Index] += frc;
                forceSums[he.End.Index] -= frc;
            }
        }


        /// <summary>
        /// Calculates forces which pull vertices within each face to a common plane.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void PlanarizeFaces(HeMesh mesh, double strength, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            foreach (HeFace f in mesh.Faces)
            {
                if (f.IsUnused) continue;

                Halfedge2 he0 = f.First;
                Halfedge2 he1 = he0.Next;
                Halfedge2 he2 = he1.Next;
                Halfedge2 he3 = he2.Next;
                if (he3 == he0) continue; // ensure the face has at least 4 edges

                if (he3.Next == he0)
                {
                    // simplified quad case
                    HeVertex v0 = he0.Start;
                    HeVertex v1 = he1.Start;
                    HeVertex v2 = he2.Start;
                    HeVertex v3 = he3.Start;

                    // calculate and apply force
                    Vec3d frc = GeometryUtil.LineLineShortestVector(v0.Position, v2.Position, v1.Position, v3.Position) * strength;
                    forceSums[v0.Index] += frc;
                    forceSums[v2.Index] += frc;
                    forceSums[v1.Index] -= frc;
                    forceSums[v3.Index] -= frc;
                }
                else
                {
                    // generalized ngon case
                    do
                    {
                        HeVertex v0 = he0.Start;
                        HeVertex v1 = he1.Start;
                        HeVertex v2 = he2.Start;
                        HeVertex v3 = he3.Start;

                        // calculate and apply force
                        Vec3d frc = GeometryUtil.LineLineShortestVector(v0.Position, v2.Position, v1.Position, v3.Position) * strength;
                        forceSums[v0.Index] += frc;
                        forceSums[v2.Index] += frc;
                        forceSums[v1.Index] -= frc;
                        forceSums[v3.Index] -= frc;

                        // advance to next set of 4 points
                        he0 = he0.Next;
                        he1 = he1.Next;
                        he2 = he2.Next;
                        he3 = he3.Next;
                    } while (he0 != f.First);
                }
            }
        }


        /// <summary>
        /// Calculates forces which pull vertices within each face to a common plane.
        /// Note that this method ignores non-quad faces.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void PlanarizeQuads(HeMesh mesh, double strength, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            foreach (HeFace f in mesh.Faces)
            {
                if (f.IsUnused) continue;

                Halfedge2 he0 = f.First;
                Halfedge2 he1 = he0.Next;
                Halfedge2 he2 = he1.Next;
                Halfedge2 he3 = he2.Next;

                // ensure face is quad
                if (he3.Next != he0) continue;

                HeVertex v0 = he0.Start;
                HeVertex v1 = he1.Start;
                HeVertex v2 = he2.Start;
                HeVertex v3 = he3.Start;

                // calculate and apply force
                Vec3d frc = GeometryUtil.LineLineShortestVector(v0.Position, v2.Position, v1.Position, v3.Position) * strength;
                forceSums[v0.Index] += frc;
                forceSums[v2.Index] += frc;
                forceSums[v1.Index] -= frc;
                forceSums[v3.Index] -= frc;
            }
        }


        /// <summary>
        /// Calculates forces which pull vertices around each vertex to a common plane.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void PlanarizeVertices(HeMesh mesh, double strength, IList<Vec3d> forceSums)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Calculates forces which pull vertices within each face to a common circle.
        /// Note that this method is intended for use on quad meshes and should be used in conjunction with PlanarizeQuads.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="halfedgeAngles"></param>
        /// <param name="forceSums"></param>
        public static void Circularize(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> halfedgeAngles, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            var hedges = mesh.Halfedges;
            hedges.HalfSizeCheck(edgeLengths);
            hedges.SizeCheck(halfedgeAngles);

            foreach (HeFace f in mesh.Faces)
            {
                if (f.IsUnused) continue; // skip unused faces

                // circular rule - opposite angles in the quad must sum to pi
                // http://www.geometrie.tugraz.at/wallner/quad06.pdf
                // http://vecg.cs.ucl.ac.uk/Projects/SmartGeometry/constrained_mesh/paper_docs/constrainedMesh_sigA_11.pdf
                Halfedge2 he0 = f.First;
                Halfedge2 he1 = he0.Next;
                Halfedge2 he2 = he1.Next;
                Halfedge2 he3 = he2.Next;
                if (he3.Next != he0) continue; // ensure face is quad

                for(int i = 0; i < 2; i++)
                {
                    // calculate angle error
                    double err = (Math.PI - halfedgeAngles[he1.Index] - halfedgeAngles[he3.Index]) * 0.25; // angle error in range [-PI/4, PI/4]
                    double tan = Math.Tan(err) * strength;

                    // apply between he0 & he1
                    Vec3d d0 = he0.Span;
                    Vec3d d1 = he1.Span;
        
                    Vec3d f0 = Vec3d.Perp(d1, d0);
                    Vec3d f1 = Vec3d.Perp(d0, d1);

                    double m = tan * (edgeLengths[he0.Index >> 1] * edgeLengths[he1.Index >> 1]) * 0.5;
                    f0 *= m / f0.Length;
                    f1 *= m / f1.Length;

                    forceSums[he0.Start.Index] -= f0;
                    forceSums[he1.Start.Index] += f0 - f1;
                    forceSums[he2.Start.Index] += f1;

                    // apply between he2 & he3
                    d0 = he2.Span;
                    d1 = he3.Span;

                    f0 = Vec3d.Perp(d1, d0);
                    f1 = Vec3d.Perp(d0, d1);

                    m = tan * (edgeLengths[he2.Index >> 1] * edgeLengths[he3.Index >> 1]) * 0.5;
                    f0 *= m / f0.Length;
                    f1 *= m / f1.Length;

                    forceSums[he2.Start.Index] -= f0;
                    forceSums[he3.Start.Index] += f0 - f1;
                    forceSums[he0.Start.Index] += f1;

                    // advance to next edges
                    he0 = he0.Next;
                    he1 = he1.Next;
                    he2 = he2.Next;
                    he3 = he3.Next;
                }
            }
        }


        /*
       /// <summary>
       /// Assumes quad mesh
       /// </summary>
       public static void CircularizeQuads(HeMesh mesh, double strength, IList<Vec3d> forceSums)
       {
           mesh.Vertices.SizeCheck(forceSums);

           // TODO
           // get curvature circle for each set of 3 verts

           // get average of circle centers C

           // make initial points equidistant from C

           foreach (HeFace f in mesh.Faces)
           {
               if (f.IsUnused) continue;

               Halfedge he0 = f.First;
               Halfedge he1 = he0.Next;
               Halfedge he2 = he1.Next;
               Halfedge he3 = he2.Next;

               // ensure quad
               if (he3.Next != he0) continue;
           }
       }
       */


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="halfedgeAngles"></param>
        /// <param name="forceSums"></param>
        public static void Circularize2(HeMesh mesh, double strength, IList<double> halfedgeAngles, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);
            mesh.Halfedges.SizeCheck(halfedgeAngles);

            foreach (HeFace f in mesh.Faces)
            {
                if (f.IsUnused) continue; // skip unused faces

                // circular condition - opposite angles in the quad must sum to pi
                // http://www.geometrie.tugraz.at/wallner/quad06.pdf
                // http://vecg.cs.ucl.ac.uk/Projects/SmartGeometry/constrained_mesh/paper_docs/constrainedMesh_sigA_11.pdf
                Halfedge he0 = f.First;
                Halfedge he1 = he0.Next;
                Halfedge he2 = he1.Next;
                Halfedge he3 = he2.Next;
                if (he3.Next != he0) continue; // ensure face is quad

                HeVertex v0 = he0.Start;
                HeVertex v1 = he1.Start;
                HeVertex v2 = he2.Start;
                HeVertex v3 = he3.Start;

                // apply force along the opposite diagonal proportional to the difference between angle sum and pi
                double mag = (halfedgeAngles[he1.Index] + halfedgeAngles[he3.Index] - Math.PI) * strength;
                Vec3d frc = v0.VectorTo(v2);
                frc *= mag / frc.Length;

                forceSums[v0.Index] += frc;
                forceSums[v2.Index] -= frc;

                // apply force along the opposite diagonal proportional to the difference between angle sum and pi
                mag = (halfedgeAngles[he0.Index] + halfedgeAngles[he2.Index] - Math.PI) * strength;
                frc = v1.VectorTo(v3);
                frc *= mag / frc.Length;

                forceSums[v1.Index] += frc;
                forceSums[v3.Index] -= frc;
            }
        }
        */


        /// <summary>
        /// Calculates forces which adjust faces around each internal degree 4 vertex to be tangent to a common cone.
        /// Note that this method is intended for use on quad meshes and should be used in conjunction with PlanarizeQuads.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="halfedgeAngles"></param>
        /// <param name="forceSums"></param>
        public static void Conicalize(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> halfedgeAngles, IList<Vec3d> forceSums)
        {
            var verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            var hedges = mesh.Halfedges;
            hedges.HalfSizeCheck(edgeLengths);
            hedges.SizeCheck(halfedgeAngles);

            for(int i = 0; i < verts.Count; i++)
            {
                var v = verts[i];
                if (v.IsUnused || v.IsBoundary) continue; // skip unused or boundary vertices

                // conical rule - 2 pairs of opposite angles must have an equal sum
                // http://www.geometrie.tugraz.at/wallner/quad06.pdf
                // http://vecg.cs.ucl.ac.uk/Projects/SmartGeometry/constrained_mesh/paper_docs/constrainedMesh_sigA_11.pdf
                Halfedge2 he0 = v.First;
                Halfedge2 he1 = he0.Twin.Next;
                Halfedge2 he2 = he1.Twin.Next;
                Halfedge2 he3 = he2.Twin.Next;
                if (he3.Twin.Next != he0) continue; // ensure vertex is degree 4

                // get angle error
                double err = ((halfedgeAngles[he0.Index] + halfedgeAngles[he2.Index]) - (halfedgeAngles[he1.Index] + halfedgeAngles[he3.Index])) * 0.25; // angle error in range [-PI/4, PI/4]
                double tan = Math.Tan(err) * strength;

                // apply angle constraint to each quadrant
                he0 = he1.Twin;
                he1 = he0.Next;
                double t = 0.5;

                for (int j = 0; j < 4; j++)
                {
                    Vec3d d0 = he0.Span;
                    Vec3d d1 = he1.Span;

                    Vec3d f0 = Vec3d.Perp(d1, d0);
                    Vec3d f1 = Vec3d.Perp(d0, d1);

                    double m = tan * (edgeLengths[he0.Index >> 1] * edgeLengths[he1.Index >> 1]) * t;
                    f0 *= m / f0.Length;
                    f1 *= m / f1.Length;

                    forceSums[he0.Start.Index] -= f0;
                    forceSums[i] += f0 - f1;
                    forceSums[he1.End.Index] += f1;

                    he0 = he1.Twin;
                    he1 = he0.Next;
                    t = -t;
                }
            }
        }


        /*
        /// <summary>
        /// Adjusts faces around each internal degree 4 vertex to be tangent to a common cone.
        /// Intended for use on quad meshes.
        /// Note that this method should be used in conjunction with planarize.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="halfedgeAngles"></param>
        /// <param name="forceSums"></param>
        public static void Conicalize(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> halfedgeAngles, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            HalfedgeList hedges = mesh.Halfedges;
            hedges.HalfSizeCheck(edgeLengths);
            hedges.SizeCheck(halfedgeAngles);

            foreach (HeVertex v in verts)
            {
                if (v.IsUnused || v.IsBoundary) continue; // skip unused or boundary vertices

                // concical - 2 pairs of opposite angles must have an equal sum
                // http://www.geometrie.tugraz.at/wallner/quad06.pdf
                // http://vecg.cs.ucl.ac.uk/Projects/SmartGeometry/constrained_mesh/paper_docs/constrainedMesh_sigA_11.pdf
                Halfedge he0 = v.First;
                Halfedge he1 = he0.Twin.Next;
                Halfedge he2 = he1.Twin.Next;
                Halfedge he3 = he2.Twin.Next;
                if (he3.Twin.Next != he0) continue; // ensure vertex is degree 4

                // sum the angles of opposite edges
                double mag0 = halfedgeAngles[he0.Index] + halfedgeAngles[he2.Index];
                double mag1 = halfedgeAngles[he1.Index] + halfedgeAngles[he3.Index];

                // compute force magnitude as deviation from mean
                double mean = (mag0 + mag1) * 0.5;
                mag0 = (mag0 - mean) * strength;
                mag1 = (mag1 - mean) * strength;

                // get edge bisectors
                Vec3d f0 = he0.Span / edgeLengths[he0.Index >> 1];
                Vec3d f1 = he1.Span / edgeLengths[he1.Index >> 1];
                Vec3d f2 = he2.Span / edgeLengths[he2.Index >> 1];
                Vec3d f3 = he3.Span / edgeLengths[he3.Index >> 1];
                Vec3d f4 = f3;
                f3 += f2;
                f2 += f1;
                f1 += f0;
                f0 += f4;

                // unitize and scale bisectors
                f0 *= mag0 / f0.Length;
                f1 *= mag1 / f1.Length;
                f2 *= mag0 / f2.Length;
                f3 *= mag1 / f3.Length;

                // apply force along bisectors
                forceSums[v.Index] -= (f0 + f1 + f2 + f3) * 2.0;
                forceSums[he0.End.Index] += f0 + f1;
                forceSums[he1.End.Index] += f1 + f2;
                forceSums[he2.End.Index] += f2 + f3;
                forceSums[he3.End.Index] += f3 + f0;
            }
        }
        */


        /// <summary>
        /// Calculates forces which adjust edge lengths to make quad faces tangential.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="forceSums"></param>
        public static void Tangentialize(HeMesh mesh, double strength, IList<double> edgeLengths, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);
            mesh.Halfedges.HalfSizeCheck(edgeLengths);

            HeFaceList faces = mesh.Faces;

            for (int i = 0; i < faces.Count; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;

                // tangentail rule - the length sum of the two pairs of opposite sides is equal.
                // http://en.wikipedia.org/wiki/Tangential_quadrilateral
                Halfedge2 he0 = f.First;
                Halfedge2 he1 = he0.Next;
                Halfedge2 he2 = he1.Next;
                Halfedge2 he3 = he2.Next;
                if (he3.Next != he0) continue; // ensure face is quad

                // collect edge lengths
                double d0 = edgeLengths[he0.Index >> 1];
                double d1 = edgeLengths[he1.Index >> 1];
                double d2 = edgeLengths[he2.Index >> 1];
                double d3 = edgeLengths[he3.Index >> 1];

                // get sums of opposite edges
                double mag0 = d0 + d2;
                double mag1 = d1 + d3;

                // force magnitude as deviation from mean
                double mean = (mag0 + mag1) * 0.5;
                mag0 = (mag0 - mean) * strength;
                mag1 = (mag1 - mean) * strength;

                // get force vectors
                Vec3d f0 = he0.Span * (mag0 / d0);
                Vec3d f1 = he1.Span * (mag1 / d1);
                Vec3d f2 = he2.Span * (mag0 / d2);
                Vec3d f3 = he3.Span * (mag1 / d3);

                forceSums[he0.Start.Index] += f0 - f3;
                forceSums[he1.Start.Index] += f1 - f0;
                forceSums[he2.Start.Index] += f2 - f1;
                forceSums[he3.Start.Index] += f3 - f2;
            }
        }


        /// <summary>
        /// Calculates forces which minimize gaussian curvature at interior vertices.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="halfedgeAngles"></param>
        /// <param name="forceSums"></param>
        public static void Developablize(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> halfedgeAngles, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            HalfedgeList hedges = mesh.Halfedges;
            hedges.HalfSizeCheck(edgeLengths);
            hedges.SizeCheck(halfedgeAngles);

            foreach (HeVertex v in verts)
            {
                if (v.IsUnused || v.IsBoundary) continue; // skip unused or boudnary vertices

                // calculate angle error
                double sum = 0.0;
                foreach (Halfedge2 he in v.OutgoingHalfedges) 
                    sum += Math.PI - halfedgeAngles[he.Index];

                double err = (sum - SlurMath.Tau) * 0.125; // angle error in range [-PI/4, PI/4]
                double tan = Math.Tan(err) * strength;
                
                // apply angle constraint around vertex
                foreach (Halfedge2 he0 in v.OutgoingHalfedges)
                {
                    Halfedge2 he1 = he0.Next;

                    Vec3d d0 = he0.Span;
                    Vec3d d1 = he1.Span;

                    Vec3d f0 = Vec3d.Perp(d1, d0);
                    Vec3d f1 = Vec3d.Perp(d0, d1);

                    double m = tan * (edgeLengths[he0.Index >> 1] * edgeLengths[he1.Index >> 1]) * 0.5;
                    f0 *= m / f0.Length;
                    f1 *= m / f1.Length;

                    forceSums[he0.Start.Index] -= f0;
                    forceSums[he1.Start.Index] += f0 - f1;
                    forceSums[he1.End.Index] += f1;
                }
            }
        }


        /// <summary>
        /// Calculates forces which pull vertices towards a target mesh.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="target"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        /// <param name="parallel"></param>
        public static void ConstrainTo(HeMesh mesh, Mesh target, double strength, IList<Vec3d> forceSums, bool parallel = false)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                ConstrainTo(mesh, target, strength, forceSums, range.Item1, range.Item2));
            else
                ConstrainTo(mesh, target, strength, forceSums, 0, verts.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="target"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        private static void ConstrainTo(HeMesh mesh, Mesh target, double strength, IList<Vec3d> forceSums, int i0, int i1)
        {
            var verts = mesh.Vertices;

            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                Point3d p = v.Position.ToPoint3d();
                forceSums[i] += (target.ClosestPoint(p) - p).ToVec3d() * strength;
            }
        }
    }
}
