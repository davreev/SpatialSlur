using System;
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
    public class GridScalarField2d : GridField2d<double>
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="mapper"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static GridScalarField2d CreateFromImage(Bitmap bitmap, Func<Color, double> mapper, Interval2d interval)
        {
            int nx = bitmap.Width;
            int ny = bitmap.Height;

            var result = new GridScalarField2d(interval, nx, ny);
            FieldIO.ReadFromImage(bitmap, result, mapper);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="mapper"></param>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static GridScalarField2d CreateFromImage(Bitmap bitmap, Func<Color, double> mapper, Vec2d origin, Vec2d scale)
        {
            int nx = bitmap.Width;
            int ny = bitmap.Height;

            var result = new GridScalarField2d(origin, scale, nx, ny);
            FieldIO.ReadFromImage(bitmap, result, mapper);

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
        /// <param name="wrapMode"></param>
        /// <param name="sampleMode"></param>
        public GridScalarField2d(Vec2d origin, Vec2d scale, int countX, int countY, WrapMode wrapMode = WrapMode.Clamp, SampleMode sampleMode = SampleMode.Linear)
            : base(origin, scale, countX, countY, wrapMode, sampleMode)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="wrapModeX"></param>
        /// <param name="wrapModeY"></param>
        /// <param name="sampleMode"></param>
        public GridScalarField2d(Vec2d origin, Vec2d scale, int countX, int countY, WrapMode wrapModeX, WrapMode wrapModeY, SampleMode sampleMode = SampleMode.Linear)
            : base(origin, scale, countX, countY, wrapModeX, wrapModeY, sampleMode)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="wrapMode"></param>
        /// <param name="sampleMode"></param>
        public GridScalarField2d(Interval2d interval, int countX, int countY, WrapMode wrapMode = WrapMode.Clamp, SampleMode sampleMode = SampleMode.Linear)
            : base(interval, countX, countY, wrapMode, sampleMode)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="wrapModeX"></param>
        /// <param name="wrapModeY"></param>
        /// <param name="sampleMode"></param>
        public GridScalarField2d(Interval2d interval, int countX, int countY, WrapMode wrapModeX, WrapMode wrapModeY, SampleMode sampleMode = SampleMode.Linear)
            : base(interval, countX, countY, wrapModeX, wrapModeY, sampleMode)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="sampleMode"></param>
        public GridScalarField2d(Grid2d other, SampleMode sampleMode = SampleMode.Linear)
            : base(other, sampleMode)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GridScalarField2d Duplicate(bool copyValues)
        {
            var result = new GridScalarField2d(this);
            if (copyValues) result.Set(this);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected sealed override GridField2d<double> DuplicateBase(bool copyValues)
        {
            return Duplicate(copyValues);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected sealed override double ValueAtLinear(Vec2d point)
        {
            point = ToGridSpace(point);
            double u = SlurMath.Fract(point.X, out int i0);
            double v = SlurMath.Fract(point.Y, out int j0);

            int i1 = WrapX(i0 + 1);
            int j1 = WrapY(j0 + 1) * CountX;

            i0 = WrapX(i0);
            j0 = WrapY(j0) * CountX;

            var vals = Values;
            return SlurMath.Lerp(
                SlurMath.Lerp(vals[i0 + j0], vals[i1 + j0], u),
                SlurMath.Lerp(vals[i0 + j1], vals[i1 + j1], u),
                v);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected sealed override double ValueAtLinearUnchecked(Vec2d point)
        {
            point = ToGridSpace(point);
            double u = SlurMath.Fract(point.X, out int i0);
            double v = SlurMath.Fract(point.Y, out int j0);

            j0 *= CountX;
            int i1 = i0 + 1;
            int j1 = j0 + CountX;

            var vals = Values;
            return SlurMath.Lerp(
                SlurMath.Lerp(vals[i0 + j0], vals[i1 + j0], u),
                SlurMath.Lerp(vals[i0 + j1], vals[i1 + j1], u),
                v);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public sealed override double ValueAt(GridPoint2d point)
        {
            return FieldUtil.ValueAt(Values, point.Corners, point.Weights);
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="value"></param>
        public void SetAt(GridPoint2d point, double value)
        {
            FieldUtil.SetAt(Values, point.Corners, point.Weights, value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="amount"></param>
        public void IncrementAt(GridPoint2d point, double amount)
        {
            FieldUtil.IncrementAt(Values, point.Corners, point.Weights, amount);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public GridScalarField2d GetLaplacian(bool parallel = false)
        {
            GridScalarField2d result = new GridScalarField2d(this);
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
                (var dx, var dy) = Scale;
                dx = 1.0 / (dx * dx);
                dy = 1.0 / (dy * dy);

                (int di,int dj) = GetBoundaryOffsets();
                (int i, int j) = IndicesAt(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    double tx0 = (i == 0) ? Values[index + di] : Values[index - 1];
                    double tx1 = (i == CountX - 1) ? Values[index - di] : Values[index + 1];

                    double ty0 = (j == 0) ? Values[index + dj] : Values[index - CountX];
                    double ty1 = (j == CountY - 1) ? Values[index - dj] : Values[index + CountX];

                    double t = Values[index] * 2.0;
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public GridVectorField2d GetGradient(bool parallel = false)
        {
            GridVectorField2d result = new GridVectorField2d(this);
            GetGradient(result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetGradient(IDiscreteField<Vec2d> result, bool parallel = false)
        {
            GetGradient(result.Values, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetGradient(Vec2d[] result, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, Count);

            void Body(int from, int to)
            {
                (var dx, var dy) = (0.5 / Scale);
                (int di, int dj) = GetBoundaryOffsets();
                (int i, int j) = IndicesAt(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    double tx0 = (i == 0) ? Values[index + di] : Values[index - 1];
                    double tx1 = (i == CountX - 1) ? Values[index - di] : Values[index + 1];

                    double ty0 = (j == 0) ? Values[index + dj] : Values[index - CountX];
                    double ty1 = (j == CountY - 1) ? Values[index - dj] : Values[index + CountX];

                    result[index] = new Vec2d((tx1 - tx0) * dx, (ty1 - ty0) * dy);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("ScalarField2d ({0} x {1})", CountX, CountY);
        }
    }
}
