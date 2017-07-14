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
    public class SphereCollide : DynamicPositionConstraint<H>
    {
        private const double TargetBinScale = 4.0;

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
        private void UpdateGrid(IReadOnlyList<IBody> particles)
        {
            // recalculate domain
            Domain3d d = new Domain3d(particles.Select(p => p.Position));
            d.Expand(_radius);

            // lazy instantiation
            if (_grid == null)
            {
                _grid = new SpatialGrid3d<H>(d, _radius * TargetBinScale);
                return;
            }

            // rebuild grid if bins are too large or too small in any one dimension
            _grid.Domain = d;
            double maxScale = _radius * TargetBinScale * 2.0;
            double minScale = _radius * TargetBinScale * 0.5;

            // if bin scale is out of range, rebuild
            if (!Contains(_grid.BinScaleX, minScale, maxScale) || !Contains(_grid.BinScaleY, minScale, maxScale) || !Contains(_grid.BinScaleZ, minScale, maxScale))
                _grid = new SpatialGrid3d<H>(d, _radius * TargetBinScale);
        }
    }
}
