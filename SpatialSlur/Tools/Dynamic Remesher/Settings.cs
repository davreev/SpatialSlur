/*
 * Notes
 */

using System;

namespace SpatialSlur.Tools.DynamicRemesher
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Settings
    {
        private Intervald _lengthRange = new Intervald(1.0, 1.0);
        private double _lengthTolerance = 0.01;
        private double _featureWeight = 100.0;
        private double _featureTolerance = 1.0e-4;
        private double _damping = 0.5;
        private double _timeStep = 1.0;
        private int _refineFreq = 3;


        /// <summary>
        /// 
        /// </summary>
        public Intervald LengthRange
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
        public double FeatureTolerance
        {
            get { return _featureTolerance; }
            set { _featureTolerance = Math.Max(value, 0.0); }
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
        public double TimeStep
        {
            get { return _timeStep; }
            set { _timeStep = Math.Max(value, 0.0); }
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