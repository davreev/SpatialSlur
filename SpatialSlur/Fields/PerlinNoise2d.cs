﻿
/*
 * Notes 
 */

using System;
using SpatialSlur;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PerlinNoise2d :
        IField2d<double>, IField2d<Vector2d>, IGradient2d<double>
    {
        /// <summary></summary>
        public Vector2d Translation;

        private Vector2d _scale;
        private Vector2d _scaleInv;


        /// <summary>
        /// 
        /// </summary>
        public PerlinNoise2d()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public PerlinNoise2d(Vector2d scale, Vector2d translation)
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
        /// Transforms the given point from model space to noise space.
        /// </summary>
        /// <param name="point"></param>
        private void ModelToNoise(ref Vector2d point)
        {
            point = (point - Translation) * _scaleInv;
        }


        /// <inheritdoc />
        public double ValueAt(Vector2d point)
        {
            ModelToNoise(ref point);
            return Noise.Perlin.ValueAt(point.X, point.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2d VectorAt(Vector2d point)
        {
            ModelToNoise(ref point);
            return Noise.Perlin.VectorAt(point.X, point.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2d GradientAt(Vector2d point)
        {
            ModelToNoise(ref point);
            return Noise.Perlin.GradientAt(point.X, point.Y) * _scaleInv;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2d CurlAt(Vector2d point)
        {
            ModelToNoise(ref point);
            return Noise.Perlin.CurlAt(point.X, point.Y) * _scaleInv;
        }


        #region Explicit Interface Implementations

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
