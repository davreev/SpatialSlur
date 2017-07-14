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
    public class AbovePlane : DynamicPositionConstraint<H>
    {
        /// <summary></summary>
        public Vec3d Origin;
        /// <summary></summary>
        public Vec3d Normal;
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <param name="weight"></param>
        public AbovePlane(Vec3d origin, Vec3d normal, double weight = 1.0)
            : base(weight)
        {
            Origin = origin;
            Normal = normal;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public AbovePlane(Vec3d origin, Vec3d normal, int capacity, double weight = 1.0)
            : base(capacity, weight)
        {
            Origin = origin;
            Normal = normal;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <param name="weight"></param>
        public AbovePlane(IEnumerable<H> handles, Vec3d origin, Vec3d normal, double weight = 1.0)
            : base(handles, weight)
        {
            Origin = origin;
            Normal = normal;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IParticle> particles)
        {
            foreach(var h in Handles)
            {
                double d = (Origin - particles[h].Position) * Normal;

                if (d > 0.0)
                    h.Delta = (d / Normal.SquareLength * Normal);
                else
                    h.Delta = new Vec3d();
            }
        }
    }
}
