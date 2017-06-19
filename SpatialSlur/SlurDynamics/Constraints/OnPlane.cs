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
    public class OnPlane<P> : DynamicConstraint<P, H>
        where P : IParticle
    {
        public Vec3d Origin;
        public Vec3d Normal;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public OnPlane(int index, Vec3d origin, Vec3d normal, int capacity, double weight = 1.0)
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
        public OnPlane(IEnumerable<H> handles, Vec3d origin, Vec3d normal, double weight = 1.0)
            :base(handles, weight)
        {
            Origin = origin;
            Normal = normal;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override void Calculate(IReadOnlyList<P> particles)
        {
            foreach(var h in Handles)
                h.Delta = Vec3d.Project(Origin - particles[h].Position, Normal);
        }
    }
}
