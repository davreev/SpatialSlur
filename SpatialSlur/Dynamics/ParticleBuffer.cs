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
        #region Static members

        /// <summary>
        /// 
        /// </summary>
        private static void Add<T>(ref T[] target, int size, in T value)
        {
            const int minCapacity = 4;

            // Reserve enough space to avoid repeated allocations over multiple calls
            if (size == target.Length)
                Array.Resize(ref target, Math.Max(size << 1, minCapacity));

            target[size] = value;
        }


        /// <summary>
        /// 
        /// </summary>
        private static int Append<T>(ref T[] target, int size, int count)
        {
            const int minCapacity = 4;
            var newSize = size + count;

            // Reserve enough space to avoid repeated allocations over multiple calls
            if (newSize > target.Length)
                Array.Resize(ref target, Math.Max(newSize << 1, minCapacity));
            
            return newSize;
        }


        /// <summary>
        /// 
        /// </summary>
        private static int Append<T>(ref T[] target, int size, T value, int count)
        {
            var newSize = Append(ref target, size, count);
            target.SetRange(value, size, count);
            return newSize;
        }


        /// <summary>
        /// 
        /// </summary>
        private static int Append<T>(ref T[] target, int size, T[] values, int count)
        {
            var newSize = Append(ref target, size, count);
            target.SetRange(size, values, 0, count);
            return newSize;
        }

        #endregion


        private ParticleHandle[] _handles = Array.Empty<ParticleHandle>();
        private int _handleCount;

        private ParticlePosition[] _positions = Array.Empty<ParticlePosition>();
        private int _positionCount;

        private ParticleRotation[] _rotations = Array.Empty<ParticleRotation>();
        private int _rotationCount;


        /// <summary>
        /// 
        /// </summary>
        public ParticleBuffer()
        {
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
        /// Adds a batch of new particles to the buffer.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasPosition"></param>
        /// <param name="hasRotation"></param>
        public void Add(int count, bool hasPosition, bool hasRotation)
        {
            // Add handles
            {
                int offset = _handleCount;
                _handleCount = Append(ref _handles, offset, ParticleHandle.Default, count);

                // Set position indices
                if (hasPosition)
                {
                    var handles = _handles;
                    int index = _positionCount;

                    for (int i = 0; i < count; i++)
                        handles[i + offset].PositionIndex = index++;
                }

                // Set rotation indices
                if (hasRotation)
                {
                    var handles = _handles;
                    int index = _rotationCount;

                    for (int i = 0; i < count; i++)
                        handles[i + offset].RotationIndex = index++;
                }
            }

            // Add components
            {
                if (hasPosition)
                    _positionCount = Append(ref _positions, _positionCount, ParticlePosition.Default, count);

                if (hasRotation)
                    _rotationCount = Append(ref _rotations, _rotationCount, ParticleRotation.Default, count);
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
                int offset = _handleCount;
                int count = other._handleCount;
                _handleCount = Append(ref _handles, offset, count);

                // Assign new handles
                {
                    var handles = _handles;
                    var otherHandles = other._handles;

                    var posIndex = _positionCount;
                    var rotIndex = _rotationCount;

                    for (int i = 0; i < count; i++)
                    {
                        ref var h0 = ref handles[i + offset];
                        ref var h1 = ref otherHandles[i];
                        h0.PositionIndex = h1.HasPosition ? posIndex++ : -1;
                        h0.RotationIndex = h1.HasRotation ? rotIndex++ : -1;
                    }
                }
            }

            // Append components
            {
                _positionCount = Append(ref _positions, _positionCount, other._positions, other._positionCount);
                _rotationCount = Append(ref _rotations, _rotationCount, other._rotations, other._rotationCount);
            }
        }


        /// <summary>
        /// Removes all flagged particles from the buffer and compacts the remaining.
        /// </summary>
        public void Compact()
        {
            var positions = _positions;
            int posCount = 0;

            var rotations = _rotations;
            int rotCount = 0;

            var handles = _handles.AsView();
            int j = 0;

            for (int i = 0; i < handles.Count; i++)
            {
                ref var h0 = ref handles[i];
                ref var h1 = ref handles[j];

                if(h0.HasPosition)
                {
                    positions[posCount] = positions[h0.PositionIndex];
                    h0.PositionIndex = -1;
                    h1.PositionIndex = posCount++;
                }

                if(h0.HasRotation)
                {
                    rotations[rotCount] = rotations[h0.RotationIndex];
                    h0.RotationIndex = -1;
                    h1.RotationIndex = rotCount++;
                }

                if (!h1.IsRemoved) j++;
            }

            _handleCount = j;
            _positionCount = posCount;
            _rotationCount = rotCount;
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
            _handles[index] = ParticleHandle.Default;
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
