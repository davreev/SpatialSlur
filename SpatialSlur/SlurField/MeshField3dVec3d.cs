#if USING_RHINO

using System;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    internal class MeshField3dVec3d : MeshField3d<Vec3d>
    {
        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        internal class Factory : MeshField3dFactory<Vec3d>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="mesh"></param>
            /// <returns></returns>
            public override MeshField3d<Vec3d> Create(HeMesh3d mesh)
            {
                return new MeshField3dVec3d(mesh);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="field"></param>
            /// <returns></returns>
            public override MeshField3d<Vec3d> Create(MeshField3d field)
            {
                return new MeshField3dVec3d(field);
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public MeshField3dVec3d(HeMesh3d mesh)
            : base(mesh)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public MeshField3dVec3d(MeshField3d other)
            : base(other)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public sealed override MeshField3d<Vec3d> Duplicate(bool setValues)
        {
            var result = MeshField3d.Vec3d.Create(this);
            if (setValues) result.Set(this);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <param name="w0"></param>
        /// <param name="w1"></param>
        /// <param name="w2"></param>
        /// <returns></returns>
        public override Vec3d ValueAt(int i0, int i1, int i2, double w0, double w1, double w2)
        {
            return Values[i0] * w0 + Values[i1] * w1 + Values[i2] * w2;
        }
    }
}

#endif