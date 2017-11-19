using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Grid3d
    {
        private double _dx, _dy, _dz;
        private double _tx, _ty, _tz;
        private double _txInv, _tyInv, _tzInv; // cached to avoid divs
        private readonly int _nx, _ny, _nz, _nxy, _n;
        private WrapMode _wrapX, _wrapY, _wrapZ;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        private Grid3d(int countX, int countY, int countZ)
        {
            if (countX < 2 || countY < 2 || countZ < 2)
                throw new System.ArgumentException("The resolution of the grid must be greater than 1 in each dimension.");

            _nx = countX;
            _ny = countY;
            _nz = countZ;
            _nxy = _nx * _ny;
            _n = _nxy * _nz;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="wrapMode"></param>
        public Grid3d(Vec3d origin, Vec3d scale, int countX, int countY, int countZ, WrapMode wrapMode = WrapMode.Clamp)
            : this(countX, countY, countZ)
        {
            Origin = origin;
            Scale = scale;
            _wrapX = _wrapY = _wrapZ = wrapMode;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="wrapModeX"></param>
        /// <param name="wrapModeY"></param>
        /// <param name="wrapModeZ"></param>
        public Grid3d(Vec3d origin, Vec3d scale, int countX, int countY, int countZ, WrapMode wrapModeX, WrapMode wrapModeY, WrapMode wrapModeZ)
            : this(countX, countY, countZ)
        {
            Origin = origin;
            Scale = scale;
            _wrapX = wrapModeX;
            _wrapY = wrapModeY;
            _wrapZ = wrapModeZ;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="wrapMode"></param>
        public Grid3d(Interval3d bounds, int countX, int countY, int countZ, WrapMode wrapMode = WrapMode.Clamp)
            : this(countX, countY, countZ)
        {
            Bounds = bounds;
            _wrapX = _wrapY = _wrapZ = wrapMode;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="wrapModeX"></param>
        /// <param name="wrapModeY"></param>
        /// <param name="wrapModeZ"></param>
        public Grid3d(Interval3d bounds, int countX, int countY, int countZ, WrapMode wrapModeX, WrapMode wrapModeY, WrapMode wrapModeZ)
            : this(countX, countY, countZ)
        {
            Bounds = bounds;
            _wrapX = wrapModeX;
            _wrapY = wrapModeY;
            _wrapZ = wrapModeZ;
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
            _n = other._n;

            _wrapX = other._wrapX;
            _wrapY = other._wrapY;
            _wrapZ = other._wrapZ;
        }


        /// <summary>
        /// Gets or sets the origin of the grid.
        /// This is the position of the first grid point.
        /// </summary>
        public Vec3d Origin
        {
            get { return new Vec3d(_dx, _dy, _dz); }
            set { (_dx, _dy, _dz) = value.Components; }
        }


        /// <summary>
        /// Gets or sets the scale of the grid in each dimension.
        /// </summary>
        public Vec3d Scale
        {
            get { return new Vec3d(_tx, _ty, _tz); }
            set
            {
                (_tx, _ty, _tz) = value.Components;
                _txInv = 1.0 / _tx;
                _tyInv = 1.0 / _ty;
                _tzInv = 1.0 / _tz;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Span
        {
            get
            {
                return new Vec3d(
                    (_nx + 1) * _tx, 
                    (_ny + 1) * _ty,
                    (_nz + 1) * _tz
                    );
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
                   _dx, _dx + (_nx + 1) * _tx,
                   _dy, _dy + (_ny + 1) * _ty,
                   _dz, _dz + (_nz + 1) * _tz
                   );
            }
            set
            {
                if (!value.IsValid)
                    throw new System.ArgumentException("The given interval must be valid.");

                Origin = value.A;
                Scale = new Vec3d(
                    value.X.Length / (_nx - 1),
                    value.Y.Length / (_ny - 1),
                    value.Z.Length / (_nz - 1)
                    );
            }
        }


        /// <summary>
        /// Returns the size of the grid.
        /// </summary>
        public int Count
        {
            get { return _n; }
        }


        /// <summary>
        /// Returns the resolution of the grid in the x direction.
        /// </summary>
        public int CountX
        {
            get { return _nx; }
        }


        /// <summary>
        /// Returns the resolution of the grid in the y direction.
        /// </summary>
        public int CountY
        {
            get { return _ny; }
        }


        /// <summary>
        /// Returns the resolution of the grid in the y direction.
        /// </summary>
        public int CountZ
        {
            get { return _nz; }
        }


        /// <summary>
        /// Returns the size of a single xy layer.
        /// </summary>
        public int CountXY
        {
            get { return _nxy; }
        }


        /// <summary>
        /// 
        /// </summary>
        public WrapMode WrapModeX
        {
            get { return _wrapX; }
            set { _wrapX = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public WrapMode WrapModeY
        {
            get { return _wrapY; }
            set { _wrapY = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public WrapMode WrapModeZ
        {
            get { return _wrapZ; }
            set { _wrapZ = value; }
        }


        /// <summary>
        /// Enumerates over coordinates of the grid.
        /// </summary>
        public IEnumerable<Vec3d> Coordinates
        {
            get
            {
                for (int k = 0; k < CountZ; k++)
                {
                    for (int j = 0; j < CountY; j++)
                    {
                        for (int i = 0; i < CountX; i++)
                            yield return CoordinateAt(i, j, k);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public Vec3d CoordinateAt(int i, int j, int k)
        {
            return new Vec3d(
                i * _tx + _dx,
                j * _ty + _dy,
                k * _tz + _dz);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vec3d CoordinateAt(int index)
        {
            (int i, int j, int k) = IndicesAt(index);
            return CoordinateAt(i, j, k);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d CoordinateAt(GridPoint3d point)
        {
            var corners = point.Corners;
            var weights = point.Weights;
            var sum = new Vec3d();

            for (int i = 0; i < 8; i++)
                sum += CoordinateAt(corners[i]) * weights[i];

            return sum;
        }


        /// <summary>
        /// Returns true if the grid has the same resolution in each dimension as another.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool ResolutionEquals(Grid3d other)
        {
            return (_nx == other._nx && _ny == other._ny && _nz == other._nz);
        }

        
        /// <summary>
        /// Convention is (i, j, k) => (x, y, z)
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public (int, int, int) Wrap(int i, int j, int k)
        {
            return (WrapX(i), WrapY(j), WrapZ(k));
        }


        /// <summary>
        /// Applies a wrap function to the given index based on the current wrap mode.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public int WrapX(int i)
        {
            return GridUtil.Wrap(i, _nx, _wrapX);
        }


        /// <summary>
        /// Applies a wrap function to the given index based on the current wrap mode.
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        public int WrapY(int j)
        {
            return GridUtil.Wrap(j, _ny, _wrapY);
        }


        /// <summary>
        /// Applies a wrap function to the given index based on the current wrap mode.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public int WrapZ(int k)
        {
            return GridUtil.Wrap(k, _nz, _wrapZ);
        }

        
        /// <summary>
        /// Flattens a 3 dimensional index into a 1 dimensional index.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public int IndexAt(int i, int j, int k)
        {
            return GridUtil.FlattenIndices(WrapX(i), WrapY(j), WrapZ(k), _nx, _nxy);
        }


        /// <summary>
        /// Flattens a 3 dimensional index into a 1 dimensional index.
        /// Assumes the given indices are within the valid range.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public int IndexAtUnchecked(int i, int j, int k)
        {
            return GridUtil.FlattenIndices(i, j, k, _nx, _nxy);
        }


        /// <summary>
        /// Returns the index of the nearest point in the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IndexAt(Vec3d point)
        {
            (int i, int j, int k) = IndicesAt(point);
            return GridUtil.FlattenIndices(WrapX(i), WrapY(j), WrapZ(k), _nx, _nxy);
        }


        /// <summary>
        /// Returns the index of the nearest point in the grid.
        /// Assumes the point is within the bounds of the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IndexAtUnchecked(Vec3d point)
        {
            (int i, int j, int k) = IndicesAt(point);
            return GridUtil.FlattenIndices(i, j, k, _nx, _nxy);
        }

        
        /// <summary>
        /// Expands a 1 dimensional index into a 3 dimensional index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public (int, int, int) IndicesAt(int index)
        {
            return GridUtil.ExpandIndex(index, _nx, _nxy);
        }

        
        /// <summary>
        /// Returns the indices of the nearest point in the grid.
        /// </summary>
        /// <param name="point"></param>
        public (int, int, int) IndicesAt(Vec3d point)
        {
            return (
                (int)Math.Round((point.X - _dx) * _txInv),
                (int)Math.Round((point.Y - _dy) * _tyInv),
                (int)Math.Round((point.Z - _dz) * _tzInv)
                );
        }


        /// <summary>
        /// Returns a grid point at the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public GridPoint3d GridPointAt(Vec3d point)
        {
            GridPoint3d result = new GridPoint3d();
            GridPointAt(point, result);
            return result;
        }


        /// <summary>
        /// Returns a grid point at the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void GridPointAt(Vec3d point, GridPoint3d result)
        {
            point = ToGridSpace(point);

            result.SetWeights(
                SlurMath.Fract(point.X, out int i0),
                SlurMath.Fract(point.Y, out int j0),
                SlurMath.Fract(point.Z, out int k0)
                );

            int i1 = WrapX(i0 + 1);
            int j1 = WrapY(j0 + 1) * _nx;
            int k1 = WrapZ(k0 + 1) * _nxy;

            i0 = WrapX(i0);
            j0 = WrapY(j0) * _nx;
            k0 = WrapZ(k0) * _nxy;

            var corners = result.Corners;
            corners[0] = i0 + j0 + k0;
            corners[1] = i1 + j0 + k0;
            corners[2] = i0 + j1 + k0;
            corners[3] = i1 + j1 + k0;
            corners[4] = i0 + j0 + k1;
            corners[5] = i1 + j0 + k1;
            corners[6] = i0 + j1 + k1;
            corners[7] = i1 + j1 + k1;
        }


        /// <summary>
        /// Returns a grid point at the given point.
        /// Assumes the point is within the bounds of the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public GridPoint3d GridPointAtUnchecked(Vec3d point)
        {
            GridPoint3d result = new GridPoint3d();
            GridPointAtUnchecked(point, result);
            return result;
        }


        /// <summary>
        /// Returns a grid point at the given point.
        /// Assumes the point is within the bounds of the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void GridPointAtUnchecked(Vec3d point, GridPoint3d result)
        {
            point = ToGridSpace(point);

            result.SetWeights(
                SlurMath.Fract(point.X, out int i0),
                SlurMath.Fract(point.Y, out int j0),
                SlurMath.Fract(point.Z, out int k0)
                );

            j0 *= _nx;
            k0 *= _nxy;
            int i1 = i0 + 1;
            int j1 = j0 + _nx;
            int k1 = k0 + _nxy;

            var corners = result.Corners;
            corners[0] = i0 + j0 + k0;
            corners[1] = i1 + j0 + k0;
            corners[2] = i0 + j1 + k0;
            corners[3] = i1 + j1 + k0;
            corners[4] = i0 + j0 + k1;
            corners[5] = i1 + j0 + k1;
            corners[6] = i0 + j1 + k1;
            corners[7] = i1 + j1 + k1;
        }


        /// <summary>
        /// Maps a point from world space to grid space.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d ToGridSpace(Vec3d point)
        {
            return new Vec3d(
                (point.X - _dx) * _txInv,
                (point.Y - _dy) * _tyInv,
                (point.Z - _dz) * _tzInv
            );
        }


        /// <summary>
        /// Maps a point from grid space to world space.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d ToWorldSpace(Vec3d point)
        {
            return new Vec3d(
               point.X * _tx + _dx,
               point.Y * _ty + _dy,
               point.Z * _tz + _dz
           );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal (int,int,int) GetBoundaryOffsets()
        {
            return (GetX(), GetY(), GetZ());

            int GetX()
            {
                switch (WrapModeX)
                {
                    case WrapMode.Clamp:
                        return 0;
                    case WrapMode.Repeat:
                        return _nx - 1;
                    case WrapMode.Mirror:
                        return 0;
                }

                throw new NotSupportedException();
            }

            int GetY()
            {
                switch (WrapModeY)
                {
                    case WrapMode.Clamp:
                        return 0;
                    case WrapMode.Repeat:
                        return _nxy - _nx;
                    case WrapMode.Mirror:
                        return 0;
                }

                throw new NotSupportedException();
            }

            int GetZ()
            {
                switch (WrapModeZ)
                {
                    case WrapMode.Clamp:
                        return 0;
                    case WrapMode.Repeat:
                        return _n - _nxy;
                    case WrapMode.Mirror:
                        return 0;
                }

                throw new NotSupportedException();
            }
        }
    }
}
