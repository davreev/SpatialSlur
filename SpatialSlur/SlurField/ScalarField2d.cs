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
    public class ScalarField2d:Field2d<double>
    {
        // delegates for boundary dependant methods
        private Action<IList<double>> _getLaplacian;
        private Action<IList<Vec2d>> _getGradient;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="boundaryType"></param>
        public ScalarField2d(Domain2d domain, int countX, int countY, FieldBoundaryType boundaryType = FieldBoundaryType.Equal)
            : base(domain, countX, countY, boundaryType)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public ScalarField2d(Field2d other)
            : base(other)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public ScalarField2d(ScalarField2d other)
            : base(other)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void OnBoundaryTypeChange()
        {
            base.OnBoundaryTypeChange();

            switch(BoundaryType)
            {
                case FieldBoundaryType.Constant:
                    {
                        _getLaplacian = GetLaplacianConstant;
                        _getGradient = GetGradientConstant;
                        break;
                    }
                case FieldBoundaryType.Equal:
                    {
                        _getLaplacian = GetLaplacianEqual;
                        _getGradient = GetGradientEqual;
                        break;
                    }
                case FieldBoundaryType.Periodic:
                    {
                        _getLaplacian = GetLaplacianPeriodic;
                        _getGradient = GetGradientPeriodic;
                        break;
                    }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override double Evaluate(FieldPoint2d point)
        {
            int[] corners = point.Corners;
            double[] weights = point.Weights;

            double result = 0.0;
            for (int i = 0; i < 4; i++)
                result += Values[corners[i]] * weights[i];

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Add(ScalarField2d other)
        {
            Add(other.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public void Add(IList<double> values)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] += values[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public void Add(ScalarField2d other, ScalarField2d result)
        {
            Add(other.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="result"></param>
        public void Add(IList<double> values, IList<double> result)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Values[i] + values[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Multiply(ScalarField2d other)
        {
            Multiply(other.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public void Multiply(IList<double> values)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] *= values[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public void Multiply(ScalarField2d other, ScalarField2d result)
        {
            Multiply(other.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="result"></param>
        public void Multiply(IList<double> values, IList<double> result)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Values[i] * values[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="factor"></param>
        public void LerpTo(double value, double factor)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] = SlurMath.Lerp(Values[i], value, factor);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        public void LerpTo(ScalarField2d other, double factor)
        {
            LerpTo(other.Values, factor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="factor"></param>
        public void LerpTo(IList<double> values, double factor)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] = SlurMath.Lerp(Values[i], values[i], factor);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="factors"></param>
        public void LerpTo(double value, ScalarField2d factors)
        {
            LerpTo(value, factors.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="factors"></param>
        public void LerpTo(double value, IList<double> factors)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] = SlurMath.Lerp(Values[i], value, factors[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factors"></param>
        public void LerpTo(ScalarField2d other, ScalarField2d factors)
        {
            LerpTo(other.Values, factors.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="factors"></param>
        public void LerpTo(IList<double> values, IList<double> factors)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] = SlurMath.Lerp(Values[i], values[i], factors[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public void Normalize()
        {
            Normalize(new Domain(Values));
        }


        /// <summary>
        /// 
        /// </summary>
        public void Normalize(Domain domain)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] = domain.Normalize(Values[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="to"></param>
        public void Remap(Domain to)
        {
            Remap(new Domain(Values), to);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void Remap(Domain from, Domain to)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] = SlurCore.Domain.Remap(Values[i], from, to);
            });
        }

   
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ScalarField2d GetLaplacian()
        {
            ScalarField2d result = new ScalarField2d((Field2d)this);
            UpdateLaplacian(result.Values);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateLaplacian(ScalarField2d result)
        {
            UpdateLaplacian(result.Values);
        }


        /// <summary>
        /// http://en.wikipedia.org/wiki/Discrete_Laplace_operator
        /// </summary>
        /// <param name="result"></param>
        public void UpdateLaplacian(IList<double> result)
        {
            _getLaplacian(result);
        }
       

        //
        private void GetLaplacianConstant(IList<double> result)
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

                    double value = Values[index];
                    double sum = 0.0;

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


        //
        private void GetLaplacianEqual(IList<double> result)
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

                    double value = Values[index];
                    double sum = 0.0;

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


        //
        private void GetLaplacianPeriodic(IList<double> result)
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

                    double value = Values[index];
                    double sum = 0.0;

                    // x
                    if (i == 0)
                        sum += (Values[index - 1 + CountX] + Values[index + 1] - 2.0 * value) * dx;
                    else if (i == CountX - 1)
                        sum += (Values[index - 1] + Values[index + 1 - CountX] - 2.0 * value) * dx;
                    else
                        sum += (Values[index - 1] + Values[index + 1] - 2.0 * value) * dx;

                    // y
                    if(j == 0)
                        sum += (Values[index - CountX + Count] + Values[index + CountX] - 2.0 * value) * dy;
                    else if(j == CountY - 1)
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
        public VectorField2d GetGradient()
        {
            VectorField2d result = new VectorField2d(this);
            UpdateGradient(result.Values);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateGradient(VectorField2d result)
        {
            UpdateGradient(result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateGradient(IList<Vec2d> result)
        {
            _getGradient(result);
        }


        //
        private void GetGradientConstant(IList<Vec2d> result)
        {
            // inverse step size for each dimension
            double dx = 1.0 / (2.0 * ScaleX);
            double dy = 1.0 / (2.0 * ScaleY);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    double gx, gy;

                    //x
                    if (i == 0)
                        gx = (Values[index + 1] - BoundaryValue) * dx;
                    else if (i == CountX - 1)
                        gx = (BoundaryValue - Values[index - 1]) * dx;
                    else
                        gx = (Values[index + 1] - Values[index - 1]) * dx;

                    //y
                    if (j == 0)
                        gy = (Values[index + CountX] - BoundaryValue) * dy;
                    else if (j == CountY - 1)
                        gy = (BoundaryValue - Values[index - CountX]) * dy;
                    else
                        gy = (Values[index + CountX] - Values[index - CountX]) * dy;

                    result[index] = new Vec2d(gx, gy);
                }
            });
        }


        //
        private void GetGradientEqual(IList<Vec2d> result)
        {
            // inverse step size for each dimension
            double dx = 1.0 / (2.0 * ScaleX);
            double dy = 1.0 / (2.0 * ScaleY);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    double value = Values[index];
                    double gx, gy;

                    //x
                    if (i == 0)
                        gx = (Values[index + 1] - value) * dx;
                    else if (i == CountX - 1)
                        gx = (value - Values[index - 1]) * dx;
                    else
                        gx = (Values[index + 1] - Values[index - 1]) * dx;

                    //y
                    if (j == 0)
                        gy = (Values[index + CountX] - value) * dy;
                    else if (j == CountY - 1)
                        gy = (value - Values[index - CountX]) * dy;
                    else
                        gy = (Values[index + CountX] - Values[index - CountX]) * dy;

                    result[index] = new Vec2d(gx, gy);
                }
            });
        }


        //
        private void GetGradientPeriodic(IList<Vec2d> result)
        {
            // inverse step size for each dimension
            double dx = 1.0 / (2.0 * ScaleX);
            double dy = 1.0 / (2.0 * ScaleY);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    double gx, gy;

                    //x
                    if (i == 0)
                        gx = (Values[index + 1] - Values[index - 1 + CountX]) * dx;
                    else if (i == CountX - 1)
                        gx = (Values[index + 1 - CountX] - Values[index - 1]) * dx;
                    else
                        gx = (Values[index + 1] - Values[index - 1]) * dx;

                    //y
                    if (j == 0)
                        gy = (Values[index + CountX] - Values[index - CountX + Count]) * dy;
                    else if (j == CountY - 1)
                        gy = (Values[index + CountX - Count] - Values[index - CountX]) * dy;
                    else
                        gy = (Values[index + CountX] - Values[index - CountX]) * dy;

                    result[index] = new Vec2d(gx, gy);
                }
            });
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
