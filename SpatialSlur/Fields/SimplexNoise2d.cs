
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
    public class SimplexNoise2d :
        IField2d<double>, IField2d<Vector2d>, IGradient2d<double>
    {
        /// <summary></summary>
        public Vector2d Translation;

        private Vector2d _scale;
        private Vector2d _scaleInv;


        /// <summary>
        /// 
        /// </summary>
        public SimplexNoise2d()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public SimplexNoise2d(Vector2d scale, Vector2d translation)
        {
            Scale = scale;
            Translation = translation;
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector2d Scale
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
        private void ToNoiseSpace(ref Vector2d point)
        {
            point = (point - Translation) * _scaleInv;
        }


        /// <inheritdoc />
        public double ValueAt(Vector2d point)
        {
            ToNoiseSpace(ref point);
            return Noise.Simplex.ValueAt(point.X, point.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2d VectorAt(Vector2d point)
        {
            ToNoiseSpace(ref point);
            return Noise.Simplex.VectorAt(point.X, point.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2d GradientAt(Vector2d point)
        {
            ToNoiseSpace(ref point);
            return Noise.Simplex.GradientAt(point.X, point.Y) * _scaleInv;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2d CurlAt(Vector2d point)
        {
            ToNoiseSpace(ref point);
            return Noise.Simplex.CurlAt(point.X, point.Y) * _scaleInv;
        }


        #region Explicit interface implementations

        Vector2d IField2d<Vector2d>.ValueAt(Vector2d point)
        {
            return VectorAt(point);
        }


        void IGradient2d<double>.GradientAt(Vector2d point, out double gx, out double gy)
        {
            (gx, gy) = GradientAt(point);
        }

        #endregion
    }
}
