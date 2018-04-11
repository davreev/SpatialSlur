using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.SlurData;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    using H = VariableSphereCollide.CustomHandle;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class VariableSphereCollide : MultiForce<H>, IConstraint
    {
        #region Nested types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class CustomHandle : ParticleHandle
        {
            private double _radius = 1.0;
      

            /// <summary>
            /// 
            /// </summary>
            public CustomHandle(int index)
                : base(index)
            {
            }


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
        }

        #endregion

        
        private HashGrid3d<H> _grid;
        private bool _parallel;


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
            Handles.AddRange(indices.Select(i => new H(i)));
        }


        /// <summary>
        /// 
        /// </summary>
        public bool Parallel
        {
            get { return _parallel; }
            set { _parallel = value; }
        }


        /// <inheritdoc />
        public ConstraintType Type
        {
            get { return ConstraintType.Position; }
        }


        /// <inheritdoc />
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            // TODO implement
            throw new NotImplementedException();
        }


        /// <inheritdoc />
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            foreach (var h in Handles)
                bodies[h].ApplyForce(h.Delta);
        }


        #region Explicit interface implementations

        /// <inheritdoc />
        IEnumerable<IHandle> IConstraint.Handles
        {
            get { return Handles; }
        }

        #endregion
    }
}
