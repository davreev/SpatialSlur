using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    using H = CustomWeight.Handle;

    /// <summary>
    /// Applies a force proportional to the mass stored on each handle.
    /// </summary>
    [Serializable]
    public class CustomWeight : DynamicPositionConstraint<H>
    {
        /// <summary></summary>
        public Vec3d Direction;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public CustomWeight(Vec3d direction, double weight = 1.0)
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
        public CustomWeight(Vec3d direction, int capacity, double weight = 1.0)
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
        public CustomWeight(IEnumerable<int> indices, Vec3d direction, double weight = 1.0)
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
        public CustomWeight(IEnumerable<H> handles, Vec3d direction, double weight = 1.0)
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
                h.Delta = Direction * h.Mass;
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class Handle : ParticleHandle
        {
            private double _mass;


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
            public Handle()
            {
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="index"></param>
            /// <param name="mass"></param>
            public Handle(int index, double mass = 1.0)
                :base(index)
            {
                Mass = mass;
            }
        }
    }
}
