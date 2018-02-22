using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.SlurData;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics.Constraints
{
    using H = VariableSphereCollide.CustomHandle;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class VariableSphereCollide : MultiConstraint<H>, IConstraint
    {
        #region Nested types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class CustomHandle : ParticleHandle
        {
            private double _radius = 1.0;
            private bool _skip;
            

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


            /// <summary>
            /// 
            /// </summary>
            public bool Skip { get => _skip; set => _skip = value; }
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
        

        /// <summary>
        /// 
        /// </summary>
        public ConstraintType Type
        {
            get { return ConstraintType.Position; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            foreach (var h in Handles)
                if (!h.Skip) bodies[h].ApplyMove(h.Delta, Weight);
        }


        #region Explicit interface implementations
        
        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<IHandle> IConstraint.Handles
        {
            get { return Handles; }
        }

        #endregion
    }
}
