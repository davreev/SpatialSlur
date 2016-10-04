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
            VecMath.AddParallel(Values, other.Values, Count, Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public void Add(ScalarField2d other, ScalarField2d result)
        {
            VecMath.AddParallel(Values, other.Values, Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Multiply(ScalarField2d other)
        {
            VecMath.MultiplyParallel(Values, other.Values, Count, Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public void Multiply(ScalarField2d other, ScalarField2d result)
        {
            VecMath.MultiplyParallel(Values, other.Values, Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Divide(ScalarField2d other)
        {
            VecMath.DivideParallel(Values, other.Values, Count, Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public void Divide(ScalarField2d other, ScalarField2d result)
        {
            VecMath.DivideParallel(Values, other.Values, Count, result.Values);
        }


        /// <summary>
        /// Linerly interpolates values in this field towards corresponding values in another.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        public void LerpTo(ScalarField2d other, double factor)
        {
            VecMath.LerpParallel(Values, other.Values, factor, Count, Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <param name="result"></param>
        public void LerpTo(ScalarField2d other, double factor, ScalarField2d result)
        {
            VecMath.LerpParallel(Values, other.Values, factor, Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factors"></param>
        public void LerpTo(ScalarField2d other, ScalarField2d factors)
        {
            VecMath.LerpParallel(Values, other.Values, factors.Values, Count, Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factors"></param>
        /// <param name="result"></param>
        public void LerpTo(ScalarField2d other, ScalarField2d factors, ScalarField2d result)
        {
            VecMath.LerpParallel(Values, other.Values, factors.Values, Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public void Normalize()
        {
            VecMath.NormalizeParallel(Values, new Domain(Values), Count, Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public void Normalize(ScalarField2d result)
        {
            VecMath.NormalizeParallel(Values, new Domain(Values), Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="to"></param>
        public void Remap(Domain to)
        {
            VecMath.RemapParallel(Values, new Domain(Values), to, Count, Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="result"></param>
        public void Remap(Domain to, ScalarField2d result)
        {
            VecMath.RemapParallel(Values, new Domain(Values), to, Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void Remap(Domain from, Domain to)
        {
            VecMath.RemapParallel(Values, from, to, Count, Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="result"></param>
        public void Remap(Domain from, Domain to, ScalarField2d result)
        {
            VecMath.RemapParallel(Values, from, to, Count, result.Values);
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
