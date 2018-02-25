using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurData;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// Contains general purpose algorithms for field processing and related.
    /// </summary>
    public static class SimulationUtil
    {
        /// <summary>
        /// Returns a streamline through the given field as an infinite sequence of points.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="point"></param>
        /// <param name="stepSize"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static IEnumerable<Vec2d> IntegrateFrom(IField2d<Vec2d> field, Vec2d point, double stepSize, IntegrationMode mode = IntegrationMode.Euler)
        {
            switch (mode)
            {
                case IntegrationMode.Euler:
                    return IntegrateFromEuler(field, point, stepSize);
                case IntegrationMode.RK2:
                    return IntegrateFromRK2(field, point, stepSize);
                case IntegrationMode.RK4:
                    return IntegrateFromRK4(field, point, stepSize);
            }

            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        private static IEnumerable<Vec2d> IntegrateFromEuler(IField2d<Vec2d> field, Vec2d point, double stepSize)
        {
            while (true)
            {
                point += field.ValueAt(point) * stepSize;
                yield return point;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static IEnumerable<Vec2d> IntegrateFromRK2(IField2d<Vec2d> field, Vec2d point, double stepSize)
        {
            while (true)
            {
                var v0 = field.ValueAt(point);
                var v1 = field.ValueAt(point + v0 * stepSize);

                point += (v0 + v1) * 0.5 * stepSize;
                yield return point;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static IEnumerable<Vec2d> IntegrateFromRK4(IField2d<Vec2d> field, Vec2d point, double stepSize)
        {
            double dt2 = stepSize * 0.5;
            double dt6 = stepSize / 6.0;

            while (true)
            {
                var v0 = field.ValueAt(point);
                var v1 = field.ValueAt(point + v0 * dt2);
                var v2 = field.ValueAt(point + v1 * dt2);
                var v3 = field.ValueAt(point + v2 * stepSize);

                point += (v0 + 2.0 * v1 + 2.0 * v2 + v3) * dt6;
                yield return point;
            }
        }


        /// <summary>
        /// Returns a streamline through the given field as an infinite sequence of points.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="point"></param>
        /// <param name="stepSize"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static IEnumerable<Vec3d> IntegrateFrom(IField3d<Vec3d> field, Vec3d point, double stepSize, IntegrationMode mode = IntegrationMode.Euler)
        {
            switch (mode)
            {
                case IntegrationMode.Euler:
                    return IntegrateFromEuler(field, point, stepSize);
                case IntegrationMode.RK2:
                    return IntegrateFromRK2(field, point, stepSize);
                case IntegrationMode.RK4:
                    return IntegrateFromRK4(field, point, stepSize);
            }

            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        private static IEnumerable<Vec3d> IntegrateFromEuler(IField3d<Vec3d> field, Vec3d point, double stepSize)
        {
            yield return point;

            while(true)
            {
                point += field.ValueAt(point) * stepSize;
                yield return point;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static IEnumerable<Vec3d> IntegrateFromRK2(IField3d<Vec3d> field, Vec3d point, double stepSize)
        {
            yield return point;

            while (true)
            {
                var v0 = field.ValueAt(point);
                var v1 = field.ValueAt(point + v0 * stepSize);

                point += (v0 + v1) * 0.5 * stepSize;
                yield return point;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static IEnumerable<Vec3d> IntegrateFromRK4(IField3d<Vec3d> field, Vec3d point, double stepSize)
        {
            double dt2 = stepSize * 0.5;
            double dt6 = stepSize / 6.0;

            yield return point;

            while (true)
            {
                var v0 = field.ValueAt(point);
                var v1 = field.ValueAt(point + v0 * dt2);
                var v2 = field.ValueAt(point + v1 * dt2);
                var v3 = field.ValueAt(point + v2 * stepSize);
                
                point += (v0 + 2.0 * v1 + 2.0 * v2 + v3) * dt6;
                yield return point;
            }
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
                ArrayMath.Parallel.AddScaled(vals, deltas, timeStep, n, vals);
            else
                ArrayMath.AddScaled(vals, deltas, timeStep, n, vals);

            Array.Clear(deltas, 0, n);
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
                ArrayMath.Parallel.AddScaled(vals, deltas, timeStep, n, vals);
            else
                ArrayMath.AddScaled(vals, deltas, timeStep, n, vals);

            Array.Clear(deltas, 0, n);
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
                ArrayMath.Parallel.AddScaled(vals, deltas, timeStep, n, vals);
            else
                ArrayMath.AddScaled(vals, deltas, timeStep, n, vals);

            Array.Clear(deltas, 0, n);
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
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.Count);
            
            void Body(int from, int to)
            {
                var vals = field.Values;

                for (int i = from; i < to; i++)
                {
                    if (vals[i] > thresh)
                        deltas[i] += rate;
                    else if (vals[i] < thresh)
                        deltas[i] -= rate;
                }
            }
        }

        
        /// <summary>
        /// Adds the Laplacian of the field to the deltas array.
        /// http://en.wikipedia.org/wiki/Discrete_Laplace_operator
        /// </summary>
        /// <param name="field"></param>
        /// <param name="deltas"></param>
        /// <param name="rate"></param>
        /// <param name="parallel"></param>
        public static void Diffuse(GridField2d<double> field, double[] deltas, double rate, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), range => Body(range.Item1,range.Item2));
            else
                Body(0, field.Count);
            
            void Body(int from, int to)
            {
                var vals = field.Values;
                int nx = field.CountX;
                int ny = field.CountY;

                (double dx, double dy) = field.Scale;
                dx = 1.0 / (dx * dx);
                dy = 1.0 / (dy * dy);

                (int di, int dj) = field.GetBoundaryOffsets();
                (int i, int j) = field.IndicesAt(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }

                    double tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    double tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    double ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    double ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    double t = vals[index] * 2.0;
                    deltas[index] += ((tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy) * rate;
                }
            }
        }


        /// <summary>
        /// Adds the Laplacian of the field to the deltas array.
        /// http://en.wikipedia.org/wiki/Discrete_Laplace_operator
        /// </summary>
        /// <param name="field"></param>
        /// <param name="deltas"></param>
        /// <param name="rate"></param>
        /// <param name="parallel"></param>
        public static void Diffuse(GridField3d<double> field, double[] deltas, double rate, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), range => Body(range.Item1,range.Item2));
            else
                Body(0, field.Count);

            void Body(int from, int to)
            {
                var vals = field.Values;
                int nx = field.CountX;
                int ny = field.CountY;
                int nz = field.CountZ;
                int nxy = field.CountXY;

                (double dx, double dy, double dz) = field.Scale;
                dx = 1.0 / (dx * dx);
                dy = 1.0 / (dy * dy);
                dz = 1.0 / (dz * dz);

                (int di, int dj, int dk) = field.GetBoundaryOffsets();
                (int i, int j, int k) = field.IndicesAt(from);

                for (int index = from; index < to; index++, i++)
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
            }
        }


        /*
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
        /// <param name="deltas"></param>
        /// <param name="slope"></param>
        /// <param name="rate"></param>
        /// <param name="parallel"></param>
        public static void ErodeThermal(GridField2d<double> field, double[] deltas, double slope, double rate, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), range => Body(range.Item1,range.Item2));
            else
                Body(0, field.Count);

            void Body(int from, int to)
            {
                var vals = field.Values;
                int nx = field.CountX;
                int ny = field.CountY;

                (double dx, double dy) = field.Scale;
                dx = 1.0 / Math.Abs(dx);
                dy = 1.0 / Math.Abs(dy);

                (int di, int dj) = field.GetBoundaryOffsets();
                (int i, int j) = field.IndicesAt(from);

                for (int index = from; index < to; index++, i++)
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
            }
        }


        /// <summary>
        /// http://micsymposium.org/mics_2011_proceedings/mics2011_submission_30.pdf
        /// </summary>
        /// <param name="field"></param>
        /// <param name="deltas"></param>
        /// <param name="slope"></param>
        /// <param name="rate"></param>
        /// <param name="parallel"></param>
        public static void ErodeThermal(GridField3d<double> field, double[] deltas, double slope, double rate, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.Count);

            void Body(int from, int to)
            {
                var vals = field.Values;
                int nx = field.CountX;
                int ny = field.CountY;
                int nz = field.CountZ;
                int nxy = field.CountXY;

                (double dx, double dy, double dz) = field.Scale;
                dx = 1.0 / Math.Abs(dx);
                dy = 1.0 / Math.Abs(dy);
                dz = 1.0 / Math.Abs(dz);

                (int di, int dj, int dk) = field.GetBoundaryOffsets();
                (int i, int j, int k) = field.IndicesAt(from);

                for (int index = from; index < to; index++, i++)
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
            }
        }
    }
}
