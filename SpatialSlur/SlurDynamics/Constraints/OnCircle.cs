using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurDynamics.Constraints
{
    using H = ParticleHandle;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class OnCircle : MultiConstraint<H>, IConstraint
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
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <param name="radius"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public OnCircle(Vec3d origin, Vec3d normal, double radius, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
            Origin = origin;
            Normal = normal;
            Radius = radius;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <param name="radius"></param>
        /// <param name="weight"></param>
        public OnCircle(IEnumerable<int> indices, Vec3d origin, Vec3d normal, double radius, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
            Handles.AddRange(indices.Select(i => new H(i)));
            Origin = origin;
            Normal = normal;
            Radius = radius;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public void Calculate(IReadOnlyList<IBody> particles)
        {
            foreach(var h in Handles)
            {
                Vec3d p = particles[h].Position;

                Vec3d d = Vec3d.Reject(p - Origin, Normal);
                d *= _radius / d.Length;

                h.Delta = ((Origin + d) - p);
            }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            foreach (var h in Handles)
                bodies[h].ApplyMove(h.Delta, Weight);
        }


        #region Explicit interface implementations

        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        bool IConstraint.AppliesRotation
        {
            get { return false; }
        }


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
