 using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurField
{
    /// <summary>
    ///
    /// </summary>
    public class VectorField2d:Field2d<Vec2d>
    {
        private Action<IList<Vec2d>> _getLaplacian;
        private Action<IList<double>> _getDivergence;
        // private Action<IList<Vec2d>> _getCurl;


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
                    _getLaplacian = GetLaplacianConstant;
                    _getDivergence = GetDivergenceConstant;
                    break;
                case FieldBoundaryType.Equal:
                    _getLaplacian = GetLaplacianEqual;
                    _getDivergence = GetDivergenceEqual;
                    break;
                case FieldBoundaryType.Periodic:
                    _getLaplacian = GetLaplacianPeriodic;
                    _getDivergence = GetDivergencePeriodic;
                    break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Field2d Duplicate()
        {
            return new VectorField2d(this);
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
        /// gets the magnitudes of all vectors in the field
        /// </summary>
        /// <returns></returns>
        public ScalarField2d GetMagnitudes()
        {
            ScalarField2d result = new ScalarField2d(this);
            GetMagnitudes(result.Values);
            return result;
        }


        /// <summary>
        /// gets the magnitudes of all vectors in the field
        /// </summary>
        /// <param name="result"></param>
        public void GetMagnitudes(ScalarField2d result)
        {
            GetMagnitudes(result.Values);
        }


        /// <summary>
        /// gets the magnitudes of all vectors in the field
        /// </summary>
        /// <param name="result"></param>
        public void GetMagnitudes(IList<double> result)
        {
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Values[i].Length;
            });
        }


        /// <summary>
        /// unitizes all vectors in the field
        /// </summary>
        public void Unitize()
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] /= Values[i].Length;
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
            SizeCheck(result);
            _getLaplacian(result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        private void GetLaplacianConstant(IList<Vec2d> result)
        {
            // inverse square step size for each dimension
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
        /// <param name="result"></param>
        private void GetLaplacianEqual(IList<Vec2d> result)
        {
            // inverse square step size for each dimension
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
        /// <param name="result"></param>
        private void GetLaplacianPeriodic(IList<Vec2d> result)
        {
            // inverse square step size for each dimension
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
            UpdateDivergence(result);
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
        /// http://www.math.harvard.edu/archive/21a_spring_09/PDF/13-05-curl-and-divergence.pdf
        /// http://mathworld.wolfram.com/Divergence.html
        /// </summary>
        /// <param name="result"></param>
        public void UpdateDivergence(IList<double> result)
        {
            SizeCheck(result);
            _getDivergence(result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
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
        /// <param name="result"></param>
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
        /// <param name="result"></param>
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
        public VectorField2d GetCurl()
        {
            VectorField2d result = new VectorField2d((Field2d)this);
            UpdateCurl(result.Values);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateCurl(VectorField2d result)
        {
            UpdateCurl(result.Values);
        }


        /// <summary>
        /// TODO
        /// http://www.math.harvard.edu/archive/21a_spring_09/PDF/13-05-curl-and-divergence.pdf
        /// </summary>
        /// <param name="result"></param>
        public void UpdateCurl(IList<Vec2d> result)
        {
            SizeCheck(result);
            throw new NotImplementedException();
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
