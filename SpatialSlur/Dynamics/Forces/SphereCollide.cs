/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
        private double _strength = 1.0;
        private bool _parallel;


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


        /// <inheritdoc />
        public override void Calculate(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        {
            var particles = Particles;

            if (_grid == null)
                _grid = new HashGrid3d<Vector3d>(particles.Count);

            // Update grid parameters
            _grid.Scale = Radius * _radiusToGridScale;

            // Insert particle positions into grid
            foreach (var p in particles)
            {
                var pp = positions[p.PositionIndex].Current;
                _grid.Insert(pp, pp);
            }

            // Range search from each particle position
            if (_parallel)
                ForEach(Partitioner.Create(0, particles.Count), range => Calculate(range.Item1, range.Item2));
            else
                Calculate(0, particles.Count);
            
            void Calculate(int from, int to)
            {
                var deltas = Deltas;
                var dia = _radius * 2.0;
                var diaSqr = dia * dia;

                for (int i = from; i < to; i++)
                {
                    var pp0 = positions[particles[i].PositionIndex].Current;
                    var sum = Vector3d.Zero;
                    var count = 0;

                    foreach (var pp1 in _grid.Search(new Interval3d(pp0, dia)))
                    {
                        var d = pp1 - pp0;
                        var m = d.SquareLength;

                        if (m < diaSqr && m > 0.0)
                        {
                            sum += d * (1.0 - dia / Math.Sqrt(m));
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
