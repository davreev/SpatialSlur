using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurRhino.SteinerFinder
{
    /// <summary>
    /// 
    /// </summary>
    public class SteinerFinderSettings
    {
        private double _minLength = 1.0e-4;
        private double _timeStep = 1.0;
        private double _damping = 0.1;
        private double _tolerance = 1.0e-4;
        private int _subSteps = 10;
        private int _refineFreq = 10;


        /// <summary>
        /// 
        /// </summary>
        public double MinLength
        {
            get { return _minLength; }
            set { _minLength = Math.Max(value, 0.0); }
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
        public double Tolerance
        {
            get { return _tolerance; }
            set { _tolerance = Math.Max(value, 0.0); }
        }


        /// <summary>
        /// 
        /// </summary>
        internal double ToleranceSquared
        {
            get { return _tolerance * _tolerance; }
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
