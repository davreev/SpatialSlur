using System;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ConstraintSolverSettings
    {
        private double _damping = 0.5;
        private double _angleDamping = 0.5;
        private double _tolerance = 1.0e-4;
        private double _angleTolerance = 1.0e-4;
        private double _timeStep = 1.0;


        /// <summary>
        /// 
        /// </summary>
        public double Damping
        {
            get { return _damping; }
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new ArgumentOutOfRangeException("Damping must be between 0.0 and 1.0.");

                _damping = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double AngleDamping
        {
            get { return _angleDamping; }
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new ArgumentOutOfRangeException("The value must be between 0.0 and 1.0.");

                _angleDamping = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Tolerance
        {
            get { return _tolerance; }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("The value cannot be negative.");

                _tolerance = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double AngleTolerance
        {
            get { return _angleTolerance; }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("The value cannot be negative.");

                _angleTolerance = value;
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        public double TimeStep
        {
            get { return _timeStep; }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("Time step cannot be negative.");

                _timeStep = value;
            }
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
        internal double AngleToleranceSquared
        {
            get { return _angleTolerance * _angleTolerance; }
        }
    }
}
