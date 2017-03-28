using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Drawing;
using System.IO;
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
    [Serializable]
    public sealed class VectorField3d : Field3d<Vec3d>
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmaps"></param>
        /// <param name="mapper"></param>
        /// <param name="domain"></param>
        /// <param name="wrapX"></param>
        /// <param name="wrapY"></param>
        /// <param name="wrapZ"></param>
        /// <returns></returns>
        public static VectorField3d CreateFromImageStack(IList<Bitmap> bitmaps, Func<Color, Vec3d> mapper, Domain3d domain, FieldWrapMode wrapX = FieldWrapMode.Clamp, FieldWrapMode wrapY = FieldWrapMode.Clamp, FieldWrapMode wrapZ = FieldWrapMode.Clamp)
        {
            var bmp0 = bitmaps[0];
            int nx = bmp0.Width;
            int ny = bmp0.Height;
            int nz = bitmaps.Count;

            var result = new VectorField3d(domain, nx, ny, nz, wrapX, wrapY, wrapZ);
            FieldIO.ReadFromImageStack(result, bitmaps, mapper);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static VectorField3d CreateFromFGA(string path)
        {
            var content = File.ReadAllText(path, Encoding.ASCII);
            var values = content.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            int nx = int.Parse(values[0]);
            int ny = int.Parse(values[1]);
            int nz = int.Parse(values[2]);

            Vec3d p0 = new Vec3d(
                double.Parse(values[3]),
                double.Parse(values[4]),
                double.Parse(values[5]));

            Vec3d p1 = new Vec3d(
               double.Parse(values[6]),
               double.Parse(values[7]),
               double.Parse(values[8]));

            VectorField3d result = new VectorField3d(new Domain3d(p0, p1), nx, ny, nz);
            var vecs = result.Values;
            int index = 0;

            for (int i = 9; i < values.Length; i += 3)
            {
                vecs[index++] = new Vec3d(
                    double.Parse(values[i]),
                    double.Parse(values[i + 1]),
                    double.Parse(values[i + 2]));
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        public static void SaveAsFGA(Field3d<Vec3d> field)
        {
            // TODO
            throw new NotImplementedException();
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="wrapMode"></param>
        public VectorField3d(Domain3d domain, int countX, int countY, int countZ, FieldWrapMode wrapMode)
            : base(domain, countX, countY, countZ, wrapMode)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="wrapModeX"></param>
        /// <param name="wrapModeY"></param>
        /// <param name="wrapModeZ"></param>
        public VectorField3d(Domain3d domain, int countX, int countY, int countZ, 
            FieldWrapMode wrapModeX = FieldWrapMode.Clamp, 
            FieldWrapMode wrapModeY = FieldWrapMode.Clamp, 
            FieldWrapMode wrapModeZ = FieldWrapMode.Clamp)
            : base(domain, countX, countY, countZ, wrapModeX,wrapModeY,wrapModeZ)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public VectorField3d(Field3d other)
            : base(other)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public VectorField3d(VectorField3d other)
            : base(other)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override Vec3d ValueAt(FieldPoint3d point)
        {
            return Values.ValueAt(point.Corners, point.Weights);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="amount"></param>
        public void IncrementAt(FieldPoint3d point, Vec3d amount)
        {
            Values.IncrementAt(point.Corners, point.Weights, amount);
        }

     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public VectorField3d GetLaplacian(bool parallel = false)
        {
            VectorField3d result = new VectorField3d((Field3d)this);
            GetLaplacian(result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian(VectorField3d result, bool parallel = false)
        {
            GetLaplacian(result.Values, parallel);
        }


        /// <summary>
        /// http://en.wikipedia.org/wiki/Discrete_Laplace_operator
        /// </summary>
        public void GetLaplacian(Vec3d[] result, bool parallel)
        {
            var vals = Values;
            int nx = CountX;
            int ny = CountY;
            int nz = CountZ;
            int nxy = CountXY;

            double dx = 1.0 / (ScaleX * ScaleX);
            double dy = 1.0 / (ScaleY * ScaleY);
            double dz = 1.0 / (ScaleZ * ScaleZ);

            int di, dj, dk;
            GetR1BoundaryOffsets(out di, out dj, out dk);

            Action<Tuple<int, int>> func = range =>
            {
                int i, j, k;
                IndicesAt(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
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
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func);
            else
                func(Tuple.Create(0, Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public ScalarField3d GetDivergence(bool parallel = false)
        {
            ScalarField3d result = new ScalarField3d(this);
            GetDivergence(result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetDivergence(ScalarField3d result, bool parallel = false)
        {
            GetDivergence(result.Values, parallel);
        }


        /// <summary>
        /// http://www.math.harvard.edu/archive/21a_spring_09/PDF/13-05-curl-and-divergence.pdf
        /// </summary>
        public void GetDivergence(double[] result, bool parallel)
        {
            var vals = Values;
            int nx = CountX;
            int ny = CountY;
            int nz = CountZ;
            int nxy = CountXY;

            double dx = 1.0 / (2.0 * ScaleX);
            double dy = 1.0 / (2.0 * ScaleY);
            double dz = 1.0 / (2.0 * ScaleZ);

            int di, dj, dk;
            GetR1BoundaryOffsets(out di, out dj, out dk);

            Action<Tuple<int, int>> func = range =>
            {
                int i, j, k;
                IndicesAt(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }
                    if (j == ny) { k++; j = 0; }

                    Vec3d tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    Vec3d tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    Vec3d ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    Vec3d ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    Vec3d tz0 = (k == 0) ? vals[index + dk] : vals[index - nxy];
                    Vec3d tz1 = (k == nz - 1) ? vals[index - dk] : vals[index + nxy];

                    result[index] = (tx1.x - tx0.x) * dx + (ty1.y - ty0.y) * dy + (tz1.z + tz0.z) * dz;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func);
            else
                func(Tuple.Create(0, Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public VectorField3d GetCurl(bool parallel = false)
        {
            VectorField3d result = new VectorField3d((Field3d)this);
            GetCurl(result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetCurl(VectorField3d result, bool parallel = false)
        {
            GetCurl(result.Values, parallel);
        }


        /// <summary>
        /// http://www.math.harvard.edu/archive/21a_spring_09/PDF/13-05-curl-and-divergence.pdf
        /// </summary>
        public void GetCurl(Vec3d[] result, bool parallel)
        {
            var vals = Values;
            int nx = CountX;
            int ny = CountY;
            int nz = CountZ;
            int nxy = CountXY;

            double dx = 1.0 / (2.0 * ScaleX);
            double dy = 1.0 / (2.0 * ScaleY);
            double dz = 1.0 / (2.0 * ScaleZ);

            int di, dj, dk;
            GetR1BoundaryOffsets(out di, out dj, out dk);

            Action<Tuple<int, int>> func = range =>
            {
                int i, j, k;
                IndicesAt(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
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
                        (ty1.z - ty0.z) * dy - (tz1.y - tz0.y) * dz,
                        (tz1.x - tz0.x) * dz - (tx1.z - tx0.z) * dx,
                        (tx1.y - tx0.y) * dx - (ty1.x - ty0.x) * dy);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func);
            else
                func(Tuple.Create(0, Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("VectorField3d ({0} x {1} x {2})", CountX, CountY, CountZ);
        }
    }
}
