
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;

using SpatialSlur.Collections;

namespace SpatialSlur.Fields
{
    using Kernel2d = ReadOnlyArrayView<(Vector2i Offset, double Weight)>;
    using Kernel3d = ReadOnlyArrayView<(Vector3i Offset, double Weight)>;


    /// <summary>
    /// 
    /// </summary>
    public static partial class Filter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="kernel"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void Convolve(GridField2d<double> field, Kernel2d kernel, double[] result, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.CountXY), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.CountXY);

            void Body(int from, int to)
            {
                var vals = field.Values;
                int nx = field.CountX;
                var k = kernel;

                var p = field.ToGridSpace(from);

                for (int i = from; i < to; i++)
                {
                    if (p.X == nx) { p.Y++; p.X = 0; }

                    var sum = 0.0;
                    
                    foreach((var d, var w) in k)
                        sum += vals[field.ToIndex(p + d)] * w;

                    result[i] = sum;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="kernel"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void Convolve(GridField2d<Vector2d> field, Kernel2d kernel, Vector2d[] result, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.CountXY), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.CountXY);

            void Body(int from, int to)
            {
                var vals = field.Values;
                int nx = field.CountX;
                var k = kernel;

                var p = field.ToGridSpace(from);

                for (int i = from; i < to; i++)
                {
                    if (p.X == nx) { p.Y++; p.X = 0; }

                    var sum = Vector2d.Zero;

                    foreach ((var d, var w) in k)
                        sum += vals[field.ToIndex(p + d)] * w;

                    result[i] = sum;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="kernel"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void Convolve(GridField3d<double> field, Kernel3d kernel, double[] result, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.CountXYZ), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.CountXYZ);

            void Body(int from, int to)
            {
                var vals = field.Values;
                int nx = field.CountX;
                int ny = field.CountY;
                var k = kernel;

                var p = field.ToGridSpace(from);

                for (int i = from; i < to; i++)
                {
                    if (p.X == nx) { p.Y++; p.X = 0; }
                    if (p.Y == ny) { p.Z++; p.Y = 0; }

                    var sum = 0.0;

                    foreach ((var d, var w) in k)
                        sum += vals[field.ToIndex(p + d)] * w;

                    result[i] = sum;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="kernel"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void Convolve(GridField3d<Vector3d> field, Kernel3d kernel, Vector3d[] result, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.CountXYZ), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.CountXYZ);

            void Body(int from, int to)
            {
                var vals = field.Values;
                int nx = field.CountX;
                int ny = field.CountY;
                var k = kernel;

                var p = field.ToGridSpace(from);

                for (int i = from; i < to; i++)
                {
                    if (p.X == nx) { p.Y++; p.X = 0; }
                    if (p.Y == ny) { p.Z++; p.Y = 0; }

                    var sum = Vector3d.Zero;

                    foreach ((var d, var w) in k)
                        sum += vals[field.ToIndex(p + d)] * w;

                    result[i] = sum;
                }
            }
        }


        /// <summary>
        /// Adds the Laplacian of the field to the deltas array.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="deltas"></param>
        /// <param name="rate"></param>
        /// <param name="parallel"></param>
        public static void Diffuse(GridField2d<double> field, double[] deltas, double rate, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.CountXY), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.CountXY);

            void Body(int from, int to)
            {
                var vals = field.Values;
                (var nx, var ny) = field.Count;
                (var dx, var dy) = field.Scale;
                dx = 1.0 / (dx * dx);
                dy = 1.0 / (dy * dy);

                (int di, int dj) = field.GetBoundaryOffsets();
                (int i, int j) = field.ToGridSpace(from);

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
        /// </summary>
        /// <param name="field"></param>
        /// <param name="deltas"></param>
        /// <param name="rate"></param>
        /// <param name="parallel"></param>
        public static void Diffuse(GridField3d<double> field, double[] deltas, double rate, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.CountXYZ), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.CountXYZ);

            void Body(int from, int to)
            {
                var vals = field.Values;
                (var nx, var ny, var nz) = field.Count;
                int nxy = field.CountXY;

                (var dx, var dy, var dz) = field.Scale;
                dx = 1.0 / (dx * dx);
                dy = 1.0 / (dy * dy);
                dz = 1.0 / (dz * dz);

                (int di, int dj, int dk) = field.GetBoundaryOffsets();
                (int i, int j, int k) = field.ToGridSpace(from);

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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="deltas"></param>
        /// <param name="slope"></param>
        /// <param name="rate"></param>
        /// <param name="parallel"></param>
        public static void ErodeThermal(GridField2d<double> field, double[] deltas, double slope, double rate, bool parallel = false)
        {
            // impl ref
            // http://micsymposium.org/mics_2011_proceedings/mics2011_submission_30.pdf

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.CountXY), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.CountXY);

            void Body(int from, int to)
            {
                var vals = field.Values;
                (var nx, var ny) = field.Count;
                (var dx, var dy) = field.Scale;
                dx = 1.0 / Math.Abs(dx);
                dy = 1.0 / Math.Abs(dy);

                (int di, int dj) = field.GetBoundaryOffsets();
                (int i, int j) = field.ToGridSpace(from);

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
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="deltas"></param>
        /// <param name="slope"></param>
        /// <param name="rate"></param>
        /// <param name="parallel"></param>
        public static void ErodeThermal(GridField3d<double> field, double[] deltas, double slope, double rate, bool parallel = false)
        {
            // impl ref
            // http://micsymposium.org/mics_2011_proceedings/mics2011_submission_30.pdf

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.CountXYZ), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.CountXYZ);

            void Body(int from, int to)
            {
                var vals = field.Values;
                (var nx, var ny, var nz) = field.Count;
                int nxy = field.CountXY;

                (var dx, var dy, var dz) = field.Scale;
                dx = 1.0 / Math.Abs(dx);
                dy = 1.0 / Math.Abs(dy);
                dz = 1.0 / Math.Abs(dz);

                (int di, int dj, int dk) = field.GetBoundaryOffsets();
                (int i, int j, int k) = field.ToGridSpace(from);

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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="deltas"></param>
        /// <param name="thresh"></param>
        /// <param name="rate"></param>
        /// <param name="parallel"></param>
        public static void Bifurcate(ISampledField<double> field, double[] deltas, double thresh, double rate, bool parallel = false)
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
    }
}
