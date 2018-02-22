using System;

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
    internal class GridField2dDouble: GridField2d<double>
    {
        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        internal class Factory : GridField2dFactory<double>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="countX"></param>
            /// <param name="countY"></param>
            /// <returns></returns>
            public override GridField2d<double> Create(int countX, int countY)
            {
                return new GridField2dDouble(countX, countY);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="grid"></param>
            /// <returns></returns>
            public override GridField2d<double> Create(Grid2d grid)
            {
                return new GridField2dDouble(grid);
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        public GridField2dDouble(int countX, int countY)
           : base(countX, countY)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="sampleMode"></param>
        public GridField2dDouble(Grid2d other)
            : base(other)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public sealed override GridField2d<double> Duplicate(bool setValues)
        {
            var result = GridField2d.Double.Create(this);
            if (setValues) result.Set(this);
            return result;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected sealed override double ValueAtLinear(Vec2d point)
        {
            point = ToGridSpace(point);
            double u = SlurMath.Fract(point.X, out int i0);
            double v = SlurMath.Fract(point.Y, out int j0);

            int i1 = WrapX(i0 + 1);
            int j1 = WrapY(j0 + 1) * CountX;

            i0 = WrapX(i0);
            j0 = WrapY(j0) * CountX;

            var vals = Values;
            return SlurMath.Lerp(
                SlurMath.Lerp(vals[i0 + j0], vals[i1 + j0], u),
                SlurMath.Lerp(vals[i0 + j1], vals[i1 + j1], u),
                v);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected sealed override double ValueAtLinearUnsafe(Vec2d point)
        {
            point = ToGridSpace(point);
            double u = SlurMath.Fract(point.X, out int i0);
            double v = SlurMath.Fract(point.Y, out int j0);

            j0 *= CountX;
            int i1 = i0 + 1;
            int j1 = j0 + CountX;

            var vals = Values;
            return SlurMath.Lerp(
                SlurMath.Lerp(vals[i0 + j0], vals[i1 + j0], u),
                SlurMath.Lerp(vals[i0 + j1], vals[i1 + j1], u),
                v);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public sealed override double ValueAt(GridPoint2d point)
        {
            return FieldUtil.ValueAt(Values, point.Corners, point.Weights);
        }
    }
}
