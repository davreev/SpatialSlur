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
        /// <param name="interval"></param>
        /// <returns></returns>
        public static GridScalarField3d CreateFromImageStack(IList<Bitmap> bitmaps, Func<Color, double> mapper, Interval3d interval)
        {
            var bmp0 = bitmaps[0];
            int nx = bmp0.Width;
            int ny = bmp0.Height;
            int nz = bitmaps.Count;

            var result = new GridScalarField3d(interval, nx, ny, nz);
            FieldIO.ReadFromImageStack(bitmaps, result, mapper);

            return result;
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="wrapMode"></param>
        /// <param name="sampleMode"></param>
        public GridScalarField3d(Vec3d origin, Vec3d scale, int countX, int countY, int countZ, WrapMode wrapMode = WrapMode.Clamp, SampleMode sampleMode = SampleMode.Linear)
            : base(origin, scale, countX, countY, countZ, wrapMode, sampleMode)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="wrapModeX"></param>
        /// <param name="wrapModeY"></param>
        /// <param name="wrapModeZ"></param>
        /// <param name="sampleMode"></param>
        public GridScalarField3d(Vec3d origin, Vec3d scale, int countX, int countY, int countZ, WrapMode wrapModeX, WrapMode wrapModeY, WrapMode wrapModeZ, SampleMode sampleMode = SampleMode.Linear)
            : base(origin, scale, countX, countY, countZ, wrapModeX, wrapModeY, wrapModeZ, sampleMode)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="wrapMode"></param>
        /// <param name="sampleMode"></param>
        public GridScalarField3d(Interval3d interval, int countX, int countY, int countZ, WrapMode wrapMode = WrapMode.Clamp, SampleMode sampleMode = SampleMode.Linear)
            : base(interval, countX, countY, countZ, wrapMode, sampleMode)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="wrapModeX"></param>
        /// <param name="wrapModeY"></param>
        /// <param name="wrapModeZ"></param>
        /// <param name="sampleMode"></param>
        public GridScalarField3d(Interval3d interval, int countX, int countY, int countZ, WrapMode wrapModeX, WrapMode wrapModeY, WrapMode wrapModeZ, SampleMode sampleMode = SampleMode.Linear)
            : base(interval, countX, countY, countZ, wrapModeX, wrapModeY, wrapModeZ, sampleMode)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="sampleMode"></param>
        public GridScalarField3d(Grid3d other, SampleMode sampleMode = SampleMode.Linear)
            : base(other, sampleMode)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="copyValues"></param>
        /// <returns></returns>
        public GridScalarField3d Duplicate(bool copyValues)
        {
            var result = new GridScalarField3d(this, SampleMode);
            if (copyValues) result.Set(this);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="copyValues"></param>
        /// <returns></returns>
        protected sealed override GridField3d<double> DuplicateBase(bool copyValues)
        {
            return Duplicate(copyValues);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected sealed override double ValueAtLinear(Vec3d point)
        {
            point = ToGridSpace(point);
            double u = SlurMath.Fract(point.X, out int i0);
            double v = SlurMath.Fract(point.Y, out int j0);
            double w = SlurMath.Fract(point.Z, out int k0);

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
        protected sealed override double ValueAtLinearUnchecked(Vec3d point)
        {
            point = ToGridSpace(point);
            double u = SlurMath.Fract(point.X, out int i0);
            double v = SlurMath.Fract(point.Y, out int j0);
            double w = SlurMath.Fract(point.Z, out int k0);

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
        public sealed override double ValueAt(GridPoint3d point)
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
            GridScalarField3d result = new GridScalarField3d((Grid3d)this);
            GetLaplacian(result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian(IDiscreteField<double> result, bool parallel = false)
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
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, Count);

            void Body(int from, int to)
            {
                (var dx, var dy, var dz) = Scale;
                dx = 1.0 / (dx * dx);
                dy = 1.0 / (dy * dy);
                dz = 1.0 / (dz * dz);

                (int di, int dj, int dk) = GetBoundaryOffsets();
                (int i, int j, int k) = IndicesAt(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    double tx0 = (i == 0) ? Values[index + di] : Values[index - 1];
                    double tx1 = (i == CountX - 1) ? Values[index - di] : Values[index + 1];

                    double ty0 = (j == 0) ? Values[index + dj] : Values[index - CountX];
                    double ty1 = (j == CountY - 1) ? Values[index - dj] : Values[index + CountX];

                    double tz0 = (k == 0) ? Values[index + dk] : Values[index - CountXY];
                    double tz1 = (k == CountZ - 1) ? Values[index - dk] : Values[index + CountXY];

                    double t = Values[index] * 2.0;
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy + (tz0 + tz1 - t) * dz;
                }
            }
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
        public void GetGradient(IDiscreteField<Vec3d> result, bool parallel = false)
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
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, Count);

            void Body(int from, int to)
            {
                (var dx, var dy, var dz) = (0.5 / Scale);

                (int di, int dj, int dk) = GetBoundaryOffsets();
                (int i, int j, int k) = IndicesAt(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    double tx0 = (i == 0) ? Values[index + di] : Values[index - 1];
                    double tx1 = (i == CountX - 1) ? Values[index - di] : Values[index + 1];

                    double ty0 = (j == 0) ? Values[index + dj] : Values[index - CountX];
                    double ty1 = (j == CountY - 1) ? Values[index - dj] : Values[index + CountX];

                    double tz0 = (k == 0) ? Values[index + dk] : Values[index - CountXY];
                    double tz1 = (k == CountZ - 1) ? Values[index - dk] : Values[index + CountXY];

                    result[index] = new Vec3d((tx1 - tx0) * dx, (ty1 - ty0) * dy, (tz1 - tz0) * dz);
                }
            }
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
