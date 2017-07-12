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
    public class OnCircle : DynamicConstraint<H>
    {
        /// <summary></summary>
        public Vec3d Origin;
        /// <summary></summary>
        public Vec3d Normal;

        private double _radius;
      

        /// <summary>
        /// 
        /// </summary>
        public double Radius
        {
            get { return _radius; }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("The value cannot be negative.");

                _radius = value;
            }
        }


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
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <param name="radius"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public OnCircle(Vec3d origin, Vec3d normal, double radius, int capacity, double weight = 1.0)
            : base(capacity, weight)
        {
            Origin = origin;
            Normal = normal;
            Radius = radius;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <param name="radius"></param>
        /// <param name="weight"></param>
        public OnCircle(IEnumerable<H> handles, Vec3d origin, Vec3d normal, double radius, double weight = 1.0)
            :base(handles, weight)
        {
            Origin = origin;
            Normal = normal;
            Radius = radius;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IParticle> particles)
        {
            foreach(var h in Handles)
            {
                Vec3d p = particles[h].Position;

                Vec3d d = Vec3d.Reject(p - Origin, Normal);
                d *= _radius / d.Length;

                h.Delta = ((Origin + d) - p);
            }
        }
    }
}
