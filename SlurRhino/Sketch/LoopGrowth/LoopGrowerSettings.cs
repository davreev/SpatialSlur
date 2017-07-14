using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurRhino.LoopGrowth
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class LoopGrowerSettings
    {
        private Domain _lengthRange = new Domain(1.0, 1.0);
        private double _radFactor = 0.75;
        
        private double _smoothWeight = 5.0;
        private double _collideWeight = 1.0;

        private double _timeStep = 1.0;
        private double _damping = 0.1;

        private int _subSteps = 10;
        private int _refineFreq = 10;


        /// <summary>
        /// 
        /// </summary>
        public LoopGrowerSettings() { }


        /// <summary>
        /// 
        /// </summary>
        public Domain LengthRange
        {
            get { return _lengthRange; }
            set { _lengthRange = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double RadiusFactor
        {
            get { return _radFactor; }
            set { _radFactor = Math.Max(value, 0.0); }
        }


        /// <summary>
        /// 
        /// </summary>
        public double CollideRadius
        {
            get { return _lengthRange.Max * _radFactor * 0.5; }
        }
        

        /// <summary>
        /// 
        /// </summary>
        public double SmoothWeight
        {
            get { return _smoothWeight; }
            set { _smoothWeight = Math.Max(value, 0.0); }
        }


        /// <summary>
        /// 
        /// </summary>
        public double CollideWeight
        {
            get { return _collideWeight; }
            set { _collideWeight = Math.Max(value, 0.0); }
        }


        /// <summary>
        /// 
        /// </summary>
        public double TimeStep
        {
            get { return _timeStep; }
            set { _timeStep = Math.Max(value, 0.0); }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Damping
        {
            get { return _damping; }
            set { _damping = SlurMath.Saturate(value); }
        }


        /// <summary>
        /// 
        /// </summary>
        public int SubSteps
        {
            get { return _subSteps; }
            set { _subSteps = Math.Max(value, 0); }
        }


        /// <summary>
        /// 
        /// </summary>
        public int RefineFrequency
        {
            get { return _refineFreq; }
            set { _refineFreq = Math.Max(value, 1); }
        }
    }
}
