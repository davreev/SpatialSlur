using SpatialSlur.SlurCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// Haphazard collection of static methods for mesh relaxation and optimization.
    /// Note that these methods only handle the calculation of forces. Integration is left up to the specific implementation.
    /// </summary>
    [Obsolete]
    public static class HePhysics
    {
        /// <summary>
        /// Delegates for boundary dependant methods
        /// </summary>
        private static Action<HeMesh, IReadOnlyList<Vec3d>, double, IList<Vec3d>, bool>[] _lapSmooth = 
        { 
            LaplacianSmoothFixed, 
            LaplacianSmoothCornerFixed, 
            LaplacianSmoothFree 
        };


        /// <summary>
        /// Delegates for boundary dependant methods
        /// </summary>
        private static Action<HeMesh, IReadOnlyList<Vec3d>, double, IReadOnlyList<double>, IList<Vec3d>, bool>[] _lapSmoothWeighted = 
        { 
            LaplacianSmoothFixed, 
            LaplacianSmoothCornerFixed, 
            LaplacianSmoothFree 
        };


        /// <summary>
        /// Delegates for boundary dependant methods
        /// </summary>
        private static Action<HeMesh, IReadOnlyList<Vec3d>, double, IList<Vec3d>>[] _lapFair =
        {
            LaplacianFairFixed,
            LaplacianFairCornerFixed,
            LaplacianFairFree
        };


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        /// <param name="boundaryType"></param>
        /// <param name="parallel"></param>
        public static void LaplacianSmooth(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IList<Vec3d> forceSums, SmoothBoundaryType boundaryType = SmoothBoundaryType.Fixed, bool parallel = false)
        {
            _lapSmooth[(int)boundaryType](mesh, vertexPositions, strength, forceSums, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        /// <param name="parallel"></param>
        private static void LaplacianSmoothFixed(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IList<Vec3d> forceSums, bool parallel)
        {
            var verts = mesh.Vertices;

            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsUnused || v.IsBoundary) continue;

                    // get average position of neighbouring vertices
                    Vec3d sum = new Vec3d();
                    int n = 0;

                    foreach (var cv in v.ConnectedVertices)
                    {
                        sum += vertexPositions[cv.Index];
                        n++;
                    }

                    // add force vector
                    forceSums[i] += (sum / n - vertexPositions[i]) * strength;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), func);
            else
                func(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        /// <param name="parallel"></param>
        private static void LaplacianSmoothCornerFixed(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IList<Vec3d> forceSums, bool parallel)
        {
            var verts = mesh.Vertices;

            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsUnused) continue;

                    // if boundary vertex, only consider neighbours which are also on the boundary
                    if (v.IsBoundary)
                    {
                        if (v.IsDegree2) continue; // skip corners

                        var he = v.First;
                        var vi0 = he.Twin.Start.Index;
                        var vi1 = he.Previous.Start.Index;

                        // add force vector
                        forceSums[i] += ((vertexPositions[vi0] + vertexPositions[vi1]) * 0.5 - vertexPositions[i]) * strength;
                    }
                    else
                    {
                        Vec3d sum = new Vec3d();
                        int n = 0;

                        foreach (var cv in v.ConnectedVertices)
                        {
                            sum += vertexPositions[cv.Index];
                            n++;
                        }

                        // add force vector
                        forceSums[i] += (sum / n - vertexPositions[i]) * strength;
                    }
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), func);
            else
                func(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        /// <param name="parallel"></param>
        private static void LaplacianSmoothFree(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IList<Vec3d> forceSums, bool parallel)
        {
            var verts = mesh.Vertices;

            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsUnused) continue;

                    // if boundary vertex, only consider neighbours which are also on the boundary
                    if (v.IsBoundary)
                    {
                        var he = v.First;
                        var vi0 = he.Twin.Start.Index;
                        var vi1 = he.Previous.Start.Index;

                        // add force vector
                        forceSums[i] += ((vertexPositions[vi0] + vertexPositions[vi1]) * 0.5 - vertexPositions[i]) * strength;
                    }
                    else
                    {
                        Vec3d sum = new Vec3d();
                        int n = 0;

                        foreach (var cv in v.ConnectedVertices)
                        {
                            sum += vertexPositions[cv.Index];
                            n++;
                        }

                        // add force vector
                        forceSums[i] += (sum / n - vertexPositions[i]) * strength;
                    }
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), func);
            else
                func(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="forceSums"></param>
        /// <param name="boundaryType"></param>
        /// <param name="parallel"></param>
        public static void LaplacianSmooth(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IReadOnlyList<double> halfedgeWeights, IList<Vec3d> forceSums, SmoothBoundaryType boundaryType = SmoothBoundaryType.Fixed, bool parallel = false)
        {
            _lapSmoothWeighted[(int)boundaryType](mesh, vertexPositions, strength, halfedgeWeights, forceSums, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="forceSums"></param>
        /// <param name="parallel"></param>
        private static void LaplacianSmoothFixed(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IReadOnlyList<double> halfedgeWeights, IList<Vec3d> forceSums, bool parallel)
        {
            var verts = mesh.Vertices;

            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsUnused || v.IsBoundary) continue;

                    Vec3d sum = new Vec3d();

                    foreach (var he in v.OutgoingHalfedges)
                        sum += he.GetDelta(vertexPositions) * halfedgeWeights[he.Index];

                    // add force vector
                    forceSums[i] += sum * strength;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), func);
            else
                func(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        private static void LaplacianSmoothCornerFixed(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IReadOnlyList<double> halfedgeWeights, IList<Vec3d> forceSums, bool parallel)
        {
            var verts = mesh.Vertices;

            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsUnused) continue;

                    Vec3d sum = new Vec3d();

                    // if boundary vertex, only consider neighbours which are also on the boundary
                    if (v.IsBoundary)
                    {
                        if (v.IsDegree2) continue; // skip corners

                        var he = v.First;
                        sum += he.GetDelta(vertexPositions) * halfedgeWeights[he.Index];
                        he = he.Previous.Twin;
                        sum += he.GetDelta(vertexPositions) * halfedgeWeights[he.Index];
                    }
                    else
                    {
                        foreach (var he in v.OutgoingHalfedges)
                            sum += he.GetDelta(vertexPositions) * halfedgeWeights[he.Index];
                    }

                    // add force vector
                    forceSums[i] += sum * strength;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), func);
            else
                func(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="forceSums"></param>
        /// <param name="parallel"></param>
        private static void LaplacianSmoothFree(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IReadOnlyList<double> halfedgeWeights, IList<Vec3d> forceSums, bool parallel)
        {
            var verts = mesh.Vertices;

            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsUnused) continue;

                    Vec3d sum = new Vec3d();

                    // if boundary vertex, only consider neighbours which are also on the boundary
                    if (v.IsBoundary)
                    {
                        var he = v.First;
                        sum += he.GetDelta(vertexPositions) * halfedgeWeights[he.Index];
                        he = he.Previous.Twin;
                        sum += he.GetDelta(vertexPositions) * halfedgeWeights[he.Index];
                    }
                    else
                    {
                        foreach (var he in v.OutgoingHalfedges)
                            sum += he.GetDelta(vertexPositions) * halfedgeWeights[he.Index];
                    }

                    // add force vector
                    forceSums[i] += sum * strength;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), func);
            else
                func(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        /// <param name="boundaryType"></param>
        public static void LaplacianFair(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IList<Vec3d> forceSums, SmoothBoundaryType boundaryType = SmoothBoundaryType.Fixed)
        {
            _lapFair[(int)boundaryType](mesh, vertexPositions, strength, forceSums);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        private static void LaplacianFairFixed(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IList<Vec3d> forceSums)
        {
            var verts = mesh.Vertices;

            for (int i = 0; i < verts.Count; i++)
            {
                var v = verts[i];
                if (v.IsUnused || v.IsBoundary) continue;

                // get average position of neighbouring vertices
                Vec3d sum = new Vec3d();
                int n = 0;

                foreach (var cv in v.ConnectedVertices)
                {
                    sum += vertexPositions[cv.Index];
                    n++;
                }

                // compute force vector
                Vec3d frc = (sum / n - vertexPositions[i]) * strength;
                forceSums[i] += frc;

                // distribute opposite force amongst neighbours
                frc /= n;
                foreach (var vn in v.ConnectedVertices)
                    forceSums[vn.Index] -= frc;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        private static void LaplacianFairCornerFixed(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IList<Vec3d> forceSums)
        {
            var verts = mesh.Vertices;

            for (int i = 0; i < verts.Count; i++)
            {
                var v = verts[i];
                if (v.IsUnused) continue;

                // if boundary vertex, only consider neighbours which are also on the boundary
                if (v.IsBoundary)
                {
                    if (v.IsDegree2) continue; // skip corners

                    var he = v.First;
                    var vi0 = he.Twin.Start.Index;
                    var vi1 = he.Previous.Start.Index;

                    // compute force vector
                    Vec3d frc = ((vertexPositions[vi0] + vertexPositions[vi1]) * 0.5 - vertexPositions[i]) * strength;
                    forceSums[i] += frc;

                    // distribute opposite force amongst neighbours
                    frc *= 0.5;
                    forceSums[vi0] -= frc;
                    forceSums[vi1] -= frc;
                }
                else
                {
                    // get average position of neighbouring vertices
                    Vec3d sum = new Vec3d();
                    int n = 0;

                    foreach (var cv in v.ConnectedVertices)
                    {
                        sum += vertexPositions[cv.Index];
                        n++;
                    }

                    // compute force vector
                    Vec3d frc = (sum / n - vertexPositions[i]) * strength;
                    forceSums[i] += frc;

                    // distribute opposite force amongst neighbours
                    frc /= n;
                    foreach (var vn in v.ConnectedVertices)
                        forceSums[vn.Index] -= frc;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        private static void LaplacianFairFree(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IList<Vec3d> forceSums)
        {
            var verts = mesh.Vertices;

            for (int i = 0; i < verts.Count; i++)
            {
                var v = verts[i];
                if (v.IsUnused) continue;

                // if boundary vertex, only consider neighbours which are also on the boundary
                if (v.IsBoundary)
                {
                    var he = v.First;
                    var vi0 = he.Twin.Start.Index;
                    var vi1 = he.Previous.Index;

                    // compute force vector
                    Vec3d frc = ((vertexPositions[vi0] + vertexPositions[vi1]) * 0.5 - vertexPositions[i]) * strength;
                    forceSums[i] += frc;

                    // reverse and distribute amongst neighbours
                    frc *= 0.5;
                    forceSums[vi0] -= frc;
                    forceSums[vi1] -= frc;
                }
                else
                {
                    // get average position of neighbouring vertices
                    Vec3d sum = new Vec3d();
                    int n = 0;

                    foreach (var cv in v.ConnectedVertices)
                    {
                        sum += vertexPositions[cv.Index];
                        n++;
                    }

                    // compute force vector
                    Vec3d frc = (sum / n - vertexPositions[i]) * strength;
                    forceSums[i] += frc;

                    // distribute opposite force amongst neighbours
                    frc /= n;
                    foreach (var vn in v.ConnectedVertices)
                        forceSums[vn.Index] -= frc;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void MinimizeEdgeLengths(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IList<Vec3d> forceSums)
        {
            var hedges = mesh.Halfedges;

            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he = hedges[i];
                if (he.IsUnused) continue;

                Vec3d frc = he.GetDelta(vertexPositions) * strength;
                forceSums[he.Start.Index] += frc;
                forceSums[he.End.Index] -= frc;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="restLength"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainEdgeLengths(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IReadOnlyList<double> edgeLengths, double restLength, IList<Vec3d> forceSums)
        {
            var hedges = mesh.Halfedges;

            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he = hedges[i];
                if (he.IsUnused) continue;
         
                Vec3d frc = he.GetDelta(vertexPositions) * ((1.0 - restLength / edgeLengths[i >> 1]) * strength);
                forceSums[he.Start.Index] += frc;
                forceSums[he.End.Index] -= frc;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="restLengths"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainEdgeLengths(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IReadOnlyList<double> edgeLengths, IReadOnlyList<double> restLengths, IList<Vec3d> forceSums)
        {
            var hedges = mesh.Halfedges;

            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he = hedges[i];
                if (he.IsUnused) continue;

                Vec3d frc = he.GetDelta(vertexPositions) * ((1.0 - restLengths[i >> 1] / edgeLengths[i >> 1]) * strength);
                forceSums[he.Start.Index] += frc;
                forceSums[he.End.Index] -= frc;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainEdgeLengths(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IReadOnlyList<double> edgeLengths, double minLength, double maxLength, IList<Vec3d> forceSums)
        {
            var hedges = mesh.Halfedges;

            if (minLength > maxLength)
                throw new ArgumentException("The given minimum length cannot be greater than the given maximum length.");

            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he = hedges[i];
                if (he.IsUnused) continue;

                Vec3d frc = he.GetDelta(vertexPositions);
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
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="minLengths"></param>
        /// <param name="maxLengths"></param>
        /// <param name="forceSums"></param>
        public static void ConstrainEdgeLengths(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IReadOnlyList<double> edgeLengths, IReadOnlyList<double> minLengths, IReadOnlyList<double> maxLengths, IList<Vec3d> forceSums)
        {
            var hedges = mesh.Halfedges;

            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he = hedges[i];
                if (he.IsUnused) continue;

                int j = i >> 1;
                double minLength = minLengths[j];
                double maxLength = maxLengths[j];

                if (minLength > maxLength)
                    throw new ArgumentException("The given minimum length cannot be greater than the given maximum length.");

                Vec3d frc = he.GetDelta(vertexPositions);
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
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="forceSums"></param>
        public static void CirclePack(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IReadOnlyList<double> edgeLengths, IList<Vec3d> forceSums)
        {
            var hedges = mesh.Halfedges;

            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he = hedges[i];
                if (he.IsUnused || he.IsBoundary) continue; // skip unused, boundary, or edges

                // collect relevant edges
                var he0 = he.Next;
                var he1 = he.Previous;
                var he2 = he.Twin.Next;
                var he3 = he.Twin.Previous;
          
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
                Vec3d f0 = he0.GetDelta(vertexPositions) * (mag0 / d0);
                Vec3d f1 = he1.GetDelta(vertexPositions) * (mag1 / d1);
                Vec3d f2 = he2.GetDelta(vertexPositions) * (mag0 / d2);
                Vec3d f3 = he3.GetDelta(vertexPositions) * (mag1 / d3);

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
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="vertexRadii"></param>
        /// <param name="forceSums"></param>
        public static void CirclePack(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IReadOnlyList<double> edgeLengths, IReadOnlyList<double> vertexRadii, IList<Vec3d> forceSums)
        {
            var edges = mesh.Halfedges;

            for (int i = 0; i < edges.Count; i += 2)
            {
                var he = edges[i];
                if (he.IsUnused) continue; // skip unused edges

                // compute force vectors
                var vi0 = he.Start.Index;
                var vi1 = he.End.Index;

                double mag = edgeLengths[i >> 1];
                double rest = vertexRadii[vi0] + vertexRadii[vi1];

                Vec3d frc = vertexPositions[vi1] - vertexPositions[vi0];
                frc *= (mag - rest) * strength / mag;
                forceSums[vi0] += frc;
                forceSums[vi1] -= frc;
            }
        }


        /// <summary>
        /// Calculates forces which adjust edge lengths towards the average around their vertex.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="forceSums"></param>
        public static void EqualizeVertexEdgeLengths(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IReadOnlyList<double> edgeLengths, IList<Vec3d> forceSums)
        {
            var verts = mesh.Vertices;

            for (int i = 0; i < verts.Count; i++)
            {
                var v = verts[i];
                if (v.IsUnused) continue; // skip unused vertices

                // get average edge length around vertex
                double mean = 0.0;
                int n = 0;

                foreach (var he in v.OutgoingHalfedges)
                {
                    mean += edgeLengths[he.Index >> 1];
                    n++;
                }
                mean /= n;

                // compute force vectors
                foreach (var he in v.OutgoingHalfedges)
                {
                    var vi0 = he.Start.Index;
                    var vi1 = he.End.Index;

                    Vec3d frc = vertexPositions[vi1] - vertexPositions[vi0];
                    double mag = edgeLengths[he.Index >> 1];

                    frc *= (mag - mean) * strength / mag;
                    forceSums[vi0] += frc;
                    forceSums[vi1] -= frc;
                }
            }
        }


        /// <summary>
        ///  Calculates forces which adjust edge lengths towards the average within their face.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="forceSums"></param>
        public static void EqualizeFaceEdgeLengths(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IReadOnlyList<double> edgeLengths, IList<Vec3d> forceSums)
        {
            foreach (var f in mesh.Faces)
            {
                if (f.IsUnused) continue; // skip unused faces

                // get average edge length within the face
                double mean = 0.0;
                int n = 0;

                foreach (var he in f.Halfedges)
                {
                    mean += edgeLengths[he.Index >> 1];
                    n++;
                }
                mean /= n;

                // compute force vectors
                foreach (var he in f.Halfedges)
                {
                    var vi0 = he.Start.Index;
                    var vi1 = he.End.Index;

                    Vec3d frc = vertexPositions[vi1] - vertexPositions[vi0];
                    double mag = edgeLengths[he.Index >> 1];

                    frc *= (mag - mean) * strength / mag;
                    forceSums[vi0] += frc;
                    forceSums[vi1] -= frc;
                }
            }
        }


        /// <summary>
        /// Calculates forces which align the edges of a mesh with a given set of vectors.
        /// http://www.eecs.berkeley.edu/~sequin/CS285/PAPERS/Pottmann_FrFrm_arch.pdf
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="vectors"></param>
        /// <param name="forceSums"></param>
        public static void AlignEdges(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IReadOnlyList<Vec3d> vectors, IList<Vec3d> forceSums)
        {
            var hedges = mesh.Halfedges;
 
            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he = hedges[i];
                if (he.IsUnused) continue; // skip unused edges

                // project edge vector onto target vector and take difference
                Vec3d frc = he.GetDelta(vertexPositions);
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
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void PlanarizeFaces(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IList<Vec3d> forceSums)
        {
            foreach (var f in mesh.Faces)
            {
                if (f.IsUnused) continue;
                var he0 = f.First;
                var he1 = he0;

                var vi0 = he1.Start.Index; he1 = he1.Next;
                var vi1 = he1.Start.Index; he1 = he1.Next;
                var vi2 = he1.Start.Index; he1 = he1.Next;

                if (he1 == he0)
                {
                    // tri case
                    continue;
                }
                else if (he1.Next == he0)
                {
                    // quad case
                    var vi3 = he1.Start.Index;
                    Vec3d frc = GeometryUtil.LineLineShortestVector(
                        vertexPositions[vi0],
                        vertexPositions[vi2],
                        vertexPositions[vi1],
                        vertexPositions[vi3]) * strength;

                    forceSums[vi0] += frc;
                    forceSums[vi2] += frc;
                    forceSums[vi1] -= frc;
                    forceSums[vi3] -= frc;
                }
                else
                {
                    // ngon case
                    var he2 = he1;
 
                    do
                    {
                        var vi3 = he2.Start.Index;
                        Vec3d frc = GeometryUtil.LineLineShortestVector(
                            vertexPositions[vi0],
                            vertexPositions[vi2],
                            vertexPositions[vi1],
                            vertexPositions[vi3]) * strength;

                        forceSums[vi0] += frc;
                        forceSums[vi2] += frc;
                        forceSums[vi1] -= frc;
                        forceSums[vi3] -= frc;

                        // advance to next set of verts
                        vi0 = vi1; vi1 = vi2; vi2 = vi3;
                        he2 = he2.Next;
                    } while (he2 != he1);
                }
            }
        }


        /// <summary>
        /// Calculates forces which pull vertices within each face to a common plane.
        /// Note that this method ignores non-quad faces.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        public static void PlanarizeQuads(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IList<Vec3d> forceSums)
        {
            foreach (var f in mesh.Faces)
            {
                if (f.IsUnused) continue;
                var he0 = f.First;
                var he1 = he0;

                var vi0 = he1.Start.Index; he1 = he1.Next;
                var vi1 = he1.Start.Index; he1 = he1.Next;
                var vi2 = he1.Start.Index; he1 = he1.Next;
                if (he1.Next != he0) continue; // skip non quads

                var vi3 = he1.Start.Index;
                Vec3d frc = GeometryUtil.LineLineShortestVector(
                    vertexPositions[vi0],
                    vertexPositions[vi2],
                    vertexPositions[vi1],
                    vertexPositions[vi3]) * strength;

                forceSums[vi0] += frc;
                forceSums[vi2] += frc;
                forceSums[vi1] -= frc;
                forceSums[vi3] -= frc;
            }
        }


        /// <summary>
        /// Calculates forces which pull vertices within each face to a common circle.
        /// Note that this method is intended for use on quad meshes and should be used in conjunction with PlanarizeQuads.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="halfedgeAngles"></param>
        /// <param name="forceSums"></param>
        public static void Circularize(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IReadOnlyList<double> edgeLengths, IReadOnlyList<double> halfedgeAngles, IList<Vec3d> forceSums)
        {
            foreach (var f in mesh.Faces)
            {
                if (f.IsUnused) continue; // skip unused faces

                // circular rule - opposite angles in the quad must sum to pi
                // http://www.geometrie.tugraz.at/wallner/quad06.pdf
                // http://vecg.cs.ucl.ac.uk/Projects/SmartGeometry/constrained_mesh/paper_docs/constrainedMesh_sigA_11.pdf
                var he0 = f.First;
                var he1 = he0.Next;
                var he2 = he1.Next;
                var he3 = he2.Next;
                if (he3.Next != he0) continue; // skip non quads

                for (int i = 0; i < 2; i++)
                {
                    // calculate angle error
                    double err = (Math.PI - halfedgeAngles[he1.Index] - halfedgeAngles[he3.Index]) * 0.25; // angle error in range [-PI/4, PI/4]
                    double tan = Math.Tan(err) * strength;

                    // apply between he0 & he1
                    Vec3d d0 = he0.GetDelta(vertexPositions);
                    Vec3d d1 = he1.GetDelta(vertexPositions);

                    Vec3d f0 = Vec3d.Reject(d1, d0);
                    Vec3d f1 = Vec3d.Reject(d0, d1);

                    double m = tan * (edgeLengths[he0.Index >> 1] * edgeLengths[he1.Index >> 1]) * 0.5;
                    f0 *= m / f0.Length;
                    f1 *= m / f1.Length;

                    forceSums[he0.Start.Index] -= f0;
                    forceSums[he1.Start.Index] += f0 - f1;
                    forceSums[he2.Start.Index] += f1;

                    // apply between he2 & he3
                    d0 = he2.GetDelta(vertexPositions);
                    d1 = he3.GetDelta(vertexPositions);

                    f0 = Vec3d.Reject(d1, d0);
                    f1 = Vec3d.Reject(d0, d1);

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


        /// <summary>
        /// Calculates forces which adjust faces around each internal degree 4 vertex to be tangent to a common cone.
        /// Note that this method is intended for use on quad meshes and should be used in conjunction with PlanarizeQuads.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="halfedgeAngles"></param>
        /// <param name="forceSums"></param>
        public static void Conicalize(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IReadOnlyList<double> edgeLengths, IReadOnlyList<double> halfedgeAngles, IList<Vec3d> forceSums)
        {
            var verts = mesh.Vertices;

            for(int i = 0; i < verts.Count; i++)
            {
                var v = verts[i];
                if (v.IsUnused || v.IsBoundary) continue; // skip unused or boundary vertices

                // conical rule - 2 pairs of opposite angles must have an equal sum
                // http://www.geometrie.tugraz.at/wallner/quad06.pdf
                // http://vecg.cs.ucl.ac.uk/Projects/SmartGeometry/constrained_mesh/paper_docs/constrainedMesh_sigA_11.pdf
                var he0 = v.First;
                var he1 = he0.Twin.Next;
                var he2 = he1.Twin.Next;
                var he3 = he2.Twin.Next;
                if (he3.Twin.Next != he0) continue; // skip non degree 4 verts

                // get angle error
                double err = ((halfedgeAngles[he0.Index] + halfedgeAngles[he2.Index]) - (halfedgeAngles[he1.Index] + halfedgeAngles[he3.Index])) * 0.25; // angle error in range [-PI/4, PI/4]
                double tan = Math.Tan(err) * strength;

                // apply angle constraint to each quadrant
                he0 = he1.Twin;
                he1 = he0.Next;
                double t = 0.5;

                for (int j = 0; j < 4; j++)
                {
                    Vec3d d0 = he0.GetDelta(vertexPositions);
                    Vec3d d1 = he1.GetDelta(vertexPositions);

                    Vec3d f0 = Vec3d.Reject(d1, d0);
                    Vec3d f1 = Vec3d.Reject(d0, d1);

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


        /// <summary>
        /// Calculates forces which adjust edge lengths to make quad faces tangential.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="forceSums"></param>
        public static void Tangentialize(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IReadOnlyList<double> edgeLengths, IList<Vec3d> forceSums)
        {
            var faces = mesh.Faces;

            for (int i = 0; i < faces.Count; i++)
            {
                var f = faces[i];
                if (f.IsUnused) continue;

                // tangentail rule - the length sum of the two pairs of opposite sides is equal.
                // http://en.wikipedia.org/wiki/Tangential_quadrilateral
                var he0 = f.First;
                var he1 = he0.Next;
                var he2 = he1.Next;
                var he3 = he2.Next;
                if (he3.Next != he0) continue; // skip non quads

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
                Vec3d f0 = he0.GetDelta(vertexPositions) * (mag0 / d0);
                Vec3d f1 = he1.GetDelta(vertexPositions) * (mag1 / d1);
                Vec3d f2 = he2.GetDelta(vertexPositions) * (mag0 / d2);
                Vec3d f3 = he3.GetDelta(vertexPositions) * (mag1 / d3);

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
        /// <param name="vertexPositions"></param>
        /// <param name="strength"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="halfedgeAngles"></param>
        /// <param name="forceSums"></param>
        public static void Developablize(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, double strength, IReadOnlyList<double> edgeLengths, IReadOnlyList<double> halfedgeAngles, IList<Vec3d> forceSums)
        {
            foreach (var v in mesh.Vertices)
            {
                if (v.IsUnused || v.IsBoundary) continue; // skip unused or boudnary vertices

                // calculate angle error
                double sum = 0.0;
                foreach (var he in v.OutgoingHalfedges) 
                    sum += Math.PI - halfedgeAngles[he.Index];

                double err = (sum - SlurMath.TwoPI) * 0.125; // angle error in range [-PI/4, PI/4]
                double tan = Math.Tan(err) * strength;
                
                // apply angle constraint around vertex
                foreach (var he0 in v.OutgoingHalfedges)
                {
                    var he1 = he0.Next;

                    Vec3d d0 = he0.GetDelta(vertexPositions);
                    Vec3d d1 = he1.GetDelta(vertexPositions);

                    Vec3d f0 = Vec3d.Reject(d1, d0);
                    Vec3d f1 = Vec3d.Reject(d0, d1);

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
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="closestPoint"></param>
        /// <param name="strength"></param>
        /// <param name="forceSums"></param>
        /// <param name="parallel"></param>
        public static void ConstrainTo(HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, Func<Vec3d,Vec3d> closestPoint, double strength, IList<Vec3d> forceSums, bool parallel = false)
        {
            var verts = mesh.Vertices;
    
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsUnused) continue;

                    Vec3d p = vertexPositions[i];
                    forceSums[i] += (closestPoint(p) - p) * strength;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), func);
            else
                func(Tuple.Create(0, verts.Count));
        }
    }
}
