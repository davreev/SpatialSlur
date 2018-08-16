
/*
 * Notes
 */
 
using System.Collections.Concurrent;
using System.Threading.Tasks;

using SpatialSlur;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    public static class GridField2dExtensions
    {
        #region double

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static GridField2d<double> GetLaplacian(this GridField2d<double> field, bool parallel = false)
        {
            var result = GridField2d.Double.Create(field);
            GetLaplacian(field, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetLaplacian(this GridField2d<double> field, double[] result, bool parallel = false)
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
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static GridField2d<Vector2d> GetGradient(this GridField2d<double> field, bool parallel = false)
        {
            var result = GridField2d.Vector2d.Create(field);
            GetGradient(field, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetGradient(this GridField2d<double> field, Vector2d[] result, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.CountXY), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.CountXY);

            void Body(int from, int to)
            {
                var vals = field.Values;
                (var nx, var ny) = field.Count;
                (var dx, var dy) = (0.5 / field.Scale);
                (int di, int dj) = field.GetBoundaryOffsets();
                (int i, int j) = field.ToGridSpace(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }

                    double tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    double tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    double ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    double ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    result[index] = new Vector2d((tx1 - tx0) * dx, (ty1 - ty0) * dy);
                }
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="point"></param>
        /// <param name="amount"></param>
        public static void IncrementAt(this GridField2d<double> field, ref GridPoint2d point, double amount)
        {
            var vals = field.Values;
            vals[point.Index0] += amount * point.Weight0;
            vals[point.Index1] += amount * point.Weight1;
            vals[point.Index2] += amount * point.Weight2;
            vals[point.Index3] += amount * point.Weight3;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="point"></param>
        /// <param name="value"></param>
        public static void SetAt(this GridField2d<double> field, ref GridPoint2d point, double value)
        {
            var vals = field.Values;
            vals[point.Index0] += (value - vals[point.Index0]) * point.Weight0;
            vals[point.Index1] += (value - vals[point.Index1]) * point.Weight1;
            vals[point.Index2] += (value - vals[point.Index2]) * point.Weight2;
            vals[point.Index3] += (value - vals[point.Index3]) * point.Weight3;
        }

        #endregion


        #region Vec2d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static GridField2d<Vector2d> GetLaplacian(this GridField2d<Vector2d> field, bool parallel = false)
        {
            var result = GridField2d.Vector2d.Create(field);
            GetLaplacian(field, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetLaplacian(this GridField2d<Vector2d> field, Vector2d[] result, bool parallel = false)
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

                    Vector2d tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    Vector2d tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    Vector2d ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    Vector2d ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    Vector2d t = vals[index] * 2.0;
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static GridField2d<double> GetDivergence(this GridField2d<Vector2d> field, bool parallel = false)
        {
            var result = GridField2d.Double.Create(field);
            GetDivergence(field, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetDivergence(this GridField2d<Vector2d> field, double[] result, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.CountXY), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.CountXY);

            void Body(int from, int to)
            {
                var vals = field.Values;
                (var nx, var ny) = field.Count;
                (var dx, var dy) = (0.5 / field.Scale);
                (int di, int dj) = field.GetBoundaryOffsets();
                (int i, int j) = field.ToGridSpace(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }

                    Vector2d tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    Vector2d tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    Vector2d ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    Vector2d ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    result[index] = (tx1.X - tx0.X) * dx + (ty1.Y - ty0.Y) * dy;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static GridField2d<double> GetCurl(this GridField2d<Vector2d> field, bool parallel = false)
        {
            var result = GridField2d.Double.Create(field);
            GetCurl(field, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetCurl(this GridField2d<Vector2d> field, double[] result, bool parallel = false)
        {
            // implementation reference
            // http://www.math.harvard.edu/archive/21a_spring_09/PDF/13-05-curl-and-divergence.pdf

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.CountXY), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.CountXY);

            void Body(int from, int to)
            {
                var vals = field.Values;
                (var nx, var ny) = field.Count;
                (var tx, var ty) = (0.5 / field.Scale);
                (int di, int dj) = field.GetBoundaryOffsets();
                (int i, int j) = field.ToGridSpace(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }

                    Vector2d tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    Vector2d tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    Vector2d ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    Vector2d ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    result[index] = (tx1.Y - tx0.Y) * tx - (ty1.X - ty0.X) * ty;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="point"></param>
        /// <param name="amount"></param>
        public static void IncrementAt(this GridField2d<Vector2d> field, ref GridPoint2d point, Vector2d amount)
        {
            var vals = field.Values;
            vals[point.Index0] += amount * point.Weight0;
            vals[point.Index1] += amount * point.Weight1;
            vals[point.Index2] += amount * point.Weight2;
            vals[point.Index3] += amount * point.Weight3;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="point"></param>
        /// <param name="value"></param>
        public static void SetAt(this GridField2d<Vector2d> field, ref GridPoint2d point, Vector2d value)
        {
            var vals = field.Values;
            vals[point.Index0] += (value - vals[point.Index0]) * point.Weight0;
            vals[point.Index1] += (value - vals[point.Index1]) * point.Weight1;
            vals[point.Index2] += (value - vals[point.Index2]) * point.Weight2;
            vals[point.Index3] += (value - vals[point.Index3]) * point.Weight3;
        }

        #endregion
    }
}
