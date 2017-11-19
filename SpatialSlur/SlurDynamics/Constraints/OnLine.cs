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
    public class OnLine : MultiParticleConstraint<H>
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
        /// <param name="weight"></param>
        public OnLine(Vec3d start, Vec3d direction, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight,capacity)
        {
            Start = start;
            Direction = direction;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="start"></param>
        /// <param name="direction"></param>
        /// <param name="weight"></param>
        public OnLine(IEnumerable<int> indices, Vec3d start, Vec3d direction, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
            Handles.AddRange(indices.Select(i => new H(i)));
            Start = start;
            Direction = direction;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            foreach (var h in Handles)
            {
                h.Delta = Vec3d.Reject(Start - particles[h].Position, Direction);
                h.Weight = Weight;
            }
        }
    }
}
