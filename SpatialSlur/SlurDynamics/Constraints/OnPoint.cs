using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurDynamics.Constraints
{
    using H = VariableSphereCollide.Handle;

    /// <summary>
    /// 
    /// </summary>
    public class OnPoint<P> : DynamicConstraint<P, H>
        where P : IParticle
    {
        private Vec3d Position;
      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public OnPoint(Vec3d point, int capacity, double weight = 1.0)
            : base(capacity, weight)
        {
            Position = point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="point"></param>
        /// <param name="weight"></param>
        public OnPoint(IEnumerable<H> handles, Vec3d point, double weight = 1.0)
            : base(handles, weight)
        {
            Position = point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override void Calculate(IReadOnlyList<P> particles)
        {
            foreach(var h in Handles)
                h.Delta = Position - particles[h].Position;
        }
    }
}
