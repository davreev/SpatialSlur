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
        /// Sets the result field to the sum of this field and another.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public void Add(ScalarField3d other, ScalarField3d result)
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
        /// Sets this field to the difference between itself and another.
        /// </summary>
        /// <param name="other"></param>
        public void Subtract(ScalarField3d other)
        {
            Subtract(other.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public void Subtract(IList<double> values)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] -= values[i];
            });
        }


        /// <summary>
        /// Sets the result field to the difference between this field and another.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public void Subtract(ScalarField3d other, ScalarField3d result)
        {
            Subtract(other.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="result"></param>
        public void Subtract(IList<double> values, IList<double> result)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Values[i] - values[i];
            });
        }


        /// <summary>
        /// Sets this field to the sum of itself and another scaled by a given factor.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        public void AddScaled(ScalarField3d other, double factor)
        {
            AddScaled(other.Values, factor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="factor"></param>
        public void AddScaled(IList<double> values, double factor)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] += values[i] * factor;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factors"></param>
        public void AddScaled(ScalarField3d other, ScalarField3d factors)
        {
            AddScaled(other.Values, factors.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="factors"></param>
        public void AddScaled(IList<double> values, IList<double> factors)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] += values[i] * factors[i];
            });
        }


        /// <summary>
        /// Sets this field to the product of itself and another.
        /// </summary>
        /// <param name="other"></param>
        public void Multiply(ScalarField3d other)
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
        /// Sets the result field to the product of this field and another.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public void Multiply(ScalarField3d other, ScalarField3d result)
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
        /// <param name="other"></param>
        public void Divide(ScalarField3d other)
        {
            Divide(other.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public void Divide(IList<double> values)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] /= values[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public void Divide(ScalarField3d other, ScalarField3d result)
        {
            Divide(other.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="result"></param>
        public void Divide(IList<double> values, IList<double> result)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Values[i] / values[i];
            });
        }


        /// <summary>
        /// Linerly interpolates values in this field towards another value.
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
        /// Linerly interpolates values in this field towards corresponding values in another.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        public void LerpTo(ScalarField3d other, double factor)
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
        public void LerpTo(double value, ScalarField3d factors)
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
        public void LerpTo(ScalarField3d other, ScalarField3d factors)
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

                    // z
                    if(k == 0)
                        sum += (BoundaryValue + Values[index + CountXY] - 2.0 * value) * dz;
                    else if(k == CountZ -1)
                        sum += (Values[index - CountXY] + BoundaryValue - 2.0 * value) * dz;
                    else
                        sum += (Values[index - CountXY] + Values[index + CountXY] - 2.0 * value) * dz;

                    result[index] = sum;
                }
            });
        }


        //
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

                    // z
                    if (k == 0)
                        sum += (Values[index + CountXY] - value) * dz;
                    else if (k == CountZ - 1)
                        sum += (Values[index - CountXY] - value) * dz;
                    else
                        sum += (Values[index - CountXY] + Values[index + CountXY] - 2.0 * value) * dz;

                    result[index] = sum;
                }
            });
        }


        //
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
                    if (j == 0)
                        sum += (Values[index - CountX + CountXY] + Values[index + CountX] - 2.0 * value) * dy;
                    else if (j == CountY - 1)
                        sum += (Values[index - CountX] + Values[index + CountX - CountXY] - 2.0 * value) * dy;
                    else
                        sum += (Values[index - CountX] + Values[index + CountX] - 2.0 * value) * dy;

                    // z
                    if (k == 0)
                        sum += (Values[index - CountXY + Count] + Values[index +CountXY] - 2.0 * value) * dz;
                    else if (k == CountZ - 1)
                        sum += (Values[index - CountXY] + Values[index + CountXY - Count] - 2.0 * value) * dz;
                    else
                        sum += (Values[index - CountXY] + Values[index + CountXY] - 2.0 * value) * dz;

                    result[index] = sum;
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

                    double gx,gy,gz;

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

                    //z
                    if(k == 0)
                        gz = (Values[index + CountXY] - BoundaryValue) * dy;
                    else if(k == CountZ-1)
                        gz = (BoundaryValue - Values[index - CountXY]) * dy;
                    else
                        gz = (Values[index + CountXY] - Values[index - CountXY]) * dy;

                    result[index] = new Vec3d(gx, gy, gz);
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

                    double value = Values[index];
                    double gx, gy, gz;

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

                    //z
                    if (k == 0)
                        gz = (Values[index + CountXY] - value) * dy;
                    else if (k == CountZ - 1)
                        gz = (value - Values[index - CountXY]) * dy;
                    else
                        gz = (Values[index + CountXY] - Values[index - CountXY]) * dy;

                    result[index] = new Vec3d(gx, gy, gz);
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

                    double gx, gy, gz;

                    //x
                    if (i == 0)
                        gx = (Values[index + 1] - Values[index - 1 + CountX]) * dx;
                    else if (i == CountX - 1)
                        gx = (Values[index + 1 - CountX] - Values[index - 1]) * dx;
                    else
                        gx = (Values[index + 1] - Values[index - 1]) * dx;

                    //y
                    if (j == 0)
                        gy = (Values[index + CountX] - Values[index - CountX + CountXY]) * dy;
                    else if (j == CountY - 1)
                        gy = (Values[index + CountX - CountXY] - Values[index - CountX]) * dy;
                    else
                        gy = (Values[index + CountX] - Values[index - CountX]) * dy;

                    //z
                    if (k == 0)
                        gz = (Values[index + CountXY] - Values[index - CountXY + Count]) * dy;
                    else if (k == CountZ - 1)
                        gz = (Values[index + CountXY - Count] - Values[index - CountXY]) * dy;
                    else
                        gz = (Values[index + CountXY] - Values[index - CountXY]) * dy;

                    result[index] = new Vec3d(gx, gy, gz);
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
