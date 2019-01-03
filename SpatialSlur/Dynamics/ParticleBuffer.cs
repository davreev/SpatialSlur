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

using static SpatialSlur.Utilities;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ParticleBuffer
    {
        private ParticleHandle[] _handles = Array.Empty<ParticleHandle>();
        private int _handleCount;

        private ParticlePosition[] _positions = Array.Empty<ParticlePosition>();
        private int _positionCount;

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
        /// <param name="other"></param>
        public ParticleBuffer(ParticleBuffer other)
        {
            _handles = other._handles.ShallowCopy();
            _handleCount = other._handleCount;

            _positions = other._positions.ShallowCopy();
            _positionCount = other._positionCount;

            _rotations = other._rotations.ShallowCopy();
            _rotationCount = other._rotationCount;
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
            get { return _positions.AsView(_positionCount); }
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
        /// Adds a new particle to the buffer
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public ParticleHandle Add(Vector3d position)
        {
            return Add(new ParticlePosition(position));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public ParticleHandle Add(Quaterniond rotation)
        {
            return Add(new ParticleRotation(rotation));
        }


        /// <summary>
        /// Adds a new particle to the buffer
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public ParticleHandle Add(Vector3d position, Quaterniond rotation)
        {
            return Add(
                new ParticlePosition(position),
                new ParticleRotation(rotation));
        }


        /// <summary>
        /// Adds a new particle to the buffer
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public ParticleHandle Add(ParticlePosition position)
        {
            var handle = new ParticleHandle(_positionCount, -1);

            Add(ref _handles, _handleCount++, handle);
            Add(ref _positions, _positionCount++, position);

            return handle;
        }


        /// <summary>
        /// Adds a new particle to the buffer
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public ParticleHandle Add(ParticleRotation rotation)
        {
            var handle = new ParticleHandle(-1, _rotationCount);

            Add(ref _handles, _handleCount++, handle);
            Add(ref _rotations, _rotationCount++, rotation);

            return handle;
        }


        /// <summary>
        /// Adds a new particle to the buffer
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public ParticleHandle Add(ParticlePosition position, ParticleRotation rotation)
        {
            var handle = new ParticleHandle(_positionCount, _rotationCount);

            Add(ref _handles, _handleCount++, handle);
            Add(ref _positions, _positionCount++, position);
            Add(ref _rotations, _rotationCount++, rotation);

            return handle;
        }


        /// <summary>
        /// 
        /// </summary>
        private static void Add<T>(ref T[] source, int index, in T item)
        {
            const int minCapacity = 4;

            if (index == source.Length)
                Array.Resize(ref source, Math.Max(index << 1, minCapacity));

            source[index] = item;
        }


        /// <summary>
        /// Adds a batch of new particles to the buffer.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasPosition"></param>
        /// <param name="hasRotation"></param>
        public void Add(int count, bool hasPosition, bool hasRotation)
        {
            // Add handles
            {
                int newCount = _handleCount + count;

                // Resize if necessary
                if (newCount > _handles.Length)
                    Array.Resize(ref _handles, newCount << 1);

                // Initialize new handles
                {
                    var handles = _handles;
                    var posIndex = _positionCount;
                    var rotIndex = _rotationCount;

                    for (int i = _handleCount; i < newCount; i++)
                    {
                        handles[i] = new ParticleHandle(
                            hasPosition ? posIndex++ : -1,
                            hasRotation ? rotIndex++ : -1);
                    }
                }

                _handleCount = newCount;
            }

            // Add positions
            if (hasPosition)
            {
                var newCount = _positionCount + count;

                // Resize if necessary
                if (newCount > _positions.Length)
                    Array.Resize(ref _positions, newCount << 1);

                _positions.SetRange(ParticlePosition.Default, _positionCount, count);
                _positionCount = newCount;
            }

            // Add rotations
            if (hasRotation)
            {
                var newCount = _rotationCount + count;

                // Resize if necessary
                if (newCount > _rotations.Length)
                    Array.Resize(ref _rotations, newCount << 1);

                _rotations.SetRange(ParticleRotation.Default, _rotationCount, count);
                _rotationCount = newCount;
            }
        }


        /// <summary>
        /// Appends the particles from another buffer to this one.
        /// </summary>
        /// <param name="other"></param>
        public void Append(ParticleBuffer other)
        {
            // Append handles
            {
                int count = _handleCount;
                int otherCount = other._handleCount;
                int newCount = count + otherCount;

                // Resize if necessary
                if (newCount > _handles.Length)
                    Array.Resize(ref _handles, newCount << 1);

                // Assign new handles
                {
                    var handles = _handles;
                    var otherHandles = other._handles;
                    var posIndex = _positionCount;
                    var rotIndex = _rotationCount;

                    for (int i = 0; i < otherCount; i++)
                        handles[i + count] = Next(otherHandles[i]);

                    ParticleHandle Next(in ParticleHandle handle)
                    {
                        return new ParticleHandle(
                            handle.HasPosition ? posIndex++ : -1,
                            handle.HasRotation ? rotIndex++ : -1);
                    }
                }

                _handleCount = newCount;
            }

            // Append positions
            {
                int otherCount = other._positionCount;
                int newCount = _positionCount + otherCount;

                // Resize if necessary
                if (newCount > _positions.Length)
                    Array.Resize(ref _positions, newCount << 1);

                _positions.SetRange(other._positions, _positionCount, 0, otherCount);
                _positionCount = newCount;
            }

            // Append rotations
            {
                int otherCount = other._rotationCount;
                int newCount = _rotationCount + otherCount;

                // Resize if necessary
                if (newCount > _rotations.Length)
                    Array.Resize(ref _rotations, newCount << 1);

                _rotations.SetRange(other._rotations, _rotationCount, 0, otherCount);
                _rotationCount = newCount;
            }
        }


        /// <summary>
        /// Removes all flagged particles from the buffer and compacts the remaining.
        /// </summary>
        public void Compact()
        {
            var handles = _handles;
            int handleCount = 0;

            var positions = _positions;
            int positionCount = 0;

            var rotations = _rotations;
            int rotationCount = 0;

            foreach(var h in handles)
            {
                if (!h.IsRemoved)
                {
                    handles[handleCount++] = h;
                    positions[positionCount++] = positions[h.PositionIndex];
                    rotations[rotationCount++] = rotations[h.RotationIndex];
                }
            }

            _handleCount = handleCount;
            _positionCount = positionCount;
            _rotationCount = rotationCount;
        }

        
        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _handleCount = 0;
            _positionCount = _rotationCount = 0;
        }


        /// <summary>
        /// Flags the particle at the given index for removal.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            BoundsCheck(index, _handleCount);
            _handles[index] = ParticleHandle.Removed;
        }


        /// <summary>
        /// Returns the handle of the particle at the given index.
        /// </summary>
        public ParticleHandle HandleAt(int index)
        {
            BoundsCheck(index, _handleCount);
            return _handles[index];
        }


        /// <summary>
        /// Returns the position of the given particle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public ref ParticlePosition Position(in ParticleHandle handle)
        {
            BoundsCheck(handle.PositionIndex, _positionCount);
            return ref _positions[handle.PositionIndex];
        }


        /// <summary>
        /// Returns the rotation of the given particle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public ref ParticleRotation Rotation(in ParticleHandle handle)
        {
            BoundsCheck(handle.RotationIndex, _rotationCount);
            return ref _rotations[handle.RotationIndex];
        }
    }
}
