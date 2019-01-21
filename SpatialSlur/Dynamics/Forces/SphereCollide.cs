﻿
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using SpatialSlur.Collections;

using static System.Threading.Tasks.Parallel;

namespace SpatialSlur.Dynamics.Forces
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SphereCollide : PositionForceBase
    {
        #region Static Members

        private const double _radiusToGridScale = 5.0;

        #endregion
        
        private HashGrid3d<Vector3d> _grid;
        private double _radius = 1.0;
        private double _strength;
        private bool _parallel;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="strength"></param>
        /// <param name="parallel"></param>
        public SphereCollide(double radius, double strength = 1.0, bool parallel = false)
        {
            Radius = radius;
            _strength = strength;
            _parallel = parallel;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="radius"></param>
        /// <param name="strength"></param>
        /// <param name="parallel"></param>
        public SphereCollide(IEnumerable<int> indices, double radius, double strength = 1.0, bool parallel = false)
            : base(indices)
        {
            Radius = radius;
            _strength = 1.0;
            _parallel = parallel;
        }


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
        public double Strength
        {
            get { return _strength; }
            set { _strength = value; }
        }


        /// <summary>
        /// If true, this constraint is calculated in parallel.
        /// </summary>
        public bool Parallel
        {
            get { return _parallel; }
            set { _parallel = value; }
        }
        

        /// <inheritdoc />
        /// 
        protected override void Calculate(
            ReadOnlyArrayView<Particle> particles, 
            ReadOnlyArrayView<int> indices, 
            ArrayView<Vector3d> deltas)
        {
            if (_grid == null)
                _grid = new HashGrid3d<Vector3d>(indices.Count);

            // update grid parameters
            _grid.Scale = Radius * _radiusToGridScale;

            // insert particle positions into grid
            for (int i = 0; i < indices.Count; i++)
            {
                var p = particles[indices[i]].Position.Current;
                _grid.Insert(p, p);
            }

            // perform radial search from each particle position
            if (_parallel)
                ForEach(Partitioner.Create(0, indices.Count), range => Calculate(range.Item1, range.Item2));
            else
                Calculate(0, indices.Count);
            
            void Calculate(int from, int to)
            {
                var diam = _radius * 2.0;
                var diamSqr = diam * diam;

                for (int i = from; i < to; i++)
                {
                    var p0 = particles[indices[i]].Position.Current;
                    var sum = Vector3d.Zero;
                    var count = 0;

                    foreach (var p1 in _grid.Search(new Interval3d(p0, diam)))
                    {
                        var d = p1 - p0;
                        var m = d.SquareLength;

                        if (m < diamSqr && m > 0.0)
                        {
                            sum += d * (1.0 - diam / Math.Sqrt(m));
                            count++;
                        }
                    }

                    deltas[i] = count > 0 ? sum * (_strength / count) : Vector3d.Zero; // average for stability
                }
            }

            _grid.Clear();
        }
    }
}
