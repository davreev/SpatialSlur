using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;


/*
 * Notes
 */

namespace SpatialSlur.SlurRhino.Remesher
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DynamicRemesherSettings
    {
        private Domain _lengthRange = new Domain(1.0, 1.0);
        private double _lengthTolerance = 0.1;
        
        private double _featureWeight = 100.0;
        private double _smoothWeight = 1.0;

        private double _timeStep = 1.0;
        private double _damping = 0.1;

        private int _subSteps = 10;
        private int _refineFreq = 10;


        /// <summary>
        /// 
        /// </summary>
        public DynamicRemesherSettings() { }
        

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
        public double LengthTolerance
        {
            get { return _lengthTolerance; }
            set { _lengthTolerance = SlurMath.Saturate(value); }
        }


        /// <summary>
        /// 
        /// </summary>
        public double FeatureWeight
        {
            get { return _featureWeight; }
            set { _featureWeight = Math.Max(value, 0.0); }
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
