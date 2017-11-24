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
    public class GridVectorField2d : GridField2d<Vec2d>, IField3d<Vec3d>
    { 
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="mapper"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static GridVectorField2d CreateFromImage(Bitmap bitmap, Func<Color, Vec2d> mapper, Interval2d interval)
        {
            int nx = bitmap.Width;
            int ny = bitmap.Height;

            var result = new GridVectorField2d(interval, nx, ny);
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
        public GridVectorField2d(Vec2d origin, Vec2d scale, int countX, int countY, WrapMode wrapMode = WrapMode.Clamp, SampleMode sampleMode = SampleMode.Linear)
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
        public GridVectorField2d(Vec2d origin, Vec2d scale, int countX, int countY, WrapMode wrapModeX, WrapMode wrapModeY, SampleMode sampleMode = SampleMode.Linear)
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
        public GridVectorField2d(Interval2d interval, int countX, int countY, WrapMode wrapMode = WrapMode.Clamp, SampleMode sampleMode = SampleMode.Linear)
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
        public GridVectorField2d(Interval2d interval, int countX, int countY, WrapMode wrapModeX, WrapMode wrapModeY, SampleMode sampleMode = SampleMode.Linear)
            : base(interval, countX, countY, wrapModeX, wrapModeY, sampleMode)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="sampleMode"></param>
        public GridVectorField2d(Grid2d other, SampleMode sampleMode = SampleMode.Linear)
            : base(other, sampleMode)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GridVectorField2d Duplicate(bool copyValues)
        {
            var result = new GridVectorField2d(this);
            if (copyValues) result.Set(this);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected sealed override GridField2d<Vec2d> DuplicateBase(bool copyValues)
        {
            return Duplicate(copyValues);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected sealed override Vec2d ValueAtLinear(Vec2d point)
        {
            point = ToGridSpace(point);
            double u = SlurMath.Fract(point.X, out int i0);
            double v = SlurMath.Fract(point.Y, out int j0);

            int i1 = WrapX(i0 + 1);
            int j1 = WrapY(j0 + 1) * CountX;

            i0 = WrapX(i0);
            j0 = WrapY(j0) * CountX;

            var vals = Values;
            return Vec2d.Lerp(
                Vec2d.Lerp(vals[i0 + j0], vals[i1 + j0], u),
                Vec2d.Lerp(vals[i0 + j1], vals[i1 + j1], u),
                v);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected sealed override Vec2d ValueAtLinearUnchecked(Vec2d point)
        {
            point = ToGridSpace(point);
            double u = SlurMath.Fract(point.X, out int i0);
            double v = SlurMath.Fract(point.Y, out int j0);

            j0 *= CountX;
            int i1 = i0 + 1;
            int j1 = j0 + CountX;

            var vals = Values;
            return Vec2d.Lerp(
                Vec2d.Lerp(vals[i0 + j0], vals[i1 + j0], u),
                Vec2d.Lerp(vals[i0 + j1], vals[i1 + j1], u),
                v);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public sealed override Vec2d ValueAt(GridPoint2d point)
        {
            return FieldUtil.ValueAt(Values, point.Corners, point.Weights);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="value"></param>
        public void SetAt(GridPoint2d point, Vec2d value)
        {
            FieldUtil.SetAt(Values, point.Corners, point.Weights, value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="amount"></param>
        public void IncrementAt(GridPoint2d point, Vec2d amount)
        {
            FieldUtil.IncrementAt(Values, point.Corners, point.Weights, amount);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public GridVectorField2d GetLaplacian(bool parallel = false)
        {
            GridVectorField2d result = new GridVectorField2d(this);
            GetLaplacian(result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian(IDiscreteField<Vec2d> result, bool parallel = false)
        {
            GetLaplacian(result.Values, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public void GetLaplacian(Vec2d[] result, bool parallel)
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

                (int di, int dj) = GetBoundaryOffsets();
                (int i, int j) = IndicesAt(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    Vec2d tx0 = (i == 0) ? Values[index + di] : Values[index - 1];
                    Vec2d tx1 = (i == CountX - 1) ? Values[index - di] : Values[index + 1];

                    Vec2d ty0 = (j == 0) ? Values[index + dj] : Values[index - CountX];
                    Vec2d ty1 = (j == base.CountY - 1) ? Values[index - dj] : Values[index + CountX];

                    Vec2d t = Values[index] * 2.0;
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public GridScalarField2d GetDivergence(bool parallel = false)
        {
            GridScalarField2d result = new GridScalarField2d(this);
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
        /// 
        /// </summary>
        public void GetDivergence(double[] result, bool parallel)
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

                    Vec2d tx0 = (i == 0) ? Values[index + di] : Values[index - 1];
                    Vec2d tx1 = (i == CountX - 1) ? Values[index - di] : Values[index + 1];

                    Vec2d ty0 = (j == 0) ? Values[index + dj] : Values[index - CountX];
                    Vec2d ty1 = (j == CountY - 1) ? Values[index - dj] : Values[index + CountX];

                    result[index] = (tx1.X - tx0.X) * dx + (ty1.Y - ty0.Y) * dy;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public GridScalarField2d GetCurl(bool parallel = false)
        {
            GridScalarField2d result = new GridScalarField2d((Grid2d)this);
            GetCurl(result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetCurl(IDiscreteField<double> result, bool parallel = false)
        {
           GetCurl(result.Values, parallel);
        }


        /// <summary>
        /// http://www.math.harvard.edu/archive/21a_spring_09/PDF/13-05-curl-and-divergence.pdf
        /// </summary>
        public void GetCurl(double[] result, bool parallel)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, Count);

            void Body(int from, int to)
            {
                (var tx, var ty) = (0.5 / Scale);
                (int di, int dj) = GetBoundaryOffsets();
                (int i, int j) = IndicesAt(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    Vec2d tx0 = (i == 0) ? Values[index + di] : Values[index - 1];
                    Vec2d tx1 = (i == CountX - 1) ? Values[index - di] : Values[index + 1];

                    Vec2d ty0 = (j == 0) ? Values[index + dj] : Values[index - CountX];
                    Vec2d ty1 = (j == CountY - 1) ? Values[index - dj] : Values[index + CountX];

                    result[index] = (tx1.Y - tx0.Y) * tx - (ty1.X - ty0.X) * ty;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("VectorField2d ({0} x {1})", CountX, CountY);
        }


        #region Explicit interface implementations

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        Vec3d IField3d<Vec3d>.ValueAt(Vec3d point)
        {
            return ValueAt(point);
        }

        #endregion
    }
}
