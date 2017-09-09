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
    public class GridVectorField3d : GridField3d<Vec3d>
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmaps"></param>
        /// <param name="mapper"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static GridVectorField3d CreateFromImageStack(IList<Bitmap> bitmaps, Func<Color, Vec3d> mapper, Interval3d interval)
        {
            var bmp0 = bitmaps[0];
            int nx = bmp0.Width;
            int ny = bmp0.Height;
            int nz = bitmaps.Count;

            var result = new GridVectorField3d(interval, nx, ny, nz);
            FieldIO.ReadFromImageStack(bitmaps, result, mapper);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static GridVectorField3d CreateFromFGA(string path)
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

            GridVectorField3d result = new GridVectorField3d(new Interval3d(p0, p1), nx, ny, nz);
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
        public static void SaveAsFGA(GridField3d<Vec3d> field)
        {
            // TODO
            throw new NotImplementedException();
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
        public GridVectorField3d(Vec3d origin, Vec3d scale, int countX, int countY, int countZ, WrapMode wrapMode = WrapMode.Clamp, SampleMode sampleMode = SampleMode.Linear)
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
        public GridVectorField3d(Vec3d origin, Vec3d scale, int countX, int countY, int countZ, WrapMode wrapModeX, WrapMode wrapModeY, WrapMode wrapModeZ, SampleMode sampleMode = SampleMode.Linear)
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
        public GridVectorField3d(Interval3d interval, int countX, int countY, int countZ, WrapMode wrapMode = WrapMode.Clamp, SampleMode sampleMode = SampleMode.Linear)
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
        public GridVectorField3d(Interval3d interval, int countX, int countY, int countZ, WrapMode wrapModeX, WrapMode wrapModeY, WrapMode wrapModeZ, SampleMode sampleMode = SampleMode.Linear)
            : base(interval, countX, countY, countZ, wrapModeX, wrapModeY, wrapModeZ, sampleMode)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="sampleMode"></param>
        public GridVectorField3d(Grid3d other, SampleMode sampleMode = SampleMode.Linear)
            : base(other, sampleMode)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="copyValues"></param>
        /// <returns></returns>
        public GridVectorField3d Duplicate(bool copyValues)
        {
            var result = new GridVectorField3d(this, SampleMode);
            if (copyValues) result.Set(this);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="copyValues"></param>
        /// <returns></returns>
        protected sealed override GridField3d<Vec3d> DuplicateBase(bool copyValues)
        {
            return Duplicate(copyValues);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected sealed override Vec3d ValueAtLinear(Vec3d point)
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
            return Vec3d.Lerp(
                Vec3d.Lerp(
                    Vec3d.Lerp(vals[i0 + j0 + k0], vals[i1 + j0 + k0], u),
                    Vec3d.Lerp(vals[i0 + j1 + k0], vals[i1 + j1 + k0], u),
                    v),
                Vec3d.Lerp(
                    Vec3d.Lerp(vals[i0 + j0 + k1], vals[i1 + j0 + k1], u),
                    Vec3d.Lerp(vals[i0 + j1 + k1], vals[i1 + j1 + k1], u),
                    v),
                w);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected sealed override Vec3d ValueAtLinearUnchecked(Vec3d point)
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
            return Vec3d.Lerp(
                Vec3d.Lerp(
                    Vec3d.Lerp(vals[i0 + j0 + k0], vals[i1 + j0 + k0], u),
                    Vec3d.Lerp(vals[i0 + j1 + k0], vals[i1 + j1 + k0], u),
                    v),
                Vec3d.Lerp(
                    Vec3d.Lerp(vals[i0 + j0 + k1], vals[i1 + j0 + k1], u),
                    Vec3d.Lerp(vals[i0 + j1 + k1], vals[i1 + j1 + k1], u),
                    v),
                w);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public sealed override Vec3d ValueAt(GridPoint3d point)
        {
            return FieldUtil.ValueAt(Values, point.Corners, point.Weights);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetAt(GridPoint3d point, Vec3d value)
        {
            FieldUtil.SetAt(Values, point.Corners, point.Weights, value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="amount"></param>
        public void IncrementAt(GridPoint3d point, Vec3d amount)
        {
            FieldUtil.IncrementAt(Values, point.Corners, point.Weights, amount);
        }

     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public GridVectorField3d GetLaplacian(bool parallel = false)
        {
            GridVectorField3d result = new GridVectorField3d((Grid3d)this);
            GetLaplacian(result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian(IDiscreteField<Vec3d> result, bool parallel = false)
        {
            GetLaplacian(result.Values, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public void GetLaplacian(Vec3d[] result, bool parallel)
        {
            (var dx, var dy, var dz) = Scale.Components;
            dx = 1.0 / (dx * dx);
            dy = 1.0 / (dy * dy);
            dz = 1.0 / (dz * dz);

            (int di, int dj, int dk) = GetBoundaryOffsets();

            Action<Tuple<int, int>> body = range =>
            {
                (int i, int j, int k) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    Vec3d tx0 = (i == 0) ? Values[index + di] : Values[index - 1];
                    Vec3d tx1 = (i == CountX - 1) ? Values[index - di] : Values[index + 1];

                    Vec3d ty0 = (j == 0) ? Values[index + dj] : Values[index - CountX];
                    Vec3d ty1 = (j == CountY - 1) ? Values[index - dj] : Values[index + CountX];

                    Vec3d tz0 = (k == 0) ? Values[index + dk] : Values[index - CountXY];
                    Vec3d tz1 = (k == CountZ - 1) ? Values[index - dk] : Values[index + CountXY];

                    Vec3d t = Values[index] * 2.0;
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
        public GridScalarField3d GetDivergence(bool parallel = false)
        {
            GridScalarField3d result = new GridScalarField3d(this);
            GetDivergence(result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetDivergence(IDiscreteField<double> result, bool parallel = false)
        {
            GetDivergence(result.Values, parallel);
        }


        /// <summary>
        /// http://www.math.harvard.edu/archive/21a_spring_09/PDF/13-05-curl-and-divergence.pdf
        /// </summary>
        public void GetDivergence(double[] result, bool parallel)
        {
            (var dx, var dy, var dz) = (0.5 / Scale).Components;
            (var di, var dj, var dk) = GetBoundaryOffsets();

            Action<Tuple<int, int>> body = range =>
            {
                (int i, int j, int k) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    Vec3d tx0 = (i == 0) ? Values[index + di] : Values[index - 1];
                    Vec3d tx1 = (i == CountX - 1) ? Values[index - di] : Values[index + 1];

                    Vec3d ty0 = (j == 0) ? Values[index + dj] : Values[index - CountX];
                    Vec3d ty1 = (j == CountY - 1) ? Values[index - dj] : Values[index + CountX];

                    Vec3d tz0 = (k == 0) ? Values[index + dk] : Values[index - CountXY];
                    Vec3d tz1 = (k == CountZ - 1) ? Values[index - dk] : Values[index + CountXY];

                    result[index] = (tx1.X - tx0.X) * dx + (ty1.Y - ty0.Y) * dy + (tz1.Z + tz0.Z) * dz;
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
        public GridVectorField3d GetCurl(bool parallel = false)
        {
            GridVectorField3d result = new GridVectorField3d((Grid3d)this);
            GetCurl(result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetCurl(IDiscreteField<Vec3d> result, bool parallel = false)
        {
            GetCurl(result.Values, parallel);
        }


        /// <summary>
        /// http://www.math.harvard.edu/archive/21a_spring_09/PDF/13-05-curl-and-divergence.pdf
        /// </summary>
        public void GetCurl(Vec3d[] result, bool parallel)
        {
            (var dx, var dy, var dz) = (0.5 / Scale).Components;
            (var di, var dj, var dk) = GetBoundaryOffsets();

            Action<Tuple<int, int>> body = range =>
            {
                (int i, int j, int k) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    Vec3d tx0 = (i == 0) ? Values[index + di] : Values[index - 1];
                    Vec3d tx1 = (i == CountX - 1) ? Values[index - di] : Values[index + 1];

                    Vec3d ty0 = (j == 0) ? Values[index + dj] : Values[index - CountX];
                    Vec3d ty1 = (j == CountY - 1) ? Values[index - dj] : Values[index + CountX];

                    Vec3d tz0 = (k == 0) ? Values[index + dk] : Values[index - CountXY];
                    Vec3d tz1 = (k == CountZ - 1) ? Values[index - dk] : Values[index + CountXY];

                    result[index] = new Vec3d(
                        (ty1.Z - ty0.Z) * dy - (tz1.Y - tz0.Y) * dz,
                        (tz1.X - tz0.X) * dz - (tx1.Z - tx0.Z) * dx,
                        (tx1.Y - tx0.Y) * dx - (ty1.X - ty0.X) * dy);
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
            return String.Format("VectorField3d ({0} x {1} x {2})", CountX, CountY, CountZ);
        }
    }
}
