/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// 
    /// </summary>
    public class ParticleBuffer
    {
        private ParticleHandle[] _handles = Array.Empty<ParticleHandle>();
        private int _handleCount;

        private ParticlePosition[] _positions = Array.Empty<ParticlePosition>();
        private int _positonCount;

        private ParticleRotation[] _rotations = Array.Empty<ParticleRotation>();
        private int _rotationCount;


        /// <summary>
        /// 
        /// </summary>
        public ParticleBuffer(int capacity)
        {
            _handles = new ParticleHandle[capacity];
            _positions = new ParticlePosition[capacity];
            _rotations = new ParticleRotation[capacity];
        }


        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyArrayView<ParticleHandle> Handles
        {
            get { return _handles.AsView(_handleCount); }
        }


        /// <summary>
        /// 
        /// </summary>
        public ArrayView<ParticlePosition> Positions
        {
            get { return _positions.AsView(_positonCount); }
        }


        /// <summary>
        /// 
        /// </summary>
        public ArrayView<ParticleRotation> Rotations
        {
            get { return _rotations.AsView(_rotationCount); }
        }


        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get => _handleCount;
        }


        /// <summary>
        /// 
        /// </summary>
        public int Capacity
        {
            get => _handles.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public ParticleHandle Add(Vector3d position)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public ParticleHandle Add(Quaterniond rotation)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public ParticleHandle Add(Vector3d position, Quaterniond rotation)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="hasPosition"></param>
        /// <param name="hasRotation"></param>
        public void Add(int quantity, bool hasPosition, bool hasRotation)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        public void Remove(int particleIndex)
        {
            _handles[particleIndex] = ParticleHandle.Removed;
        }


        /// <summary>
        /// 
        /// </summary>
        public void Compact()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        public ParticleHandle HandleAt(int particleIndex)
        {
            return _handles[particleIndex];
        }


        /// <summary>
        /// 
        /// </summary>
        public ParticlePosition PositionAt(int particleIndex)
        {
            return _positions[_handles[particleIndex].PositionIndex];
        }


        /// <summary>
        /// 
        /// </summary>
        public ParticlePosition RotationAt(int particleIndex)
        {
            return _positions[_handles[particleIndex].PositionIndex];
        }
    }
}
