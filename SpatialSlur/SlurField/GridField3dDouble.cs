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
    internal class GridField3dDouble : GridField3d<double>
    {
        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        internal class Factory : GridField3dFactory<double>
        {
            /// <inheritdoc />
            public override GridField3d<double> Create(int countX, int countY, int countZ)
            {
                return new GridField3dDouble(countX, countY, countZ);
            }


            /// <inheritdoc />
            public override GridField3d<double> Create(Grid3d grid)
            {
                return new GridField3dDouble(grid);
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        public GridField3dDouble(int countX, int countY, int countZ)
           : base(countX, countY, countZ)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public GridField3dDouble(Grid3d other)
            : base(other)
        {
        }


        /// <inheritdoc />
        public sealed override GridField3d<double> Duplicate(bool setValues)
        {
            var result = GridField3d.Double.Create(this);
            if (setValues) result.Set(this);
            return result;
        }


        /// <inheritdoc />
        protected sealed override double ValueAtLinear(Vec3d point)
        {
            point = ToGridSpace(point);
            double u = SlurMath.Fract(point.X, out int i0);
            double v = SlurMath.Fract(point.Y, out int j0);
            double w = SlurMath.Fract(point.Z, out int k0);

            int i1 = WrapX(i0 + 1);
            int j1 = WrapY(j0 + 1) * CountX;
            int k1 = WrapZ(k0 + 1) * CountXY;

            i0 = WrapX(i0);
            j0 = WrapY(j0) * CountX;
            k0 = WrapZ(k0) * CountXY;

            var vals = Values;
            return SlurMath.Lerp(
                SlurMath.Lerp(
                    SlurMath.Lerp(vals[i0 + j0 + k0], vals[i1 + j0 + k0], u),
                    SlurMath.Lerp(vals[i0 + j1 + k0], vals[i1 + j1 + k0], u),
                    v),
                SlurMath.Lerp(
                    SlurMath.Lerp(vals[i0 + j0 + k1], vals[i1 + j0 + k1], u),
                    SlurMath.Lerp(vals[i0 + j1 + k1], vals[i1 + j1 + k1], u),
                    v),
                w);
        }


        /// <inheritdoc />
        protected sealed override double ValueAtLinearUnsafe(Vec3d point)
        {
            point = ToGridSpace(point);
            double u = SlurMath.Fract(point.X, out int i0);
            double v = SlurMath.Fract(point.Y, out int j0);
            double w = SlurMath.Fract(point.Z, out int k0);

            j0 *= CountX;
            k0 *= CountXY;
            int i1 = i0 + 1;
            int j1 = j0 + CountX;
            int k1 = k0 + CountXY;

            var vals = Values;
            return SlurMath.Lerp(
                SlurMath.Lerp(
                    SlurMath.Lerp(vals[i0 + j0 + k0], vals[i1 + j0 + k0], u),
                    SlurMath.Lerp(vals[i0 + j1 + k0], vals[i1 + j1 + k0], u),
                    v),
                SlurMath.Lerp(
                    SlurMath.Lerp(vals[i0 + j0 + k1], vals[i1 + j0 + k1], u),
                    SlurMath.Lerp(vals[i0 + j1 + k1], vals[i1 + j1 + k1], u),
                    v),
                w);
        }


        /// <inheritdoc />
        public sealed override double ValueAt(GridPoint3d point)
        {
            return FieldUtil.ValueAt(Values, point.Corners, point.Weights);
        }
    }
}
