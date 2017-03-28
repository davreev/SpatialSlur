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
    public sealed class HeGraphHalfedge : Halfedge<HeGraphHalfedge, HeGraphVertex>
    {
        /// <summary>
        /// 
        /// </summary>
        internal override HeGraphHalfedge Self
        {
            get { return this; }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsAtDegree1
        {
            get { return Next == this; }
        }


        /// <summary>
        /// Returns true if the halfedge starts at a degree 2 vertex
        /// </summary>
        public bool IsAtDegree2
        {
            get { return !IsAtDegree1 && Next == Previous; }
        }


        /// <summary>
        /// Returns true if the halfedge starts at a degree 3 vertex.
        /// </summary>
        public bool IsAtDegree3
        {
            get { return !IsAtDegree1 && Next.Next == Previous; }
        }


        /// <summary>
        /// Circulates the start vertex starting from this halfedge.
        /// </summary>
        public override IEnumerable<HeGraphHalfedge> CirculateStart
        {
            get
            {
                var he0 = Self;
                var he1 = he0;

                do
                {
                    yield return he1;
                    he1 = he1.Next;
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
                he1 = he1.Next;
            } while (he1 != he0);

            return count;
        }


        /// <summary>
        /// 
        /// </summary>
        public void Remove()
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

            if (Start.First == this) Start.First = Next;
            MakeConsecutive(Previous, Next);
        }
    }
}
