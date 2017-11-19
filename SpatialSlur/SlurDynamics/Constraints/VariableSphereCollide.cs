using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurData;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurDynamics
{
    using H = VariableSphereCollide.Handle;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class VariableSphereCollide : MultiParticleConstraint<H>
    {
        private HashGrid3d<H> _grid;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public VariableSphereCollide(double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="weight"></param>
        public VariableSphereCollide(IEnumerable<int> indices, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class Handle : ParticleHandle
        {
            private double _radius = 1.0;


            /// <summary>
            /// 
            /// </summary>
            public double Radius
            {
                get { return _radius; }
                set
                {
                    if (value < 0.0)
                        throw new ArgumentOutOfRangeException("The value can not be negative.");

                    _radius = value;
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
            /// <param name="radius"></param>
            public Handle(int index, double radius)
                :base(index)
            {
                Radius = radius;
            }
        }
    }
}
