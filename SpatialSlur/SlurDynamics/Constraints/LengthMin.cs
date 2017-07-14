using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * Notes 
 */

namespace SpatialSlur.SlurDynamics.Constraints
{
    using H = PositionHandle;

    /// <summary>
    ///
    /// </summary>
    public class MinimizeLength : PositionConstraint<H>
    {
        private H _h0 = new H();
        private H _h1 = new H();


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
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="weight"></param>
        public MinimizeLength(int start, int end, double weight = 1.0)
        {
            SetHandles(start, end);
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IParticle> particles)
        {
            _h0.Delta = (particles[_h1].Position - particles[_h0].Position) * 0.5;
            _h1.Delta = -_h0.Delta;
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
