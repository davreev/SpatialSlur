using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

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
    public class LengthConstraint : ParticleConstraint<H>
    {
        private H _h0 = new H();
        private H _h1 = new H();

        private double _length;


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
        public double Length
        {
            get { return _length; }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("The value cannot be negative.");

                _length = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="length"></param>
        /// <param name="weight"></param>
        public LengthConstraint(int start, int end, double length, double weight = 1.0)
        {
            _h0.Index = start;
            _h1.Index = end;
            
            Length = length;
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            var d = particles[_h1].Position - particles[_h0].Position;
            _h0.Delta = d * (1.0 - _length / d.Length) * 0.5;
            _h1.Delta = -_h0.Delta;
            _h0.Weight = _h1.Weight = Weight;
        }
    }
}
