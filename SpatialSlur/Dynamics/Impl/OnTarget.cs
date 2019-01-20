/*
 * Notes
 */ 
 
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Impl
{
    /// <summary>
    /// Base class for constraints that project groups of particles to target objects
    /// </summary>
    public abstract class OnTarget<T> : PositionConstraint
    {
        private SlurList<int> _targetIndices = new SlurList<int>();
        private SlurList<T> _targets = new SlurList<T>();

        /// <summary>
        /// 
        /// </summary>
        public SlurList<int> TargetIndices
        {
            get => _targetIndices;
        }

        /// <summary>
        /// 
        /// </summary>
        public SlurList<T> Targets
        {
            get => _targets;
        }
    }
}
