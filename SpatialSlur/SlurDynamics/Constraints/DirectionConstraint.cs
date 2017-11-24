using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class DirectionConstraint : Constraint, IConstraint
    {
        private H _h0 = new H();
        private H _h1 = new H();

        /// <summary></summary>
        public Vec3d Direction;


        /// <summary>
        /// 
        /// </summary>
        public H Start
        {
            get { return _h0; }
        }


        /// <summary>
        /// 
        /// </summary>
        public H End
        {
            get { return _h1; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="direction"></param>
        /// <param name="weight"></param>
        public DirectionConstraint(int start, int end, Vec3d direction, double weight = 1.0)
        {
            _h0.Index = start;
            _h1.Index = end;

            Direction = direction;
            Weight = weight;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public void Calculate(IReadOnlyList<IBody> particles)
        {
            var d = Vec3d.Reject(particles[_h1].Position - particles[_h0].Position, Direction) * 0.5;

            _h0.Delta = d;
            _h1.Delta = -d;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            bodies[_h0].ApplyMove(_h0.Delta, Weight);
            bodies[_h1].ApplyMove(_h1.Delta, Weight);
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


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<IHandle> IConstraint.Handles
        {
            get
            {
                yield return _h0;
                yield return _h1;
            }
        }

        #endregion
    }
}
