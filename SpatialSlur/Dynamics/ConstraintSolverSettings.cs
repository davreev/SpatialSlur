
/*
 * Notes
 */

using System;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ConstraintSolverSettings
    {
        private double _linearDamping = 0.1;
        private double _angularDamping = 0.1;

        private double _linearTolerance = 1.0e-4;
        private double _angularTolerance = 1.0e-4;

        private double _timeStep = 1.0;


        /// <summary>
        /// 
        /// </summary>
        public double LinearDamping
        {
            get => _linearDamping;
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new ArgumentOutOfRangeException("The value must be between 0.0 and 1.0.");

                _linearDamping = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double AngularDamping
        {
            get => _angularDamping;
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new ArgumentOutOfRangeException("The value must be between 0.0 and 1.0.");

                _angularDamping = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double LinearTolerance
        {
            get => _linearTolerance;
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("The value cannot be negative.");

                _linearTolerance = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double AngularTolerance
        {
            get => _angularTolerance;
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("The value cannot be negative.");

                _angularTolerance = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double TimeStep
        {
            get => _timeStep;
            set
            {
                if (value <= 0.0)
                    throw new ArgumentOutOfRangeException("The value must be greater than zero.");

                _timeStep = value;
            }
        }
    }
}
