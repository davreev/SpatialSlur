using Rhino.Geometry;
using SpatialSlur.SlurCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Static force methods that make use of hemesh topological querying for their calculation
/// 
/// Notes on threading
/// Threaded force calculations are problematic since multiple threads need to write to the same entry in the resulting force array.
/// This requires excessive locking ultimately resulting in performance loss.
/// </summary>

namespace SpatialSlur.SlurMesh
{
    public static class HePhysics
    {
        // Delegates for different SmoothBoundary types
        private static Action<HeMesh, double, IList<Vec3d>>[] _laplacianSmoothUniform = { LaplacianSmoothFixed, LaplacianSmoothCornerFixed, LaplacianSmoothFree };
        private static Action<HeMesh, double, IList<double>, IList<Vec3d>>[] _laplacianSmoothWeighted = { LaplacianSmoothFixed, LaplacianSmoothCornerFixed, LaplacianSmoothFree };
        private static Action<HeMesh, double, IList<Vec3d>>[] _laplacianFair = { LaplacianFairFixed, LaplacianFairCornerFixed, LaplacianFairFree };


        /// <summary>
        /// Calculates forces which pull vertices towards a target mesh.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="target"></param>
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <param name="edgeLengths"></param>
        /// <param name="restLength"></param>
        /// <returns></returns>
        public static void ConstrainEdgeLengths(HeMesh mesh, double strength, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HeEdgeList edges = mesh.Edges;

            // skip every other edge to avoid applying forces twice
            for (int i = 0; i < edges.Count; i += 2)
            {
                // skip unused edges
                HeEdge e = edges[i];
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
        /// <param name="edgeLengths"></param>
        /// <param name="restLength"></param>
        /// <returns></returns>
        public static void ConstrainEdgeLengths(HeMesh mesh, double strength, IList<double> edgeLengths, double restLength, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HeEdgeList edges = mesh.Edges;
            edges.SizeCheck(edgeLengths);

            // skip every other edge to avoid applying forces twice
            for (int i = 0; i < edges.Count; i += 2)
            {
                // skip unused edges
                HeEdge e = edges[i];
                if (e.IsUnused) continue;

                Vec3d frc = e.Span;
                double mag = edgeLengths[i];
                frc *= (mag - restLength) * strength / mag;

                forceSums[e.Start.Index] += frc;
                forceSums[e.End.Index] -= frc;
            }
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="restLengths"></param>
        /// <returns></returns>
        public static void ConstrainEdgeLengths(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> restLengths, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HeEdgeList edges = mesh.Edges;
            edges.SizeCheck(edgeLengths);
            edges.SizeCheck(restLengths);

            // skip every other edge to avoid applying forces twice
            for (int i = 0; i < edges.Count; i += 2)
            {
                // skip unused edges
                HeEdge e = edges[i];
                if (e.IsUnused) continue;

                Vec3d frc = e.Span;
                double mag = edgeLengths[i];
                frc *= (mag - restLengths[i]) * strength / mag;

                forceSums[e.Start.Index] += frc;
                forceSums[e.End.Index] -= frc;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="restLength"></param>
        /// <returns></returns>
        public static void ConstrainEdgeLengths(HeMesh mesh, double strength, IList<double> edgeLengths, double minLength, double maxLength, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HeEdgeList edges = mesh.Edges;
            edges.SizeCheck(edgeLengths);

            if (minLength > maxLength)
                throw new ArgumentException("the given maximum length must be larger than the given minimum length");

            // skip every other edge to avoid applying forces twice
            for (int i = 0; i < edges.Count; i += 2)
            {
                // skip unused edges
                HeEdge e = edges[i];
                if (e.IsUnused) continue;

                Vec3d frc = e.Span;
                double mag = edgeLengths[i];

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
        /// TODO
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="restLength"></param>
        /// <returns></returns>
        public static void ConstrainEdgeLengths(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> minLengths, IList<double> maxLengths, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HeEdgeList edges = mesh.Edges;
            edges.SizeCheck(edgeLengths);
            edges.SizeCheck(minLengths);
            edges.SizeCheck(maxLengths);

            /*
            // skip every other edge to avoid applying forces twice
            for (int i = 0; i < edges.Count; i += 2)
            {
                // skip unused edges
                HeEdge e = edges[i];
                if (e.IsUnused) continue;

                Vec3d frc = e.Span;
                double mag = edgeLengths[i];

                if (mag > maxLength)
                    frc *= (mag - maxLength) * strength / mag;
                else if (mag < minLength)
                    frc *= (mag - minLength) * strength / mag;
                else
                    continue;

                result[e.Start.Index] += frc;
                result[e.End.Index] -= frc;
            }
            */
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="diagonalLengths"></param>
        /// <param name="restLengths"></param>
        /// <returns></returns>
        public static void ConstrainFaceDiagonalLengths(HeMesh mesh, double strength, IList<double> diagonalLengths, double restLength, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);
            mesh.Edges.SizeCheck(diagonalLengths);

            //
            foreach (HeFace f in mesh.Faces)
            {
                if (f.IsUnused) continue;
                int ne = f.CountEdges();

                if (ne > 4)
                {
                    // general ngon case
                    foreach (HeEdge e in f.Edges)
                    {
                        HeVertex v0 = e.Start;
                        HeVertex v1 = e.Next.End;

                        Vec3d frc = v0.VectorTo(v1);
                        double mag = diagonalLengths[e.Index];
                        frc *= (mag - restLength) * strength / mag;

                        forceSums[v0.Index] += frc;
                        forceSums[v1.Index] -= frc;
                    }
                }
                else if (ne == 4)
                {
                    // simplified quad case
                    HeEdge e = f.First;
                    for (int i = 0; i < 2; i++)
                    {
                        HeVertex v0 = e.Start;
                        HeVertex v1 = e.Next.End;

                        Vec3d frc = v0.VectorTo(v1);
                        double mag = diagonalLengths[e.Index];
                        frc *= (mag - restLength) * strength / mag;

                        forceSums[v0.Index] += frc;
                        forceSums[v1.Index] -= frc;
                        e = e.Next;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="diagonalLengths"></param>
        /// <param name="restLengths"></param>
        /// <returns></returns>
        public static void ConstrainFaceDiagonalLengths(HeMesh mesh, double strength, IList<double> diagonalLengths, IList<double> restLengths, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HeEdgeList edges = mesh.Edges;
            edges.SizeCheck(diagonalLengths);
            edges.SizeCheck(restLengths);

            //
            foreach (HeFace f in mesh.Faces)
            {
                if (f.IsUnused) continue;
                int ne = f.CountEdges();

                if (ne > 4)
                {
                    // general ngon case
                    foreach (HeEdge e in f.Edges)
                    {
                        HeVertex v0 = e.Start;
                        HeVertex v1 = e.Next.End;

                        Vec3d frc = v0.VectorTo(v1);
                        double mag = diagonalLengths[e.Index];
                        frc *= (mag - restLengths[e.Index]) * strength / mag;

                        forceSums[v0.Index] += frc;
                        forceSums[v1.Index] -= frc;
                    }
                }
                else if (ne == 4)
                {
                    // simplified quad case
                    HeEdge e = f.First;
                    for (int i = 0; i < 2; i++)
                    {
                        HeVertex v0 = e.Start;
                        HeVertex v1 = e.Next.End;

                        Vec3d frc = v0.VectorTo(v1);
                        double mag = diagonalLengths[e.Index];
                        frc *= (mag - restLengths[e.Index]) * strength / mag;

                        forceSums[v0.Index] += frc;
                        forceSums[v1.Index] -= frc;
                        e = e.Next;
                    }
                }
            }
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="diagonalLengths"></param>
        /// <param name="restLengths"></param>
        /// <returns></returns>
        public static void ConstrainFaceDiagonalLengths(HeMesh mesh, double strength, IList<double> diagonalLengths, double minLength, double maxLength, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);
            mesh.Edges.SizeCheck(diagonalLengths);

            if (minLength > maxLength)
                throw new ArgumentException("the given maximum length must be larger than the given minimum length");

            //
            foreach (HeFace f in mesh.Faces)
            {
                if (f.IsUnused) continue;
                int ne = f.CountEdges();

                if (ne > 4)
                {
                    // general ngon case
                    foreach (HeEdge e in f.Edges)
                    {
                        HeVertex v0 = e.Start;
                        HeVertex v1 = e.Next.End;

                        Vec3d frc = v0.VectorTo(v1);
                        double mag = diagonalLengths[e.Index];

                        if (mag > maxLength)
                            frc *= (mag - maxLength) * strength / mag;
                        else if (mag < minLength)
                            frc *= (mag - minLength) * strength / mag;
                        else
                            continue;

                        forceSums[v0.Index] += frc;
                        forceSums[v1.Index] -= frc;
                    }
                }
                else if (ne == 4)
                {
                    // simplified quad case
                    HeEdge e = f.First;
                    for (int i = 0; i < 2; i++)
                    {
                        HeVertex v0 = e.Start;
                        HeVertex v1 = e.Next.End;

                        Vec3d frc = v0.VectorTo(v1);
                        double mag = diagonalLengths[e.Index];

                        if (mag > maxLength)
                            frc *= (mag - maxLength) * strength / mag;
                        else if (mag < minLength)
                            frc *= (mag - minLength) * strength / mag;
                        else
                            continue;

                        forceSums[v0.Index] += frc;
                        forceSums[v1.Index] -= frc;
                        e = e.Next;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainEdgeAngles(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> edgeAngles, double restAngle, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HeEdgeList edges = mesh.Edges;
            edges.SizeCheck(edgeLengths);
            edges.SizeCheck(edgeAngles);

            for (int i = 0; i < edges.Count; i++)
            {
                HeEdge e = edges[i];
                if (e.IsUnused || e.Face == null) continue; // skip unused or boundary edges

                // get unitized edge bisector
                HeEdge prev = e.Prev;
                Vec3d frc = (e.Span / edgeLengths[i] - prev.Span / edgeLengths[prev.Index]) * 0.5;
                frc *= (edgeAngles[i] - restAngle) * strength / frc.Length; // unitize and scale

                forceSums[prev.Start.Index] += frc;
                forceSums[e.Start.Index] -= frc * 2.0;
                forceSums[e.End.Index] += frc;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainEdgeAngles(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> edgeAngles, IList<double> restAngles, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HeEdgeList edges = mesh.Edges;
            edges.SizeCheck(edgeLengths);
            edges.SizeCheck(edgeAngles);
            edges.SizeCheck(restAngles);

            for (int i = 0; i < edges.Count; i++)
            {
                HeEdge e = edges[i];
                if (e.IsUnused || e.Face == null) continue; // skip unused or boundary edges

                // get unitized edge bisector
                HeEdge prev = e.Prev;
                Vec3d frc = (e.Span / edgeLengths[i] - prev.Span / edgeLengths[prev.Index]) * 0.5;
                frc *= (edgeAngles[i] - restAngles[i]) * strength / frc.Length; // unitize and scale

                forceSums[prev.Start.Index] += frc;
                forceSums[e.Start.Index] -= frc * 2.0;
                forceSums[e.End.Index] += frc;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainEdgeAngles(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> edgeAngles, double minAngle, double maxAngle, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HeEdgeList edges = mesh.Edges;
            edges.SizeCheck(edgeLengths);
            edges.SizeCheck(edgeAngles);

            if (minAngle > maxAngle)
                throw new ArgumentException("the given maximum angle must be larger than the given minimum angle");

            for (int i = 0; i < edges.Count; i++)
            {
                HeEdge e = edges[i];
                if (e.IsUnused || e.Face == null) continue; // skip unused or boundary edges

                // get unitized edge bisector
                HeEdge prev = e.Prev;
                Vec3d frc = (e.Span / edgeLengths[i] - prev.Span / edgeLengths[prev.Index]) * 0.5;
                double mag = edgeAngles[i];

                if (mag > maxAngle)
                    frc *= (edgeAngles[i] - maxAngle) * strength / frc.Length; // unitize and scale
                else if (mag < minAngle)
                    frc *= (edgeAngles[i] - minAngle) * strength / frc.Length; // unitize and scale
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
        /// <param name="faceAngles"></param>
        /// <param name="restAngle"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainDihedralAngles(HeMesh mesh, double strength, IList<Vec3d> faceNormals, IList<double> faceAngles, double restAngle, IList<Vec3d> forceSums)
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// http://www.miralab.ch/repository/papers/165.pdf
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="faceAngles"></param>
        /// <param name="restAngle"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainDihedralAngles(HeMesh mesh, double strength, IList<Vec3d> faceNormals, IList<double> faceAngles, IList<double> restAngles, IList<Vec3d> forceSums)
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// Adjusts edge lengths to make adjacent faces have tangent incircles.
        /// Intended for use on triangle meshes.
        /// http://www.geometrie.tuwien.ac.at/hoebinger/mhoebinger_files/circlepackings.pdf
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static void CirclePack(HeMesh mesh, double strength, IList<double> edgeLengths, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HeEdgeList edges = mesh.Edges;
            edges.SizeCheck(edgeLengths);

            // skip every other edge to avoid applying forces twice
            for (int i = 0; i < edges.Count; i += 2)
            {
                HeEdge e = edges[i];
                if (e.IsUnused || e.IsBoundary) continue; // skip unused, boundary, or edges

                // collect relevant edges
                HeEdge e0 = e.Next;
                HeEdge e1 = e.Prev;
                HeEdge e2 = e.Twin.Next;
                HeEdge e3 = e.Twin.Prev;
                if (!(e0.Next == e1 && e2.Next == e3)) continue; // ensure both faces are tris

                // collect edge lengths
                double d0 = edgeLengths[e0.Index];
                double d1 = edgeLengths[e1.Index];
                double d2 = edgeLengths[e2.Index];
                double d3 = edgeLengths[e3.Index];

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
        /// <param name="edgeLengths"></param>
        /// <param name="radii"></param>
        /// <returns></returns>
        public static void CirclePack(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> radii, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);
            verts.SizeCheck(radii);

            HeEdgeList edges = mesh.Edges;
            edges.SizeCheck(edgeLengths);

            // skip every other edge to avoid applying forces twice
            for (int i = 0; i < edges.Count; i += 2)
            {
                HeEdge e = edges[i];
                if (e.IsUnused) continue; // skip unused edges

                // compute force vectors
                HeVertex v0 = e.Start;
                HeVertex v1 = e.End;

                double mag = edgeLengths[i];
                double rest = radii[v0.Index] + radii[v1.Index];
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
        /// <returns></returns>
        public static void LaplacianSmooth(HeMesh mesh, double strength, IList<Vec3d> forceSums)
        {
            LaplacianSmoothFixed(mesh, strength, forceSums);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static void LaplacianSmooth(HeMesh mesh, double strength, IList<Vec3d> forceSums, SmoothBoundaryType boundaryType)
        {
            _laplacianSmoothUniform[(int)boundaryType](mesh, strength, forceSums);
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
                        if (v.IsDeg2) continue; // skip corners

                        HeEdge e = v.Outgoing;
                        HeVertex v0 = e.Twin.Start;
                        HeVertex v1 = e.Prev.Start;

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
                        HeEdge e = v.Outgoing;
                        HeVertex v0 = e.Twin.Start;
                        HeVertex v1 = e.Prev.Start;

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
        /// <param name="edgeWeights"></param>
        /// <param name="forceSums"></param>
        public static void LaplacianSmooth(HeMesh mesh, double strength, IList<double> edgeWeights, IList<Vec3d> forceSums)
        {
            LaplacianSmoothFixed(mesh, strength, edgeWeights, forceSums);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="strength"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="forceSums"></param>
        /// <param name="boundaryType"></param>
        public static void LaplacianSmooth(HeMesh mesh, double strength, IList<double> edgeWeights, IList<Vec3d> forceSums, SmoothBoundaryType boundaryType)
        {
            _laplacianSmoothWeighted[(int)boundaryType](mesh, strength, edgeWeights, forceSums);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        private static void LaplacianSmoothFixed(HeMesh mesh, double strength, IList<double> edgeWeights, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);
            mesh.Edges.SizeCheck(edgeWeights);

            Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = verts[i];
                    if (v.IsUnused || v.IsBoundary) continue;

                    Vec3d sum = new Vec3d();

                    foreach (HeEdge e in v.OutgoingEdges)
                        sum += e.Span * edgeWeights[e.Index];

                    // add force vector
                    forceSums[i] += sum * strength;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        private static void LaplacianSmoothCornerFixed(HeMesh mesh, double strength, IList<double> edgeWeights, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);
            mesh.Edges.SizeCheck(edgeWeights);

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
                        if (v.IsDeg2) continue; // skip corners

                        HeEdge e = v.Outgoing;
                        sum += e.Span * edgeWeights[e.Index];
                        e = e.Prev.Twin;
                        sum += e.Span * edgeWeights[e.Index];
                    }
                    else
                    {
                        foreach (HeEdge e in v.OutgoingEdges)
                            sum += e.Span * edgeWeights[e.Index];
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
        /// <param name="edgeWeights"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        private static void LaplacianSmoothFree(HeMesh mesh, double strength, IList<double> edgeWeights, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);
            mesh.Edges.SizeCheck(edgeWeights);

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
                        HeEdge e = v.Outgoing;
                        sum += e.Span * edgeWeights[e.Index];
                        e = e.Prev.Twin;
                        sum += e.Span * edgeWeights[e.Index];
                    }
                    else
                    {
                        foreach (HeEdge e in v.OutgoingEdges)
                            sum += e.Span * edgeWeights[e.Index];
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
        /// <returns></returns>
        public static void LaplacianFair(HeMesh mesh, double strength, IList<Vec3d> forceSums, SmoothBoundaryType boundaryType = SmoothBoundaryType.Fixed)
        {
            _laplacianFair[(int)boundaryType](mesh, strength, forceSums);
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
                    if (v.IsDeg2) continue; // skip corners

                    HeEdge e = v.Outgoing;
                    HeVertex v0 = e.Twin.Start;
                    HeVertex v1 = e.Prev.Start;

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
                    HeEdge e = v.Outgoing;
                    HeVertex v0 = e.Twin.Start;
                    HeVertex v1 = e.Prev.Start;

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
        /// <param name="edgeLengths"></param>
        /// <param name="?"></param>
        /// <param name="forceSums"></param>
        public static void EqualizeVertexEdgeLengths(HeMesh mesh, double strength, IList<double> edgeLengths, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            mesh.Edges.SizeCheck(edgeLengths);

            foreach (HeVertex v in verts)
            {
                if (v.IsUnused) continue; // skip unused vertices

                // get average edge length around vertex
                double mean = 0.0;
                int n = 0;

                foreach (HeEdge e in v.OutgoingEdges)
                {
                    mean += edgeLengths[e.Index];
                    n++;
                }
                mean /= n;

                // compute force vectors
                foreach (HeEdge e in v.OutgoingEdges)
                {
                    HeVertex v0 = e.Start;
                    HeVertex v1 = e.End;

                    Vec3d frc = v0.VectorTo(v1);
                    double mag = edgeLengths[e.Index];

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
        /// <param name="edgeLengths"></param>
        /// <returns></returns>
        public static void EqualizeFaceEdgeLengths(HeMesh mesh, double strength, IList<double> edgeLengths, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);
            mesh.Edges.SizeCheck(edgeLengths);

            foreach (HeFace f in mesh.Faces)
            {
                // skip unused faces
                if (f.IsUnused) continue;

                // get average edge length within the face
                double mean = 0.0;
                int n = 0;

                foreach (HeEdge e in f.Edges)
                {
                    mean += edgeLengths[e.Index];
                    n++;
                }
                mean /= n;

                // compute force vectors
                foreach (HeEdge e in f.Edges)
                {
                    HeVertex v0 = e.Start;
                    HeVertex v1 = e.End;

                    Vec3d frc = v0.VectorTo(v1);
                    double mag = edgeLengths[e.Index];

                    frc *= (mag - mean) * strength / mag;
                    forceSums[v0.Index] += frc;
                    forceSums[v1.Index] -= frc;
                }
            }
        }


        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="diagonalLengths"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void EqualizeVertexDiagonalLengths(HeMesh mesh, double strength, IList<double> diagonalLengths, IList<Vec3d> forceSums)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="diagonalLengths"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void EqualizeFaceDiagonalLengths(HeMesh mesh, double strength, IList<double> diagonalLengths, IList<Vec3d> forceSums)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Adjusts edge angles towards the average around their vertex.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="?"></param>
        /// <param name="forceSums"></param>
        public static void EqualizeVertexEdgeAngles(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> edgeAngles, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            HeEdgeList edges = mesh.Edges;
            edges.SizeCheck(edgeLengths);
            edges.SizeCheck(edgeAngles);

            foreach (HeVertex v in verts)
            {
                if (v.IsUnused || v.IsBoundary) continue; // skip unused or boundary vertices

                // get average angle
                double mean = 0.0;
                int n = 0;
                foreach (HeEdge e in v.OutgoingEdges)
                {
                    mean += edgeAngles[e.Index];
                    n++;
                }
                mean /= n;

                // calculate forces
                foreach (HeEdge e in v.OutgoingEdges)
                {
                    // get angle gradient vector (unitized edge bisector)
                    HeEdge prev = e.Prev;
                    Vec3d frc = (e.Span / edgeLengths[e.Index] - prev.Span / edgeLengths[prev.Index]) * 0.5;
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
        /// <param name="edgeLengths"></param>
        /// <returns></returns>
        public static void EqualizeFaceEdgeAngles(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> edgeAngles, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HeEdgeList edges = mesh.Edges;
            edges.SizeCheck(edgeLengths);
            edges.SizeCheck(edgeAngles);

            foreach (HeFace f in mesh.Faces)
            {
                if (f.IsUnused) continue; // skip unused faces

                // get average angle
                double mean = 0.0;
                int n = 0;
                foreach (HeEdge e in f.Edges)
                {
                    mean += edgeAngles[e.Index];
                    n++;
                }
                mean /= n;

                // calculate forces based on angular deviation from mean
                foreach (HeEdge e in f.Edges)
                {
                    // get edge bisector
                    HeEdge prev = e.Prev;
                    Vec3d frc = (e.Span / edgeLengths[e.Index] - prev.Span / edgeLengths[prev.Index]) * 0.5;
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
        /// <param name="meshB"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void AlignEdges(HeMesh mesh, double strength, IList<Vec3d> vectors, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HeEdgeList edges = mesh.Edges;
            edges.SizeCheck(vectors);

            for (int i = 0; i < edges.Count; i += 2)
            {
                HeEdge e = edges[i];
                if (e.IsUnused) continue; // skip unused edges

                Vec3d v = vectors[i];

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
        /// <returns></returns>
        public static void PlanarizeFaces(HeMesh mesh, double strength, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

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
        /// <returns></returns>
        public static void PlanarizeQuads(HeMesh mesh, double strength, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            foreach (HeFace f in mesh.Faces)
            {
                if (f.IsUnused) continue;

                HeEdge e0 = f.First;
                HeEdge e1 = e0.Next;
                HeEdge e2 = e1.Next;
                HeEdge e3 = e2.Next;

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
        /// <returns></returns>
        public static void PlanarizeVertices(HeMesh mesh, double strength, IList<Vec3d> forceSums)
        {
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
        /// <param name="faceCenters"></param>
        /// <returns></returns>
        public static void Circularize(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> edgeAngles, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);

            HeEdgeList edges = mesh.Edges;
            edges.SizeCheck(edgeLengths);
            edges.SizeCheck(edgeAngles);

            foreach (HeFace f in mesh.Faces)
            {
                if (f.IsUnused) continue; // skip unused faces

                // circular condition - opposite angles in the quad must sum to pi
                // http://www.geometrie.tugraz.at/wallner/quad06.pdf
                // http://vecg.cs.ucl.ac.uk/Projects/SmartGeometry/constrained_mesh/paper_docs/constrainedMesh_sigA_11.pdf
                HeEdge e0 = f.First;
                HeEdge e1 = e0.Next;
                HeEdge e2 = e1.Next;
                HeEdge e3 = e2.Next;
                if (e3.Next != e0) continue; // ensure face is quad

                // apply forces along angle gradient (bisector)
                double mag0 = (edgeAngles[e0.Index] + edgeAngles[e2.Index] - Math.PI) * strength;
                double mag1 = (edgeAngles[e1.Index] + edgeAngles[e3.Index] - Math.PI) * strength;

                // get edge bisectors
                Vec3d f0 = e0.Span / edgeLengths[e0.Index];
                Vec3d f1 = e1.Span / edgeLengths[e1.Index];
                Vec3d f2 = e2.Span / edgeLengths[e2.Index];
                Vec3d f3 = e3.Span / edgeLengths[e3.Index];
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
        /// Experimental alternative method.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="faceCenters"></param>
        /// <returns></returns>
        public static void Circularize2(HeMesh mesh, double strength, IList<double> edgeAngles, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);
            mesh.Edges.SizeCheck(edgeAngles);

            foreach (HeFace f in mesh.Faces)
            {
                if (f.IsUnused) continue; // skip unused faces

                // circular condition - opposite angles in the quad must sum to pi
                // http://www.geometrie.tugraz.at/wallner/quad06.pdf
                // http://vecg.cs.ucl.ac.uk/Projects/SmartGeometry/constrained_mesh/paper_docs/constrainedMesh_sigA_11.pdf
                HeEdge e0 = f.First;
                HeEdge e1 = e0.Next;
                HeEdge e2 = e1.Next;
                HeEdge e3 = e2.Next;
                if (e3.Next != e0) continue; // ensure face is quad

                HeVertex v0 = e0.Start;
                HeVertex v1 = e1.Start;
                HeVertex v2 = e2.Start;
                HeVertex v3 = e3.Start;

                // apply force along the opposite diagonal proportional to the difference between angle sum and pi
                double mag = (edgeAngles[e1.Index] + edgeAngles[e3.Index] - Math.PI) * strength;
                Vec3d frc = v0.VectorTo(v2);
                frc *= mag / frc.Length;

                forceSums[v0.Index] += frc;
                forceSums[v2.Index] -= frc;

                // apply force along the opposite diagonal proportional to the difference between angle sum and pi
                mag = (edgeAngles[e0.Index] + edgeAngles[e2.Index] - Math.PI) * strength;
                frc = v1.VectorTo(v3);
                frc *= mag / frc.Length;

                forceSums[v1.Index] += frc;
                forceSums[v3.Index] -= frc;
            }
        }



        /// <summary>
        /// Adjusts faces around each internal degree 4 vertex to be tangent to a common cone.
        /// Intended for use on quadrilateral meshes.
        /// Note that this method should be used in conjunction with planarize.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="faceCenters"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void Conicalize(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> edgeAngles, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            HeEdgeList edges = mesh.Edges;
            edges.SizeCheck(edgeLengths);
            edges.SizeCheck(edgeAngles);

            foreach (HeVertex v in verts)
            {
                if (v.IsUnused || v.IsBoundary) continue; // skip unused or boundary vertices

                // concical - 2 pairs of opposite angles must have an equal sum
                // http://www.geometrie.tugraz.at/wallner/quad06.pdf
                // http://vecg.cs.ucl.ac.uk/Projects/SmartGeometry/constrained_mesh/paper_docs/constrainedMesh_sigA_11.pdf
                HeEdge e0 = v.Outgoing;
                HeEdge e1 = e0.Twin.Next;
                HeEdge e2 = e1.Twin.Next;
                HeEdge e3 = e2.Twin.Next;
                if (e3.Twin.Next != e0) continue; // ensure vertex is degree 4

                // sum the angles of opposite edges
                double mag0 = edgeAngles[e0.Index] + edgeAngles[e2.Index];
                double mag1 = edgeAngles[e1.Index] + edgeAngles[e3.Index];

                // compute force magnitude as standard deviation from mean sum
                double mean = (mag0 + mag1) * 0.5;
                mag0 = (mag0 - mean) * strength;
                mag1 = (mag1 - mean) * strength;

                // get edge bisectors
                Vec3d f0 = e0.Span / edgeLengths[e0.Index];
                Vec3d f1 = e1.Span / edgeLengths[e1.Index];
                Vec3d f2 = e2.Span / edgeLengths[e2.Index];
                Vec3d f3 = e3.Span / edgeLengths[e3.Index];
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
        /// Experimental alternative method.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="faceCenters"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void Conicalize2(HeMesh mesh, double strength, IList<double> edgeAngles, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            mesh.Edges.SizeCheck(edgeAngles);

            foreach (HeVertex v in verts)
            {
                if (v.IsUnused || v.IsBoundary) continue; // skip unused or boundary vertices

                // concical - 2 pairs of opposite angles must have an equal sum
                // http://www.geometrie.tugraz.at/wallner/quad06.pdf
                // http://vecg.cs.ucl.ac.uk/Projects/SmartGeometry/constrained_mesh/paper_docs/constrainedMesh_sigA_11.pdf
                // push/pull opposite vertices apart/together based on the deviation of the angle sum from Pi
                HeEdge e0 = v.Outgoing;
                HeEdge e1 = e0.Twin.Next;
                HeEdge e2 = e1.Twin.Next;
                HeEdge e3 = e2.Twin.Next;
                if (e3.Twin.Next != e0) continue; // ensure vertex is degree 4

                HeVertex v0 = e0.End;
                HeVertex v1 = e1.End;
                HeVertex v2 = e2.End;
                HeVertex v3 = e3.End;

                // sum the angles of opposite edges
                double mag0 = edgeAngles[e0.Index] + edgeAngles[e2.Index];
                double mag1 = edgeAngles[e1.Index] + edgeAngles[e3.Index];

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
        /// 
        /// combine with physical smooth to produce circle packing quad meshes
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static void Tangentialize(HeMesh mesh, double strength, IList<double> edgeLengths, IList<Vec3d> forceSums)
        {
            mesh.Vertices.SizeCheck(forceSums);
            mesh.Edges.SizeCheck(edgeLengths);

            HeFaceList faces = mesh.Faces;

            for (int i = 0; i < faces.Count; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;

                // collect relevant edges
                HeEdge e0 = f.First;
                HeEdge e1 = e0.Next;
                HeEdge e2 = e1.Next;
                HeEdge e3 = e2.Next;
                if (e3.Next != e0) continue; // ensure face is quad

                // collect edge lengths
                double d0 = edgeLengths[e0.Index];
                double d1 = edgeLengths[e1.Index];
                double d2 = edgeLengths[e2.Index];
                double d3 = edgeLengths[e3.Index];

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
        /// <param name="edgeAngles"></param>
        /// <returns></returns>
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
                foreach (HeEdge e in v.OutgoingEdges)
                    sum += Vec3d.Angle(e.Span, e.Prev.Twin.Span);

                double mag = (sum - pi2) * strength;
                foreach (HeEdge e0 in v.OutgoingEdges)
                {
                    HeEdge e1 = e0.Prev;
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
        /// <param name="edgeAngles"></param>
        /// <returns></returns>
        public static void Developablize(HeMesh mesh, double strength, IList<double> edgeLengths, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            HeEdgeList edges = mesh.Edges;
            edges.SizeCheck(edgeLengths);

            double pi2 = Math.PI * 2.0;
            foreach (HeVertex v in verts)
            {
                if (v.IsUnused || v.IsBoundary) continue; // skip unused or boudnary vertices

                // get sum of angles around vertex
                double sum = 0.0;
                foreach (HeEdge e in v.OutgoingEdges)
                    sum += Vec3d.Angle(e.Span, e.Prev.Twin.Span);

                double mag = (sum - pi2) * strength;
                foreach (HeEdge e0 in v.OutgoingEdges)
                {
                    HeEdge e1 = e0.Prev;

                    // force acts along bisector (angle gradient)
                    Vec3d frc = (e0.Span / edgeLengths[e0.Index] - e1.Span / edgeLengths[e1.Index]) * 0.5;
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
        /// <param name="edgeAngles"></param>
        /// <returns></returns>
        public static void Developablize(HeMesh mesh, double strength, IList<double> edgeLengths, IList<double> edgeAngles, IList<Vec3d> forceSums)
        {
            HeVertexList verts = mesh.Vertices;
            verts.SizeCheck(forceSums);

            HeEdgeList edges = mesh.Edges;
            edges.SizeCheck(edgeLengths);
            edges.SizeCheck(edgeAngles);

            double pi2 = Math.PI * 2.0;
            foreach (HeVertex v in verts)
            {
                if (v.IsUnused || v.IsBoundary) continue; // skip unused or boudnary vertices

                // get sum of angles around vertex
                double sum = 0.0;
                foreach (HeEdge e in v.OutgoingEdges) sum += edgeAngles[e.Index];

                double mag = (sum - pi2) * strength;
                foreach (HeEdge e0 in v.OutgoingEdges)
                {
                    HeEdge e1 = e0.Prev;

                    // force acts along bisector (angle gradient)
                    Vec3d frc = (e0.Span / edgeLengths[e0.Index] - e1.Span / edgeLengths[e1.Index]) * 0.5;
                    frc *= mag / frc.Length;

                    forceSums[e1.Start.Index] += frc;
                    forceSums[e0.Start.Index] -= frc * 2.0;
                    forceSums[e0.End.Index] += frc;
                }
            }
        }


        /// <summary>
        /// TODO
        /// Experimental method.
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
            mesh.Edges.SizeCheck(edgeLengths);

            for (int i = 0; i < verts.Count; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused || (vertexDepths[i] & 1) == 1) continue; // skip unused or masked vertices

                // get average edge length around vertex
                double mean = 0.0;
                int n = 0;

                foreach (HeEdge e in v.OutgoingEdges)
                {
                    mean += edgeLengths[e.Index];
                    n++;
                }
                mean /= n;

                // compute force vectors
                foreach (HeEdge e in v.OutgoingEdges)
                {
                    HeVertex v0 = e.Start;
                    HeVertex v1 = e.End;

                    Vec3d frc = v0.VectorTo(v1);
                    double mag = edgeLengths[e.Index];

                    frc *= (mag - mean) * strength / mag;
                    forceSums[v0.Index] += frc;
                    forceSums[v1.Index] -= frc;
                }
            }
        }
    }
}
