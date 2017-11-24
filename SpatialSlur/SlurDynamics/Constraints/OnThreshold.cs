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

namespace SpatialSlur.SlurDynamics.Constraints
{
    using H = ParticleHandle;

    /// <summary>
    /// 
    /// </summary>
    public class OnThreshold : MultiConstraint<H>, IConstraint
    {
        /// <summary></summary>
        public double Threshold;

        private GridScalarField3d _field;
        private GridVectorField3d _gradient;

        
        /// <summary>
        /// 
        /// </summary>
        public GridScalarField3d Field
        {
            get { return _field; }
            set { _field = value ?? throw new ArgumentNullException(); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="threshold"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public OnThreshold(GridScalarField3d field, double threshold, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
            Field = field;
            Threshold = threshold;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="field"></param>
        /// <param name="threshold"></param>
        /// <param name="weight"></param>
        public OnThreshold(IEnumerable<int> indices, GridScalarField3d field, double threshold, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
            Handles.AddRange(indices.Select(i => new H(i)));
            Field = field;
            Threshold = threshold;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public void Calculate(IReadOnlyList<IBody> particles)
        {
            var gp = new GridPoint3d();

            for (int i = 0; i < Handles.Count; i++)
            {
                var h = Handles[i];
                var p = particles[h].Position;

                _field.GridPointAt(p, gp);
                var t = _field.ValueAt(gp);
                var g = _gradient.ValueAt(gp).Direction;

                h.Delta = g * (Threshold - t);
            }
        }

        
        /// <summary>
        /// This must be called after any changes to the field.
        /// </summary>
        public void UpdateGradient()
        {
            if (_gradient == null || _gradient.Count != _field.Count)
                _gradient = new GridVectorField3d(_field);

            _field.GetGradient(_gradient);
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
