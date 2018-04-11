
using SpatialSlur.SlurCore;

/*
 * Notes 
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public class SimplexNoiseField : 
        IField2d<double>, IGradient2d<double>, IField2d<Vec2d>,
        IField3d<double>, IGradient3d<double>, IField3d<Vec3d>
    {
        private double _dx = 0.0;
        private double _dy = 0.0;
        private double _dz = 0.0;
        
        private double _tx = 1.0;
        private double _ty = 1.0;
        private double _tz = 1.0;

        private double _txInv = 1.0;
        private double _tyInv = 1.0;
        private double _tzInv = 1.0;


        /// <summary>
        /// 
        /// </summary>
        public SimplexNoiseField()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public SimplexNoiseField(Vec2d scale, Vec2d offset)
        {
            ScaleX = scale.X;
            ScaleY = scale.Y;

            _dx = offset.X;
            _dy = offset.Y;
        }


        /// <summary>
        /// 
        /// </summary>
        public SimplexNoiseField(Vec3d scale, Vec3d offset)
        {
            ScaleX = scale.X;
            ScaleY = scale.Y;
            ScaleZ = scale.Z;

            _dx = offset.X;
            _dy = offset.Y;
            _dz = offset.Z;
        }


        /// <summary>
        /// 
        /// </summary>
        public double Scale
        {
            set
            {
                _tx = _ty = _tz = value;
                _txInv = _tyInv = _tzInv = 1.0 / value;
            }
        }


        /// <summary></summary>
        public double ScaleX
        {
            get { return _tx; }
            set
            {
                _tx = value;
                _txInv = 1.0 / value;
            }
        }


        /// <summary></summary>
        public double ScaleY
        {
            get { return _ty; }
            set
            {
                _ty = value;
                _tyInv = 1.0 / value;
            }
        }


        /// <summary></summary>
        public double ScaleZ
        {
            get { return _tz; }
            set
            {
                _tz = value;
                _tzInv = 1.0 / value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double OffsetX
        {
            get => _dx;
            set => _dx = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public double OffsetY
        {
            get => _dy;
            set => _dy = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public double OffsetZ
        {
            get => _dz;
            set => _dz = value;
        }


        #region 2d operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        private void ToNoiseSpace(ref Vec2d point)
        {
            point.X = (point.X + _dx) * _txInv;
            point.Y = (point.Y + _dy) * _tyInv;
        }


        /// <inheritdoc />
        public double ValueAt(Vec2d point)
        {
            ToNoiseSpace(ref point);
            return SimplexNoise.ValueAt(point.X, point.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec2d VectorAt(Vec2d point)
        {
            ToNoiseSpace(ref point);
            return SimplexNoise.VectorAt(point.X, point.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec2d GradientAt(Vec2d point)
        {
            ToNoiseSpace(ref point);
            var g =  SimplexNoise.GradientAt(point.X, point.Y);
            g.X *= _txInv;
            g.Y *= _tyInv;
            return g;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec2d CurlAt(Vec2d point)
        {
            ToNoiseSpace(ref point);
            var c = SimplexNoise.CurlAt(point.X, point.Y);
            c.X *= _txInv;
            c.Y *= _tyInv;
            return c;
        }

        #endregion


        #region 3d operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        private void ToNoiseSpace(ref Vec3d point)
        {
            point.X = (point.X + _dx) * _txInv;
            point.Y = (point.Y + _dy) * _tyInv;
            point.Z = (point.Z + _dz) * _tzInv;
        }


        /// <inheritdoc />
        public double ValueAt(Vec3d point)
        {
            ToNoiseSpace(ref point);
            return SimplexNoise.ValueAt(point.X, point.Y, point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d VectorAt(Vec3d point)
        {
            ToNoiseSpace(ref point);
            return SimplexNoise.VectorAt(point.X, point.Y, point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d GradientAt(Vec3d point)
        {
            ToNoiseSpace(ref point);
            var g = SimplexNoise.GradientAt(point.X, point.Y, point.Z);
            g.X *= _txInv;
            g.Y *= _tyInv;
            g.Z *= _tzInv;
            return g;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d CurlAt(Vec3d point)
        {
            ToNoiseSpace(ref point);
            var c = SimplexNoise.CurlAt(point.X, point.Y, point.Z);
            c.X *= _txInv;
            c.Y *= _tyInv;
            c.Z *= _tzInv;
            return c;
        }

        #endregion


        #region Explicit interface implementations

        /// <inheritdoc />
        Vec2d IField2d<Vec2d>.ValueAt(Vec2d point)
        {
            return VectorAt(point);
        }


        /// <inheritdoc />
        Vec3d IField3d<Vec3d>.ValueAt(Vec3d point)
        {
            return VectorAt(point);
        }


        /// <inheritdoc />
        void IGradient2d<double>.GradientAt(Vec2d point, out double gx, out double gy)
        {
            (gx, gy) = GradientAt(point);
        }


        /// <inheritdoc />
        void IGradient3d<double>.GradientAt(Vec3d point, out double gx, out double gy, out double gz)
        {
            (gx, gy, gz) = GradientAt(point);
        }

        #endregion
    }
}
