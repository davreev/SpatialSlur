
/*
 * Notes
 */

using System;

using SpatialSlur;
using SpatialSlur.Fields;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    internal class IDWField3dVector3d : IDWField3d<Vector3d>
    {
        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        internal class Factory : IDWFieldFactory<Vector3d>
        {
            /// <inheritdoc />
            public override IDWField3d<Vector3d> Create(double power, double epsilon = D.ZeroTolerance)
            {
                return new IDWField3dVector3d(power, epsilon);
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        public IDWField3dVector3d(double power, double epsilon = D.ZeroTolerance)
            : base(power, epsilon)
        {
        }


        /// <inheritdoc />
        public sealed override IDWField3d<Vector3d> Duplicate()
        {
            return IDWField3d.Vector3d.CreateCopy(this);
        }


        /// <inheritdoc />
        public sealed override Vector3d ValueAt(Vector3d point)
        {
            Vector3d sum = Vector3d.Zero;
            double wsum = 0.0;

            foreach (var obj in Objects)
            {
                double w = obj.Influence / Math.Pow(obj.DistanceTo(point) + Epsilon, Power);
                sum += obj.Value * w;
                wsum += w;
            }

            return (wsum > 0.0) ? sum / wsum : new Vector3d();
        }


        /// <inheritdoc />
        public sealed override void GradientAt(Vector3d point, out Vector3d gx, out Vector3d gy, out Vector3d gz)
        {
            throw new NotImplementedException();
        }
    }
}
