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
    /// Static methods for mesh relaxation.
    /// Only handles the calculation of forces - integration is left up to the implementation.
    /// </summary>
    public static class HePhysics
    {
        /// <summary>
        /// Delegates for boundary dependant methods
        /// </summary>
        private static Action<HeMesh, double, IList<Vec3d>>[] _lapSmooth = 
        { 
            LaplacianSmoothFixed, 
            LaplacianSmoothCornerFixed, 
            LaplacianSmoothFree 
        };

        private static Action<HeMesh, double, IList<double>, IList<Vec3d>>[] _lapSmooth2 = 
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
        /// Calculates forces which pull vertices towards a target mesh.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="target"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainTo(HeMesh mesh, Mesh target, double strength, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = verts[i];
                    if (v.IsUnused) continue;

                    Point3d p = v.Position.ToPoint3d();
                    forceSums[i] += (target.ClosestPoint(p) - p).ToVec3d() * strength;
                }
            });
        }


        /// <summary>
        /// Calculates forces which pull vertices towards a target brep.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="target"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainTo(HeMesh mesh, Brep target, double strength, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = verts[i];
                    if (v.IsUnused) continue;

                    Point3d p = v.Position.ToPoint3d();
                    forceSums[i] += (target.ClosestPoint(p) - p).ToVec3d() * strength;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainEdgeLengths(HeMesh mesh, double strength, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HalfEdgeList edges = mesh.HalfEdges;

            // skip every other edge to avoid applying forces twice
            for (int i = 0; i < edges.Count; i += 2)
            {
                // skip unused edges
                HalfEdge e = edges[i];
                if (e.IsUnused) continue;

                Vec3d frc = e.Span * strength;
                forceSums[e.Start.Index] += frc;
                forceSums[e.End.Index] -= frc;
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

            HalfEdgeList edges = mesh.HalfEdges;
            edges.HalfSizeCheck(edgeLengths);

            // skip every other edge to avoid applying forces twice
            for (int i = 0; i < edges.Count; i += 2)
            {
                // skip unused edges
                HalfEdge e = edges[i];
                if (e.IsUnused) continue;

                Vec3d frc = e.Span;
                double mag = edgeLengths[i >> 1];
                frc *= (mag - restLength) * strength / mag;

                forceSums[e.Start.Index] += frc;
                forceSums[e.End.Index] -= frc;
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

            HalfEdgeList edges = mesh.HalfEdges;
            edges.HalfSizeCheck(edgeLengths);
            edges.HalfSizeCheck(restLengths);

            // skip every other edge to avoid applying forces twice
            for (int i = 0; i < edges.Count; i += 2)
            {
                // skip unused edges
                HalfEdge e = edges[i];
                if (e.IsUnused) continue;

                Vec3d frc = e.Span;
                double mag = edgeLengths[i >> 1];
                frc *= (mag - restLengths[i >> 1]) * strength / mag;

                forceSums[e.Start.Index] += frc;
                forceSums[e.End.Index] -= frc;
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

            HalfEdgeList edges = mesh.HalfEdges;
            edges.HalfSizeCheck(edgeLengths);

            if (minLength > maxLength)
                throw new ArgumentException("The given minimum length cannot be greater than the given maximum length.");

            // skip every other edge to avoid applying forces twice
            for (int i = 0; i < edges.Count; i += 2)
            {
                // skip unused edges
                HalfEdge e = edges[i];
                if (e.IsUnused) continue;

                Vec3d frc = e.Span;
                double mag = edgeLengths[i >> 1];

                if (mag > maxLength)
                    frc *= (mag - maxLength) * strength / mag;
                else if (mag < minLength)
                    frc *= (mag - minLength) * strength / mag;
                else
                    continue;

                forceSums[e.Start.Index] += frc;
                forceSums[e.End.Index] -= frc;
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

            HalfEdgeList edges = mesh.HalfEdges;
            edges.HalfSizeCheck(edgeLengths);
            edges.HalfSizeCheck(minLengths);
            edges.HalfSizeCheck(maxLengths);

            // skip every other edge to avoid applying forces twice
            for (int i = 0; i < edges.Count; i += 2)
            {
                // skip unused edges
                HalfEdge e = edges[i];
                if (e.IsUnused) continue;

                int j = i >> 1;
                double min = minLengths[j];
                double max = maxLengths[j];

                if (min > max)
                    throw new ArgumentException("The given minimum length cannot be greater than the given maximum length.");

                Vec3d frc = e.Span;
                double mag = edgeLengths[j];
       
                if (mag > max)
                    frc *= (mag - max) * strength / mag;
                else if (mag < min)
                    frc *= (mag - min) * strength / mag;
                else
                    continue;

                forceSums[e.Start.Index] += frc;
                forceSums[e.End.Index] -= frc;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="halfEdgeAngles"></param>
        /// <param name="restAngle"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainHalfEdgeAngles(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> halfEdgeAngles, double restAngle, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HalfEdgeList edges = mesh.HalfEdges;
            edges.HalfSizeCheck(edgeLengths);
            edges.SizeCheck(halfEdgeAngles);

            for (int i = 0; i < edges.Count; i++)
            {
                HalfEdge e = edges[i];
                if (e.IsUnused || e.Face == null) continue; // skip unused or boundary edges

                // get unitized edge bisector
                HalfEdge prev = e.Previous;
                Vec3d frc = (e.Span / edgeLengths[i >> 1] - prev.Span / edgeLengths[prev.Index >> 1]) * 0.5;
                frc *= (halfEdgeAngles[i] - restAngle) * strength / frc.Length; // unitize and scale

                forceSums[prev.Start.Index] += frc;
                forceSums[e.Start.Index] -= frc * 2.0;
                forceSums[e.End.Index] += frc;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="halfEdgeAngles"></param>
        /// <param name="restAngles"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainHalfEdgeAngles(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> halfEdgeAngles, IList<double> restAngles, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HalfEdgeList edges = mesh.HalfEdges;
            edges.HalfSizeCheck(edgeLengths);
            edges.SizeCheck(halfEdgeAngles);
            edges.SizeCheck(restAngles);

            for (int i = 0; i < edges.Count; i++)
            {
                HalfEdge e = edges[i];
                if (e.IsUnused || e.Face == null) continue; // skip unused or boundary edges

                // get unitized edge bisector
                HalfEdge prev = e.Previous;
                Vec3d frc = (e.Span / edgeLengths[i >> 1] - prev.Span / edgeLengths[prev.Index >> 1]) * 0.5;
                frc *= (halfEdgeAngles[i] - restAngles[i]) * strength / frc.Length; // unitize and scale

                forceSums[prev.Start.Index] += frc;
                forceSums[e.Start.Index] -= frc * 2.0;
                forceSums[e.End.Index] += frc;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="halfEdgeAngles"></param>
        /// <param name="minAngle"></param>
        /// <param name="maxAngle"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainHalfEdgeAngles(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> halfEdgeAngles, double minAngle, double maxAngle, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HalfEdgeList edges = mesh.HalfEdges;
            edges.HalfSizeCheck(edgeLengths);
            edges.SizeCheck(halfEdgeAngles);

            if (minAngle > maxAngle)
                throw new ArgumentException("the given maximum angle must be larger than the given minimum angle");

            for (int i = 0; i < edges.Count; i++)
            {
                HalfEdge e = edges[i];
                if (e.IsUnused || e.Face == null) continue; // skip unused or boundary edges

                // get unitized edge bisector
                HalfEdge prev = e.Previous;
                Vec3d frc = (e.Span / edgeLengths[i >> 1] - prev.Span / edgeLengths[prev.Index >> 1]) * 0.5;
                double mag = halfEdgeAngles[i];

                if (mag > maxAngle)
                    frc *= (halfEdgeAngles[i] - maxAngle) * strength / frc.Length; // unitize and scale
                else if (mag < minAngle)
                    frc *= (halfEdgeAngles[i] - minAngle) * strength / frc.Length; // unitize and scale
                else
                    continue;

                forceSums[prev.Start.Index] += frc;
                forceSums[e.Start.Index] -= frc * 2.0;
                forceSums[e.End.Index] += frc;
            }
        }


        /// <summary>
        /// http://www.miralab.ch/repository/papers/165.pdf
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="faceNormals"></param>
        /// <param name="dihedralAngles"></param>
        /// <param name="restAngle"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainDihedralAngles(HeMesh mesh, double strength, IList<Vec3d> faceNormals, IList<double> dihedralAngles, double restAngle, IList<Vec3d> forceSums)
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// http://www.miralab.ch/repository/papers/165.pdf
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="faceNormals"></param>
        /// <param name="dihedralAngles"></param>
        /// <param name="restAngles"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainDihedralAngles(HeMesh mesh, double strength, IList<Vec3d> faceNormals, IList<double> dihedralAngles, IList<double> restAngles, IList<Vec3d> forceSums)
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// Adjusts edge lengths to make adjacent faces have tangent incircles.
        /// Intended for use on triangle meshes.
        /// http://www.geometrie.tuwien.ac.at/hoebinger/mhoebinger_files/circlepackings.pdf
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="forceSums"></param>
        public static void CirclePack(HeMesh mesh, double strength, IList<double> edgeLengths, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HalfEdgeList edges = mesh.HalfEdges;
            edges.HalfSizeCheck(edgeLengths);

            // skip every other edge to avoid applying forces twice
            for (int i = 0; i < edges.Count; i += 2)
            {
                HalfEdge e = edges[i];
                if (e.IsUnused || e.IsBoundary) continue; // skip unused, boundary, or edges

                // collect relevant edges
                HalfEdge e0 = e.Next;
                HalfEdge e1 = e.Previous;
                HalfEdge e2 = e.Twin.Next;
                HalfEdge e3 = e.Twin.Previous;
                if (!(e0.Next == e1 && e2.Next == e3)) continue; // ensure both faces are tris

                // collect edge lengths
                double d0 = edgeLengths[e0.Index >> 1];
                double d1 = edgeLengths[e1.Index >> 1];
                double d2 = edgeLengths[e2.Index >> 1];
                double d3 = edgeLengths[e3.Index >> 1];

                // get sums of opposite edges
                double mag0 = d0 + d2;
                double mag1 = d1 + d3;

                // compute force magnitude as deviation from mean sum
                double mean = (mag0 + mag1) * 0.5;
                mag0 = (mag0 - mean) * strength;
                mag1 = (mag1 - mean) * strength;

                // get force vectors
                Vec3d f0 = e0.Span * (mag0 / d0);
                Vec3d f1 = e1.Span * (mag1 / d1);
                Vec3d f2 = e2.Span * (mag0 / d2);
                Vec3d f3 = e3.Span * (mag1 / d3);

                forceSums[e0.Start.Index] += f0 - f3;
                forceSums[e1.Start.Index] += f1 - f0;
                forceSums[e2.Start.Index] += f2 - f1;
                forceSums[e3.Start.Index] += f3 - f2;
            }
        }


        /// <summary>
        /// Adjusts edge lengths to match a pair of radii associated with its end vertices.
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

            HalfEdgeList edges = mesh.HalfEdges;
            edges.HalfSizeCheck(edgeLengths);

            // skip every other edge to avoid applying forces twice
            for (int i = 0; i < edges.Count; i += 2)
            {
                HalfEdge e = edges[i];
                if (e.IsUnused) continue; // skip unused edges

                // compute force vectors
                HeVertex v0 = e.Start;
                HeVertex v1 = e.End;

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
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        /// <param name="boundaryType"></param>
        public static void LaplacianSmooth(HeMesh mesh, double strength, IList<Vec3d> forceSums, SmoothBoundaryType boundaryType = SmoothBoundaryType.Fixed)
        {
            _lapSmooth[(int)boundaryType](mesh, strength, forceSums);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        private static void LaplacianSmoothFixed(HeMesh mesh, double strength, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
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
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        private static void LaplacianSmoothCornerFixed(HeMesh mesh, double strength, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = verts[i];
                    if (v.IsUnused) continue;

                    // if boundary vertex, only consider neighbours which are also on the boundary
                    if (v.IsBoundary)
                    {
                        if (v.IsDegree2) continue; // skip corners

                        HalfEdge e = v.First;
                        HeVertex v0 = e.Twin.Start;
                        HeVertex v1 = e.Previous.Start;

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
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        private static void LaplacianSmoothFree(HeMesh mesh, double strength, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = verts[i];
                    if (v.IsUnused) continue;

                    // if boundary vertex, only consider neighbours which are also on the boundary
                    if (v.IsBoundary)
                    {
                        HalfEdge e = v.First;
                        HeVertex v0 = e.Twin.Start;
                        HeVertex v1 = e.Previous.Start;

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
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="halfEdgeWeights"></param>
        /// <param name="forceSums"></param>
        /// <param name="boundaryType"></param>
        public static void LaplacianSmooth(HeMesh mesh, double strength, IList<double> halfEdgeWeights, IList<Vec3d> forceSums, SmoothBoundaryType boundaryType = SmoothBoundaryType.Fixed)
        {
            _lapSmooth2[(int)boundaryType](mesh, strength, halfEdgeWeights, forceSums);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="halfEdgeWeights"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        private static void LaplacianSmoothFixed(HeMesh mesh, double strength, IList<double> halfEdgeWeights, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);
            mesh.HalfEdges.SizeCheck(halfEdgeWeights);

            Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = verts[i];
                    if (v.IsUnused || v.IsBoundary) continue;

                    Vec3d sum = new Vec3d();

                    foreach (HalfEdge e in v.OutgoingHalfEdges)
                        sum += e.Span * halfEdgeWeights[e.Index];

                    // add force vector
                    forceSums[i] += sum * strength;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="halfEdgeWeights"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        private static void LaplacianSmoothCornerFixed(HeMesh mesh, double strength, IList<double> halfEdgeWeights, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);
            mesh.HalfEdges.SizeCheck(halfEdgeWeights);

            Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = verts[i];
                    if (v.IsUnused) continue;

                    Vec3d sum = new Vec3d();

                    // if boundary vertex, only consider neighbours which are also on the boundary
                    if (v.IsBoundary)
                    {
                        if (v.IsDegree2) continue; // skip corners

                        HalfEdge e = v.First;
                        sum += e.Span * halfEdgeWeights[e.Index];
                        e = e.Previous.Twin;
                        sum += e.Span * halfEdgeWeights[e.Index];
                    }
                    else
                    {
                        foreach (HalfEdge e in v.OutgoingHalfEdges)
                            sum += e.Span * halfEdgeWeights[e.Index];
                    }

                    // add force vector
                    forceSums[i] += sum * strength;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="halfEdgeWeights"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        private static void LaplacianSmoothFree(HeMesh mesh, double strength, IList<double> halfEdgeWeights, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);
            mesh.HalfEdges.SizeCheck(halfEdgeWeights);

            Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = verts[i];
                    if (v.IsUnused) continue;

                    Vec3d sum = new Vec3d();

                    // if boundary vertex, only consider neighbours which are also on the boundary
                    if (v.IsBoundary)
                    {
                        HalfEdge e = v.First;
                        sum += e.Span * halfEdgeWeights[e.Index];
                        e = e.Previous.Twin;
                        sum += e.Span * halfEdgeWeights[e.Index];
                    }
                    else
                    {
                        foreach (HalfEdge e in v.OutgoingHalfEdges)
                            sum += e.Span * halfEdgeWeights[e.Index];
                    }

                    // add force vector
                    forceSums[i] += sum * strength;
                }
            });
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

                    HalfEdge e = v.First;
                    HeVertex v0 = e.Twin.Start;
                    HeVertex v1 = e.Previous.Start;

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
                    HalfEdge e = v.First;
                    HeVertex v0 = e.Twin.Start;
                    HeVertex v1 = e.Previous.Start;

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


        /// <summary>
        /// Adjusts edge lengths towards the average around their vertex.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="forceSums"></param>
        public static void EqualizeVertexEdgeLengths(HeMesh mesh, double strength, IList<double> edgeLengths, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            mesh.HalfEdges.HalfSizeCheck(edgeLengths);

            foreach (HeVertex v in verts)
            {
                if (v.IsUnused) continue; // skip unused vertices

                // get average edge length around vertex
                double mean = 0.0;
                int n = 0;

                foreach (HalfEdge e in v.OutgoingHalfEdges)
                {
                    mean += edgeLengths[e.Index >> 1];
                    n++;
                }
                mean /= n;

                // compute force vectors
                foreach (HalfEdge e in v.OutgoingHalfEdges)
                {
                    HeVertex v0 = e.Start;
                    HeVertex v1 = e.End;

                    Vec3d frc = v0.VectorTo(v1);
                    double mag = edgeLengths[e.Index >> 1];

                    frc *= (mag - mean) * strength / mag;
                    forceSums[v0.Index] += frc;
                    forceSums[v1.Index] -= frc;
                }
            }
        }


        /// <summary>
        /// Adjust edge lengths towards the average within their face.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="forceSums"></param>
        public static void EqualizeFaceEdgeLengths(HeMesh mesh, double strength, IList<double> edgeLengths, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);
            mesh.HalfEdges.HalfSizeCheck(edgeLengths);

            foreach (HeFace f in mesh.Faces)
            {
                // skip unused faces
                if (f.IsUnused) continue;

                // get average edge length within the face
                double mean = 0.0;
                int n = 0;

                foreach (HalfEdge e in f.HalfEdges)
                {
                    mean += edgeLengths[e.Index >> 1];
                    n++;
                }
                mean /= n;

                // compute force vectors
                foreach (HalfEdge e in f.HalfEdges)
                {
                    HeVertex v0 = e.Start;
                    HeVertex v1 = e.End;

                    Vec3d frc = v0.VectorTo(v1);
                    double mag = edgeLengths[e.Index >> 1];

                    frc *= (mag - mean) * strength / mag;
                    forceSums[v0.Index] += frc;
                    forceSums[v1.Index] -= frc;
                }
            }
        }


        /// <summary>
        /// Adjusts edge angles towards the average around their vertex.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="edgeAngles"></param>
        /// <param name="forceSums"></param>
        public static void EqualizeVertexEdgeAngles(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> edgeAngles, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            HalfEdgeList edges = mesh.HalfEdges;
            edges.HalfSizeCheck(edgeLengths);
            edges.SizeCheck(edgeAngles);

            foreach (HeVertex v in verts)
            {
                if (v.IsUnused || v.IsBoundary) continue; // skip unused or boundary vertices

                // get average angle
                double mean = 0.0;
                int n = 0;
                foreach (HalfEdge e in v.OutgoingHalfEdges)
                {
                    mean += edgeAngles[e.Index];
                    n++;
                }
                mean /= n;

                // calculate forces
                foreach (HalfEdge e in v.OutgoingHalfEdges)
                {
                    // get angle gradient vector (unitized edge bisector)
                    HalfEdge prev = e.Previous;
                    Vec3d frc = (e.Span / edgeLengths[e.Index >> 1] - prev.Span / edgeLengths[prev.Index >> 1]) * 0.5;
                    frc *= (edgeAngles[e.Index] - mean) * strength / frc.Length; // unitize and scale

                    forceSums[prev.Start.Index] += frc;
                    forceSums[e.Start.Index] -= frc * 2.0;
                    forceSums[e.End.Index] += frc;
                }
            }
        }


        /// <summary>
        /// Adjusts edge angles towards the average within their face.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="edgeAngles"></param>
        /// <param name="forceSums"></param>
        public static void EqualizeFaceEdgeAngles(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> edgeAngles, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HalfEdgeList edges = mesh.HalfEdges;
            edges.HalfSizeCheck(edgeLengths);
            edges.SizeCheck(edgeAngles);

            foreach (HeFace f in mesh.Faces)
            {
                if (f.IsUnused) continue; // skip unused faces

                // get average angle
                double mean = 0.0;
                int n = 0;
                foreach (HalfEdge e in f.HalfEdges)
                {
                    mean += edgeAngles[e.Index];
                    n++;
                }
                mean /= n;

                // calculate forces based on angular deviation from mean
                foreach (HalfEdge e in f.HalfEdges)
                {
                    // get edge bisector
                    HalfEdge prev = e.Previous;
                    Vec3d frc = (e.Span / edgeLengths[e.Index >> 1] - prev.Span / edgeLengths[prev.Index >> 1]) * 0.5;
                    frc *= (edgeAngles[e.Index] - mean) * strength / frc.Length; // unitize and scale

                    forceSums[prev.Start.Index] += frc;
                    forceSums[e.Start.Index] -= frc * 2.0;
                    forceSums[e.End.Index] += frc;
                }
            }
        }


        /// <summary>
        /// Aligns the edges of a mesh with a given set of vectors.
        /// http://www.eecs.berkeley.edu/~sequin/CS285/PAPERS/Pottmann_FrFrm_arch.pdf
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="vectors"></param>
        /// <param name="forceSums"></param>
        public static void AlignEdges(HeMesh mesh, double strength, IList<Vec3d> vectors, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HalfEdgeList edges = mesh.HalfEdges;
            edges.HalfSizeCheck(vectors);

            for (int i = 0; i < edges.Count; i += 2)
            {
                HalfEdge e = edges[i];
                if (e.IsUnused) continue; // skip unused edges

                Vec3d v = vectors[i >> 1];

                // unitize if necessary
                double d = v.SquareLength;
                if (d != 1.0) v /= d;

                // project edge vector onto target vector and take difference
                Vec3d frc = e.Span;
                frc -= frc * v * v;
                frc *= strength;

                forceSums[e.Start.Index] += frc;
                forceSums[e.End.Index] -= frc;
            }
        }


        /// <summary>
        /// Pulls vertices within each face to a common plane.
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

                HalfEdge e0 = f.First;
                HalfEdge e1 = e0.Next;
                HalfEdge e2 = e1.Next;
                HalfEdge e3 = e2.Next;
                if (e3 == e0) continue; // ensure the face has at least 4 edges

                if (e3.Next == e0)
                {
                    // simplified quad case
                    HeVertex v0 = e0.Start;
                    HeVertex v1 = e1.Start;
                    HeVertex v2 = e2.Start;
                    HeVertex v3 = e3.Start;

                    // calculate and apply force
                    Vec3d frc = GeometryUtil.GetShortestVector(v0.Position, v2.Position, v1.Position, v3.Position) * strength;
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
                        HeVertex v0 = e0.Start;
                        HeVertex v1 = e1.Start;
                        HeVertex v2 = e2.Start;
                        HeVertex v3 = e3.Start;

                        // calculate and apply force
                        Vec3d frc = GeometryUtil.GetShortestVector(v0.Position, v2.Position, v1.Position, v3.Position) * strength;
                        forceSums[v0.Index] += frc;
                        forceSums[v2.Index] += frc;
                        forceSums[v1.Index] -= frc;
                        forceSums[v3.Index] -= frc;

                        // advance to next set of 4 points
                        e0 = e0.Next;
                        e1 = e1.Next;
                        e2 = e2.Next;
                        e3 = e3.Next;
                    } while (e0 != f.First);
                }
            }
        }


        /// <summary>
        /// Pulls vertices within each face to a common plane.
        /// Ignores non-quad faces.
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

                HalfEdge e0 = f.First;
                HalfEdge e1 = e0.Next;
                HalfEdge e2 = e1.Next;
                HalfEdge e3 = e2.Next;

                // ensure face is quad
                if (e3.Next == e0)
                {
                    HeVertex v0 = e0.Start;
                    HeVertex v1 = e1.Start;
                    HeVertex v2 = e2.Start;
                    HeVertex v3 = e3.Start;

                    // calculate and apply force
                    Vec3d frc = GeometryUtil.GetShortestVector(v0.Position, v2.Position, v1.Position, v3.Position) * strength;
                    forceSums[v0.Index] += frc;
                    forceSums[v2.Index] += frc;
                    forceSums[v1.Index] -= frc;
                    forceSums[v3.Index] -= frc;
                }
            }
        }


        /// <summary>
        /// Pulls vertices around each vertex to a common plane.
        /// TODO
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void PlanarizeVertices(HeMesh mesh, double strength, IList<Vec3d> forceSums)
        {
            throw new NotImplementedException();

            mesh.Vertices.SizeCheck(forceSums);

            /*
            foreach (HeFace f in mesh.Faces)
            {
                if (f.IsUnused) continue;

                HeEdge e0 = f.First;
                HeEdge e1 = e0.Next;
                HeEdge e2 = e1.Next;
                HeEdge e3 = e2.Next;
                if (e3 == e0) continue; // ensure the face has at least 4 edges

                if (e3.Next == e0)
                {
                    // simplified quad case
                    HeVertex v0 = e0.Start;
                    HeVertex v1 = e1.Start;
                    HeVertex v2 = e2.Start;
                    HeVertex v3 = e3.Start;

                    // calculate and apply force
                    Vec3d frc = GeometryUtil.GetVolumeGradient(v0.Position, v1.Position, v2.Position, v3.Position) * strength;
                    result[v0.Index] -= frc;
                    result[v2.Index] -= frc;
                    result[v1.Index] += frc;
                    result[v3.Index] += frc;
                }
                else
                {
                    // generalized ngon case
                    do
                    {
                        HeVertex v0 = e0.Start;
                        HeVertex v1 = e1.Start;
                        HeVertex v2 = e2.Start;
                        HeVertex v3 = e3.Start;

                        // calculate and apply force
                        Vec3d frc = GeometryUtil.GetVolumeGradient(v0.Position, v1.Position, v2.Position, v3.Position) * strength;
                        result[v0.Index] -= frc;
                        result[v2.Index] -= frc;
                        result[v1.Index] += frc;
                        result[v3.Index] += frc;

                        // advance to next set of 4 points
                        e0 = e0.Next;
                        e1 = e1.Next;
                        e2 = e2.Next;
                        e3 = e3.Next;
                    } while (e0 != f.First);
                }
            }
             */
        }


        /// <summary>
        /// Pulls vertices within each face to a common circle.
        /// Intended for use on quad meshes.
        /// Note that this method should be used in conjunction with planarize.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="halfEdgeAngles"></param>
        /// <param name="forceSums"></param>
        public static void Circularize(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> halfEdgeAngles, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HalfEdgeList edges = mesh.HalfEdges;
            edges.HalfSizeCheck(edgeLengths);
            edges.SizeCheck(halfEdgeAngles);

            foreach (HeFace f in mesh.Faces)
            {
                if (f.IsUnused) continue; // skip unused faces

                // circular condition - opposite angles in the quad must sum to pi
                // http://www.geometrie.tugraz.at/wallner/quad06.pdf
                // http://vecg.cs.ucl.ac.uk/Projects/SmartGeometry/constrained_mesh/paper_docs/constrainedMesh_sigA_11.pdf
                HalfEdge e0 = f.First;
                HalfEdge e1 = e0.Next;
                HalfEdge e2 = e1.Next;
                HalfEdge e3 = e2.Next;
                if (e3.Next != e0) continue; // ensure face is quad

                // apply forces along angle gradient (bisector)
                double mag0 = (halfEdgeAngles[e0.Index] + halfEdgeAngles[e2.Index] - Math.PI) * strength;
                double mag1 = (halfEdgeAngles[e1.Index] + halfEdgeAngles[e3.Index] - Math.PI) * strength;

                // get edge bisectors
                Vec3d f0 = e0.Span / edgeLengths[e0.Index >> 1];
                Vec3d f1 = e1.Span / edgeLengths[e1.Index >> 1];
                Vec3d f2 = e2.Span / edgeLengths[e2.Index >> 1];
                Vec3d f3 = e3.Span / edgeLengths[e3.Index >> 1];
                Vec3d f4 = f3;
                f3 -= f2;
                f2 -= f1;
                f1 -= f0;
                f0 -= f4;

                // unitize and scale bisectors
                f0 *= mag0 / f0.Length;
                f1 *= mag1 / f1.Length;
                f2 *= mag0 / f2.Length;
                f3 *= mag1 / f3.Length;

                // apply force along bisectors
                forceSums[e0.Start.Index] += f3 + f1 - (2.0 * f0);
                forceSums[e1.Start.Index] += f0 + f2 - (2.0 * f1);
                forceSums[e2.Start.Index] += f1 + f3 - (2.0 * f2);
                forceSums[e3.Start.Index] += f2 + f0 - (2.0 * f3);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="halfEdgeAngles"></param>
        /// <param name="forceSums"></param>
        public static void Circularize2(HeMesh mesh, double strength, IList<double> halfEdgeAngles, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);
            mesh.HalfEdges.SizeCheck(halfEdgeAngles);

            foreach (HeFace f in mesh.Faces)
            {
                if (f.IsUnused) continue; // skip unused faces

                // circular condition - opposite angles in the quad must sum to pi
                // http://www.geometrie.tugraz.at/wallner/quad06.pdf
                // http://vecg.cs.ucl.ac.uk/Projects/SmartGeometry/constrained_mesh/paper_docs/constrainedMesh_sigA_11.pdf
                HalfEdge e0 = f.First;
                HalfEdge e1 = e0.Next;
                HalfEdge e2 = e1.Next;
                HalfEdge e3 = e2.Next;
                if (e3.Next != e0) continue; // ensure face is quad

                HeVertex v0 = e0.Start;
                HeVertex v1 = e1.Start;
                HeVertex v2 = e2.Start;
                HeVertex v3 = e3.Start;

                // apply force along the opposite diagonal proportional to the difference between angle sum and pi
                double mag = (halfEdgeAngles[e1.Index] + halfEdgeAngles[e3.Index] - Math.PI) * strength;
                Vec3d frc = v0.VectorTo(v2);
                frc *= mag / frc.Length;

                forceSums[v0.Index] += frc;
                forceSums[v2.Index] -= frc;

                // apply force along the opposite diagonal proportional to the difference between angle sum and pi
                mag = (halfEdgeAngles[e0.Index] + halfEdgeAngles[e2.Index] - Math.PI) * strength;
                frc = v1.VectorTo(v3);
                frc *= mag / frc.Length;

                forceSums[v1.Index] += frc;
                forceSums[v3.Index] -= frc;
            }
        }


        /// <summary>
        /// Adjusts faces around each internal degree 4 vertex to be tangent to a common cone.
        /// Intended for use on quad meshes.
        /// Note that this method should be used in conjunction with planarize.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="halfEdgeAngles"></param>
        /// <param name="forceSums"></param>
        public static void Conicalize(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> halfEdgeAngles, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            HalfEdgeList edges = mesh.HalfEdges;
            edges.HalfSizeCheck(edgeLengths);
            edges.SizeCheck(halfEdgeAngles);

            foreach (HeVertex v in verts)
            {
                if (v.IsUnused || v.IsBoundary) continue; // skip unused or boundary vertices

                // concical - 2 pairs of opposite angles must have an equal sum
                // http://www.geometrie.tugraz.at/wallner/quad06.pdf
                // http://vecg.cs.ucl.ac.uk/Projects/SmartGeometry/constrained_mesh/paper_docs/constrainedMesh_sigA_11.pdf
                HalfEdge e0 = v.First;
                HalfEdge e1 = e0.Twin.Next;
                HalfEdge e2 = e1.Twin.Next;
                HalfEdge e3 = e2.Twin.Next;
                if (e3.Twin.Next != e0) continue; // ensure vertex is degree 4

                // sum the angles of opposite edges
                double mag0 = halfEdgeAngles[e0.Index] + halfEdgeAngles[e2.Index];
                double mag1 = halfEdgeAngles[e1.Index] + halfEdgeAngles[e3.Index];

                // compute force magnitude as standard deviation from mean sum
                double mean = (mag0 + mag1) * 0.5;
                mag0 = (mag0 - mean) * strength;
                mag1 = (mag1 - mean) * strength;

                // get edge bisectors
                Vec3d f0 = e0.Span / edgeLengths[e0.Index >> 1];
                Vec3d f1 = e1.Span / edgeLengths[e1.Index >> 1];
                Vec3d f2 = e2.Span / edgeLengths[e2.Index >> 1];
                Vec3d f3 = e3.Span / edgeLengths[e3.Index >> 1];
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
                forceSums[e0.End.Index] += f0 + f1;
                forceSums[e1.End.Index] += f1 + f2;
                forceSums[e2.End.Index] += f2 + f3;
                forceSums[e3.End.Index] += f3 + f0;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="halfEdgeAngles"></param>
        /// <param name="forceSums"></param>
        public static void Conicalize2(HeMesh mesh, double strength, IList<double> halfEdgeAngles, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            mesh.HalfEdges.SizeCheck(halfEdgeAngles);

            foreach (HeVertex v in verts)
            {
                if (v.IsUnused || v.IsBoundary) continue; // skip unused or boundary vertices

                // concical - 2 pairs of opposite angles must have an equal sum
                // http://www.geometrie.tugraz.at/wallner/quad06.pdf
                // http://vecg.cs.ucl.ac.uk/Projects/SmartGeometry/constrained_mesh/paper_docs/constrainedMesh_sigA_11.pdf
                // push/pull opposite vertices apart/together based on the deviation of the angle sum from Pi
                HalfEdge e0 = v.First;
                HalfEdge e1 = e0.Twin.Next;
                HalfEdge e2 = e1.Twin.Next;
                HalfEdge e3 = e2.Twin.Next;
                if (e3.Twin.Next != e0) continue; // ensure vertex is degree 4

                HeVertex v0 = e0.End;
                HeVertex v1 = e1.End;
                HeVertex v2 = e2.End;
                HeVertex v3 = e3.End;

                // sum the angles of opposite edges
                double mag0 = halfEdgeAngles[e0.Index] + halfEdgeAngles[e2.Index];
                double mag1 = halfEdgeAngles[e1.Index] + halfEdgeAngles[e3.Index];

                // compute force magnitude as standard deviation from mean sum
                double mean = (mag0 + mag1) * 0.5;
                mag0 = (mag0 - mean) * strength;
                mag1 = (mag1 - mean) * strength;

                // get force vectors
                Vec3d f0 = v3.VectorTo(v0);
                Vec3d f1 = v0.VectorTo(v1);
                Vec3d f2 = v1.VectorTo(v2);
                Vec3d f3 = v2.VectorTo(v3);

                // scale force vectors
                f0 *= mag0 / f0.Length;
                f1 *= mag1 / f1.Length;
                f2 *= mag0 / f2.Length;
                f3 *= mag1 / f3.Length;

                forceSums[v0.Index] += f1 - f0;
                forceSums[v1.Index] += f2 - f1;
                forceSums[v2.Index] += f3 - f2;
                forceSums[v3.Index] += f0 - f3;
            }
        }


        /// <summary>
        /// Adjusts edge lengths to make quad faces tangential.
        /// Note that opposite sides of a tangential quad have equal sums.
        /// http://en.wikipedia.org/wiki/Tangential_quadrilateral
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="forceSums"></param>
        public static void Tangentialize(HeMesh mesh, double strength, IList<double> edgeLengths, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);
            mesh.HalfEdges.HalfSizeCheck(edgeLengths);

            HeFaceList faces = mesh.Faces;

            for (int i = 0; i < faces.Count; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;

                // collect relevant edges
                HalfEdge e0 = f.First;
                HalfEdge e1 = e0.Next;
                HalfEdge e2 = e1.Next;
                HalfEdge e3 = e2.Next;
                if (e3.Next != e0) continue; // ensure face is quad

                // collect edge lengths
                double d0 = edgeLengths[e0.Index >> 1];
                double d1 = edgeLengths[e1.Index >> 1];
                double d2 = edgeLengths[e2.Index >> 1];
                double d3 = edgeLengths[e3.Index >> 1];

                // get sums of opposite edges
                double mag0 = d0 + d2;
                double mag1 = d1 + d3;

                // compute force magnitude as standard deviation from mean sum
                double mean = (mag0 + mag1) * 0.5;
                mag0 = (mag0 - mean) * strength;
                mag1 = (mag1 - mean) * strength;

                // get force vectors
                Vec3d f0 = e0.Span * (mag0 / d0);
                Vec3d f1 = e1.Span * (mag1 / d1);
                Vec3d f2 = e2.Span * (mag0 / d2);
                Vec3d f3 = e3.Span * (mag1 / d3);

                forceSums[e0.Start.Index] += f0 - f3;
                forceSums[e1.Start.Index] += f1 - f0;
                forceSums[e2.Start.Index] += f2 - f1;
                forceSums[e3.Start.Index] += f3 - f2;
            }
        }


        /// <summary>
        /// Minimizes gaussian curvature at interior vertices.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void Developablize(HeMesh mesh, double strength, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            double pi2 = Math.PI * 2.0;
            foreach (HeVertex v in verts)
            {
                if (v.IsUnused || v.IsBoundary) continue; // skip unused or boudnary vertices

                // get sum of angles around vertex
                double sum = 0.0;
                foreach (HalfEdge e in v.OutgoingHalfEdges)
                    sum += Vec3d.Angle(e.Span, e.Previous.Twin.Span);

                double mag = (sum - pi2) * strength;
                foreach (HalfEdge e0 in v.OutgoingHalfEdges)
                {
                    HalfEdge e1 = e0.Previous;
                    Vec3d v0 = e0.Span;
                    Vec3d v1 = e1.Span;

                    // force acts along bisector (angle gradient)
                    Vec3d frc = (v0 / v0.Length - v1 / v1.Length) * 0.5;
                    frc *= mag / frc.Length;

                    forceSums[e1.Start.Index] += frc;
                    forceSums[e0.Start.Index] -= frc * 2.0;
                    forceSums[e0.End.Index] += frc;
                }
            }
        }


        /// <summary>
        /// Minimizes gaussian curvature at interior vertices.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="forceSums"></param>
        public static void Developablize(HeMesh mesh, double strength, IList<double> edgeLengths, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            HalfEdgeList edges = mesh.HalfEdges;
            edges.HalfSizeCheck(edgeLengths);

            double pi2 = Math.PI * 2.0;
            foreach (HeVertex v in verts)
            {
                if (v.IsUnused || v.IsBoundary) continue; // skip unused or boudnary vertices

                // get sum of angles around vertex
                double sum = 0.0;
                foreach (HalfEdge e in v.OutgoingHalfEdges)
                    sum += Vec3d.Angle(e.Span, e.Previous.Twin.Span);

                double mag = (sum - pi2) * strength;
                foreach (HalfEdge e0 in v.OutgoingHalfEdges)
                {
                    HalfEdge e1 = e0.Previous;

                    // force acts along bisector (angle gradient)
                    Vec3d frc = (e0.Span / edgeLengths[e0.Index >> 1] - e1.Span / edgeLengths[e1.Index >> 1]) * 0.5;
                    frc *= mag / frc.Length;

                    forceSums[e1.Start.Index] += frc;
                    forceSums[e0.Start.Index] -= frc * 2.0;
                    forceSums[e0.End.Index] += frc;
                }
            }
        }


        /// <summary>
        /// Minimizes gaussian curvature at interior vertices.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="halfEdgeAngles"></param>
        /// <param name="forceSums"></param>
        public static void Developablize(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> halfEdgeAngles, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            HalfEdgeList edges = mesh.HalfEdges;
            edges.HalfSizeCheck(edgeLengths);
            edges.SizeCheck(halfEdgeAngles);

            double pi2 = Math.PI * 2.0;
            foreach (HeVertex v in verts)
            {
                if (v.IsUnused || v.IsBoundary) continue; // skip unused or boudnary vertices

                // get sum of angles around vertex
                double sum = 0.0;
                foreach (HalfEdge e in v.OutgoingHalfEdges) sum += halfEdgeAngles[e.Index];

                double mag = (sum - pi2) * strength;
                foreach (HalfEdge e0 in v.OutgoingHalfEdges)
                {
                    HalfEdge e1 = e0.Previous;

                    // force acts along bisector (angle gradient)
                    Vec3d frc = (e0.Span / edgeLengths[e0.Index >> 1] - e1.Span / edgeLengths[e1.Index >> 1]) * 0.5;
                    frc *= mag / frc.Length;

                    forceSums[e1.Start.Index] += frc;
                    forceSums[e0.Start.Index] -= frc * 2.0;
                    forceSums[e0.End.Index] += frc;
                }
            }
        }


        /// <summary>
        /// TODO
        /// Experimental method
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexDepths"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void Conformalize(HeMesh mesh, double strength, IList<int> vertexDepths, IList<double> edgeLengths, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);
            verts.SizeCheck(vertexDepths);
            mesh.HalfEdges.HalfSizeCheck(edgeLengths);

            for (int i = 0; i < verts.Count; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused || (vertexDepths[i] & 1) == 1) continue; // skip unused or masked vertices

                // get average edge length around vertex
                double mean = 0.0;
                int n = 0;

                foreach (HalfEdge e in v.OutgoingHalfEdges)
                {
                    mean += edgeLengths[e.Index >> 1];
                    n++;
                }
                mean /= n;

                // compute force vectors
                foreach (HalfEdge e in v.OutgoingHalfEdges)
                {
                    HeVertex v0 = e.Start;
                    HeVertex v1 = e.End;

                    Vec3d frc = v0.VectorTo(v1);
                    double mag = edgeLengths[e.Index >> 1];

                    frc *= (mag - mean) * strength / mag;
                    forceSums[v0.Index] += frc;
                    forceSums[v1.Index] -= frc;
                }
            }
        }
    }
}
