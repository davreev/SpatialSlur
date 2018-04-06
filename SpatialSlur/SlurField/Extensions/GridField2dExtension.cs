using System.Collections.Concurrent;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public static class GridField2dExtension
    {
        #region double

        /// <summary>
        /// 
        /// </summary>
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
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetLaplacian(this GridField2d<double> field, double[] result, bool parallel = false)
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

                (var dx, var dy) = field.Scale;
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
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static GridField2d<Vec2d> GetGradient(this GridField2d<double> field, bool parallel = false)
        {
            var result = GridField2d.Vec2d.Create(field);
            GetGradient(field, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetGradient(this GridField2d<double> field, Vec2d[] result, bool parallel = false)
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

                (var dx, var dy) = (0.5 / field.Scale);
                (int di, int dj) = field.GetBoundaryOffsets();
                (int i, int j) = field.IndicesAt(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }

                    double tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    double tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    double ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    double ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    result[index] = new Vec2d((tx1 - tx0) * dx, (ty1 - ty0) * dy);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="value"></param>
        public static void SetAt(this GridField2d<double> field, GridPoint2d point, double value)
        {
            FieldUtil.SetAt(field, point.Corners, point.Weights, value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="amount"></param>
        public static void IncrementAt(this GridField2d<double> field, GridPoint2d point, double amount)
        {
            FieldUtil.IncrementAt(field, point.Corners, point.Weights, amount);
        }

        #endregion


        #region Vec2d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static GridField2d<Vec2d> GetLaplacian(this GridField2d<Vec2d> field, bool parallel = false)
        {
            var result = GridField2d.Vec2d.Create(field);
            GetLaplacian(field, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetLaplacian(this GridField2d<Vec2d> field, Vec2d[] result, bool parallel = false)
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

                (var dx, var dy) = field.Scale;
                dx = 1.0 / (dx * dx);
                dy = 1.0 / (dy * dy);

                (int di, int dj) = field.GetBoundaryOffsets();
                (int i, int j) = field.IndicesAt(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }

                    Vec2d tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    Vec2d tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    Vec2d ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    Vec2d ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    Vec2d t = vals[index] * 2.0;
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static GridField2d<double> GetDivergence(this GridField2d<Vec2d> field, bool parallel = false)
        {
            var result = GridField2d.Double.Create(field);
            GetDivergence(field, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetDivergence(this GridField2d<Vec2d> field, double[] result, bool parallel = false)
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

                (var dx, var dy) = (0.5 / field.Scale);
                (int di, int dj) = field.GetBoundaryOffsets();
                (int i, int j) = field.IndicesAt(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }

                    Vec2d tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    Vec2d tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    Vec2d ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    Vec2d ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    result[index] = (tx1.X - tx0.X) * dx + (ty1.Y - ty0.Y) * dy;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static GridField2d<double> GetCurl(this GridField2d<Vec2d> field, bool parallel = false)
        {
            var result = GridField2d.Double.Create(field);
            GetCurl(field, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetCurl(this GridField2d<Vec2d> field, double[] result, bool parallel = false)
        {
            // implementation reference
            // http://www.math.harvard.edu/archive/21a_spring_09/PDF/13-05-curl-and-divergence.pdf

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.Count);

            void Body(int from, int to)
            {
                var vals = field.Values;
                int nx = field.CountX;
                int ny = field.CountY;

                (var tx, var ty) = (0.5 / field.Scale);
                (int di, int dj) = field.GetBoundaryOffsets();
                (int i, int j) = field.IndicesAt(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }

                    Vec2d tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    Vec2d tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    Vec2d ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    Vec2d ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    result[index] = (tx1.Y - tx0.Y) * tx - (ty1.X - ty0.X) * ty;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="value"></param>
        public static void SetAt(this GridField2d<Vec2d> field, GridPoint2d point, Vec2d value)
        {
            FieldUtil.SetAt(field, point.Corners, point.Weights, value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="amount"></param>
        public static void IncrementAt(this GridField2d<Vec2d> field, GridPoint2d point, Vec2d amount)
        {
            FieldUtil.IncrementAt(field, point.Corners, point.Weights, amount);
        }

        #endregion
    }
}
