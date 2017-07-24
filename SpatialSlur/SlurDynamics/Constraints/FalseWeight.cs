using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    using H = FalseWeight.Handle;

    /// <summary>
    /// Applies a force proportional to the mass defined on each handle.
    /// </summary>
    [Serializable]
    public class FalseWeight : DynamicPositionConstraint<H>
    {
        /// <summary></summary>
        public Vec3d Direction;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="weight"></param>
        public FalseWeight(Vec3d direction, double weight = 1.0)
            : base(weight)
        {
            Direction = direction;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public FalseWeight(Vec3d direction, int capacity, double weight = 1.0)
            : base(capacity, weight)
        {
            Direction = direction;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="direction"></param>
        /// <param name="weight"></param>
        public FalseWeight(IEnumerable<int> indices, Vec3d direction, double weight = 1.0)
            : base(indices.Select(i => new H(i)), weight)
        {
            Direction = direction;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="direction"></param>
        /// <param name="weight"></param>
        public FalseWeight(IEnumerable<H> handles, Vec3d direction, double weight = 1.0)
            : base(handles, weight)
        {
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
                h.Delta = Direction * h.Mass;
                h.Weight = Weight;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class Handle : ParticleHandle
        {
            private double _mass = 1.0;


            /// <summary>
            /// 
            /// </summary>
            public double Mass
            {
                get { return _mass; }
                set
                {
                    if (value < 0.0)
                        throw new ArgumentOutOfRangeException("The value can not be negative.");

                    _mass = value;
                }
            }


            /// <summary>
            /// 
            /// </summary>
            public Handle(int index)
                :base(index)
            {
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="index"></param>
            /// <param name="mass"></param>
            public Handle(int index, double mass)
                :base(index)
            {
                Mass = mass;
            }
        }
    }
}
