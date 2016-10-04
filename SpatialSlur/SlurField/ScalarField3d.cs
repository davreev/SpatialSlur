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
    public class ScalarField3d : Field3d<double>
    {
        // delegates for boundary dependant functions
        private Action<IList<double>> _getLaplacian;
        private Action<IList<Vec3d>> _getGradient;

     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="boundaryType"></param>
        public ScalarField3d(Domain3d domain, int countX, int countY, int countZ, FieldBoundaryType boundaryType = FieldBoundaryType.Equal)
            : base(domain, countX, countY, countZ, boundaryType)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public ScalarField3d(Field3d other)
            : base(other)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public ScalarField3d(ScalarField3d other)
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
        public override double Evaluate(FieldPoint3d point)
        {
            int[] corners = point.Corners;
            double[] weights = point.Weights;

            double result = 0.0;
            for (int i = 0; i < 8; i++)
                result += Values[corners[i]] * weights[i];

            return result;
        }


        /// <summary>
        /// Sets this field to the sum of itself and another.
        /// </summary>
        /// <param name="other"></param>
        public void Add(ScalarField3d other)
        {
            VecMath.AddParallel(Values, other.Values, Count, Values);
        }


        /// <summary>
        /// Sets the result field to the sum of this field and another.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public void Add(ScalarField3d other, ScalarField3d result)
        {
            VecMath.AddParallel(Values, other.Values, Count, result.Values);
        }


        /// <summary>
        /// Sets this field to the difference between itself and another.
        /// </summary>
        /// <param name="other"></param>
        public void Subtract(ScalarField3d other)
        {
            VecMath.SubtractParallel(Values, other.Values, Count, Values);
        }


        /// <summary>
        /// Sets the result field to the difference between this field and another.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public void Subtract(ScalarField3d other, ScalarField3d result)
        {
            VecMath.SubtractParallel(Values, other.Values, Count, result.Values);
        }


        /// <summary>
        /// Sets this field to the sum of itself and another scaled by a given factor.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        public void AddScaled(ScalarField3d other, double factor)
        {
            VecMath.AddScaledParallel(Values, other.Values, factor, Count, Values);
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factors"></param>
        public void AddScaled(ScalarField3d other, ScalarField3d factors)
        {
            VecMath.AddScaledParallel(Values, other.Values, factors.Values, Count, Values);
        }
        */


        /// <summary>
        /// Sets this field to the sum of itself and another scaled by a given factor.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <param name="result"></param>
        public void AddScaled(ScalarField3d other, double factor, ScalarField3d result)
        {
            VecMath.AddScaledParallel(Values, other.Values, factor, Count, result.Values);
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factors"></param>
        public void AddScaled(ScalarField3d other, ScalarField3d factors, ScalarField3d result)
        {
            VecMath.AddScaledParallel(Values, other.Values, factors.Values, Count, result.Values);
        }
        */


        /// <summary>
        /// Sets this field to the product of itself and another.
        /// </summary>
        /// <param name="other"></param>
        public void Multiply(ScalarField3d other)
        {
            VecMath.MultiplyParallel(Values, other.Values, Count, Values);
        }


        /// <summary>
        /// Sets the result field to the product of this field and another.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public void Multiply(ScalarField3d other, ScalarField3d result)
        {
            VecMath.MultiplyParallel(Values, other.Values, Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Divide(ScalarField3d other)
        {
            VecMath.DivideParallel(Values, other.Values, Count, Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public void Divide(ScalarField3d other, ScalarField3d result)
        {
            VecMath.DivideParallel(Values, other.Values, Count, result.Values);
        }


        /// <summary>
        /// Linerly interpolates values in this field towards corresponding values in another.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        public void LerpTo(ScalarField3d other, double factor)
        {
            VecMath.LerpParallel(Values, other.Values, factor, Count, Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <param name="result"></param>
        public void LerpTo(ScalarField3d other, double factor, ScalarField3d result)
        {
            VecMath.LerpParallel(Values, other.Values, factor, Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factors"></param>
        public void LerpTo(ScalarField3d other, ScalarField3d factors)
        {
            VecMath.LerpParallel(Values, other.Values, factors.Values, Count, Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factors"></param>
        /// <param name="result"></param>
        public void LerpTo(ScalarField3d other, ScalarField3d factors, ScalarField3d result)
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
        public void Normalize(ScalarField3d result)
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
        public void Remap(Domain to, ScalarField3d result)
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
        public void Remap(Domain from, Domain to, ScalarField3d result)
        {
            VecMath.RemapParallel(Values, from, to, Count, result.Values);
        }


        /// <summary>
        /// http://en.wikipedia.org/wiki/Discrete_Laplace_operator
        /// </summary>
        /// <returns></returns>
        public ScalarField3d GetLaplacian()
        {
            ScalarField3d result = new ScalarField3d((Field3d)this);
            UpdateLaplacian(result.Values);
            return result;
        }


        /// <summary>
        /// http://en.wikipedia.org/wiki/Discrete_Laplace_operator
        /// </summary>
        /// <param name="result"></param>
        public void UpdateLaplacian(ScalarField3d result)
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
        private void GetLaplacianConstant(IList<double> result)
        {
            double dx = 1.0 / (ScaleX * ScaleX);
            double dy = 1.0 / (ScaleY * ScaleY);
            double dz = 1.0 / (ScaleZ * ScaleZ);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    double tx0 = (i == 0) ? BoundaryValue : Values[index - 1];
                    double tx1 = (i == CountX - 1) ? BoundaryValue : Values[index + 1];
                    
                    double ty0 = (j == 0) ? BoundaryValue : Values[index - CountX];
                    double ty1 = (j == CountY - 1) ? BoundaryValue : Values[index + CountX];

                    double tz0 = (k == 0) ? BoundaryValue : Values[index - CountXY];
                    double tz1 = (k == CountZ - 1) ? BoundaryValue : Values[index + CountXY];
                  
                    double t = Values[index] * 2.0;
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy + (tz0 + tz1 - t) * dz;
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
            double dz = 1.0 / (ScaleZ * ScaleZ);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    double tx0 = (i == 0) ? Values[index] : Values[index - 1];
                    double tx1 = (i == CountX - 1) ? Values[index] : Values[index + 1];

                    double ty0 = (j == 0) ? Values[index] : Values[index - CountX];
                    double ty1 = (j == CountY - 1) ? Values[index] : Values[index + CountX];

                    double tz0 = (k == 0) ? Values[index] : Values[index - CountXY];
                    double tz1 = (k == CountZ - 1) ? Values[index] : Values[index + CountXY];
         
                    double t = Values[index] * 2.0;
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy + (tz0 + tz1 - t) * dz;
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
            double dz = 1.0 / (ScaleZ * ScaleZ);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    double tx0 = (i == 0) ? Values[index - 1 + CountX] : Values[index - 1];
                    double tx1 = (i == CountX - 1) ? Values[index + 1 - CountX] : Values[index + 1];

                    double ty0 = (j == 0) ? Values[index - CountX + CountXY] : Values[index - CountX];
                    double ty1 = (j == CountY - 1) ? Values[index + CountX - CountXY] : Values[index + CountX];

                    double tz0 = (k == 0) ? Values[index - CountXY + Count] : Values[index - CountXY];
                    double tz1 = (k == CountZ - 1) ? Values[index + CountXY - Count] : Values[index + CountXY];

                    double t = Values[index] * 2.0;
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy + (tz0 + tz1 - t) * dz;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public VectorField3d GetGradient()
        {
            VectorField3d result = new VectorField3d(this);
            UpdateGradient(result.Values);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateGradient(VectorField3d result)
        {
            UpdateGradient(result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateGradient(IList<Vec3d> result)
        {
            _getGradient(result);
        }
    

        /// <summary>
        /// 
        /// </summary>
        private void GetGradientConstant(IList<Vec3d> result)
        {
            double dx = 1.0 / (2.0 * ScaleX);
            double dy = 1.0 / (2.0 * ScaleY);
            double dz = 1.0 / (2.0 * ScaleZ);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    double tx0 = (i == 0) ? BoundaryValue : Values[index - 1];
                    double tx1 = (i == CountX - 1) ? BoundaryValue : Values[index + 1];

                    double ty0 = (j == 0) ? BoundaryValue : Values[index - CountX];
                    double ty1 = (j == CountY - 1) ? BoundaryValue : Values[index + CountX];

                    double tz0 = (k == 0) ? BoundaryValue : Values[index - CountXY];
                    double tz1 = (k == CountZ - 1) ? BoundaryValue : Values[index + CountXY];

                    result[index] = new Vec3d((tx1 - tx0) * dx, (ty1 - ty0) * dy, (tz1 - tz0) * dz);
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetGradientEqual(IList<Vec3d> result)
        {
            double dx = 1.0 / (2.0 * ScaleX);
            double dy = 1.0 / (2.0 * ScaleY);
            double dz = 1.0 / (2.0 * ScaleZ);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    double tx0 = (i == 0) ? Values[index] : Values[index - 1];
                    double tx1 = (i == CountX - 1) ? Values[index] : Values[index + 1];

                    double ty0 = (j == 0) ? Values[index] : Values[index - CountX];
                    double ty1 = (j == CountY - 1) ? Values[index] : Values[index + CountX];

                    double tz0 = (k == 0) ? Values[index] : Values[index - CountXY];
                    double tz1 = (k == CountZ - 1) ? Values[index] : Values[index + CountXY];

                    result[index] = new Vec3d((tx1 - tx0) * dx, (ty1 - ty0) * dy, (tz1 - tz0) * dz);
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetGradientPeriodic(IList<Vec3d> result)
        {
            double dx = 1.0 / (2.0 * ScaleX);
            double dy = 1.0 / (2.0 * ScaleY);
            double dz = 1.0 / (2.0 * ScaleZ);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    double tx0 = (i == 0) ? Values[index - 1 + CountX] : Values[index - 1];
                    double tx1 = (i == CountX - 1) ? Values[index + 1 - CountX] : Values[index + 1];

                    double ty0 = (j == 0) ? Values[index - CountX + CountXY] : Values[index - CountX];
                    double ty1 = (j == CountY - 1) ? Values[index + CountX - CountXY] : Values[index + CountX];

                    double tz0 = (k == 0) ? Values[index - CountXY + Count] : Values[index - CountXY];
                    double tz1 = (k == CountZ - 1) ? Values[index + CountXY - Count] : Values[index + CountXY];

                    result[index] = new Vec3d((tx1 - tx0) * dx, (ty1 - ty0) * dy, (tz1 - tz0) * dz);
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("ScalarField3d ({0} x {1} x {2})", CountX, CountY, CountZ);
        }
    }
}
