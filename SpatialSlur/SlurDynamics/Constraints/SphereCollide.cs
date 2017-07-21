using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurData;

using static SpatialSlur.SlurCore.SlurMath;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    using H = ParticleHandle;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SphereCollide : DynamicPositionConstraint<H>
    {
        private const double TargetBinScale = 4.0; // as a factor of radius

        private SpatialGrid3d<H> _grid;
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

                    var delta = new Vec3d();
                    var weight = 0.0;

                    _grid.Search(new Domain3d(p0, rad2), h1 =>
                    {
                        if (h0.Index == h1.Index) return true;
                        
                        var d = particles[h1].Position - p0;
                        var m = d.SquareLength;

                        if (m < rad2Sqr && m > 0.0)
                        {
                            delta += d * ((1.0 - rad2 / Math.Sqrt(m)) * 0.5);
                            weight = Weight;
                        }

                        return true;
                    });

                    h0.Delta = delta;
                    h0.Weight = weight;
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
                _grid = new SpatialGrid3d<H>(d0, _radius * TargetBinScale);
                return;
            }

            var d1 = _grid.Domain;
            UpdateGridDomain(Domain3d.Union(d0, d1 + (d0.Mid - d1.Mid)));
        }


        /// <summary>
        /// Sets the domain of the grid and resizes if necessary.
        /// </summary>
        /// <param name="domain"></param>
        private void UpdateGridDomain(Domain3d domain)
        {
            _grid.Domain = domain;
            var dx = _grid.BinScaleX;
            var dy = _grid.BinScaleY;
            var dz = _grid.BinScaleZ;

            // rebuild grid if bins are too large in any one dimension
            double max = _radius * TargetBinScale * 4.0;

            if (dx > max || dy > max || dz > max)
                _grid = new SpatialGrid3d<H>(domain, _radius * TargetBinScale);
        }


        /*
        /// <summary>
        /// Sets the domain of the grid and resizes if necessary.
        /// </summary>
        /// <param name="domain"></param>
        private void UpdateGridDomain(Domain3d domain)
        {
            _grid.Domain = domain;
            var dx = _grid.BinScaleX;
            var dy = _grid.BinScaleY;
            var dz = _grid.BinScaleZ;

            // rebuild grid if bins are too large or small in any one dimension
            double min = _radius * TargetBinScale * 0.25;
            double max = _radius * TargetBinScale * 4.0;
 
            if (dx < min || dy < min || dz < min || dx > max || dy > max || dz > max)
                _grid = new SpatialGrid3d<H>(domain, _radius * TargetBinScale);
        }
        */
    }
}
