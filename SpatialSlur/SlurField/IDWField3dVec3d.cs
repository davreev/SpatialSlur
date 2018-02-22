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
    internal class IDWField3dVec3d : IDWField3d<Vec3d>
    {
        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        internal class Factory : IDWFieldFactory<Vec3d>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override IDWField3d<Vec3d> Create(double power, double epsilon = SlurMath.ZeroTolerance)
            {
                return new IDWField3dVec3d(power, epsilon);
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        public IDWField3dVec3d(double power, double epsilon = SlurMath.ZeroTolerance)
            : base(power, epsilon)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public sealed override IDWField3d<Vec3d> Duplicate()
        {
            return IDWField3d.Vec3d.CreateCopy(this);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public sealed override Vec3d ValueAt(Vec3d point)
        {
            Vec3d sum = DefaultValue * DefaultWeight;
            double wsum = DefaultWeight;

            foreach (var dp in Points)
            {
                double w = dp.Influence / Math.Pow(point.DistanceTo(dp.Point) + Epsilon, Power);
                sum += dp.Value * w;
                wsum += w;
            }

            return (wsum > 0.0) ? sum / wsum : new Vec3d();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="gx"></param>
        /// <param name="gy"></param>
        /// <param name="gz"></param>
        public sealed override void GradientAt(Vec3d point, out Vec3d gx, out Vec3d gy, out Vec3d gz)
        {
            throw new NotImplementedException();
        }
    }
}
