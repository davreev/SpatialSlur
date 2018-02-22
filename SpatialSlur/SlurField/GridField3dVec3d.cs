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
    internal class GridField3dVec3d : GridField3d<Vec3d>
    {
        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        internal class Factory : GridField3dFactory<Vec3d>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="countX"></param>
            /// <param name="countY"></param>
            /// <returns></returns>
            public override GridField3d<Vec3d> Create(int countX, int countY, int countZ)
            {
                return new GridField3dVec3d(countX, countY, countZ);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="grid"></param>
            /// <returns></returns>
            public override GridField3d<Vec3d> Create(Grid3d grid)
            {
                return new GridField3dVec3d(grid);
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        public GridField3dVec3d(int countX, int countY, int countZ)
           : base(countX, countY, countZ)
        {
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public GridField3dVec3d(Grid3d other)
            : base(other)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="setValues"></param>
        /// <returns></returns>
        public sealed override GridField3d<Vec3d> Duplicate(bool setValues)
        {
            var result = GridField3d.Vec3d.Create(this);
            if (setValues) result.Set(this);
            return result;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected sealed override Vec3d ValueAtLinear(Vec3d point)
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
            return Vec3d.Lerp(
                Vec3d.Lerp(
                    Vec3d.Lerp(vals[i0 + j0 + k0], vals[i1 + j0 + k0], u),
                    Vec3d.Lerp(vals[i0 + j1 + k0], vals[i1 + j1 + k0], u),
                    v),
                Vec3d.Lerp(
                    Vec3d.Lerp(vals[i0 + j0 + k1], vals[i1 + j0 + k1], u),
                    Vec3d.Lerp(vals[i0 + j1 + k1], vals[i1 + j1 + k1], u),
                    v),
                w);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected sealed override Vec3d ValueAtLinearUnsafe(Vec3d point)
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
            return Vec3d.Lerp(
                Vec3d.Lerp(
                    Vec3d.Lerp(vals[i0 + j0 + k0], vals[i1 + j0 + k0], u),
                    Vec3d.Lerp(vals[i0 + j1 + k0], vals[i1 + j1 + k0], u),
                    v),
                Vec3d.Lerp(
                    Vec3d.Lerp(vals[i0 + j0 + k1], vals[i1 + j0 + k1], u),
                    Vec3d.Lerp(vals[i0 + j1 + k1], vals[i1 + j1 + k1], u),
                    v),
                w);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public sealed override Vec3d ValueAt(GridPoint3d point)
        {
            return FieldUtil.ValueAt(Values, point.Corners, point.Weights);
        }
    }
}
