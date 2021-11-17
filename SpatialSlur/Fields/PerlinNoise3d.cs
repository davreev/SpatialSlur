
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
    public class PerlinNoise3d :
        IField3d<double>, IField3d<Vector3d>, IGradient3d<double>
    {
        /// <summary></summary>
        public Vector3d Translation;

        private Vector3d _scale;
        private Vector3d _scaleInv;


        /// <summary>
        /// 
        /// </summary>
        public PerlinNoise3d()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public PerlinNoise3d(Vector3d scale, Vector3d translation)
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
        /// Transforms the given point from model space to noise space.
        /// </summary>
        /// <param name="point"></param>
        private void ModelToNoise(ref Vector3d point)
        {
            point = (point - Translation) * _scaleInv;
        }


        /// <inheritdoc />
        public double ValueAt(Vector3d point)
        {
            ModelToNoise(ref point);
            return Noise.Perlin.ValueAt(point.X, point.Y, point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector3d VectorAt(Vector3d point)
        {
            ModelToNoise(ref point);
            return Noise.Perlin.VectorAt(point.X, point.Y, point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector3d GradientAt(Vector3d point)
        {
            ModelToNoise(ref point);
            return Noise.Perlin.GradientAt(point.X, point.Y, point.Z) * _scaleInv;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector3d CurlAt(Vector3d point)
        {
            ModelToNoise(ref point);
            return Noise.Perlin.CurlAt(point.X, point.Y, point.Z) * _scaleInv;
        }
        
        
        #region Explicit Interface Implementations

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
