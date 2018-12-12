/*
 * Notes
 */

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct ParticleHandle
    {
        #region Static

        public static readonly ParticleHandle Removed = new ParticleHandle(-1, -1); 

        #endregion

        /// <summary>Index of this particle's position in the buffer</summary>
        public readonly int PositionIndex;

        /// <summary>Index of this particle's rotation in the buffer</summary>
        public readonly int RotationIndex;


        /// <summary>
        /// 
        /// </summary>
        public ParticleHandle(int positionIndex, int rotationIndex)
        {
            PositionIndex = positionIndex;
            RotationIndex = rotationIndex;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsRemoved
        {
            get => PositionIndex == -1 && RotationIndex == -1;
        }
    }
}
