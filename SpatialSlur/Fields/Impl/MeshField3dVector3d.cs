
/*
 * Notes
 */

#if USING_RHINO

using System;
using SpatialSlur;
using SpatialSlur.Fields;
using SpatialSlur.Meshes;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    internal class MeshField3dVector3d : MeshField3d<Vector3d>
    {
        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        internal class Factory : MeshField3dFactory<Vector3d>
        {
            /// <inheritdoc />
            public override MeshField3d<Vector3d> Create(HeMesh3d mesh)
            {
                return new MeshField3dVector3d(mesh);
            }


            /// <inheritdoc />
            public override MeshField3d<Vector3d> Create(MeshField3d field)
            {
                return new MeshField3dVector3d(field);
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public MeshField3dVector3d(HeMesh3d mesh)
            : base(mesh)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public MeshField3dVector3d(MeshField3d other)
            : base(other)
        {
        }


        /// <inheritdoc />
        public sealed override MeshField3d<Vector3d> Duplicate(bool setValues)
        {
            var result = MeshField3d.Vector3d.Create(this);
            if (setValues) result.Set(this);
            return result;
        }


        /// <inheritdoc />
        public override Vector3d ValueAt(Vector3d weights, Vector3i indices)
        {
            return
                Values[indices.X] * weights.X +
                Values[indices.Y] * weights.Y +
                Values[indices.Z] * weights.Z;
        }
    }
}

#endif