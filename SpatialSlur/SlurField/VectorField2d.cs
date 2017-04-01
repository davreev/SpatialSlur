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
    public sealed class VectorField2d : Field2d<Vec2d>
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="mapper"></param>
        /// <param name="domain"></param>
        /// <param name="wrapX"></param>
        /// <param name="wrapY"></param>
        /// <returns></returns>
        public static VectorField2d CreateFromImage(Bitmap bitmap, Func<Color, Vec2d> mapper, Domain2d domain, FieldWrapMode wrapX = FieldWrapMode.Clamp, FieldWrapMode wrapY = FieldWrapMode.Clamp)
        {
            int nx = bitmap.Width;
            int ny = bitmap.Height;

            var result = new VectorField2d(domain, nx, ny, wrapX, wrapY);
            FieldIO.ReadFromImage(result, bitmap, mapper);

            return result;
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="wrapMode"></param>
        public VectorField2d(Domain2d domain, int countX, int countY, FieldWrapMode wrapMode)
            : base(domain, countX, countY, wrapMode)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="wrapModeX"></param>
        /// <param name="wrapModeY"></param>
        public VectorField2d(Domain2d domain, int countX, int countY, 
            FieldWrapMode wrapModeX = FieldWrapMode.Clamp, 
            FieldWrapMode wrapModeY = FieldWrapMode.Clamp)
            : base(domain, countX, countY, wrapModeX, wrapModeY)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public VectorField2d(Field2d other)
            : base(other)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public VectorField2d(VectorField2d other)
            : base(other)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override Vec2d ValueAt(FieldPoint2d point)
        {
            return Values.ValueAt(point.Corners, point.Weights);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="amount"></param>
        public void IncrementAt(FieldPoint2d point, Vec2d amount)
        {
            Values.IncrementAt(point.Corners, point.Weights, amount);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public VectorField2d GetLaplacian(bool parallel = false)
        {
            VectorField2d result = new VectorField2d((Field2d)this);
            GetLaplacian(result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian(VectorField2d result, bool parallel = false)
        {
            GetLaplacian(result.Values, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public void GetLaplacian(Vec2d[] result, bool parallel)
        {
            var vals = Values;
            int nx = CountX;
            int ny = CountY;

            double dx = 1.0 / (ScaleX * ScaleX);
            double dy = 1.0 / (ScaleY * ScaleY);

            (int di, int dj) = this.GetBoundaryOffsets();

            Action<Tuple<int, int>> func = range =>
            {
                (int i, int j) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }

                    Vec2d tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    Vec2d tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    Vec2d ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    Vec2d ty1 = (j == CountY - 1) ? vals[index - dj] : vals[index + nx];

                    Vec2d t = vals[index] * 2.0;
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy;
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
        public ScalarField2d GetDivergence(bool parallel = false)
        {
            ScalarField2d result = new ScalarField2d(this);
            GetDivergence(result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetDivergence(ScalarField2d result, bool parallel = false)
        {
            GetDivergence(result.Values, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public void GetDivergence(double[] result, bool parallel)
        {
            var vals = Values;
            int nx = CountX;
            int ny = CountY;

            double dx = 1.0 / (2.0 * ScaleX);
            double dy = 1.0 / (2.0 * ScaleY);

            (int di, int dj) = this.GetBoundaryOffsets();

            Action<Tuple<int, int>> func = range =>
            {
                (int i, int j) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }

                    Vec2d tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    Vec2d tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    Vec2d ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    Vec2d ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    result[index] = (tx1.x - tx0.x) * dx + (ty1.y - ty0.y) * dy;
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
        public ScalarField2d GetCurl(bool parallel = false)
        {
            ScalarField2d result = new ScalarField2d((Field2d)this);
            GetCurl(result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetCurl(ScalarField2d result, bool parallel = false)
        {
           GetCurl(result.Values, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public void GetCurl(double[] result, bool parallel)
        {
            var vals = Values;
            int nx = CountX;
            int ny = CountY;

            double dx = 1.0 / (2.0 * ScaleX);
            double dy = 1.0 / (2.0 * ScaleY);

            (int di, int dj) = this.GetBoundaryOffsets();

            Action<Tuple<int, int>> func = range =>
            {
                (int i, int j) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == nx) { j++; i = 0; }

                    Vec2d tx0 = (i == 0) ? vals[index + di] : vals[index - 1];
                    Vec2d tx1 = (i == nx - 1) ? vals[index - di] : vals[index + 1];

                    Vec2d ty0 = (j == 0) ? vals[index + dj] : vals[index - nx];
                    Vec2d ty1 = (j == ny - 1) ? vals[index - dj] : vals[index + nx];

                    result[index] = (tx1.y - tx0.y) * dx - (ty1.x - ty0.x) * dy;
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
            return String.Format("VectorField2d ({0} x {1})", CountX, CountY);
        }
    }
}
