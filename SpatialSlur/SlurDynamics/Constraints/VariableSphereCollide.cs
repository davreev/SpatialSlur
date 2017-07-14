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

namespace SpatialSlur.SlurDynamics.Constraints
{
    using H = VariableSphereCollide.Handle;

    /// <summary>
    /// 
    /// </summary>
    public class VariableSphereCollide : DynamicPositionConstraint<H>
    {
        private SpatialGrid3d<H> _grid;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public VariableSphereCollide(int capacity, double weight = 1.0)
            : base(capacity, weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="weight"></param>
        public VariableSphereCollide(IEnumerable<H> handles, double weight = 1.0)
            : base(handles, weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IParticle> particles)
        {
            _grid.Clear();
            var r0 = double.MaxValue;
            var r1 = double.MinValue;

            // insert particles
            foreach(var h in Handles)
            {
                _grid.Insert(particles[h].Position, h);
              
                var r = h.Radius;
                r0 = Math.Min(r, r0);
                r1 = Math.Max(r, r1);
            }

            // search from particles

            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        public class Handle : PositionHandle
        {
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
                        throw new ArgumentOutOfRangeException("The value can not be negative");

                    _radius = value;
                }
            }
        }
    }
}
