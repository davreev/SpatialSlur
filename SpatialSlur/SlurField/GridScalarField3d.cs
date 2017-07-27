using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Drawing;
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
    public class GridScalarField3d : GridField3d<double>
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmaps"></param>
        /// <param name="mapper"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static GridScalarField3d CreateFromImageStack(IList<Bitmap> bitmaps, Func<Color, double> mapper, Domain3d domain)
        {
            var bmp0 = bitmaps[0];
            int nx = bmp0.Width;
            int ny = bmp0.Height;
            int nz = bitmaps.Count;

            var result = new GridScalarField3d(domain, nx, ny, nz);
            FieldIO.ReadFromImageStack(result, bitmaps, mapper);

            return result;
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        public GridScalarField3d(Domain3d domain, int countX, int countY, int countZ)
            : base(domain, countX, countY, countZ)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="sampleMode"></param>
        /// <param name="wrapMode"></param>
        public GridScalarField3d(Domain3d domain, int countX, int countY, int countZ, SampleMode sampleMode, WrapMode wrapMode)
            : base(domain, countX, countY, countZ, wrapMode, sampleMode)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="sampleMode"></param>
        /// <param name="wrapModeX"></param>
        /// <param name="wrapModeY"></param>
        /// <param name="wrapModeZ"></param>
        public GridScalarField3d(Domain3d domain, int countX, int countY, int countZ, SampleMode sampleMode, WrapMode wrapModeX, WrapMode wrapModeY, WrapMode wrapModeZ)
            : base(domain, countX, countY, countZ, wrapModeX, wrapModeY, wrapModeZ, sampleMode)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public GridScalarField3d(GridField3d other)
            : base(other)
        {
        }


        /// <summary>
        /// Creates a shallow copy of the internal array.
        /// </summary>
        /// <returns></returns>
        public GridScalarField3d Duplicate()
        {
            var copy = new GridScalarField3d(this);
            copy.Set(this);
            return copy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override GridField3d<double> DuplicateBase()
        {
            return Duplicate();
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected override double ValueAtLinear(Vec3d point)
        {
            (double u, double v, double w) = Fract(point, out int i0, out int j0, out int k0);

            int i1 = WrapX(i0 + 1);
            int j1 = WrapY(j0 + 1) * CountX;
            int k1 = WrapZ(k0 + 1) * CountXY;

            i0 = WrapX(i0);
            j0 = WrapY(j0) * CountX;
            k0 = WrapZ(k0) * CountXY;

            var vals = Values;
            return SlurMath.Lerp(
                SlurMath.Lerp(
                    SlurMath.Lerp(vals[i0 + j0 + k0], vals[i1 + j0 + k0], u),
                    SlurMath.Lerp(vals[i0 + j1 + k0], vals[i1 + j1 + k0], u),
                    v),
                SlurMath.Lerp(
                    SlurMath.Lerp(vals[i0 + j0 + k1], vals[i1 + j0 + k1], u),
                    SlurMath.Lerp(vals[i0 + j1 + k1], vals[i1 + j1 + k1], u),
                    v),
                w);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected override double ValueAtLinearUnchecked(Vec3d point)
        {
            (double u, double v, double w) = Fract(point, out int i0, out int j0, out int k0);

            j0 *= CountX;
            k0 *= CountXY;
            int i1 = i0 + 1;
            int j1 = j0 + CountX;
            int k1 = k0 + CountXY;

            var vals = Values;
            return SlurMath.Lerp(
                SlurMath.Lerp(
                    SlurMath.Lerp(vals[i0 + j0 + k0], vals[i1 + j0 + k0], u),
                    SlurMath.Lerp(vals[i0 + j1 + k0], vals[i1 + j1 + k0], u),
                    v),
                SlurMath.Lerp(
                    SlurMath.Lerp(vals[i0 + j0 + k1], vals[i1 + j0 + k1], u),
                    SlurMath.Lerp(vals[i0 + j1 + k1], vals[i1 + j1 + k1], u),
                    v),
                w);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override double ValueAt(GridPoint3d point)
        {
            return FieldUtil.ValueAt(Values, point.Corners, point.Weights);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="value"></param>
        public void SetAt(GridPoint3d point, double value)
        {
            FieldUtil.SetAt(Values, point.Corners, point.Weights, value);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="amount"></param>
        public void IncrementAt(GridPoint3d point, double amount)
        {
            FieldUtil.IncrementAt(Values, point.Corners, point.Weights, amount);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public GridScalarField3d GetLaplacian(bool parallel = false)
        {
            GridScalarField3d result = new GridScalarField3d((GridField3d)this);
            GetLaplacian(result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian(GridScalarField3d result, bool parallel = false)
        {
            GetLaplacian(result.Values, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian(double[] result, bool parallel)
        {
            var vals = Values;
            int nx = CountX;
            int ny = CountY;
            int nz = CountZ;
            int nxy = CountXY;

            double dx = 1.0 / (ScaleX * ScaleX);
            double dy = 1.0 / (ScaleY * ScaleY);
            double dz = 1.0 / (ScaleZ * ScaleZ);

            (int di, int dj, int dk) = GetBoundaryOffsets();

            Action<Tuple<int, int>> body = range =>
            {
                (int i, int j, int k) = IndicesAt(range.Item1);

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
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy + (tz0 + tz1 - t) * dz;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), body);
            else
                body(Tuple.Create(0, Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public GridVectorField3d GetGradient(bool parallel = false)
        {
            GridVectorField3d result = new GridVectorField3d(this);
            GetGradient(result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetGradient(GridVectorField3d result, bool parallel = false)
        {
            GetGradient(result.Values, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetGradient(Vec3d[] result, bool parallel = false)
        {
            var vals = Values;
            int nx = CountX;
            int ny = CountY;
            int nz = CountZ;
            int nxy = CountXY;

            double dx = 0.5 / ScaleX;
            double dy = 0.5 / ScaleY;
            double dz = 0.5 / ScaleZ;

            (int di, int dj, int dk) = GetBoundaryOffsets();

            Action<Tuple<int, int>> body = range =>
            {
                (int i, int j, int k) = IndicesAt(range.Item1);

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

                    result[index] = new Vec3d((tx1 - tx0) * dx, (ty1 - ty0) * dy, (tz1 - tz0) * dz);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), body);
            else
                body(Tuple.Create(0, Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("ScalarField3d ({0} x {1} x {2})", CountX, CountY, CountZ);
        }
    }
}
