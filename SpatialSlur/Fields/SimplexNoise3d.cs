
/*
 * Notes
 */

using System;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SimplexNoise3d :
        IField3d<double>, IField3d<Vector3d>, IGradient3d<double>
    {
        /// <summary></summary>
        public Vector3d Translation;

        private Vector3d _scale;
        private Vector3d _scaleInv;


        /// <summary>
        /// 
        /// </summary>
        public SimplexNoise3d()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public SimplexNoise3d(Vector3d scale, Vector3d translation)
        {
            Scale = scale;
            Translation = translation;
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector3d Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                _scaleInv = 1.0 / value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        private void ToNoiseSpace(ref Vector3d point)
        {
            point = (point - Translation) * _scaleInv;
        }


        /// <inheritdoc />
        public double ValueAt(Vector3d point)
        {
            ToNoiseSpace(ref point);
            return Noise.Simplex.ValueAt(point.X, point.Y, point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector3d VectorAt(Vector3d point)
        {
            ToNoiseSpace(ref point);
            return Noise.Simplex.VectorAt(point.X, point.Y, point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector3d GradientAt(Vector3d point)
        {
            ToNoiseSpace(ref point);
            return Noise.Simplex.GradientAt(point.X, point.Y, point.Z) * _scaleInv;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector3d CurlAt(Vector3d point)
        {
            ToNoiseSpace(ref point);
            return Noise.Simplex.CurlAt(point.X, point.Y, point.Z) * _scaleInv;
        }


        #region Explicit interface implementations

        Vector3d IField3d<Vector3d>.ValueAt(Vector3d point)
        {
            return VectorAt(point);
        }


        void IGradient3d<double>.GradientAt(Vector3d point, out double gx, out double gy, out double gz)
        {
            (gx, gy, gz) = GradientAt(point);
        }

        #endregion
    }
}
