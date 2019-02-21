/*
 * Notes
 */

using System;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public struct Particle
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        public static readonly Particle Default = new Particle()
        {
            PositionIndex = -1,
            RotationIndex = -1
        };

        #endregion

        /// <summary>Index of this particle's position in the buffer</summary>
        public int PositionIndex;

        /// <summary>Index of this particle's rotation in the buffer</summary>
        public int RotationIndex;


        /// <summary>
        /// 
        /// </summary>
        public bool HasPosition
        {
            get => PositionIndex != -1;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool HasRotation
        {
            get => RotationIndex != -1;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsRemoved
        {
            get => PositionIndex == -1 && RotationIndex == -1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="positionIndex"></param>
        /// <param name="rotationIndex"></param>
        public void Deconstruct(out int positionIndex, out int rotationIndex)
        {
            positionIndex = PositionIndex;
            rotationIndex = RotationIndex;
        }
    }
}
