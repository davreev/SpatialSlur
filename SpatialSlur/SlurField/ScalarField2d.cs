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
      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        private void GetLaplacianConstant(IList<double> result)
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

                    double tx0 = (i == 0) ? BoundaryValue : Values[index - 1];
                    double tx1 = (i == CountX - 1) ? BoundaryValue : Values[index + 1];
           
                    double ty0 = (j == 0) ? BoundaryValue : Values[index - CountX];
                    double ty1 = (j == CountY - 1) ? BoundaryValue : Values[index + CountX];

                    double t = Values[index] * 2.0;
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        private void GetLaplacianEqual(IList<double> result)
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

                    double tx0 = (i == 0) ? Values[index] : Values[index - 1];
                    double tx1 = (i == CountX - 1) ? Values[index] : Values[index + 1];

                    double ty0 = (j == 0) ? Values[index] : Values[index - CountX];
                    double ty1 = (j == CountY - 1) ? Values[index] : Values[index + CountX];

                    double t = Values[index] * 2.0;
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        private void GetLaplacianPeriodic(IList<double> result)
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

                    double tx0 = (i == 0) ? Values[index - 1 + CountX] : Values[index - 1];
                    double tx1 = (i == CountX - 1) ? Values[index + 1 - CountX] : Values[index + 1];

                    double ty0 = (j == 0) ? Values[index - CountX + Count] : Values[index - CountX];
                    double ty1 = (j == CountY - 1) ? Values[index + CountX - Count] : Values[index + CountX];

                    double t = Values[index] * 2.0;
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy;
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        private void GetGradientConstant(IList<Vec2d> result)
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

                    double tx0 = (i == 0) ? BoundaryValue : Values[index - 1];
                    double tx1 = (i == CountX - 1) ? BoundaryValue : Values[index + 1];

                    double ty0 = (j == 0) ? BoundaryValue : Values[index - CountX];
                    double ty1 = (j == CountY - 1) ? BoundaryValue : Values[index + CountX];

                    result[index] = new Vec2d((tx1 - tx0) * dx, (ty1 - ty0) * dy);
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        private void GetGradientEqual(IList<Vec2d> result)
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

                    double tx0 = (i == 0) ? Values[index] : Values[index - 1];
                    double tx1 = (i == CountX - 1) ? Values[index] : Values[index + 1];

                    double ty0 = (j == 0) ? Values[index] : Values[index - CountX];
                    double ty1 = (j == CountY - 1) ? Values[index] : Values[index + CountX];

                    result[index] = new Vec2d((tx1 - tx0) * dx, (ty1 - ty0) * dy);
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        private void GetGradientPeriodic(IList<Vec2d> result)
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

                    double tx0 = (i == 0) ? Values[index - 1 + CountX] : Values[index - 1];
                    double tx1 = (i == CountX - 1) ? Values[index + 1 - CountX] : Values[index + 1];

                    double ty0 = (j == 0) ? Values[index - CountX + Count] : Values[index - CountX];
                    double ty1 = (j == CountY - 1) ? Values[index + CountX - Count] : Values[index + CountX];

                    result[index] = new Vec2d((tx1 - tx0) * dx, (ty1 - ty0) * dy);
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
