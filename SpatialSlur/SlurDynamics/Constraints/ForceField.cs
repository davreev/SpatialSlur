using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    using H = ParticleHandle;

    /// <summary>
    /// 
    /// </summary>
    public class ForceField : MultiParticleConstraint<H>
    {
        private GridVectorField3d _field;


        /// <summary>
        /// 
        /// </summary>
        public GridVectorField3d Field
        {
            get { return _field; }
            set { _field = value ?? throw new ArgumentNullException(); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="strength"></param>
        /// <param name="weight"></param>
        public ForceField(GridVectorField3d field, double weight = 1.0)
            : base(weight)
        {
            Field = field;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="strength"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public ForceField(GridVectorField3d field, int capacity, double weight = 1.0)
            : base(capacity, weight)
        {
            Field = field;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="field"></param>
        /// <param name="strength"></param>
        /// <param name="weight"></param>
        public ForceField(IEnumerable<int> indices, GridVectorField3d field, double weight = 1.0)
            : base(weight)
        {
            Handles.AddRange(indices.Select(i => new H(i)));
            Field = field;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            for (int i = 0; i < Handles.Count; i++)
            {
                var h = Handles[i];
                h.Delta = _field.ValueAt(particles[h].Position);
                h.Weight = Weight;
            }
        }
    }
}
