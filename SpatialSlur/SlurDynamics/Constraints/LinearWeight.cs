using System;
using System.Collections.Generic;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    using H = ParticleHandle;

    /// <summary>
    /// Applies a force proportional to the distance between 2 particles.
    /// </summary>
    [Serializable]
    public class LinearWeight : ParticleConstraint<H>
    {
        private H _h0 = new H();
        private H _h1 = new H();

        /// <summary>Describes the direction and magnitude of the applied weight</summary>
        public Vec3d Vector;
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
        public override sealed IEnumerable<H> Handles
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
        /// <param name="vector"></param>
        /// <param name="massPerLength"></param>
        /// <param name="weight"></param>
        public LinearWeight(int start, int end, Vec3d vector, double massPerLength = 1.0, double weight = 1.0)
        {
            _h0.Index = start;
            _h1.Index = end;

            Vector = vector;
            MassPerLength = massPerLength;
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            _h0.Delta = _h1.Delta = Vector * (particles[_h0].Position.DistanceTo(particles[_h1].Position) * _massPerLength * 0.5);
            _h0.Weight = _h1.Weight = Weight;
        }
    }
}
