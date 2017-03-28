using System;
using System.Collections.Generic;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class HeMeshHalfedge : Halfedge<HeMeshHalfedge, HeMeshVertex, HeMeshFace>
    {
        /// <summary>
        /// 
        /// </summary>
        internal override HeMeshHalfedge Self
        {
            get { return this; }
        }


        /// <summary>
        /// Returns true if the halfedge and its twin have different faces.
        /// </summary>
        internal bool IsValid
        {
            get { return Face != Twin.Face; }
        }


        /// <summary>
        /// Returns true if the halfedge starts at a degree 1 vertex.
        /// </summary>
        internal bool IsAtDegree1
        {
            get { return Twin == Previous; }
        }


        /// <summary>
        /// Returns true if the halfedge starts at a degree 2 vertex.
        /// </summary>
        public bool IsAtDegree2
        {
            get { return Twin.Next == Previous.Twin; }
        }


        /// <summary>
        /// Returns true if the halfedge starts at a degree 3 vertex.
        /// </summary>
        public bool IsAtDegree3
        {
            get { return Twin.Next.Twin == Previous.Twin.Previous; }
        }


        /// <summary>
        /// Returns true if the halfedge is in a one-sided hole.
        /// </summary>
        internal bool IsInDegree1
        {
            get { return ReferenceEquals(this, Next); }
        }


        /// <summary>
        /// Returns true if the halfedge is in a two-sided face or hole.
        /// </summary>
        public bool IsInDegree2
        {
            get { return Next == Previous; }
        }


        /// <summary>
        /// Returns true if the halfedge is in a three-sided face or hole.
        /// </summary>
        public bool IsInDegree3
        {
            get { return Next.Next == Previous; }
        }


        /// <summary>
        /// Circulates the start vertex starting from this halfedge.
        /// </summary>
        public override IEnumerable<HeMeshHalfedge> CirculateStart
        {
            get
            {
                var he0 = Self;
                var he1 = he0;

                do
                {
                    yield return he1;
                    he1 = he1.Twin.Next;
                } while (he1 != he0);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int CountEdgesAtStart()
        {
            var he0 = Self;
            var he1 = he0;
            int count = 0;

            do
            {
                count++;
                he1 = he1.Twin.Next;
            } while (he1 != he0);

            return count;
        }


        /// <summary>
        /// Removes a halfedge and its twin from the mesh.
        /// Note that this method does not update face->halfedge references.
        /// </summary>
        internal void Remove()
        {
            Bypass();
            Twin.Bypass();
            MakeUnused();
        }


        /// <summary>
        /// 
        /// </summary>
        internal void Bypass()
        {
            if (IsAtDegree1)
            {
                Start.MakeUnused();
                return;
            }

            if (Start.First == this)
                Start.First = Twin.Next;

            MakeConsecutive(Previous, Twin.Next);
        }


        /// <summary>
        /// 
        /// </summary>
        public void MakeFirstInFace()
        {
            Face.First = this;
        }
    }
}
