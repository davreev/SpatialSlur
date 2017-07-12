using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurDynamics.Constraints
{
    using H = ParticleHandle;

    /// <summary>
    /// 
    /// </summary>
    public class TangentialSmooth : DynamicConstraint<H>
    {
        /// <summary>
        /// 
        /// </summary>
        protected override sealed bool AppliesRotation
        {
            get { return false; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public TangentialSmooth(Vec3d normal, int capacity, double weight = 1.0)
            : base(capacity, weight)
        {
            Handles.Add(new H()); // add central vertex
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="weight"></param>
        public TangentialSmooth(IEnumerable<H> handles, double weight = 1.0)
            :base(handles,weight)
        {
            Handles.Add(new H()); // add central vertex
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
            var n = ComputeNormal(particles);
            var d = Vec3d.Reject((sum * nInv - particles[h0].Position) * 0.5, n);

            // apply to central particle
            h0.Delta = d;

            // distribute reverse among neighbours
            d *= -nInv;
            for (int i = 1; i < Handles.Count; i++)
                Handles[i].Delta = d;
        }


        /// <summary>
        /// Calculates the normal as the sum of triangle area gradients
        /// </summary>
        /// <returns></returns>
        private Vec3d ComputeNormal(IReadOnlyList<IParticle> particles)
        {
            var p = particles[Handles[0]].Position;
            var sum = new Vec3d();
            var n = Handles.Count - 1;

            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                var p0 = particles[Handles[i + 1]].Position;
                var p1 = particles[Handles[j + 1]].Position;
                sum += GeometryUtil.GetTriAreaGrad(p, p0, p1);
            }

            return sum;
        }
    }
}
