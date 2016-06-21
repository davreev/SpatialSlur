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
    public class VectorField3d : Field3d<Vec3d>
    {
        // delegates for boundary dependant functions
        private Action<IList<Vec3d>> _getLaplacian;
        private Action<IList<double>> _getDivergence;
        private Action<IList<Vec3d>> _getCurl;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="boundaryType"></param>
        public VectorField3d(Domain3d domain, int countX, int countY, int countZ, FieldBoundaryType boundaryType = FieldBoundaryType.Equal)
            : base(domain, countX, countY, countZ, boundaryType)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public VectorField3d(Field3d other)
            : base(other)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public VectorField3d(VectorField3d other)
            : base(other)
        {
        }


        /// <summary>
        /// Updates any boundary sensitive methods
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
        public override Vec3d Evaluate(FieldPoint3d point)
        {
            int[] corners = point.Corners;
            double[] weights = point.Weights;

            Vec3d result = new Vec3d();
            for (int i = 0; i < 8; i++)
                result += Values[corners[i]] * weights[i];

            return result;
        }


        /// <summary>
        /// Gets the magnitudes of all vectors in the field.
        /// </summary>
        /// <returns></returns>
        public ScalarField3d GetMagnitudes()
        {
            ScalarField3d result = new ScalarField3d(this);
            UpdateMagnitudes(result.Values);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateMagnitudes(ScalarField3d result)
        {
            UpdateMagnitudes(result.Values);
        }


        /// <summary>
        /// 
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
        public void Unitize(VectorField3d result)
        {
            Unitize(result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public void Unitize(IList<Vec3d> result)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    Vec3d v = Values[i];
                    v.Unitize();
                    result[i] = v;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Cross(VectorField3d other)
        {
            Function(Vec3d.Cross, other);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        public void Cross(IList<Vec3d> vectors)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] = Vec3d.Cross(Values[i], vectors[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public void Cross(VectorField3d other, VectorField3d result)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result.Values[i] = Vec3d.Cross(Values[i], other.Values[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="result"></param>
        public void Cross(IList<Vec3d> vectors, IList<Vec3d> result)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Cross(Values[i], vectors[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="factor"></param>
        public void LerpTo(Vec3d value, double factor)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] = Vec3d.Lerp(Values[i], value, factor);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        public void LerpTo(VectorField3d other, double factor)
        {
            LerpTo(other.Values, factor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="factor"></param>
        public void LerpTo(IList<Vec3d> vectors, double factor)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] = Vec3d.Lerp(Values[i], vectors[i], factor);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="factors"></param>
        public void LerpTo(Vec3d value, ScalarField3d factors)
        {
            LerpTo(value, factors.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="factors"></param>
        public void LerpTo(Vec3d value, IList<double> factors)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] = Vec3d.Lerp(Values[i], value, factors[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factors"></param>
        public void LerpTo(VectorField3d other, ScalarField3d factors)
        {
            LerpTo(other.Values, factors.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="factors"></param>
        public void LerpTo(IList<Vec3d> vectors, IList<double> factors)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] = Vec3d.Lerp(Values[i], vectors[i], factors[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public VectorField3d GetLaplacian()
        {
            VectorField3d result = new VectorField3d((Field3d)this);
            UpdateLaplacian(result.Values);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateLaplacian(VectorField3d result)
        {
            UpdateLaplacian(result.Values);
        }


        /// <summary>
        /// http://en.wikipedia.org/wiki/Discrete_Laplace_operator
        /// </summary>
        /// <param name="result"></param>
        public void UpdateLaplacian(IList<Vec3d> result)
        {
            _getLaplacian(result);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetLaplacianConstant(IList<Vec3d> result)
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

                    Vec3d value = Values[index];
                    Vec3d sum = new Vec3d();

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
                    if (k == 0)
                        sum += (BoundaryValue + Values[index + CountXY] - 2.0 * value) * dz;
                    else if (k == CountZ - 1)
                        sum += (Values[index - CountXY] + BoundaryValue - 2.0 * value) * dz;
                    else
                        sum += (Values[index - CountXY] + Values[index + CountXY] - 2.0 * value) * dz;

                    result[index] = sum;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetLaplacianEqual(IList<Vec3d> result)
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

                    Vec3d value = Values[index];
                    Vec3d sum = new Vec3d();

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


        /// <summary>
        /// 
        /// </summary>
        private void GetLaplacianPeriodic(IList<Vec3d> result)
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

                    Vec3d value = Values[index];
                    Vec3d sum = new Vec3d();

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
                        sum += (Values[index - CountXY + Count] + Values[index + CountXY] - 2.0 * value) * dz;
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
        public ScalarField3d GetDivergence()
        {
            ScalarField3d result = new ScalarField3d(this);
            UpdateDivergence(result.Values);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateDivergence(ScalarField3d result)
        {
            UpdateDivergence(result.Values);
        }


        /// <summary>
        /// http://www.math.harvard.edu/archive/21a_spring_09/PDF/13-05-curl-and-divergence.pdf
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
            double dz = 1.0 / (2.0 * ScaleZ);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

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

                    //z
                    if (k == 0)
                        sum += (Values[index + CountXY].z - BoundaryValue.z) * dz;
                    else if (k == CountZ - 1)
                        sum += (BoundaryValue.z - Values[index - CountXY].z) * dz;
                    else
                        sum += (Values[index + CountXY].z - Values[index - CountXY].z) * dz;

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
            double dz = 1.0 / (2.0 * ScaleZ);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    Vec3d value = Values[index];
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

                    //z
                    if (k == 0)
                        sum += (Values[index + CountXY].z - value.z) * dz;
                    else if (k == CountZ - 1)
                        sum += (value.z - Values[index - CountXY].z) * dz;
                    else
                        sum += (Values[index + CountXY].z - Values[index - CountXY].z) * dz;

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
            double dz = 1.0 / (2.0 * ScaleZ);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

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
                        sum += (Values[index + CountX].y - Values[index - CountX + CountXY].y) * dy;
                    else if (j == CountY - 1)
                        sum += (Values[index + CountX - CountXY].y - Values[index - CountX].y) * dy;
                    else
                        sum += (Values[index + CountX].y - Values[index - CountX].y) * dy;

                    //z
                    if (k == 0)
                        sum += (Values[index + CountXY].z - Values[index - CountXY + Count].z) * dz;
                    else if (k == CountZ - 1)
                        sum += (Values[index + CountXY - Count].z - Values[index - CountXY].z) * dz;
                    else
                        sum += (Values[index + CountXY].z - Values[index - CountXY].z) * dz;

                    result[index] = sum;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public VectorField3d GetCurl()
        {
            VectorField3d result = new VectorField3d((Field3d)this);
            UpdateCurl(result.Values);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateCurl(VectorField3d result)
        {
            UpdateCurl(result.Values);
        }


        /// <summary>
        /// http://www.math.harvard.edu/archive/21a_spring_09/PDF/13-05-curl-and-divergence.pdf
        /// </summary>
        /// <param name="result"></param>
        public void UpdateCurl(IList<Vec3d> result)
        {
            _getCurl(result);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetCurlConstant(IList<Vec3d> result)
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

                    double dp, dq, dr;

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

                    //z
                    if (k == 0)
                        dr = Values[index + CountXY].z - BoundaryValue.z;
                    else if (k == CountZ - 1)
                        dr = BoundaryValue.z - Values[index - CountXY].z;
                    else
                        dr = Values[index + CountXY].z - Values[index - CountXY].z;

                    result[index] = new Vec3d(
                        dr * dy - dq * dz,
                        dp * dz - dr * dx, 
                        dq * dx - dp * dy);
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetCurlEqual(IList<Vec3d> result)
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

                    Vec3d value = Values[i];
                    double dp, dq, dr;

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

                    //z
                    if (k == 0)
                        dr = Values[index + CountXY].z - value.z;
                    else if (k == CountZ - 1)
                        dr = value.z - Values[index - CountXY].z;
                    else
                        dr = Values[index + CountXY].z - Values[index - CountXY].z;

                    result[index] = new Vec3d(
                        dr * dy - dq * dz,
                        dp * dz - dr * dx,
                        dq * dx - dp * dy);
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetCurlPeriodic(IList<Vec3d> result)
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

                    double dp, dq, dr;

                    //x
                    if (i == 0)
                        dp = Values[index + 1].x - Values[index - 1 + CountX].x;
                    else if (i == CountX - 1)
                        dp = Values[index + 1 - CountX].x - Values[index - 1].x;
                    else
                        dp = Values[index + 1].x - Values[index - 1].x;

                    //y
                    if (j == 0)
                        dq = Values[index + CountX].y - Values[index - CountX + CountXY].y;
                    else if (j == CountY - 1)
                        dq = Values[index + CountX - CountXY].y - Values[index - CountX].y;
                    else
                        dq = Values[index + CountX].y - Values[index - CountX].y;

                    //z
                    if (k == 0)
                        dr = Values[index + CountXY].z - Values[index - CountXY + Count].z;
                    else if (k == CountZ - 1)
                        dr = Values[index + CountXY - Count].z - Values[index - CountXY].z;
                    else
                        dr = Values[index + CountXY].z - Values[index - CountXY].z;

                    result[index] = new Vec3d(
                        dr * dy - dq * dz,
                        dp * dz - dr * dx,
                        dq * dx - dp * dy);
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("VectorField3d ({0} x {1} x {2})", CountX, CountY, CountZ);
        }
    }
}
