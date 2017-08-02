using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurData;

using static SpatialSlur.SlurData.DataUtil;
using static System.Threading.Tasks.Parallel;

/*
 * Notes
 * 
 * TODO check grid resizing
 */

namespace SpatialSlur.SlurDynamics
{
    using H = ParticleHandle;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SphereCollide : MultiParticleConstraint<H>
    {
        private const double TargetLoadFactor = 3.0;

        /// <summary>If set to true, collisions are calculated in parallel</summary>
        public bool Parallel;

        private HashGrid3d<H> _grid;
        private double _radius = 1.0;


        /// <summary>
        /// 
        /// </summary>
        public double Radius
        {
            get { return _radius; }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("The value can not be negative");

                _radius = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="parallel"></param>
        /// <param name="weight"></param>
        public SphereCollide(double radius, bool parallel, double weight = 1.0)
            : base(weight)
        {
            Radius = radius;
            Parallel = parallel;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="parallel"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public SphereCollide(double radius, bool parallel, int capacity, double weight = 1.0)
            : base(capacity, weight)
        {
            Radius = radius;
            Parallel = parallel;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="radius"></param>
        /// <param name="parallel"></param>
        /// <param name="weight"></param>
        public SphereCollide(IEnumerable<int> indices, double radius, bool parallel, double weight = 1.0)
            : base(weight)
        {
            Handles.AddRange(indices.Select(i => new H(i)));
            Radius = radius;
            Parallel = parallel;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            UpdateGrid(particles);

            if (Parallel)
                CalculateImplParallel(particles);
            else
                CalculateImpl(particles);

            _grid.Clear();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        private void CalculateImplParallel(IReadOnlyList<IBody> particles)
        {
            var diam = _radius * 2.0;
            var diamSqr = diam * diam;

            // insert particles
            foreach (var h in Handles)
                _grid.Insert(particles[h].Position, h);

            // search from particles
            ForEach(Partitioner.Create(0, Handles.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var h0 = Handles[i];
                    var p0 = particles[h0].Position;

                    var deltaSum = new Vec3d();
                    int count = 0;

                    _grid.Search(new Domain3d(p0, diam), h1 =>
                    {
                        var d = particles[h1].Position - p0;
                        var m = d.SquareLength;

                        if (m < diamSqr && m > 0.0)
                        {
                            deltaSum += d * (1.0 - diam / Math.Sqrt(m));
                            count++;
                        }

                        return true;
                    });

                    if (count == 0)
                    {
                        h0.Weight = 0.0;
                        continue;
                    }

                    h0.Delta = deltaSum * 0.5;
                    h0.Weight = Weight;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        private void CalculateImpl(IReadOnlyList<IBody> particles)
        {
            var diam = _radius * 2.0;
            var diamSqr = diam * diam;

            // clear handles
            foreach(var h in Handles)
            {
                h.Delta = Vec3d.Zero;
                h.Weight = 0.0;
            }

            // calculate collisions
            foreach(var h0 in Handles)
            {
                var p0 = particles[h0].Position;

                // search from h0
                foreach (var h1 in _grid.Search(new Domain3d(p0, diam)))
                {
                    var d = particles[h1].Position - p0;
                    var m = d.SquareLength;

                    if (m < diamSqr && m > 0.0)
                    {
                        d *= (1.0 - diam / Math.Sqrt(m)) * 0.5;
                        h0.Delta += d;
                        h1.Delta -= d;
                        h0.Weight = h1.Weight = Weight;
                    }
                }

                // insert h0
                _grid.Insert(p0, h0);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        private void UpdateGrid(IReadOnlyList<IBody> particles)
        {
            if (_grid == null)
            {
                _grid = new HashGrid3d<H>((int)(particles.Count * TargetLoadFactor), Radius * RadiusToBinScale);
                return;
            }

            _grid.BinScale = Radius * RadiusToBinScale;

            int minCount = (int)(particles.Count * TargetLoadFactor * 0.5);
            if (_grid.BinCount < minCount) _grid.Resize((int)(particles.Count * TargetLoadFactor));
        }
    }


    /*
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SphereCollide : DynamicPositionConstraint<H>
    {
        private const double TargetBinScale = 3.5; // as a factor of radius

        private FiniteGrid3d<H> _grid;
        private double _radius = 1.0;


        /// <summary>
        /// 
        /// </summary>
        public double Radius
        {
            get { return _radius; }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("The value can not be negative");

                _radius = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="weight"></param>
        public SphereCollide(double radius, double weight = 1.0)
            : base(weight)
        {
            Radius = radius;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public SphereCollide(double radius, int capacity, double weight = 1.0)
            : base(capacity, weight)
        {
            Radius = radius;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="radius"></param>
        /// <param name="weight"></param>
        public SphereCollide(IEnumerable<int> indices, double radius, double weight = 1.0)
            : base(indices.Select(i => new H(i)), weight)
        {
            Radius = radius;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="radius"></param>
        /// <param name="weight"></param>
        public SphereCollide(IEnumerable<H> handles, double radius, double weight = 1.0)
            : base(handles, weight)
        {
            Radius = radius;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            UpdateGrid(particles);

            var rad2 = _radius * 2.0;
            var rad2Sqr = rad2 * rad2;

            // insert particles
            foreach (var h in Handles)
                _grid.Insert(particles[h].Position, h);

            // search from particles
            Parallel.ForEach(Partitioner.Create(0, Handles.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var h0 = Handles[i];
                    var p0 = particles[h0].Position;

                    var deltaSum = new Vec3d();
                    int count = 0;

                    _grid.Search(new Domain3d(p0, rad2), h1 =>
                    {
                        var d = particles[h1].Position - p0;
                        var m = d.SquareLength;

                        if (m < rad2Sqr && m > 0.0)
                        {
                            deltaSum += d * (1.0 - rad2 / Math.Sqrt(m));
                            count++;
                        }

                        return true;
                    });

                    if (count == 0)
                    {
                        h0.Weight = 0.0;
                        continue;
                    }

                    h0.Delta = deltaSum * 0.5;
                    h0.Weight = Weight;
                }
            });

            _grid.Clear();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        private void UpdateGrid(IReadOnlyList<IBody> particles)
        {
            Domain3d d0 = new Domain3d(particles.Select(p => p.Position));
            d0.Expand(_radius);

            if (_grid == null)
            {
                _grid = new FiniteGrid3d<H>(d0, _radius * TargetBinScale);
                return;
            }

            //var d1 = _grid.Domain;
            //_grid.Domain = Domain3d.Union(d0, d1 + (d0.Mid - d1.Mid));

            _grid.Domain = d0;
            CheckBinScale();
        }


        /// <summary>
        /// Rebuilds the grid if the bins are too large or too small in any one dimension
        /// </summary>
        private void CheckBinScale()
        {
            var dx = _grid.BinScaleX;
            var dy = _grid.BinScaleY;
            var dz = _grid.BinScaleZ;

            double min = _radius * TargetBinScale * 0.5;
            double max = _radius * TargetBinScale * 2.0;

            if (dx < min || dy < min || dz < min || dx > max || dy > max || dz > max)
                _grid.Resize(_radius * TargetBinScale);
        }
    }
    */
}
