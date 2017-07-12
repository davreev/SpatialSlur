using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;
using SpatialSlur.SlurData;

using static SpatialSlur.SlurCore.ArrayMath;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public static class FieldSim
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="delta"></param>
        /// <param name="timeStep"></param>
        /// <param name="parallel"></param>
        public static void Update(IDiscreteField<double> field, IDiscreteField<double> delta, double timeStep, bool parallel = false)
        {
            Update(field, delta.Values, timeStep, parallel);
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="field"></param>
        /// <param name="deltas"></param>
        /// <param name="timeStep"></param>
        /// <param name="parallel"></param>
        public static void Update(IDiscreteField<double> field, double[] deltas, double timeStep, bool parallel = false)
        {
            var vals = field.Values;
            int n = field.Count;

            if (parallel)
                AddScaledParallel(vals, deltas, timeStep, n, vals);
            else
                AddScaled(vals, deltas, timeStep, n, vals);

            Array.Clear(deltas, 0, n);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="delta"></param>
        /// <param name="timeStep"></param>
        /// <param name="parallel"></param>
        public static void Update(IDiscreteField<Vec2d> field, IDiscreteField<Vec2d> delta, double timeStep, bool parallel = false)
        {
            Update(field, delta.Values, timeStep, parallel);
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="field"></param>
        /// <param name="deltas"></param>
        /// <param name="timeStep"></param>
        /// <param name="parallel"></param>
        public static void Update(IDiscreteField<Vec2d> field, Vec2d[] deltas, double timeStep, bool parallel = false)
        {
            var vals = field.Values;
            int n = field.Count;

            if (parallel)
                AddScaledParallel(vals, deltas, timeStep, n, vals);
            else
                AddScaled(vals, deltas, timeStep, n, vals);

            Array.Clear(deltas, 0, n);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="delta"></param>
        /// <param name="timeStep"></param>
        /// <param name="parallel"></param>
        public static void Update(IDiscreteField<Vec3d> field, IDiscreteField<Vec3d> delta, double timeStep, bool parallel = false)
        {
            Update(field, delta.Values, timeStep, parallel);
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="field"></param>
        /// <param name="deltas"></param>
        /// <param name="timeStep"></param>
        /// <param name="parallel"></param>
        public static void Update(IDiscreteField<Vec3d> field, Vec3d[] deltas, double timeStep, bool parallel = false)
        {
            var vals = field.Values;
            int n = field.Count;

            if (parallel)
                AddScaledParallel(vals, deltas, timeStep, n, vals);
            else
                AddScaled(vals, deltas, timeStep, n, vals);

            Array.Clear(deltas, 0, n);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="delta"></param>
        /// <param name="thresh"></param>
        /// <param name="rate"></param>
        /// <param name="parallel"></param>
        public static void Bifurcate(IDiscreteField<double> field, IDiscreteField<double> delta, double thresh, double rate, bool parallel = false)
        {
            Bifurcate(field, delta.Values, thresh, rate, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="deltas"></param>
        /// <param name="thresh"></param>
        /// <param name="rate"></param>
        /// <param name="parallel"></param>
        public static void Bifurcate(IDiscreteField<double> field, double[] deltas, double thresh, double rate, bool parallel = false)
        {
            var vals = field.Values;

            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    if (vals[i] > thresh)
                        deltas[i] += rate;
                    else if (vals[i] < thresh)
                        deltas[i] -= rate;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), func);
            else
                func(Tuple.Create(0, field.Count));
        }


        /// <summary>
        /// Adds the Laplacian of the field to the delta field.
        /// http://en.wikipedia.org/wiki/Discrete_Laplace_operator
        /// </summary>
        /// <param name="field"></param>
        /// <param name="delta"></param>
        /// <param name="rate"></param>
        /// <param name="parallel"></param>
        public static void Diffuse(GridScalarField2d field, GridScalarField2d delta, double rate, bool parallel = false)
        {
            Diffuse(field, delta.Values, rate, parallel);
        }


        /// <summary>
        /// Adds the Laplacian of the field to the deltas array.
        /// http://en.wikipedia.org/wiki/Discrete_Laplace_operator
        /// </summary>
        /// <param name="field"></param>
        /// <param name="deltas"></param>
        /// <param name="rate"></param>
        /// <param name="parallel"></param>
        public static void Diffuse(GridScalarField2d field, double[] deltas, double rate, bool parallel = false)
        {
            var vals = field.Values;
            int nx = field.CountX;
            int ny = field.CountY;

            double dx = 1.0 / (field.ScaleX * field.ScaleX);
            double dy = 1.0 / (field.ScaleY * field.ScaleY);
            
            (int di, int dj) = GridUtil.GetBoundaryOffsets(field);

            Action<Tuple<int, int>> func = range =>
            {
                (int i , int j) = field.IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }

                    double tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    double tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    double ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    double ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    double t = vals[index] * 2.0;
                    deltas[index] += ((tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy) * rate;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), func);
            else
                func(Tuple.Create(0, field.Count));
        }


        /// <summary>
        /// Adds the Laplacian of the field to the delta field.
        /// http://en.wikipedia.org/wiki/Discrete_Laplace_operator
        /// </summary>
        /// <param name="field"></param>
        /// <param name="delta"></param>
        /// <param name="rate"></param>
        /// <param name="parallel"></param>
        public static void Diffuse(GridScalarField3d field, GridScalarField3d delta, double rate, bool parallel = false)
        {
            Diffuse(field, delta.Values, rate, parallel);
        }


        /// <summary>
        /// Adds the Laplacian of the field to the deltas array.
        /// http://en.wikipedia.org/wiki/Discrete_Laplace_operator
        /// </summary>
        /// <param name="field"></param>
        /// <param name="deltas"></param>
        /// <param name="rate"></param>
        /// <param name="parallel"></param>
        public static void Diffuse(GridScalarField3d field, double[] deltas, double rate, bool parallel = false)
        {
            var vals = field.Values;
            int nx = field.CountX;
            int ny = field.CountY;
            int nz = field.CountZ;
            int nxy = field.CountXY;

            double dx = 1.0 / (field.ScaleX * field.ScaleX);
            double dy = 1.0 / (field.ScaleY * field.ScaleY);
            double dz = 1.0 / (field.ScaleZ * field.ScaleZ);

            (int di, int dj, int dk) = GridUtil.GetBoundaryOffsets(field);

            Action<Tuple<int, int>> func = range =>
            {
                (int i, int j, int k) = field.IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }
                    if (j == ny) { k++; j = 0; }

                    double tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    double tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    double ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    double ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    double tz0 = (k == 0) ? vals[index + dk] : vals[index - nxy];
                    double tz1 = (k == nz - 1) ? vals[index - dk] : vals[index + nxy];

                    double t = vals[index] * 2.0;
                    deltas[index] += ((tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy + (tz0 + tz1 - t) * dz) * rate;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), func);
            else
                func(Tuple.Create(0, field.Count));
        }


        /*
        /// <summary>
        /// Adds the Laplacian of the field to the delta field.
        /// http://en.wikipedia.org/wiki/Discrete_Laplace_operator
        /// </summary>
        /// <param name="field"></param>
        /// <param name="delta"></param>
        /// <param name="rate"></param>
        /// <param name="parallel"></param>
        public static void Diffuse(HeVertexScalarField field, HeVertexScalarField delta, double rate, bool parallel = false)
        {
            Diffuse(field, delta.Values, rate, parallel);
        }


        /// <summary>
        /// Adds the Laplacian of the field to the deltas array.
        /// http://en.wikipedia.org/wiki/Discrete_Laplace_operator
        /// </summary>
        /// <param name="field"></param>
        /// <param name="deltas"></param>
        /// <param name="rate"></param>
        /// <param name="parallel"></param>
        public static void Diffuse(HeVertexScalarField field, double[] deltas, double rate, bool parallel = false)
        {
            var vals = field.Values;
            var verts = field.Vertices;

            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    double sum = 0.0;
                    int n = 0;

                    foreach (var he in verts[i].IncomingHalfedges)
                    {
                        sum += vals[he.Start.Index];
                        n++;
                    }

                    deltas[i] += (sum / n - vals[i]) * rate;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), func);
            else
                func(Tuple.Create(0, field.Count));
        }


        /// <summary>
        /// Adds the Laplacian of the field to the delta field.
        /// http://en.wikipedia.org/wiki/Discrete_Laplace_operator
        /// </summary>
        /// <param name="field"></param>
        /// <param name="delta"></param>
        /// <param name="rate"></param>
        /// <param name="hedgeWeights"></param>
        /// <param name="parallel"></param>
        public static void Diffuse(HeVertexScalarField field, HeVertexScalarField delta, double rate, IReadOnlyList<double> hedgeWeights, bool parallel = false)
        {
            Diffuse(field, delta.Values, rate, hedgeWeights, parallel);
        }


        /// <summary>
        /// Adds the Laplacian of the field to the deltas array. 
        /// The Laplacian is calculated with a user-defined weighting scheme.
        /// http://www.cs.princeton.edu/courses/archive/fall10/cos526/papers/sorkine05.pdf
        /// http://www.igl.ethz.ch/projects/Laplacian-mesh-processing/Laplacian-mesh-optimization/lmo.pdf
        /// </summary>
        /// <param name="field"></param>
        /// <param name="deltas"></param>
        /// <param name="rate"></param>
        /// <param name="hedgeWeights"></param>
        /// <param name="parallel"></param>
        public static void Diffuse(HeVertexScalarField field, double[] deltas, double rate, IReadOnlyList<double> hedgeWeights, bool parallel = false)
        {
            var vals = field.Values;
            var verts = field.Vertices;

            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    double value = vals[i];
                    double sum = 0.0;

                    foreach (var he in verts[i].OutgoingHalfedges)
                        sum += (vals[he.End.Index] - value) * hedgeWeights[he.Index];

                    deltas[i] += sum * rate;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), func);
            else
                func(Tuple.Create(0, field.Count));
        }
        */


        /// <summary>
        /// http://micsymposium.org/mics_2011_proceedings/mics2011_submission_30.pdf
        /// </summary>
        /// <param name="field"></param>
        /// <param name="delta"></param>
        /// <param name="slope"></param>
        /// <param name="rate"></param>
        /// <param name="parallel"></param>
        public static void ErodeThermal(GridScalarField2d field, GridScalarField2d delta, double slope, double rate, bool parallel = false)
        {
            ErodeThermal(field, delta.Values, slope, rate, parallel);
        }


        /// <summary>
        /// http://micsymposium.org/mics_2011_proceedings/mics2011_submission_30.pdf
        /// </summary>
        /// <param name="field"></param>
        /// <param name="deltas"></param>
        /// <param name="slope"></param>
        /// <param name="rate"></param>
        /// <param name="parallel"></param>
        public static void ErodeThermal(GridScalarField2d field, double[] deltas, double slope, double rate, bool parallel = false)
        {
            var vals = field.Values;
            int nx = field.CountX;
            int ny = field.CountY;

            double dx = 1.0 / Math.Abs(field.ScaleX);
            double dy = 1.0 / Math.Abs(field.ScaleY);

            (int di, int dj) = GridUtil.GetBoundaryOffsets(field);

            Action<Tuple<int, int>> func = range =>
            {
                (int i, int j) = field.IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }

                    double value = vals[index];
                    double sum = 0.0;
                    double m, md;

                    //-x
                    m = ((i == 0) ? vals[index + di] : vals[index - 1]) - value;
                    m *= dx;
                    md = Math.Abs(m) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    //+x
                    m = ((i == nx - 1) ? vals[index - di] : vals[index + 1]) - value;
                    m *= dx;
                    md = Math.Abs(m) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    //-y
                    m = ((j == 0) ? vals[index + dj] : vals[index - nx]) - value;
                    m *= dy;
                    md = Math.Abs(m) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    //+y
                    m = ((j == ny - 1) ? vals[index - dj] : vals[index + nx]) - value;
                    m *= dy;
                    md = Math.Abs(m) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    deltas[index] += sum * rate;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), func);
            else
                func(Tuple.Create(0, field.Count));
        }


        /// <summary>
        /// http://micsymposium.org/mics_2011_proceedings/mics2011_submission_30.pdf
        /// </summary>
        /// <param name="field"></param>
        /// <param name="delta"></param>
        /// <param name="slope"></param>
        /// <param name="rate"></param>
        /// <param name="parallel"></param>
        public static void ErodeThermal(GridScalarField3d field, GridScalarField3d delta, double slope, double rate, bool parallel = false)
        {
            ErodeThermal(field, delta.Values, slope, rate, parallel);
        }


        /// <summary>
        /// http://micsymposium.org/mics_2011_proceedings/mics2011_submission_30.pdf
        /// </summary>
        /// <param name="field"></param>
        /// <param name="deltas"></param>
        /// <param name="slope"></param>
        /// <param name="rate"></param>
        /// <param name="parallel"></param>
        public static void ErodeThermal(GridScalarField3d field, double[] deltas, double slope, double rate, bool parallel = false)
        {
            var vals = field.Values;
            int nx = field.CountX;
            int ny = field.CountY;
            int nz = field.CountZ;
            int nxy = field.CountXY;

            double dx = 1.0 / Math.Abs(field.ScaleX);
            double dy = 1.0 / Math.Abs(field.ScaleY);
            double dz = 1.0 / Math.Abs(field.ScaleZ);

            (int di, int dj, int dk) = GridUtil.GetBoundaryOffsets(field);

            Action<Tuple<int, int>> func = range =>
            {
                (int i, int j, int k) = field.IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }
                    if (j == ny) { k++; j = 0; }

                    double value = vals[index];
                    double sum = 0.0;
                    double m, md;

                    //-x
                    m = ((i == 0) ? vals[index + di] : vals[index - 1]) - value;
                    md = Math.Abs(m * dx) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    //+x
                    m = ((i == nx - 1) ? vals[index - di] : vals[index + 1]) - value;
                    md = Math.Abs(m * dx) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    //-y
                    m = ((j == 0) ? vals[index + dj] : vals[index - nx]) - value;
                    md = Math.Abs(m * dy) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    //+y
                    m = ((j == ny - 1) ? vals[index - dj] : vals[index + nx]) - value;
                    md = Math.Abs(m * dy) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    //-z
                    m = (k == 0) ? vals[index + dk] - value : vals[index - nxy] - value;
                    md = Math.Abs(m * dz) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    //+z
                    m = ((k == nz - 1) ? vals[index - dk] : vals[index + nxy]) - value;
                    md = Math.Abs(m * dz) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    deltas[index] += sum * rate;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), func);
            else
                func(Tuple.Create(0, field.Count));
        }


        /// <summary>
        /// Calculates L1 (Manhattan) geodesic distance via Dijksta's algorithm as detailed in 
        /// http://www.numerical-tours.com/matlab/fastmarching_0_implementing/
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        public static void GeodesicDistanceL1(GridScalarField2d cost, IEnumerable<int> sources, GridScalarField2d result)
        {
            GeodesicDistanceL1(cost, sources, result.Values);
        }


        /// <summary>
        /// Calculates L1 (Manhattan) geodesic distance via Dijksta's algorithm as detailed in 
        /// http://www.numerical-tours.com/matlab/fastmarching_0_implementing/
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        public static void GeodesicDistanceL1(GridScalarField2d cost, IEnumerable<int> sources, double[] result)
        {
            // TODO consider wrap modes

            var costVals = cost.Values;
            int nx = cost.CountX;
            int ny = cost.CountY;
            double dx = Math.Abs(cost.ScaleX);
            double dy = Math.Abs(cost.ScaleY);

            var pq = new PriorityQueue<(double, int)>((a, b) => a.Item1.CompareTo(b.Item1));
            result.SetRange(double.PositiveInfinity, 0, cost.Count);

            // enqueue sources
            foreach (int i in sources)
            {
                result[i] = 0.0;
                pq.Insert((0.0, i));
            }

            // breadth first search from sources
            while (pq.Count > 0)
            {
                (double t0, int i0) = pq.RemoveMin();
                if (t0 > result[i0]) continue; // skip if node has already been processed

                (int x0, int y0) = cost.IndicesAt(i0);

                // x neighbours
                for (int j = -1; j < 2; j += 2)
                {
                    if (!SlurMath.Contains(x0 + j, 0, nx)) continue;

                    int i1 = i0 + j;
                    double t1 = t0 + costVals[i1] * dx;

                    if (t1 < result[i1])
                    {
                        result[i1] = t1;
                        pq.Insert((t1, i1));
                    }
                }

                // y neigbours
                for (int j = -1; j < 2; j += 2)
                {
                    if (!SlurMath.Contains(y0 + j, 0, ny)) continue;

                    int i1 = i0 + j * nx;
                    double t1 = t0 + costVals[i1] * dy;

                    if (t1 < result[i1])
                    {
                        result[i1] = t1;
                        pq.Insert((t1, i1));
                    }
                }
            }
        }


        /// <summary>
        /// Calculates L2 (Euclidean) geodesic distance via the Fast marching algorithm as detailed in 
        /// http://www.numerical-tours.com/matlab/fastmarching_0_implementing/
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        public static void GeodesicDistanceL2(GridScalarField2d cost, IEnumerable<int> sources, GridScalarField2d result)
        {
            var states = new bool[cost.Count];
            GeodesicDistanceL2Impl(cost, sources, states, result.Values);
        }


        /// <summary>
        /// Calculates L2 (Euclidean) geodesic distance via the Fast marching algorithm as detailed in 
        /// http://www.numerical-tours.com/matlab/fastmarching_0_implementing/
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static void GeodesicDistanceL2(GridScalarField2d cost, IEnumerable<int> sources, double[] result)
        {
            var tags = new bool[cost.Count];
            GeodesicDistanceL2Impl(cost, sources, tags, result);
        }


        /// <summary>
        /// Calculates L2 (Euclidean) geodesic distance via the Fast marching algorithm as detailed in 
        /// http://www.numerical-tours.com/matlab/fastmarching_0_implementing/
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="sources"></param>
        /// <param name="tags"></param>
        /// <param name="result"></param>
        public static void GeodesicDistanceL2(GridScalarField2d cost, IEnumerable<int> sources, bool[] tags, GridScalarField2d result)
        {
            tags.Clear();
            GeodesicDistanceL2Impl(cost, sources, tags, result.Values);
        }


        /// <summary>
        /// Calculates L2 (Euclidean) geodesic distance via the Fast marching algorithm as detailed in 
        /// http://www.numerical-tours.com/matlab/fastmarching_0_implementing/
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="sources"></param>
        /// <param name="tags"></param>
        /// <param name="result"></param>
        public static void GeodesicDistanceL2(GridScalarField2d cost, IEnumerable<int> sources, bool[] tags, double[] result)
        {
            tags.Clear();
            GeodesicDistanceL2Impl(cost, sources, tags, result);
        }


       /// <summary>
       /// 
       /// </summary>
       /// <param name="cost"></param>
       /// <param name="sources"></param>
       /// <param name="tags"></param>
       /// <param name="result"></param>
        private static void GeodesicDistanceL2Impl(GridScalarField2d cost, IEnumerable<int> sources, bool[] tags, double[] result)
        {
            // TODO consider wrap modes

            var costVals = cost.Values;
            var nx = cost.CountX;
            var ny = cost.CountY;
            var dx = Math.Abs(cost.ScaleX);
            var dy = Math.Abs(cost.ScaleY);

            var pq = new PriorityQueue<(double, int)>((a, b) => a.Item1.CompareTo(b.Item1));
            var eikonal = new Eikonal2d(dx, dy);

            result.SetRange(double.PositiveInfinity, 0, cost.Count);

            // enqueue sources
            foreach (int i in sources)
            {
                result[i] = 0.0;
                pq.Insert((0.0, i));
            }

            // breadth first search from sources
            while (pq.Count > 0)
            {
                (double t0, int i0) = pq.RemoveMin();
                if (tags[i0]) continue; // skip if already accepted
                tags[i0] = true;

                (int x0, int y0) = cost.IndicesAt(i0);

                // x neigbours
                for (int j = -1; j < 2; j += 2)
                {
                    if (!SlurMath.Contains(x0 + j, 0, nx)) continue;

                    int i1 = i0 + j;
                    if (!tags[i1])
                    {
                        double y = (y0 == 0) ? 
                          result[i1 + nx] : 
                          (y0 == ny - 1) ? result[i1 - nx] : 
                          Math.Min(result[i1 - nx], result[i1 + nx]);

                        double t1 = (y > double.MaxValue) ? 
                            t0 + dx * costVals[i1] :
                            eikonal.Evaluate(t0, y, costVals[i1]);

                        if (t1 < result[i1])
                        {
                            result[i1] = t1;
                            pq.Insert((t1, i1));
                        }
                    }
                }

                // y neigbours
                for (int j = -1; j < 2; j += 2)
                {
                    if (!SlurMath.Contains(y0 + j, 0, ny)) continue;

                    int i1 = i0 + j * nx;
                    if (!tags[i1])
                    {
                        double x = (x0 == 0) ? 
                          result[i1 + 1] :
                          (x0 == nx - 1) ? result[i1 - 1] :
                          Math.Min(result[i1 - 1], result[i1 + 1]);

                        double t1 = (x > double.MaxValue) ? 
                            t0 + dy * costVals[i1] :
                            eikonal.Evaluate(x, t0, costVals[i1]);

                        if (t1 < result[i1])
                        {
                            result[i1] = t1;
                            pq.Insert((t1, i1));
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private class Eikonal2d
        {
            private double _dx, _dy, _tx, _ty, _a, _a2Inv; // constants used during evaluation


            /// <summary>
            /// 
            /// </summary>
            /// <param name="dx"></param>
            /// <param name="dy"></param>
            public Eikonal2d(double dx, double dy)
            {
                _dx = dx;
                _dy = dy;

                _tx = 1.0 / (_dx * _dx);
                _ty = 1.0 / (_dy * _dy);
                _a = _tx + _ty;
                _a2Inv = 1.0 / (2.0 * _a);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="w"></param>
            /// <returns></returns>
            public double Evaluate(double x, double y, double w)
            {
                double b = -2.0 * (x * _tx + y * _ty);
                double c = x * x * _tx + y * y * _ty - w * w;
                double d = b * b - 4.0 * _a * c;

                if (d < 0.0)
                    return Math.Min(x + _dx * w, y + _dy * w);

                return (Math.Sqrt(d) - b) * _a2Inv;
            }
        }
    }
}
