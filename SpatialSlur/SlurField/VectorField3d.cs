using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static VectorField3d CreateFromFGA(string path)
        {
            var content = File.ReadAllText(path,Encoding.ASCII);
            var values = content.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);

            int nx = int.Parse(values[0]);
            int ny = int.Parse(values[1]);
            int nz = int.Parse(values[2]);

            Vec3d p0 = new Vec3d(
                double.Parse(values[3]),
                double.Parse(values[4]),
                double.Parse(values[5]));

            Vec3d p1 = new Vec3d(
               double.Parse(values[6]),
               double.Parse(values[7]),
               double.Parse(values[8]));

            VectorField3d result = new VectorField3d(new Domain3d(p0, p1), nx, ny, nz);
            var vecs = result.Values;
            int index = 0;

            for (int i = 9; i < values.Length; i += 3)
            {
                vecs[index++] = new Vec3d(
                    double.Parse(values[i]),
                    double.Parse(values[i + 1]),
                    double.Parse(values[i + 2]));
            }

            return result;
        }

        #endregion


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
        /// <param name="indices"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public override Vec3d Evaluate(int[] indices, double[] weights)
        {
            Vec3d result = new Vec3d();
            for (int i = 0; i < indices.Length; i++)
                result += Values[indices[i]] * weights[i];

            return result;
        }


        /// <summary>
        /// Gets the magnitudes of all vectors in the field.
        /// </summary>
        /// <returns></returns>
        public ScalarField3d GetMagnitudes()
        {
            ScalarField3d result = new ScalarField3d(this);
            GetMagnitudes(result.Values);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void GetMagnitudes(ScalarField3d result)
        {
            GetMagnitudes(result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void GetMagnitudes(IList<double> result)
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
            GetLaplacian(result.Values);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void GetLaplacian(VectorField3d result)
        {
            _getLaplacian(result.Values);
        }


        /// <summary>
        /// http://en.wikipedia.org/wiki/Discrete_Laplace_operator
        /// </summary>
        /// <param name="result"></param>
        public void GetLaplacian(IList<Vec3d> result)
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

                    Vec3d tx0 = (i == 0) ? BoundaryValue : Values[index - 1];
                    Vec3d tx1 = (i == CountX - 1) ? BoundaryValue : Values[index + 1];

                    Vec3d ty0 = (j == 0) ? BoundaryValue : Values[index - CountX];
                    Vec3d ty1 = (j == CountY - 1) ? BoundaryValue : Values[index + CountX];

                    Vec3d tz0 = (k == 0) ? BoundaryValue : Values[index - CountXY];
                    Vec3d tz1 = (k == CountZ - 1) ? BoundaryValue : Values[index + CountXY];

                    Vec3d t = Values[index] * 2.0;
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy + (tz0 + tz1 - t) * dz;
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

                    Vec3d tx0 = (i == 0) ? Values[index] : Values[index - 1];
                    Vec3d tx1 = (i == CountX - 1) ? Values[index] : Values[index + 1];

                    Vec3d ty0 = (j == 0) ? Values[index] : Values[index - CountX];
                    Vec3d ty1 = (j == CountY - 1) ? Values[index] : Values[index + CountX];

                    Vec3d tz0 = (k == 0) ? Values[index] : Values[index - CountXY];
                    Vec3d tz1 = (k == CountZ - 1) ? Values[index] : Values[index + CountXY];

                    Vec3d t = Values[index] * 2.0;
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy + (tz0 + tz1 - t) * dz;
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

                    Vec3d tx0 = (i == 0) ? Values[index - 1 + CountX] : Values[index - 1];
                    Vec3d tx1 = (i == CountX - 1) ? Values[index + 1 - CountX] : Values[index + 1];

                    Vec3d ty0 = (j == 0) ? Values[index - CountX + CountXY] : Values[index - CountX];
                    Vec3d ty1 = (j == CountY - 1) ? Values[index + CountX - CountXY] : Values[index + CountX];

                    Vec3d tz0 = (k == 0) ? Values[index - CountXY + Count] : Values[index - CountXY];
                    Vec3d tz1 = (k == CountZ - 1) ? Values[index + CountXY - Count] : Values[index + CountXY];

                    Vec3d t = Values[index] * 2.0;
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy + (tz0 + tz1 - t) * dz;
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
            GetDivergence(result.Values);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void GetDivergence(ScalarField3d result)
        {
            _getDivergence(result.Values);
        }


        /// <summary>
        /// http://www.math.harvard.edu/archive/21a_spring_09/PDF/13-05-curl-and-divergence.pdf
        /// </summary>
        /// <param name="result"></param>
        public void GetDivergence(IList<double> result)
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

                    Vec3d tx0 = (i == 0) ? BoundaryValue : Values[index - 1];
                    Vec3d tx1 = (i == CountX - 1) ? BoundaryValue : Values[index + 1];

                    Vec3d ty0 = (j == 0) ? BoundaryValue : Values[index - CountX];
                    Vec3d ty1 = (j == CountY - 1) ? BoundaryValue : Values[index + CountX];

                    Vec3d tz0 = (k == 0) ? BoundaryValue : Values[index - CountXY];
                    Vec3d tz1 = (k == CountZ - 1) ? BoundaryValue : Values[index + CountXY];

                    result[index] = (tx1.x - tx0.x) * dx + (ty1.y - ty0.y) * dy + (tz1.z + tz0.z) * dz;
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

                    Vec3d tx0 = (i == 0) ? Values[index] : Values[index - 1];
                    Vec3d tx1 = (i == CountX - 1) ? Values[index] : Values[index + 1];

                    Vec3d ty0 = (j == 0) ? Values[index] : Values[index - CountX];
                    Vec3d ty1 = (j == CountY - 1) ? Values[index] : Values[index + CountX];

                    Vec3d tz0 = (k == 0) ? Values[index] : Values[index - CountXY];
                    Vec3d tz1 = (k == CountZ - 1) ? Values[index] : Values[index + CountXY];

                    result[index] = (tx1.x - tx0.x) * dx + (ty1.y - ty0.y) * dy + (tz1.z + tz0.z) * dz;
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

                    Vec3d tx0 = (i == 0) ? Values[index - 1 + CountX] : Values[index - 1];
                    Vec3d tx1 = (i == CountX - 1) ? Values[index + 1 - CountX] : Values[index + 1];

                    Vec3d ty0 = (j == 0) ? Values[index - CountX + CountXY] : Values[index - CountX];
                    Vec3d ty1 = (j == CountY - 1) ? Values[index + CountX - CountXY] : Values[index + CountX];

                    Vec3d tz0 = (k == 0) ? Values[index - CountXY + Count] : Values[index - CountXY];
                    Vec3d tz1 = (k == CountZ - 1) ? Values[index + CountXY - Count] : Values[index + CountXY];

                    result[index] = (tx1.x - tx0.x) * dx + (ty1.y - ty0.y) * dy + (tz1.z + tz0.z) * dz;
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
            GetCurl(result.Values);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void GetCurl(VectorField3d result)
        {
            _getCurl(result.Values);
        }


        /// <summary>
        /// http://www.math.harvard.edu/archive/21a_spring_09/PDF/13-05-curl-and-divergence.pdf
        /// </summary>
        /// <param name="result"></param>
        public void GetCurl(IList<Vec3d> result)
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

                    Vec3d tx0 = (i == 0) ? BoundaryValue : Values[index - 1];
                    Vec3d tx1 = (i == CountX - 1) ? BoundaryValue : Values[index + 1];

                    Vec3d ty0 = (j == 0) ? BoundaryValue : Values[index - CountX];
                    Vec3d ty1 = (j == CountY - 1) ? BoundaryValue : Values[index + CountX];

                    Vec3d tz0 = (k == 0) ? BoundaryValue : Values[index - CountXY];
                    Vec3d tz1 = (k == CountZ - 1) ? BoundaryValue : Values[index + CountXY];

                    result[index] = new Vec3d(
                        (ty1.z - ty0.z) * dy - (tz1.y - tz0.y) * dz,
                        (tz1.x - tz0.x) * dz - (tx1.z - tx0.z) * dx,
                        (tx1.y - tx0.y) * dx - (ty1.x - ty0.x) * dy);
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

                    Vec3d tx0 = (i == 0) ? Values[index] : Values[index - 1];
                    Vec3d tx1 = (i == CountX - 1) ? Values[index] : Values[index + 1];

                    Vec3d ty0 = (j == 0) ? Values[index] : Values[index - CountX];
                    Vec3d ty1 = (j == CountY - 1) ? Values[index] : Values[index + CountX];

                    Vec3d tz0 = (k == 0) ? Values[index] : Values[index - CountXY];
                    Vec3d tz1 = (k == CountZ - 1) ? Values[index] : Values[index + CountXY];

                    result[index] = new Vec3d(
                        (ty1.z - ty0.z) * dy - (tz1.y - tz0.y) * dz,
                        (tz1.x - tz0.x) * dz - (tx1.z - tx0.z) * dx,
                        (tx1.y - tx0.y) * dx - (ty1.x - ty0.x) * dy);
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

                    Vec3d tx0 = (i == 0) ? Values[index - 1 + CountX] : Values[index - 1];
                    Vec3d tx1 = (i == CountX - 1) ? Values[index + 1 - CountX] : Values[index + 1];

                    Vec3d ty0 = (j == 0) ? Values[index - CountX + CountXY] : Values[index - CountX];
                    Vec3d ty1 = (j == CountY - 1) ? Values[index + CountX - CountXY] : Values[index + CountX];

                    Vec3d tz0 = (k == 0) ? Values[index - CountXY + Count] : Values[index - CountXY];
                    Vec3d tz1 = (k == CountZ - 1) ? Values[index + CountXY - Count] : Values[index + CountXY];

                    result[index] = new Vec3d(
                        (ty1.z - ty0.z) * dy - (tz1.y - tz0.y) * dz,
                        (tz1.x - tz0.x) * dz - (tx1.z - tx0.z) * dx,
                        (tx1.y - tx0.y) * dx - (ty1.x - ty0.x) * dy);
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
