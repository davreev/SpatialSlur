using System;
using System.Collections.Generic;
using System.Linq;

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
    public class LaplacianSmooth : DynamicPositionConstraint<H>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        public LaplacianSmooth(double weight = 1.0)
            : base(weight)
        {
            Handles.Add(new H()); // add central vertex
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public LaplacianSmooth(int capacity, double weight = 1.0)
            : base(capacity, weight)
        {
            Handles.Add(new H()); // add central vertex
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="weight"></param>
        public LaplacianSmooth(IEnumerable<H> handles, double weight = 1.0)
            : base(handles, weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IParticle> particles)
        {
            Vec3d sum = new Vec3d();
            double nInv = 1.0 / (Handles.Count - 1);

            for (int i = 1; i < Handles.Count; i++)
                sum += particles[Handles[i]].Position;

            var h0 = Handles[0];
            var d = (sum * nInv - particles[h0].Position) * 0.5;

            // apply to central particle
            h0.Delta = d;

            // distribute reverse among neighbours
            d *= -nInv;
            for (int i = 1; i < Handles.Count; i++)
                Handles[i].Delta = d;
        }
    }
}
