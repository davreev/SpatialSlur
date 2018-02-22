#if USING_RHINO

using System;
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
    internal class MeshField3dDouble : MeshField3d<double>
    {
        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        internal class Factory : MeshField3dFactory<double>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="mesh"></param>
            /// <returns></returns>
            public override MeshField3d<double> Create(HeMesh3d mesh)
            {
                return new MeshField3dDouble(mesh);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="field"></param>
            /// <returns></returns>
            public override MeshField3d<double> Create(MeshField3d field)
            {
                return new MeshField3dDouble(field);
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public MeshField3dDouble(HeMesh3d mesh)
            : base(mesh)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public MeshField3dDouble(MeshField3d other)
            : base(other)
        {
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public sealed override MeshField3d<double> Duplicate(bool setValues)
        {
            var result = MeshField3d.Double.Create(this);
            if(setValues) result.Set(this);
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
        public override double ValueAt(int i0, int i1, int i2, double w0, double w1, double w2)
        {
            return Values[i0] * w0 + Values[i1] * w1 + Values[i2] * w2;
        }
    }
}

#endif