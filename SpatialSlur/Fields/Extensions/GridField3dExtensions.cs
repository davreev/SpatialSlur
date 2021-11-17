
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
    public static class GridField3dExtensions
    {
        #region double
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
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
        /// <param name="field"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetLaplacian(this GridField3d<double> field, double[] result, bool parallel = false)
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
                (int i, int j, int k) = field.IndexToGrid(from);

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
        /// <param name="field"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static GridField3d<Vector3d> GetGradient(this GridField3d<double> field, bool parallel = false)
        {
            var result = GridField3d.Vector3d.Create(field);
            GetGradient(field, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetGradient(this GridField3d<double> field, Vector3d[] result, bool parallel = false)
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

                (var dx, var dy, var dz) = (0.5 / field.Scale);

                (int di, int dj, int dk) = field.GetBoundaryOffsets();
                (int i, int j, int k) = field.IndexToGrid(from);

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

                    result[index] = new Vector3d((tx1 - tx0) * dx, (ty1 - ty0) * dy, (tz1 - tz0) * dz);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="point"></param>
        /// <param name="amount"></param>
        public static void IncrementAt(this GridField3d<double> field, ref GridPoint3d point, double amount)
        {
            var vals = field.Values;

            vals[point.Index0] += amount * point.Weight0;
            vals[point.Index1] += amount * point.Weight1;
            vals[point.Index2] += amount * point.Weight2;
            vals[point.Index3] += amount * point.Weight3;

            vals[point.Index4] += amount * point.Weight4;
            vals[point.Index5] += amount * point.Weight5;
            vals[point.Index6] += amount * point.Weight6;
            vals[point.Index7] += amount * point.Weight7;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="point"></param>
        /// <param name="value"></param>
        public static void SetAt(this GridField3d<double> field, ref GridPoint3d point, double value)
        {
            var vals = field.Values;

            vals[point.Index0] += (value - vals[point.Index0]) * point.Weight0;
            vals[point.Index1] += (value - vals[point.Index1]) * point.Weight1;
            vals[point.Index2] += (value - vals[point.Index2]) * point.Weight2;
            vals[point.Index3] += (value - vals[point.Index3]) * point.Weight3;

            vals[point.Index4] += (value - vals[point.Index4]) * point.Weight4;
            vals[point.Index5] += (value - vals[point.Index5]) * point.Weight5;
            vals[point.Index6] += (value - vals[point.Index6]) * point.Weight6;
            vals[point.Index7] += (value - vals[point.Index7]) * point.Weight7;
        }

        #endregion


        #region Vector3d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static GridField3d<Vector3d> GetLaplacian(this GridField3d<Vector3d> field, bool parallel = false)
        {
            var result = GridField3d.Vector3d.Create(field);
            GetLaplacian(field, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetLaplacian(this GridField3d<Vector3d> field, Vector3d[] result, bool parallel = false)
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
                (int i, int j, int k) = field.IndexToGrid(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }
                    if (j == ny) { k++; j = 0; }

                    Vector3d tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    Vector3d tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    Vector3d ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    Vector3d ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    Vector3d tz0 = (k == 0) ? vals[index + dk] : vals[index - nxy];
                    Vector3d tz1 = (k == nz - 1) ? vals[index - dk] : vals[index + nxy];

                    Vector3d t = vals[index] * 2.0;
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy + (tz0 + tz1 - t) * dz;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static GridField3d<double> GetDivergence(this GridField3d<Vector3d> field, bool parallel = false)
        {
            var result = GridField3d.Double.Create(field);
            GetDivergence(field, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetDivergence(this GridField3d<Vector3d> field, double[] result, bool parallel = false)
        {
            // implementation reference
            // http://www.math.harvard.edu/archive/21a_spring_09/PDF/13-05-curl-and-divergence.pdf

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.CountXYZ), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.CountXYZ);

            void Body(int from, int to)
            {
                var vals = field.Values;

                (var nx, var ny, var nz) = field.Count;
                int nxy = field.CountXY;

                (var dx, var dy, var dz) = (0.5 / field.Scale);

                (int di, int dj, int dk) = field.GetBoundaryOffsets();
                (int i, int j, int k) = field.IndexToGrid(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }
                    if (j == ny) { k++; j = 0; }

                    Vector3d tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    Vector3d tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    Vector3d ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    Vector3d ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    Vector3d tz0 = (k == 0) ? vals[index + dk] : vals[index - nxy];
                    Vector3d tz1 = (k == nz - 1) ? vals[index - dk] : vals[index + nxy];

                    result[index] = (tx1.X - tx0.X) * dx + (ty1.Y - ty0.Y) * dy + (tz1.Z + tz0.Z) * dz;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static GridField3d<Vector3d> GetCurl(this GridField3d<Vector3d> field, bool parallel = false)
        {
            var result = GridField3d.Vector3d.Create(field);
            GetCurl(field, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetCurl(this GridField3d<Vector3d> field, Vector3d[] result, bool parallel = false)
        {
            // implementation reference
            // http://www.math.harvard.edu/archive/21a_spring_09/PDF/13-05-curl-and-divergence.pdf

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.CountXYZ), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.CountXYZ);

            void Body(int from, int to)
            {
                var vals = field.Values;

                (var nx, var ny, var nz) = field.Count;
                int nxy = field.CountXY;

                (var dx, var dy, var dz) = (0.5 / field.Scale);

                (int di, int dj, int dk) = field.GetBoundaryOffsets();
                (int i, int j, int k) = field.IndexToGrid(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }
                    if (j == ny) { k++; j = 0; }

                    Vector3d tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    Vector3d tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    Vector3d ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    Vector3d ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    Vector3d tz0 = (k == 0) ? vals[index + dk] : vals[index - nxy];
                    Vector3d tz1 = (k == nz - 1) ? vals[index - dk] : vals[index + nxy];

                    result[index] = new Vector3d(
                        (ty1.Z - ty0.Z) * dy - (tz1.Y - tz0.Y) * dz,
                        (tz1.X - tz0.X) * dz - (tx1.Z - tx0.Z) * dx,
                        (tx1.Y - tx0.Y) * dx - (ty1.X - ty0.X) * dy);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="point"></param>
        /// <param name="amount"></param>
        public static void IncrementAt(this GridField3d<Vector3d> field, ref GridPoint3d point, Vector3d amount)
        {
            var vals = field.Values;

            vals[point.Index0] += amount * point.Weight0;
            vals[point.Index1] += amount * point.Weight1;
            vals[point.Index2] += amount * point.Weight2;
            vals[point.Index3] += amount * point.Weight3;

            vals[point.Index4] += amount * point.Weight4;
            vals[point.Index5] += amount * point.Weight5;
            vals[point.Index6] += amount * point.Weight6;
            vals[point.Index7] += amount * point.Weight7;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="point"></param>
        /// <param name="value"></param>
        public static void SetAt(this GridField3d<Vector3d> field, ref GridPoint3d point, Vector3d value)
        {
            var vals = field.Values;

            vals[point.Index0] += (value - vals[point.Index0]) * point.Weight0;
            vals[point.Index1] += (value - vals[point.Index1]) * point.Weight1;
            vals[point.Index2] += (value - vals[point.Index2]) * point.Weight2;
            vals[point.Index3] += (value - vals[point.Index3]) * point.Weight3;

            vals[point.Index4] += (value - vals[point.Index4]) * point.Weight4;
            vals[point.Index5] += (value - vals[point.Index5]) * point.Weight5;
            vals[point.Index6] += (value - vals[point.Index6]) * point.Weight6;
            vals[point.Index7] += (value - vals[point.Index7]) * point.Weight7;
        }

        #endregion
    }
}
