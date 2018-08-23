
/*
 * Notes
 */

using System;
using System.Collections.Generic;

using SpatialSlur;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Grid3d
    {
        private double _dx;
        private double _dy;
        private double _dz;

        private double _tx = 1.0;
        private double _ty = 1.0;
        private double _tz = 1.0;
        
        private double _txInv = 1.0; // cached to avoid divs
        private double _tyInv = 1.0; // ''
        private double _tzInv = 1.0; // ''

        private readonly int _nx;
        private readonly int _ny;
        private readonly int _nz;
        private readonly int _nxy;
        private readonly int _nxyz;

        private WrapMode _wrapX;
        private WrapMode _wrapY;
        private WrapMode _wrapZ;
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        public Grid3d(int countX, int countY, int countZ)
        {
            const string errorMessage = "The resolution of the grid must be greater than 1 in each dimension.";

            if (countX < 2 || countY < 2 || countZ < 2)
                throw new ArgumentException(errorMessage);

            _nx = countX;
            _ny = countY;
            _nz = countZ;
            _nxy = countX * countY;
            _nxyz = _nxy * countZ;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public Grid3d(Grid3d other)
        {
            _dx = other._dx;
            _dy = other._dy;
            _dz = other._dz;

            _tx = other._tx;
            _ty = other._ty;
            _tz = other._tz;

            _txInv = other._txInv;
            _tyInv = other._tyInv;
            _tzInv = other._tzInv;

            _nx = other._nx;
            _ny = other._ny;
            _nz = other._nz;
            _nxy = other._nxy;
            _nxyz = other._nxyz;

            _wrapX = other._wrapX;
            _wrapY = other._wrapY;
            _wrapZ = other._wrapZ;
        }


        /// <summary>
        /// Gets or sets the origin of the grid.
        /// </summary>
        public Vector3d Origin
        {
            get => new Vector3d(_dx, _dy, _dz);
            set
            {
                _dx = value.X;
                _dy = value.Y;
                _dz = value.Z;
            }
        }


        /// <summary>
        /// Gets or sets the scale of the grid in each dimension.
        /// </summary>
        public Vector3d Scale
        {
            get => new Vector3d(_tx, _ty, _tz);
            set
            {
                ScaleX = value.X;
                ScaleY = value.Y;
                ScaleZ = value.Z;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double ScaleX
        {
            get => _tx;
            set
            {
                if (value == 0.0)
                    throw new DivideByZeroException();

                _tx = value;
                _txInv = 1.0 / value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double ScaleY
        {
            get => _ty;
            set
            {
                if (value == 0.0)
                    throw new DivideByZeroException();

                _ty = value;
                _tyInv = 1.0 / value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double ScaleZ
        {
            get => _tz;
            set
            {
                if (value == 0.0)
                    throw new DivideByZeroException();

                _tz = value;
                _tzInv = 1.0 / value;
            }
        }


        /// <summary>
        /// Gets or sets the bounds of the grid.
        /// </summary>
        public Interval3d Bounds
        {
            get
            {
                return new Interval3d(
                   _dx, _dx + (_nx - 1) * _tx,
                   _dy, _dy + (_ny - 1) * _ty,
                   _dz, _dz + (_nz - 1) * _tz
                   );
            }
            set
            {
                Origin = value.A;
                ScaleX = value.X.Delta / (_nx - 1);
                ScaleY = value.Y.Delta / (_ny - 1);
                ScaleZ = value.Z.Delta / (_nz - 1);
            }
        }


        /// <summary>
        /// Returns the number of samples in each dimension.
        /// </summary>
        public Vector3i Count
        {
            get => new Vector3i(_nx, _ny, _nz);
        }


        /// <summary>
        /// Returns the number of samples in the x dimension.
        /// </summary>
        public int CountX
        {
            get => _nx;
        }


        /// <summary>
        /// Returns the number of samples in the y dimension.
        /// </summary>
        public int CountY
        {
            get => _ny;
        }


        /// <summary>
        /// Returns the number of samples in the z dimension
        /// </summary>
        public int CountZ
        {
            get => _nz;
        }


        /// <summary>
        /// Returns the number of samples in a single layer of the grid.
        /// </summary>
        public int CountXY
        {
            get => _nxy;
        }


        /// <summary>
        /// Returns the total number of samples in the grid.
        /// </summary>
        public int CountXYZ
        {
            get => _nxyz;
        }


        /// <summary>
        /// 
        /// </summary>
        public WrapMode WrapMode
        {
            set => _wrapX = _wrapY = _wrapZ = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public WrapMode WrapModeX
        {
            get => _wrapX;
            set => _wrapX = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public WrapMode WrapModeY
        {
            get => _wrapY;
            set => _wrapY = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public WrapMode WrapModeZ
        {
            get => _wrapZ;
            set => _wrapZ = value;
        }
        

        /// <summary>
        /// Converts from grid space to world space.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector3d ToWorldSpace(Vector3d point)
        {
            return new Vector3d(
               point.X * _tx + _dx,
               point.Y * _ty + _dy,
               point.Z * _tz + _dz);
        }


        /// <summary>
        /// Converts from index to world space.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3d ToWorldSpace(int index)
        {
            return ToWorldSpace(ToGridSpace(index));
        }


        /// <summary>
        /// Converts from world space to grid space.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector3d ToGridSpace(Vector3d point)
        {
            return new Vector3d(
                (point.X - _dx) * _txInv,
                (point.Y - _dy) * _tyInv,
                (point.Z - _dz) * _tzInv);
        }


        /// <summary>
        /// Converts from index to grid space.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3i ToGridSpace(int index)
        {
            int z = index / _nxy;
            index -= z * _nxy;
            int y = index / _nx;
            return new Vector3i(index - y * _nx, y, z);
        }


        /// <summary>
        /// Converts from grid space to index.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int ToIndex(Vector3i point)
        {
            return WrapX(point.X) + WrapY(point.Y) * _nx + WrapZ(point.Z) * _nxy;
        }


        /// <summary>
        /// Converts from grid space to index.
        /// Assumes the given point is within the bounds of the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int ToIndexUnsafe(Vector3i point)
        {
            return point.X + point.Y * _nx + point.Z * _nxy;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector3i Wrap(Vector3i point)
        {
            return new Vector3i(
                WrapX(point.X), 
                WrapY(point.Y), 
                WrapZ(point.Z));
        }


        /// <summary>
        /// Applies a wrap function to the given index based on the current wrap mode.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int WrapX(int x)
        {
            return Grid.Wrap(x, _nx, _wrapX);
        }


        /// <summary>
        /// Applies a wrap function to the given index based on the current wrap mode.
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public int WrapY(int y)
        {
            return Grid.Wrap(y, _ny, _wrapY);
        }


        /// <summary>
        /// Applies a wrap function to the given index based on the current wrap mode.
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public int WrapZ(int z)
        {
            return Grid.Wrap(z, _nz, _wrapZ);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal Vector3i GetBoundaryOffsets()
        {
            return new Vector3i(
                WrapModeX == WrapMode.Repeat ? _nx - 1 : 0,
                WrapModeY == WrapMode.Repeat ? _nxy - _nx : 0,
                WrapModeZ == WrapMode.Repeat ? _nxyz - _nxy : 0);
        }
    }
}
