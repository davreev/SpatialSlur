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
        private DynamicArray<int> _targetIndices = new DynamicArray<int>();
        private DynamicArray<T> _targets = new DynamicArray<T>();

        /// <summary>
        /// 
        /// </summary>
        public DynamicArray<int> TargetIndices
        {
            get => _targetIndices;
        }

        /// <summary>
        /// 
        /// </summary>
        public DynamicArray<T> Targets
        {
            get => _targets;
        }
    }
}
