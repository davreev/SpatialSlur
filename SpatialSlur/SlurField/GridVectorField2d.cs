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
    public class GridVectorField2d : GridField2d<Vec2d>
    { 
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="mapper"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static GridVectorField2d CreateFromImage(Bitmap bitmap, Func<Color, Vec2d> mapper, Domain2d domain)
        {
            int nx = bitmap.Width;
            int ny = bitmap.Height;

            var result = new GridVectorField2d(domain, nx, ny);
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
        public GridVectorField2d(Domain2d domain, int countX, int countY)
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
        public GridVectorField2d(Domain2d domain, int countX, int countY, SampleMode sampleMode, WrapMode wrapMode)
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
        public GridVectorField2d(Domain2d domain, int countX, int countY, SampleMode sampleMode, WrapMode wrapModeX, WrapMode wrapModeY)
            : base(domain, countX, countY, wrapModeX, wrapModeY, sampleMode)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public GridVectorField2d(GridField2d other)
            : base(other)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GridVectorField2d Duplicate()
        {
            var copy = new GridVectorField2d(this);
            copy.Set(this);
            return copy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected sealed override GridField2d<Vec2d> DuplicateBase()
        {
            return Duplicate();
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected sealed override Vec2d ValueAtLinear(Vec2d point)
        {
            (double u, double v) = Fract(point, out int i0, out int j0);

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
            (double u, double v) = Fract(point, out int i0, out int j0);

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
            double dx = 1.0 / (ScaleX * ScaleX);
            double dy = 1.0 / (ScaleY * ScaleY);

            (int di, int dj) = GetBoundaryOffsets();

            Action<Tuple<int, int>> body = range =>
            {
                (int i, int j) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    Vec2d tx0 = (i == 0) ? Values[index + di] : Values[index - 1];
                    Vec2d tx1 = (i == CountX - 1) ? Values[index - di] : Values[index + 1];

                    Vec2d ty0 = (j == 0) ? Values[index + dj] : Values[index - CountX];
                    Vec2d ty1 = (j == base.CountY - 1) ? Values[index - dj] : Values[index + CountX];

                    Vec2d t = Values[index] * 2.0;
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
            double dx = 0.5 / ScaleX;
            double dy = 0.5 / ScaleY;

            (int di, int dj) = GetBoundaryOffsets();

            Action<Tuple<int, int>> body = range =>
            {
                (int i, int j) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    Vec2d tx0 = (i == 0) ? Values[index + di] : Values[index - 1];
                    Vec2d tx1 = (i == CountX - 1) ? Values[index - di] : Values[index + 1];

                    Vec2d ty0 = (j == 0) ? Values[index + dj] : Values[index - CountX];
                    Vec2d ty1 = (j == CountY - 1) ? Values[index - dj] : Values[index + CountX];

                    result[index] = (tx1.X - tx0.X) * dx + (ty1.Y - ty0.Y) * dy;
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
        public GridScalarField2d GetCurl(bool parallel = false)
        {
            GridScalarField2d result = new GridScalarField2d((GridField2d)this);
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
            double dx = 0.5 / ScaleX;
            double dy = 0.5 / ScaleY;

            (int di, int dj) = GetBoundaryOffsets();

            Action<Tuple<int, int>> body = range =>
            {
                (int i, int j) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    Vec2d tx0 = (i == 0) ? Values[index + di] : Values[index - 1];
                    Vec2d tx1 = (i == CountX - 1) ? Values[index - di] : Values[index + 1];

                    Vec2d ty0 = (j == 0) ? Values[index + dj] : Values[index - CountX];
                    Vec2d ty1 = (j == CountY - 1) ? Values[index - dj] : Values[index + CountX];

                    result[index] = (tx1.Y - tx0.Y) * dx - (ty1.X - ty0.X) * dy;
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
            return String.Format("VectorField2d ({0} x {1})", CountX, CountY);
        }
    }
}
