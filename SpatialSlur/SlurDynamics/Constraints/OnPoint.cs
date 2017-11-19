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
        public Vec3d Point;
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public OnPoint(Vec3d point, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
            Point = point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="point"></param>
        /// <param name="weight"></param>
        public OnPoint(IEnumerable<int> indices, Vec3d point, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
            Handles.AddRange(indices.Select(i => new H(i)));
            Point = point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            foreach (var h in Handles)
            {
                h.Delta = Point - particles[h].Position;
                h.Weight = Weight;
            }
        }
    }
}
