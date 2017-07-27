using System;
using System.Collections.Generic;
using System.Linq;
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
    public class OnPoint : MultiParticleConstraint<H>
    {
        /// <summary></summary>
        public Vec3d Position;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="weight"></param>
        public OnPoint(Vec3d point, double weight = 1.0)
            : base(weight)
        {
            Position = point;
        }


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
        /// <param name="indices"></param>
        /// <param name="point"></param>
        /// <param name="weight"></param>
        public OnPoint(IEnumerable<int> indices, Vec3d point, double weight = 1.0)
            : base(weight)
        {
            Handles.AddRange(indices.Select(i => new H(i)));
            Position = point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            foreach (var h in Handles)
            {
                h.Delta = Position - particles[h].Position;
                h.Weight = Weight;
            }
        }
    }
}
