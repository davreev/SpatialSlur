/*
 * Notes
 */

using System;
using System.Collections.Generic;

using SpatialSlur;
using SpatialSlur.Collections;

using static SpatialSlur.Utilities;

namespace SpatialSlur.Dynamics
{
    using Buffer = SpatialSlur.Collections.Buffer;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ParticleBuffer
    {
        private Particle[] _particles = Array.Empty<Particle>();
        private int _count;

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
            _particles = other._particles.ShallowCopy();
            _count = other._count;

            _positions = other._positions.ShallowCopy();
            _positionCount = other._positionCount;

            _rotations = other._rotations.ShallowCopy();
            _rotationCount = other._rotationCount;
        }


        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyArrayView<Particle> Particles
        {
            get { return _particles.AsView(_count); }
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
            get => _count;
        }


        /// <summary>
        /// 
        /// </summary>
        public int Capacity
        {
            get => _particles.Length;
        }


        /// <summary>
        /// Adds a new particle to the buffer
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Particle Add(Vector3d position)
        {
            return Add(new ParticlePosition(position));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public Particle Add(Quaterniond rotation)
        {
            return Add(new ParticleRotation(rotation));
        }


        /// <summary>
        /// Adds a new particle to the buffer
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public Particle Add(Vector3d position, Quaterniond rotation)
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
        public Particle Add(ParticlePosition position)
        {
            var p = new Particle()
            {
                PositionIndex = _positionCount,
                RotationIndex = -1
            };

            Buffer.Append(ref _particles, _count++, p);
            Buffer.Append(ref _positions, _positionCount++, position);

            return p;
        }


        /// <summary>
        /// Adds a new particle to the buffer
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public Particle Add(ParticleRotation rotation)
        {
            var p = new Particle()
            {
                PositionIndex = -1,
                RotationIndex = _rotationCount
            };

            Buffer.Append(ref _particles, _count++, p);
            Buffer.Append(ref _particles, _count++, p);
            Buffer.Append(ref _rotations, _rotationCount++, rotation);

            return p;
        }


        /// <summary>
        /// Adds a new particle to the buffer
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public Particle Add(ParticlePosition position, ParticleRotation rotation)
        {
            var p = new Particle()
            {
                PositionIndex = _positionCount,
                RotationIndex = _rotationCount
            };

            Buffer.Append(ref _particles, _count++, p);
            Buffer.Append(ref _positions, _positionCount++, position);
            Buffer.Append(ref _rotations, _rotationCount++, rotation);

            return p;
        }


        /// <summary>
        /// Adds a batch of new particles to the buffer.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasPosition"></param>
        /// <param name="hasRotation"></param>
        public void Add(int count, bool hasPosition, bool hasRotation)
        {
            // Add particles
            {
                int offset = _count;
                _count = Buffer.Append(ref _particles, offset, Particle.Default, count);

                // Set position indices
                if (hasPosition)
                {
                    var particles = _particles;
                    int index = _positionCount;

                    for (int i = 0; i < count; i++)
                        particles[i + offset].PositionIndex = index++;
                }

                // Set rotation indices
                if (hasRotation)
                {
                    var particles = _particles;
                    int index = _rotationCount;

                    for (int i = 0; i < count; i++)
                        particles[i + offset].RotationIndex = index++;
                }
            }

            // Add components
            {
                if (hasPosition)
                    _positionCount = Buffer.Append(ref _positions, _positionCount, ParticlePosition.Default, count);

                if (hasRotation)
                    _rotationCount = Buffer.Append(ref _rotations, _rotationCount, ParticleRotation.Default, count);
            }
        }


        /// <summary>
        /// Appends the particles from another buffer to this one.
        /// </summary>
        /// <param name="other"></param>
        public void Append(ParticleBuffer other)
        {
            // Append particles
            {
                int offset = _count;
                int count = other._count;

                _count = offset + count;
                Buffer.ExpandToFit(ref _particles, _count);

                // Set particle indices
                {
                    var particles = _particles;
                    var otherParticles = other._particles;

                    var posIndex = _positionCount;
                    var rotIndex = _rotationCount;

                    for (int i = 0; i < count; i++)
                    {
                        ref var p0 = ref particles[i + offset];
                        ref var p1 = ref otherParticles[i];
                        p0.PositionIndex = p1.HasPosition ? posIndex++ : -1;
                        p0.RotationIndex = p1.HasRotation ? rotIndex++ : -1;
                    }
                }
            }

            // Append components
            {
                _positionCount = Buffer.Append(ref _positions, _positionCount, other._positions, other._positionCount);
                _rotationCount = Buffer.Append(ref _rotations, _rotationCount, other._rotations, other._rotationCount);
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

            var particles = _particles.AsView();
            int j = 0;

            for (int i = 0; i < particles.Count; i++)
            {
                ref var p0 = ref particles[i];
                ref var p1 = ref particles[j];

                if(p0.HasPosition)
                {
                    positions[posCount] = positions[p0.PositionIndex];
                    p0.PositionIndex = -1;
                    p1.PositionIndex = posCount++;
                }

                if(p0.HasRotation)
                {
                    rotations[rotCount] = rotations[p0.RotationIndex];
                    p0.RotationIndex = -1;
                    p1.RotationIndex = rotCount++;
                }

                if (!p1.IsRemoved) j++;
            }

            _count = j;
            _positionCount = posCount;
            _rotationCount = rotCount;
        }

        
        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _count = 0;
            _positionCount = _rotationCount = 0;
        }


        /// <summary>
        /// Flags the particle at the given index for removal.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            if (index >= _count)
                throw new IndexOutOfRangeException();
            
            _particles[index] = Particle.Default;
        }


        /// <summary>
        /// Returns the handle of the particle at the given index.
        /// </summary>
        public Particle ParticleAt(int index)
        {
            if (index >= _count)
                throw new IndexOutOfRangeException();

            return _particles[index];
        }


        /// <summary>
        /// Returns the position of the given particle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public ref ParticlePosition Position(in Particle handle)
        {
            int index = handle.PositionIndex;

            if (index >= _positionCount)
                throw new IndexOutOfRangeException();
            
            return ref _positions[index];
        }


        /// <summary>
        /// Returns the rotation of the given particle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public ref ParticleRotation Rotation(in Particle handle)
        {
            int index = handle.RotationIndex;

            if (index >= _rotationCount)
                throw new IndexOutOfRangeException();
            
            return ref _rotations[index];
        }
    }
}
