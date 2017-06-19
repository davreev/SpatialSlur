using System;
using System.Collections.Generic;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics.Constraints
{
    using H = ParticleHandle;

    /// <summary>
    /// Applies a force proportional to the distance between 2 particles.
    /// </summary>
    public class LinearWeight<P> : Constraint<P, H>
        where P : IParticle
    {
        private H _h0 = new H();
        private H _h1 = new H();

        public Vec3d Direction;
        private double _massPerLength;


        /// <summary>
        /// 
        /// </summary>
        public H Start
        {
            get { return _h0; }
        }


        /// <summary>
        /// 
        /// </summary>
        public H End
        {
            get { return _h1; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override IEnumerable<H> Handles
        {
            get
            {
                yield return _h0;
                yield return _h1;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double MassPerLength
        {
            get { return _massPerLength; }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentException("Mass must be greater than zero.");

                _massPerLength = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="index2"></param>
        /// <param name="index3"></param>
        /// <param name="restAngle"></param>
        /// <param name="weight"></param>
        public LinearWeight(int start, int end, Vec3d direction, double massPerLength = 1.0, double weight = 1.0)
        {
            SetHandles(start, end);
            Direction = direction;
            MassPerLength = massPerLength;
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override void Calculate(IReadOnlyList<P> particles)
        {
            _h0.Delta = _h1.Delta = Direction * (particles[_h0].Position.DistanceTo(particles[_h1].Position) * _massPerLength * 0.5);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void SetHandles(int start, int end)
        {
            _h0.Index = start;
            _h1.Index = end;
        }
    }
}
