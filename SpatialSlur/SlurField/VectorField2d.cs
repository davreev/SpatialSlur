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
        /// <param name="indices"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public override Vec2d Evaluate(int[] indices, double[] weights)
        {
            Vec2d result = new Vec2d();
            for (int i = 0; i < indices.Length; i++)
                result += Values[indices[i]] * weights[i];

            return result;
        }


        /// <summary>
        /// Gets the magnitudes of all vectors in the field
        /// </summary>
        /// <returns></returns>
        public ScalarField2d GetMagnitudes()
        {
            ScalarField2d result = new ScalarField2d(this);
            GetMagnitudes(result.Values);
            return result;
        }


        /// <summary>
        /// Gets the magnitudes of all vectors in the field
        /// </summary>
        /// <param name="result"></param>
        public void GetMagnitudes(ScalarField2d result)
        {
            GetMagnitudes(result.Values);
        }


        /// <summary>
        /// Gets the magnitudes of all vectors in the field
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
            GetLaplacian(result.Values);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void GetLaplacian(VectorField2d result)
        {
            _getLaplacian(result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void GetLaplacian(IList<Vec2d> result)
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

                    Vec2d tx0 = (i == 0) ? BoundaryValue : Values[index - 1];
                    Vec2d tx1 = (i == CountX - 1) ? BoundaryValue : Values[index + 1];

                    Vec2d ty0 = (j == 0) ? BoundaryValue : Values[index - CountX];
                    Vec2d ty1 = (j == CountY - 1) ? BoundaryValue : Values[index + CountX];

                    Vec2d t = Values[index] * 2.0;
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy;
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

                    Vec2d tx0 = (i == 0) ? Values[index] : Values[index - 1];
                    Vec2d tx1 = (i == CountX - 1) ? Values[index] : Values[index + 1];

                    Vec2d ty0 = (j == 0) ? Values[index] : Values[index - CountX];
                    Vec2d ty1 = (j == CountY - 1) ? Values[index] : Values[index + CountX];

                    Vec2d t = Values[index] * 2.0;
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy;
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

                    Vec2d tx0 = (i == 0) ? Values[index - 1 + CountX] : Values[index - 1];
                    Vec2d tx1 = (i == CountX - 1) ? Values[index + 1 - CountX] : Values[index + 1];

                    Vec2d ty0 = (j == 0) ? Values[index - CountX + Count] : Values[index - CountX];
                    Vec2d ty1 = (j == CountY - 1) ? Values[index + CountX - Count] : Values[index + CountX];

                    Vec2d t = Values[index] * 2.0;
                    result[index] = (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy;
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
            GetDivergence(result.Values);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void GetDivergence(ScalarField2d result)
        {
            _getDivergence(result.Values);
        }


        /// <summary>
        /// 
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

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    Vec2d tx0 = (i == 0) ? BoundaryValue : Values[index - 1];
                    Vec2d tx1 = (i == CountX - 1) ? BoundaryValue : Values[index + 1];

                    Vec2d ty0 = (j == 0) ? BoundaryValue : Values[index - CountX];
                    Vec2d ty1 = (j == CountY - 1) ? BoundaryValue : Values[index + CountX];

                    result[index] = (tx1.x - tx0.x) * dx + (ty1.y - ty0.y) * dy; 
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

                    Vec2d tx0 = (i == 0) ? Values[index] : Values[index - 1];
                    Vec2d tx1 = (i == CountX - 1) ? Values[index] : Values[index + 1];

                    Vec2d ty0 = (j == 0) ? Values[index] : Values[index - CountX];
                    Vec2d ty1 = (j == CountY - 1) ? Values[index] : Values[index + CountX];

                    result[index] = (tx1.x - tx0.x) * dx + (ty1.y - ty0.y) * dy; 
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

                    Vec2d tx0 = (i == 0) ? Values[index - 1 + CountX] : Values[index - 1];
                    Vec2d tx1 = (i == CountX - 1) ? Values[index + 1 - CountX] : Values[index + 1];

                    Vec2d ty0 = (j == 0) ? Values[index - CountX + Count] : Values[index - CountX];
                    Vec2d ty1 = (j == CountY - 1) ? Values[index + CountX - Count] : Values[index + CountX];

                    result[index] = (tx1.x - tx0.x) * dx + (ty1.y - ty0.y) * dy; 
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
            GetCurl(result.Values);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void GetCurl(ScalarField2d result)
        {
            _getCurl(result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void GetCurl(IList<double> result)
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

                    Vec2d tx0 = (i == 0) ? BoundaryValue : Values[index - 1];
                    Vec2d tx1 = (i == CountX - 1) ? BoundaryValue : Values[index + 1];

                    Vec2d ty0 = (j == 0) ? BoundaryValue : Values[index - CountX];
                    Vec2d ty1 = (j == CountY - 1) ? BoundaryValue : Values[index + CountX];

                    result[index] = (tx1.y - tx0.y) * dx - (ty1.x - ty0.x) * dy;
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

                    Vec2d tx0 = (i == 0) ? Values[index] : Values[index - 1];
                    Vec2d tx1 = (i == CountX - 1) ? Values[index] : Values[index + 1];

                    Vec2d ty0 = (j == 0) ? Values[index] : Values[index - CountX];
                    Vec2d ty1 = (j == CountY - 1) ? Values[index] : Values[index + CountX];

                    result[index] = (tx1.y - tx0.y) * dx - (ty1.x - ty0.x) * dy;
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

                    Vec2d tx0 = (i == 0) ? Values[index - 1 + CountX] : Values[index - 1];
                    Vec2d tx1 = (i == CountX - 1) ? Values[index + 1 - CountX] : Values[index + 1];

                    Vec2d ty0 = (j == 0) ? Values[index - CountX + Count] : Values[index - CountX];
                    Vec2d ty1 = (j == CountY - 1) ? Values[index + CountX - Count] : Values[index + CountX];

                    result[index] = (tx1.y - tx0.y) * dx - (ty1.x - ty0.x) * dy;
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
