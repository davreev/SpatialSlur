using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;


/*
 * Notes 
 * TODO refactor names of methods that modify the field deltas
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DynamicScalarField3d : ScalarField3d
    {
        private readonly double[] _deltas;

        // delegates for boundary dependant functions
        private Action<double> _diffuse;
        private Action<double, double> _erode;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="boundaryType"></param>
        public DynamicScalarField3d(Domain3d domain, int countX, int countY, int countZ, FieldBoundaryType boundaryType = FieldBoundaryType.Equal)
            : base(domain, countX, countY, countZ, boundaryType)
        {
            _deltas = new double[Count + 1];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public DynamicScalarField3d(Field3d other)
            : base(other)
        {
            _deltas = new double[Count + 1];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public DynamicScalarField3d(ScalarField3d other)
            : base(other)
        {
            _deltas = new double[Count + 1];
        }


        /// <summary>
        /// 
        /// </summary>
        public double[] Deltas
        {
            get { return _deltas; }
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
                        _diffuse = DiffuseConstant;
                        _erode = ErodeConstant;
                        break;
                    }
                case FieldBoundaryType.Equal:
                    {
                        _diffuse = DiffuseEqual;
                        _erode = ErodeEqual;
                        break;
                    }
                case FieldBoundaryType.Periodic:
                    {
                        _diffuse = DiffusePeriodic;
                        _erode = ErodePeriodic;
                        break;
                    }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeStep"></param>
        public void Update(double timeStep)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    Values[i] += _deltas[i] * timeStep;
                    _deltas[i] = 0.0;
                }
            });
        }


        /// <summary>
        /// Adds the Laplacian of the field to the delta array. 
        /// http://en.wikipedia.org/wiki/Discrete_Laplace_operator
        /// </summary>
        /// <param name="rate"></param>
        public void Diffuse(double rate)
        {
            _diffuse(rate);
        }


        /// <summary>
        /// 
        /// </summary>
        private void DiffuseConstant(double rate)
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
                    _deltas[index] += (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy + (tz0 + tz1 - t) * dz;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        private void DiffuseEqual(double rate)
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
                    _deltas[index] += (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy + (tz0 + tz1 - t) * dz;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        private void DiffusePeriodic(double rate)
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
                    _deltas[index] += (tx0 + tx1 - t) * dx + (ty0 + ty1 - t) * dy + (tz0 + tz1 - t) * dz;
                }
            });
        }


        /// <summary>
        /// Simulates thermal erosion.
        /// http://micsymposium.org/mics_2011_proceedings/mics2011_submission_30.pdf
        /// </summary>
        /// <param name="slope"></param>
        /// <param name="rate"></param>
        public void Erode(double slope, double rate)
        {
            _erode(slope, rate);
        }


        /// <summary>
        /// 
        /// </summary>
        private void ErodeConstant(double slope, double rate)
        {
            // inverse step size for each dimension
            double dx = 1.0 / Math.Abs(ScaleX);
            double dy = 1.0 / Math.Abs(ScaleY);
            double dz = 1.0 / Math.Abs(ScaleZ);

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
                    double m, md;

                    //-x
                    m = (i == 0) ? BoundaryValue - value : Values[index - 1] - value;
                    m *= dx;
                    md = Math.Abs(m) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    //+x
                    m = (i == CountX - 1) ? BoundaryValue - value : Values[index + 1] - value;
                    m *= dx;
                    md = Math.Abs(m) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    //-y
                    m = (j == 0) ? BoundaryValue - value : Values[index - CountX] - value;
                    m *= dy;
                    md = Math.Abs(m) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    //+y
                    m = (j == CountY - 1) ? BoundaryValue - value : Values[index + CountX] - value;
                    m *= dy;
                    md = Math.Abs(m) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    //-z
                    m = (k == 0) ? BoundaryValue - value : Values[index - CountXY] - value;
                    m *= dz;
                    md = Math.Abs(m) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    //+z
                    m = (k == CountZ - 1) ? BoundaryValue - value : Values[index + CountXY] - value;
                    m *= dz;
                    md = Math.Abs(m) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    _deltas[index] += sum * rate;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        private void ErodeEqual(double slope, double rate)
        {
            // inverse step size for each dimension
            double dx = 1.0 / Math.Abs(ScaleX);
            double dy = 1.0 / Math.Abs(ScaleY);
            double dz = 1.0 / Math.Abs(ScaleZ);

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
                    double m, md;

                    //-x
                    if (i > 0)
                    {
                        m = (Values[index - 1] - value) * dx;
                        md = Math.Abs(m) - slope;
                        if (md > 0.0) sum += Math.Sign(m) * md;
                    }

                    //+x
                    if (i < CountX - 1)
                    {
                        m = (Values[index + 1] - value) * dx;
                        md = Math.Abs(m) - slope;
                        if (md > 0.0) sum += Math.Sign(m) * md;
                    }

                    //-y
                    if (j > 0)
                    {
                        m = (Values[index - CountX] - value) * dy;
                        md = Math.Abs(m) - slope;
                        if (md > 0.0) sum += Math.Sign(m) * md;
                    }

                    //+y
                    if (j < CountY - 1)
                    {
                        m = (Values[index + CountX] - value) * dy;
                        md = Math.Abs(m) - slope;
                        if (md > 0.0) sum += Math.Sign(m) * md;
                    }

                    //-z
                    if (k > 0)
                    {
                        m = (Values[index - CountXY] - value) * dz;
                        md = Math.Abs(m) - slope;
                        if (md > 0.0) sum += Math.Sign(m) * md;
                    }

                    //+z
                    if (k < CountZ - 1)
                    {
                        m = (Values[index + CountXY] - value) * dz;
                        md = Math.Abs(m) - slope;
                        if (md > 0.0) sum += Math.Sign(m) * md;
                    }

                    _deltas[index] += sum * rate;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        private void ErodePeriodic(double slope, double rate)
        {
            // inverse step size for each dimension
            double dx = 1.0 / Math.Abs(ScaleX);
            double dy = 1.0 / Math.Abs(ScaleY);
            double dz = 1.0 / Math.Abs(ScaleZ);

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
                    double m, md;

                    //-x
                    m = (i == 0) ? Values[index - 1 + CountX] - value : Values[index - 1] - value;
                    m *= dx;
                    md = Math.Abs(m) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    //+x
                    m = (i == CountX - 1) ? Values[index + 1 - CountX] - value : Values[index + 1] - value;
                    m *= dx;
                    md = Math.Abs(m) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    //-y
                    m = (j == 0) ? Values[index - CountX + CountXY] - value : Values[index - CountX] - value;
                    m *= dy;
                    md = Math.Abs(m) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    //+y
                    m = (j == CountY - 1) ? Values[index + CountX - CountXY] - value : Values[index + CountX] - value;
                    m *= dy;
                    md = Math.Abs(m) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    //-z
                    m = (k == 0) ? Values[index - CountXY + Count] - value : Values[index - CountXY] - value;
                    m *= dz;
                    md = Math.Abs(m) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    //+z
                    m = (k == CountZ - 1) ? Values[index + CountXY - Count] - value : Values[index + CountXY] - value;
                    m *= dz;
                    md = Math.Abs(m) - slope;
                    if (md > 0.0) sum += Math.Sign(m) * md;

                    _deltas[index] += sum * rate;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="thresh"></param>
        /// <param name="amount"></param>
        public void Bifurcate(double thresh, double amount)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    if (Values[i] > thresh)
                        _deltas[i] += amount;
                    else if (Values[i] < thresh)
                        _deltas[i] -= amount;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="thresh"></param>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <param name="rate"></param>
        public void Bifurcate(double thresh, double lower, double upper, double rate)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    if (Values[i] > thresh)
                        _deltas[i] += (upper - Values[i]) * rate;
                    else if (Values[i] < thresh)
                        _deltas[i] -= (lower - Values[i]) * rate;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        public void Deposit(double amount)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    _deltas[i] += amount;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="rate"></param>
        public void Deposit(double target, double rate)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    _deltas[i] += (target - Values[i]) * rate;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rate"></param>
        public void Decay(double rate)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    _deltas[i] -= Values[i] * rate;
            });
        }
  

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="amount"></param>
        public void DepositAt(int index, double amount)
        {
            _deltas[index] += amount;
        }
 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="amount"></param>
        public void DepositAt(FieldPoint3d point, double amount)
        {
            int[] corners = point.Corners;
            double[] weights = point.Weights;

            for (int i = 0; i < 8; i++)
                _deltas[corners[i]] += amount * weights[i];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="target"></param>
        /// <param name="rate"></param>
        public void DepositAt(int index, double target, double rate)
        {
            DepositAt(index, (target - Values[index]) * rate);
        }
     

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="target"></param>
        /// <param name="rate"></param>
        public void DepositAt(FieldPoint3d point, double target, double rate)
        {
            DepositAt(point, (target - Evaluate(point)) * rate);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="rate"></param>
        public void DecayAt(int index, double rate)
        {
            _deltas[index] -= Values[index] * rate;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="rate"></param>
        public void DecayAt(FieldPoint3d point, double rate)
        {
            DepositAt(point, -Evaluate(point) * rate);
        }
    }
}
