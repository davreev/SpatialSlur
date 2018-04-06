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
    public static class GridField3dExtension
    {
        #region double
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static GridField3d<double> GetLaplacian(this GridField3d<double> field, bool parallel = false)
        {
            var result = GridField3d.Double.Create(field);
            GetLaplacian(field, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetLaplacian(this GridField3d<double> field, double[] result, bool parallel = false)
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
                int nxy = nx * ny;

                (var dx, var dy, var dz) = field.Scale;
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
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy + (tz0 + tz1 - t) * dz;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static GridField3d<Vec3d> GetGradient(this GridField3d<double> field, bool parallel = false)
        {
            var result = GridField3d.Vec3d.Create(field);
            GetGradient(field, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetGradient(this GridField3d<double> field, Vec3d[] result, bool parallel = false)
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
                int nxy = nx * ny;

                (var dx, var dy, var dz) = (0.5 / field.Scale);

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

                    result[index] = new Vec3d((tx1 - tx0) * dx, (ty1 - ty0) * dy, (tz1 - tz0) * dz);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="value"></param>
        public static void SetAt(this GridField3d<double> field, GridPoint3d point, double value)
        {
            FieldUtil.SetAt(field, point.Corners, point.Weights, value);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="amount"></param>
        public static void IncrementAt(this GridField3d<double> field, GridPoint3d point, double amount)
        {
            FieldUtil.IncrementAt(field, point.Corners, point.Weights, amount);
        }

        #endregion


        #region Vec3d
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static GridField3d<Vec3d> GetLaplacian(this GridField3d<Vec3d> field, bool parallel = false)
        {
            var result = GridField3d.Vec3d.Create(field);
            GetLaplacian(field, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetLaplacian(this GridField3d<Vec3d> field, Vec3d[] result, bool parallel = false)
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
                int nxy = nx * ny;

                (var dx, var dy, var dz) = field.Scale;
                dx = 1.0 / (dx * dx);
                dy = 1.0 / (dy * dy);
                dz = 1.0 / (dz * dz);

                (int di, int dj, int dk) = field.GetBoundaryOffsets();
                (int i, int j, int k) = field.IndicesAt(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }
                    if (j == ny) { k++; j = 0; }

                    Vec3d tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    Vec3d tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    Vec3d ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    Vec3d ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    Vec3d tz0 = (k == 0) ? vals[index + dk] : vals[index - nxy];
                    Vec3d tz1 = (k == nz - 1) ? vals[index - dk] : vals[index + nxy];

                    Vec3d t = vals[index] * 2.0;
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy + (tz0 + tz1 - t) * dz;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static GridField3d<double> GetDivergence(this GridField3d<Vec3d> field, bool parallel = false)
        {
            var result = GridField3d.Double.Create(field);
            GetDivergence(field, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetDivergence(this GridField3d<Vec3d> field, double[] result, bool parallel = false)
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
                int nz = field.CountZ;
                int nxy = nx * ny;

                (var dx, var dy, var dz) = (0.5 / field.Scale);

                (int di, int dj, int dk) = field.GetBoundaryOffsets();
                (int i, int j, int k) = field.IndicesAt(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }
                    if (j == ny) { k++; j = 0; }

                    Vec3d tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    Vec3d tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    Vec3d ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    Vec3d ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    Vec3d tz0 = (k == 0) ? vals[index + dk] : vals[index - nxy];
                    Vec3d tz1 = (k == nz - 1) ? vals[index - dk] : vals[index + nxy];

                    result[index] = (tx1.X - tx0.X) * dx + (ty1.Y - ty0.Y) * dy + (tz1.Z + tz0.Z) * dz;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static GridField3d<Vec3d> GetCurl(this GridField3d<Vec3d> field, bool parallel = false)
        {
            var result = GridField3d.Vec3d.Create(field);
            GetCurl(field, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetCurl(this GridField3d<Vec3d> field, Vec3d[] result, bool parallel = false)
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
                int nz = field.CountZ;
                int nxy = nx * ny;

                (var dx, var dy, var dz) = (0.5 / field.Scale);

                (int di, int dj, int dk) = field.GetBoundaryOffsets();
                (int i, int j, int k) = field.IndicesAt(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }
                    if (j == ny) { k++; j = 0; }

                    Vec3d tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    Vec3d tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    Vec3d ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    Vec3d ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    Vec3d tz0 = (k == 0) ? vals[index + dk] : vals[index - nxy];
                    Vec3d tz1 = (k == nz - 1) ? vals[index - dk] : vals[index + nxy];

                    result[index] = new Vec3d(
                        (ty1.Z - ty0.Z) * dy - (tz1.Y - tz0.Y) * dz,
                        (tz1.X - tz0.X) * dz - (tx1.Z - tx0.Z) * dx,
                        (tx1.Y - tx0.Y) * dx - (ty1.X - ty0.X) * dy);
                }
            }
        }
        

        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void SetAt(this GridField3d<Vec3d> field, GridPoint3d point, Vec3d value)
        {
            FieldUtil.SetAt(field, point.Corners, point.Weights, value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="amount"></param>
        public static void IncrementAt(this GridField3d<Vec3d> field, GridPoint3d point, Vec3d amount)
        {
            FieldUtil.IncrementAt(field, point.Corners, point.Weights, amount);
        }

        #endregion
    }
}
