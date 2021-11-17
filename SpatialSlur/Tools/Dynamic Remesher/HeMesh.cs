/*
 * Notes
 */

using System;
using SpatialSlur.Meshes.Impl;

namespace SpatialSlur.Tools.DynamicRemesher
{
    using V = HeMesh.Vertex;
    using E = HeMesh.Halfedge;
    using F = HeMesh.Face;


    /// <summary>
    /// Contains HeMesh element classes used in dynamic remeshing
    /// </summary>
    [Serializable]
    public class HeMesh : HeMesh<V, E, F>
    {
        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new class Vertex : HeMesh<V, E, F>.Vertex
        {
            /// <summary></summary>
            public Vector3d Position;
            /// <summary></summary>
            public Vector3d Velocity;

            internal Vector3d MoveSum;
            internal double WeightSum;
            internal int FeatureCount = 0;
            internal int Stamp = int.MinValue;
            internal int DegreeCached;
            private int _featureIndex = -1;


            /// <summary>
            /// 
            /// </summary>
            public int FeatureIndex
            {
                get => _featureIndex;
                internal set => _featureIndex = value;
            }


            /// <summary>
            /// 
            /// </summary>
            public bool IsFeature
            {
                get => FeatureIndex != -1;
            }


            /// <summary>
            /// 
            /// </summary>
            public bool IsFixed
            {
                get => FeatureCount == -1;
            }


            /// <summary>
            /// 
            /// </summary>
            public void Fix()
            {
                FeatureCount = -1;
            }


            /// <summary>
            /// 
            /// </summary>
            public void Unfix()
            {
                if (IsFixed)
                    FeatureCount = 0;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new class Halfedge : HeMesh<V, E, F>.Halfedge
        {
            private double _targetLength;
            private int _featureIndex = -1;


            /// <summary>
            /// 
            /// </summary>
            public double TargetLength
            {
                get { return _targetLength; }
                internal set { _targetLength = Twin._targetLength = value; }
            }


            /// <summary>
            /// 
            /// </summary>
            public int FeatureIndex
            {
                get { return _featureIndex; }
                internal set { _featureIndex = Twin._featureIndex = value; }
            }


            /// <summary>
            /// 
            /// </summary>
            public bool IsFeature
            {
                get { return _featureIndex != -1; }
            }


            /// <summary>
            /// 
            /// </summary>
            internal bool CanCollapse
            {
                get { return !Start.IsFixed && FeatureIndex == Start.FeatureIndex; }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new class Face : HeMesh<V, E, F>.Face
        {
            /// <summary></summary>
            internal int Stamp = int.MinValue;
        }

        #endregion


        #region Static Members

        /// <summary></summary>
        public static readonly HeMeshFactory Factory = new HeMeshFactory();

        #endregion


        /// <summary>
        /// 
        /// </summary>
        public HeMesh()
            : base()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <param name="faceCapacity"></param>
        public HeMesh(int vertexCapacity, int hedgeCapacity, int faceCapacity)
            : base(vertexCapacity, hedgeCapacity, faceCapacity)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public HeMesh(HeMesh other)
        {
            Append(other);
        }


        /// <inheritdoc />
        protected sealed override V NewVertex()
        {
            return new V();
        }


        /// <inheritdoc />
        protected sealed override E NewHalfedge()
        {
            return new E();
        }


        /// <inheritdoc />
        protected sealed override F NewFace()
        {
            return new F();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HeMeshFactory : HeMeshFactory<HeMesh, V, E, F>
    {
        /// <inheritdoc />
        public sealed override HeMesh Create(int vertexCapacity, int halfedgeCapacity, int faceCapacity)
        {
            return new HeMesh(vertexCapacity, halfedgeCapacity, faceCapacity);
        }
    }
}