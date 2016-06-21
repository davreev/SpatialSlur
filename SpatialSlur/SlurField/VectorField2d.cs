 using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
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
    public class VectorField2d:Field2d<Vec2d>
    {
        private Action<IList<Vec2d>> _getLaplacian;
        private Action<IList<double>> _getDivergence;
        private Action<IList<double>> _getCurl;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="boundaryType"></param>
        public VectorField2d(Domain2d domain, int countX, int countY, FieldBoundaryType boundaryType = FieldBoundaryType.Equal)
            : base(domain, countX, countY, boundaryType)
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
        protected override void OnBoundaryTypeChange()
        {
            base.OnBoundaryTypeChange();

            switch (BoundaryType)
            {
                case FieldBoundaryType.Constant:
                    {
                        _getLaplacian = GetLaplacianConstant;
                        _getDivergence = GetDivergenceConstant;
                        _getCurl = GetCurlConstant;
                        break;
                    }
                case FieldBoundaryType.Equal:
                    {
                        _getLaplacian = GetLaplacianEqual;
                        _getDivergence = GetDivergenceEqual;
                        _getCurl = GetCurlEqual;
                        break;
                    }
                case FieldBoundaryType.Periodic:
                    {
                        _getLaplacian = GetLaplacianPeriodic;
                        _getDivergence = GetDivergencePeriodic;
                        _getCurl = GetCurlPeriodic;
                        break;
                    }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override Vec2d Evaluate(FieldPoint2d point)
        {
            int[] corners = point.Corners;
            double[] weights = point.Weights;

            Vec2d result = new Vec2d();
            for (int i = 0; i < 4; i++)
                result += Values[corners[i]] * weights[i];

            return result;
        }


        /// <summary>
        /// Gets the magnitudes of all vectors in the field
        /// </summary>
        /// <returns></returns>
        public ScalarField2d GetMagnitudes()
        {
            ScalarField2d result = new ScalarField2d(this);
            UpdateMagnitudes(result.Values);
            return result;
        }


        /// <summary>
        /// Gets the magnitudes of all vectors in the field
        /// </summary>
        /// <param name="result"></param>
        public void UpdateMagnitudes(ScalarField2d result)
        {
            UpdateMagnitudes(result.Values);
        }


        /// <summary>
        /// Gets the magnitudes of all vectors in the field
        /// </summary>
        /// <param name="result"></param>
        public void UpdateMagnitudes(IList<double> result)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Values[i].Length;
            });
        }


        /// <summary>
        /// Unitizes all vectors in the field.
        /// </summary>
        public void Unitize()
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i].Unitize();
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public void Unitize(VectorField2d result)
        {
            Unitize(result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public void Unitize(IList<Vec2d> result)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    Vec2d v = Values[i];
                    v.Unitize();
                    result[i] = v;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public VectorField2d GetLaplacian()
        {
            VectorField2d result = new VectorField2d((Field2d)this);
            UpdateLaplacian(result.Values);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateLaplacian(VectorField2d result)
        {
            UpdateLaplacian(result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateLaplacian(IList<Vec2d> result)
        {
            _getLaplacian(result);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetLaplacianConstant(IList<Vec2d> result)
        {
            double dx = 1.0 / (ScaleX * ScaleX);
            double dy = 1.0 / (ScaleY * ScaleY);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    Vec2d value = Values[index];
                    Vec2d sum = new Vec2d();

                    // x
                    if (i == 0)
                        sum += (BoundaryValue + Values[index + 1] - 2.0 * value) * dx;
                    else if (i == CountX - 1)
                        sum += (Values[index - 1] + BoundaryValue - 2.0 * value) * dx;
                    else
                        sum += (Values[index - 1] + Values[index + 1] - 2.0 * value) * dx;

                    // y
                    if (j == 0)
                        sum += (BoundaryValue + Values[index + CountX] - 2.0 * value) * dy;
                    else if (j == CountY - 1)
                        sum += (Values[index - CountX] + BoundaryValue - 2.0 * value) * dy;
                    else
                        sum += (Values[index - CountX] + Values[index + CountX] - 2.0 * value) * dy;

                    result[index] = sum;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetLaplacianEqual(IList<Vec2d> result)
        {
            double dx = 1.0 / (ScaleX * ScaleX);
            double dy = 1.0 / (ScaleY * ScaleY);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    Vec2d value = Values[index];
                    Vec2d sum = new Vec2d();

                    // x
                    if (i == 0)
                        sum += (Values[index + 1] - value) * dx;
                    else if (i == CountX - 1)
                        sum += (Values[index - 1] - value) * dx;
                    else
                        sum += (Values[index - 1] + Values[index + 1] - 2.0 * value) * dx;

                    // y
                    if (j == 0)
                        sum += (Values[index + CountX] - value) * dy;
                    else if (j == CountY - 1)
                        sum += (Values[index - CountX] - value) * dy;
                    else
                        sum += (Values[index - CountX] + Values[index + CountX] - 2.0 * value) * dy;

                    result[index] = sum;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetLaplacianPeriodic(IList<Vec2d> result)
        {
            double dx = 1.0 / (ScaleX * ScaleX);
            double dy = 1.0 / (ScaleY * ScaleY);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    Vec2d value = Values[index];
                    Vec2d sum = new Vec2d();

                    // x
                    if (i == 0)
                        sum += (Values[index - 1 + CountX] + Values[index + 1] - 2.0 * value) * dx;
                    else if (i == CountX - 1)
                        sum += (Values[index - 1] + Values[index + 1 - CountX] - 2.0 * value) * dx;
                    else
                        sum += (Values[index - 1] + Values[index + 1] - 2.0 * value) * dx;

                    // y
                    if (j == 0)
                        sum += (Values[index - CountX + Count] + Values[index + CountX] - 2.0 * value) * dy;
                    else if (j == CountY - 1)
                        sum += (Values[index - CountX] + Values[index + CountX - Count] - 2.0 * value) * dy;
                    else
                        sum += (Values[index - CountX] + Values[index + CountX] - 2.0 * value) * dy;

                    result[index] = sum;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ScalarField2d GetDivergence()
        {
            ScalarField2d result = new ScalarField2d(this);
            UpdateDivergence(result.Values);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateDivergence(ScalarField2d result)
        {
            UpdateDivergence(result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateDivergence(IList<double> result)
        {
            _getDivergence(result);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetDivergenceConstant(IList<double> result)
        {
            double dx = 1.0 / (2.0 * ScaleX);
            double dy = 1.0 / (2.0 * ScaleY);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    double sum = 0.0;

                    //x
                    if (i == 0)
                        sum += (Values[index + 1].x - BoundaryValue.x) * dx;
                    else if (i == CountX - 1)
                        sum += (BoundaryValue.x - Values[index - 1].x) * dx;
                    else
                       sum += (Values[index + 1].x - Values[index - 1].x) * dx;

                    //y
                    if (j == 0)
                        sum += (Values[index + CountX].y - BoundaryValue.y) * dy;
                    else if (j == CountY - 1)
                        sum += (BoundaryValue.y - Values[index - CountX].y) * dy;
                    else
                        sum += (Values[index + CountX].y - Values[index - CountX].y) * dy;

                    result[index] = sum;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetDivergenceEqual(IList<double> result)
        {
            double dx = 1.0 / (2.0 * ScaleX);
            double dy = 1.0 / (2.0 * ScaleY);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    Vec2d value = Values[index];
                    double sum = 0.0;

                    //x
                    if (i == 0)
                        sum += (Values[index + 1].x - value.x) * dx;
                    else if (i == CountX - 1)
                        sum += (value.x - Values[index - 1].x) * dx;
                    else
                        sum += (Values[index + 1].x - Values[index - 1].x) * dx;

                    //y
                    if (j == 0)
                        sum += (Values[index + CountX].y - value.y) * dy;
                    else if (j == CountY - 1)
                        sum += (value.y - Values[index - CountX].y) * dy;
                    else
                        sum += (Values[index + CountX].y - Values[index - CountX].y) * dy;

                    result[index] = sum;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetDivergencePeriodic(IList<double> result)
        {
            double dx = 1.0 / (2.0 * ScaleX);
            double dy = 1.0 / (2.0 * ScaleY);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    double sum = 0.0;

                    //x
                    if (i == 0)
                        sum += (Values[index + 1].x - Values[index - 1 + CountX].x) * dx;
                    else if (i == CountX - 1)
                        sum += (Values[index + 1 - CountX].x - Values[index - 1].x) * dx;
                    else
                       sum += (Values[index + 1].x - Values[index - 1].x) * dx;

                    //y
                    if (j == 0)
                        sum += (Values[index + CountX].y - Values[index - CountX + Count].y) * dy;
                    else if (j == CountY - 1)
                        sum += (Values[index + CountX - Count].y - Values[index - CountX].y) * dy;
                    else
                        sum += (Values[index + CountX].y - Values[index - CountX].y) * dy;

                    result[index] = sum;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ScalarField2d GetCurl()
        {
            ScalarField2d result = new ScalarField2d((Field2d)this);
            UpdateCurl(result.Values);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateCurl(ScalarField2d result)
        {
            UpdateCurl(result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateCurl(IList<double> result)
        {
            _getCurl(result);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetCurlConstant(IList<double> result)
        {
            double dx = 1.0 / (2.0 * ScaleX);
            double dy = 1.0 / (2.0 * ScaleY);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    double dp, dq;

                    //x
                    if (i == 0)
                        dp = Values[index + 1].x - BoundaryValue.x;
                    else if (i == CountX - 1)
                        dp = BoundaryValue.x - Values[index - 1].x;
                    else
                        dp = Values[index + 1].x - Values[index - 1].x;

                    //y
                    if (j == 0)
                        dq = Values[index + CountX].y - BoundaryValue.y;
                    else if (j == CountY - 1)
                        dq = BoundaryValue.y - Values[index - CountX].y;
                    else
                        dq = Values[index + CountX].y - Values[index - CountX].y;

                    result[index] = dq * dx - dp * dy;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetCurlEqual(IList<double> result)
        {
            double dx = 1.0 / (2.0 * ScaleX);
            double dy = 1.0 / (2.0 * ScaleY);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    Vec2d value = Values[i];
                    double dp, dq;

                    //x
                    if (i == 0)
                        dp = Values[index + 1].x - value.x;
                    else if (i == CountX - 1)
                        dp = value.x - Values[index - 1].x;
                    else
                        dp = Values[index + 1].x - Values[index - 1].x;

                    //y
                    if (j == 0)
                        dq = Values[index + CountX].y - value.y;
                    else if (j == CountY - 1)
                        dq = value.y - Values[index - CountX].y;
                    else
                        dq = Values[index + CountX].y - Values[index - CountX].y;

                    result[index] = dq * dx - dp * dy;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetCurlPeriodic(IList<double> result)
        {
            double dx = 1.0 / (2.0 * ScaleX);
            double dy = 1.0 / (2.0 * ScaleY);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    double dp, dq;

                    //x
                    if (i == 0)
                        dp = Values[index + 1].x - Values[index - 1 + CountX].x;
                    else if (i == CountX - 1)
                        dp = Values[index + 1 - CountX].x - Values[index - 1].x;
                    else
                        dp = Values[index + 1].x - Values[index - 1].x;

                    //y
                    if (j == 0)
                        dq = Values[index + CountX].y - Values[index - CountX + Count].y;
                    else if (j == CountY - 1)
                        dq = Values[index + CountX - Count].y - Values[index - CountX].y;
                    else
                        dq = Values[index + CountX].y - Values[index - CountX].y;

                    result[index] = dq * dx - dp * dy;
                }
            });
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
