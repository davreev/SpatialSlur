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

namespace SpatialSlur.SlurDynamics.Constraints
{
    using H = ParticleHandle;

    /// <summary>
    /// 
    /// </summary>
    public class SphereCollide<P> : DynamicConstraint<P, H>
        where P : IParticle
    {
        private const double TargetBinScale = 2.0;
        private const double MinBinScale = 0.5;
        private const double MaxBinScale = 4.0;
        private const double GridPadding = 1.0e-8;

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
        /// <param name="capacity"></param>
        /// <param name="radius"></param>
        /// <param name="weight"></param>
        public SphereCollide(int capacity, double radius, double weight = 1.0)
            : base(capacity, weight)
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
        public override void Calculate(IReadOnlyList<P> particles)
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

                    _grid.Search(new Domain3d(p0, rad2), h1 =>
                    {
                        if (h0.Index == h1.Index) return true;
                        
                        var delta = particles[h1].Position - p0;
                        var m = delta.SquareLength;

                        if (m < rad2Sqr && m > 0.0)
                            deltaSum += delta * ((1.0 - rad2 / Math.Sqrt(m)) * 0.5);

                        return true;
                    });

                    h0.Delta += deltaSum;
                }
            });
            
            _grid.Clear();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        private void UpdateGrid(IReadOnlyList<P> particles)
        {
            Domain3d d = new Domain3d(particles.Select(p => p.Position));
            d.Expand(GridPadding);

            // lazy instantiation
            if (_grid == null)
            {
                InitGrid(d, _radius * TargetBinScale);
                return;
            }

            // rebuild grid if bins are too large or too small in any one dimension
            _grid.Domain = d;
            double t0 = _radius * MaxBinScale;
            double t1 = _radius * MinBinScale;
   
            if (!(Contains(_grid.BinScaleX, t0, t1) && Contains(_grid.BinScaleY, t0, t1) && Contains(_grid.BinScaleZ, t0, t1)))
                InitGrid(d, _radius * TargetBinScale);
        }


        /// <summary>
        /// 
        /// </summary>
        private void InitGrid(Domain3d domain, double binScale)
        {
            int nx = (int)Math.Ceiling(domain.X.Span / binScale);
            int ny = (int)Math.Ceiling(domain.Y.Span / binScale);
            int nz = (int)Math.Ceiling(domain.Z.Span / binScale);
            _grid = new SpatialGrid3d<H>(domain, nx, ny, nz);
        }
    }
}
