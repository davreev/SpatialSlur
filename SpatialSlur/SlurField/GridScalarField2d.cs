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
        /// <param name="domain"></param>
        /// <returns></returns>
        public static GridScalarField2d CreateFromImage(Bitmap bitmap, Func<Color, double> mapper, Domain2d domain)
        {
            int nx = bitmap.Width;
            int ny = bitmap.Height;

            var result = new GridScalarField2d(domain, nx, ny);
            FieldIO.ReadFromImage(bitmap, result, mapper);

            return result;
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        public GridScalarField2d(Domain2d domain, int countX, int countY)
            : base(domain, countX, countY)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="sampleMode"></param>
        /// <param name="wrapMode"></param>
        public GridScalarField2d(Domain2d domain, int countX, int countY, SampleMode sampleMode, WrapMode wrapMode)
            : base(domain, countX, countY, wrapMode, sampleMode)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="sampleMode"></param>
        /// <param name="wrapModeX"></param>
        /// <param name="wrapModeY"></param>
        public GridScalarField2d(Domain2d domain, int countX, int countY, SampleMode sampleMode, WrapMode wrapModeX, WrapMode wrapModeY)
            : base(domain, countX, countY, wrapModeX, wrapModeY, sampleMode)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public GridScalarField2d(GridField2d other)
            : base(other)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GridScalarField2d Duplicate()
        {
            var copy = new GridScalarField2d(this);
            copy.Set(this);
            return copy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected sealed override GridField2d<double> DuplicateBase()
        {
            return Duplicate();
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected sealed override double ValueAtLinear(Vec2d point)
        {
            (double u, double v) = Fract(point, out int i0, out int j0);

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
            (double u, double v) = Fract(point, out int i0, out int j0);

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
        public void GetLaplacian(GridScalarField2d result, bool parallel = false)
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

            double dx = 1.0 / (ScaleX * ScaleX);
            double dy = 1.0 / (ScaleY * ScaleY);

            (int di, int dj) = GetBoundaryOffsets();

            Action<Tuple<int, int>> body = range =>
            {
                (int i, int j) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }

                    double tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    double tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    double ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    double ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    double t = vals[index] * 2.0;
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy;
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
        public void GetGradient(GridVectorField2d result, bool parallel = false)
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
            var vals = Values;
            int nx = CountX;
            int ny = CountY;

            double dx = 0.5 / ScaleX;
            double dy = 0.5 / ScaleY;

            (int di, int dj) = GetBoundaryOffsets();

            Action<Tuple<int, int>> body = range =>
            {
                (int i, int j) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }

                    double tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    double tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    double ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    double ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    result[index] = new Vec2d((tx1 - tx0) * dx, (ty1 - ty0) * dy);
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
            return String.Format("ScalarField2d ({0} x {1})", CountX, CountY);
        }
    }
}
