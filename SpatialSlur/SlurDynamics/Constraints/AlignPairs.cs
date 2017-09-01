using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * Notes 
 */

namespace SpatialSlur.SlurDynamics
{
    using H = ParticleHandle;

    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class AlignPairs : ParticleConstraint<H>
    {
        private H _hA0 = new H();
        private H _hA1 = new H();
        private H _hB0 = new H();
        private H _hB1 = new H();


        /// <summary>
        /// 
        /// </summary>
        public H StartA
        {
            get { return _hA0; }
        }


        /// <summary>
        /// 
        /// </summary>
        public H EndA
        {
            get { return _hA1; }
        }


        /// <summary>
        /// 
        /// </summary>
        public H StartB
        {
            get { return _hB0; }
        }


        /// <summary>
        /// 
        /// </summary>
        public H EndB
        {
            get { return _hB1; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override sealed IEnumerable<H> Handles
        {
            get
            {
                yield return _hA0;
                yield return _hA1;
                yield return _hB0;
                yield return _hB1;
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startA"></param>
        /// <param name="endA"></param>
        /// <param name="startB"></param>
        /// <param name="endB"></param>
        /// <param name="weight"></param>
        public AlignPairs(int startA, int endA, int startB, int endB, double weight = 1.0)
        {
            SetHandles(startA, endA, startB, endB);
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            Vec3d d0 = particles[_hA1].Position - particles[_hA0].Position;
            Vec3d d1 = particles[_hB1].Position - particles[_hB0].Position;
            double d01 = Vec3d.Dot(d0, d1);
            
            _hA1.Delta = (d1 - d01 / d0.SquareLength * d0) * 0.25; // perp of d1 realtive to d0
            _hB1.Delta = (d0 - d01 / d1.SquareLength * d1) * 0.25; // perp of d0 realtive to d1

            _hA0.Delta = -_hA1.Delta;
            _hB0.Delta = -_hB1.Delta;

            _hA0.Weight = _hA1.Weight = _hB0.Weight = _hB1.Weight = Weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="startA"></param>
        /// <param name="endA"></param>
        /// <param name="startB"></param>
        /// <param name="endB"></param>
        public void SetHandles(int startA, int endA, int startB, int endB)
        {
            _hA0.Index = startA;
            _hA1.Index = endA;
            _hB0.Index = startB;
            _hB1.Index = endB;
        }
    }
}
