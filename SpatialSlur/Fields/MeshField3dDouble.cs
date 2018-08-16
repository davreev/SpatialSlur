
/*
 * Notes
 */

#if USING_RHINO

using System;
using SpatialSlur;
using SpatialSlur.Meshes;
using SpatialSlur.Fields;

namespace SpatialSlur.Fields
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
            /// <inheritdoc />
            public override MeshField3d<double> Create(HeMesh3d mesh)
            {
                return new MeshField3dDouble(mesh);
            }


            /// <inheritdoc />
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


        /// <inheritdoc />
        public sealed override MeshField3d<double> Duplicate(bool setValues)
        {
            var result = MeshField3d.Double.Create(this);
            if(setValues) result.Set(this);
            return result;
        }


        /// <inheritdoc />
        public override double ValueAt(Vector3d weights, Vector3i indices)
        {
            return 
                Values[indices.X] * weights.X + 
                Values[indices.Y] * weights.Y + 
                Values[indices.Z] * weights.Z;
        }
    }
}

#endif