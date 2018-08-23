
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
    public class Grid2d
    {
        private double _dx;
        private double _dy;

        private double _tx = 1.0;
        private double _ty = 1.0;

        private double _txInv = 1.0; // cached to avoid divs
        private double _tyInv = 1.0; // ''

        private readonly int _nx;
        private readonly int _ny;
        private readonly int _nxy;

        private WrapMode _wrapX;
        private WrapMode _wrapY;

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        public Grid2d(int countX, int countY)
        {
            const string errorMessage = "The resolution of the grid must be greater than 1 in each dimension.";

            if (countX < 2 || countY < 2)
                throw new ArgumentException(errorMessage);

            _nx = countX;
            _ny = countY;
            _nxy = countX * countY;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public Grid2d(Grid2d other)
        {
            _dx = other._dx;
            _dy = other._dy;

            _tx = other._tx;
            _ty = other._ty;

            _txInv = other._txInv;
            _tyInv = other._tyInv;

            _nx = other._nx;
            _ny = other._ny;
            _nxy = other._nxy;

            _wrapX = other._wrapX;
            _wrapY = other._wrapY;
        }

        
        /// <summary>
        /// Gets or sets the origin of the grid.
        /// </summary>
        public Vector2d Origin
        {
            get => new Vector2d(_dx, _dy);
            set
            {
                _dx = value.X;
                _dy = value.Y;
            }
        }


        /// <summary>
        /// Gets or sets the scale of the grid in each dimension.
        /// </summary>
        public Vector2d Scale
        {
            get => new Vector2d(_tx, _ty);
            set
            {
                ScaleX = value.X;
                ScaleY = value.Y;
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
        /// Gets or sets the bounds of the grid.
        /// </summary>
        public Interval2d Bounds
        {
            get
            {
                return new Interval2d(
                    _dx, _dx + (_nx - 1) * _tx,
                    _dy, _dy + (_ny - 1) * _ty
                    );
            }
            set
            {
                Origin = value.A;
                ScaleX = value.X.Delta / (_nx - 1);
                ScaleY = value.Y.Delta / (_ny - 1);
            }
        }


        /// <summary>
        /// Gets the number of samples in each dimension.
        /// </summary>
        public Vector2i Count
        {
            get => new Vector2i(_nx, _ny);
        }


        /// <summary>
        /// Gets the number of samples in the x direction.
        /// </summary>
        public int CountX
        {
            get => _nx;
        }


        /// <summary>
        /// Gets the number of samples in the y direction.
        /// </summary>
        public int CountY
        {
            get => _ny;
        }


        /// <summary>
        /// Gets the total number of samples in the grid.
        /// </summary>
        public int CountXY
        {
            get => _nxy;
        }


        /// <summary>
        /// 
        /// </summary>
        public WrapMode WrapMode
        {
            set => _wrapX = _wrapY = value;
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
        /// Converts from grid space to world space.
        /// </summary>
        public Vector2d ToWorldSpace(Vector2d point)
        {
            return new Vector2d(
              point.X * _tx + _dx,
              point.Y * _ty + _dy
          );
        }


        /// <summary>
        /// Converts from index to world space.
        /// </summary>
        public Vector2d ToWorldSpace(int index)
        {
            return ToWorldSpace(ToGridSpace(index));
        }
        

        /// <summary>
        /// Converts from world space to grid space.
        /// </summary>
        public Vector2d ToGridSpace(Vector2d point)
        {
            return new Vector2d(
               (point.X - _dx) * _txInv,
               (point.Y - _dy) * _tyInv
           );
        }


        /// <summary>
        /// Converts from index to grid space.
        /// </summary>
        public Vector2i ToGridSpace(int index)
        {
            int y = index / _nx;
            return new Vector2i(index - y * _nx, y);
        }


        /// <summary>
        /// Converts from grid space to index.
        /// </summary>
        public int ToIndex(Vector2i point)
        {
            return WrapX(point.X) + WrapY(point.Y) * _nx;
        }


        /// <summary>
        /// Converts from grid space to index.
        /// Assumes the given point is within the bounds of the grid.
        /// </summary>
        public int ToIndexUnsafe(Vector2i point)
        {
            return point.X + point.Y * _nx;
        }


        /// <summary>
        /// Wraps the given point in grid space to the bounds of the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2i Wrap(Vector2i point)
        {
            return new Vector2i(
                WrapX(point.X),
                WrapY(point.Y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int WrapX(int x)
        {
            return Grid.Wrap(x, _nx, _wrapX);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public int WrapY(int y)
        {
            return Grid.Wrap(y, _ny, _wrapY);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal Vector2i GetBoundaryOffsets()
        {
            return new Vector2i(
                WrapModeX == WrapMode.Repeat ? _nx - 1 : 0,
                WrapModeY == WrapMode.Repeat ? _nxy - _nx : 0);
        }
    }
}