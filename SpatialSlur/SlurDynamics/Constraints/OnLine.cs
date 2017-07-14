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
    public class OnLine : DynamicPositionConstraint<H>
    {
        /// <summary></summary>
        public Vec3d Start;
        /// <summary> </summary>
        public Vec3d Direction;

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="direction"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public OnLine(Vec3d start, Vec3d direction, int capacity, double weight = 1.0)
            : base(capacity, weight)
        {
            Start = start;
            Direction = direction;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="start"></param>
        /// <param name="direction"></param>
        /// <param name="weight"></param>
        public OnLine(IEnumerable<H> handles, Vec3d start, Vec3d direction, double weight = 1.0)
            : base(handles, weight)
        {
            Start = start;
            Direction = direction;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IParticle> particles)
        {
            foreach(var h in Handles)
                h.Delta = Vec3d.Reject(Start - particles[h].Position, Direction);
        }
    }
}
