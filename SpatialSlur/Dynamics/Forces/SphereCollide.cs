
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
    public class SphereCollide : Impl.PositionForce
    {
        #region Static

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
        /// <param name="handles"></param>
        /// <param name="radius"></param>
        /// <param name="strength"></param>
        /// <param name="parallel"></param>
        public SphereCollide(IEnumerable<ParticleHandle> handles, double radius, double strength = 1.0, bool parallel = false)
        {
            SetHandles(handles);
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
        public override void Calculate(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        {
            var handles = Handles;

            if (_grid == null)
                _grid = new HashGrid3d<Vector3d>(handles.Count);

            // Update grid parameters
            _grid.Scale = Radius * _radiusToGridScale;

            // Insert particle positions into grid
            foreach (var h in handles)
            {
                var p = positions[h.PositionIndex].Current;
                _grid.Insert(p, p);
            }

            // Range search from each particle position
            if (_parallel)
                ForEach(Partitioner.Create(0, handles.Count), range => Calculate(range.Item1, range.Item2));
            else
                Calculate(0, handles.Count);
            
            void Calculate(int from, int to)
            {
                var deltas = Deltas;
                var diam = _radius * 2.0;
                var diamSqr = diam * diam;

                for (int i = from; i < to; i++)
                {
                    var p0 = positions[handles[i].PositionIndex].Current;
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

                    deltas[i] = count > 0 ? sum * (_strength / count) : Vector3d.Zero; // Average for stability
                }
            }

            _grid.Clear();
        }
    }
}
