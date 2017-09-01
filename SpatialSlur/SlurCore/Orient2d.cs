using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    public struct Orient2d
    {
        #region Static

        /// <summary></summary>
        public static readonly Orient3d Identity = new Orient3d(Rotate3d.Identity, Vec3d.Zero);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotate"></param>
        public static implicit operator Orient2d(Rotate2d rotate)
        {
            return new Orient2d(rotate, Vec2d.Zero);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="orient"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vec2d operator *(Orient2d orient, Vec2d point)
        {
            return orient.Apply(point);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static Orient2d operator *(Orient2d t0, Orient2d t1)
        {
            return t0.Apply(t1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="orient"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vec2d Multiply(ref Orient2d orient, Vec2d point)
        {
            return orient.Apply(point);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static Orient2d Multiply(ref Orient2d t0, ref Orient2d t1)
        {
            return t0.Apply(t1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        public static Orient2d CreateFrom2Points(Vec2d p0, Vec2d p1)
        {
            return new Orient2d(p0, p1 - p0);
        }


        /// <summary>
        /// Creates a change of basis transformation from t0 to t1
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Orient2d CreateChangeBasis(ref Orient2d from, ref Orient2d to)
        {
            return to.Apply(from.Inverse);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Orient3d CreateRotationAtPoint(AxisAngle3d rotation, Vec3d point)
        {
            throw new NotImplementedException();
        }

        #endregion


        /// <summary></summary>
        public Rotate2d Rotation;
        /// <summary></summary>
        public Vec2d Translation;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="translation"></param>
        public Orient2d(Rotate2d rotation, Vec2d translation)
        {
            Rotation = rotation;
            Translation = translation;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="x"></param>
        public Orient2d(Vec2d origin, Vec2d x)
        {
            Rotation = new Rotate2d(x);
            Translation = origin;
        }


        /// <summary>
        /// 
        /// </summary>
        public Orient2d Inverse
        {
            get
            {
                var result = this;
                result.Invert();
                return result;
            }
        }


        /// <summary>
        /// Return false if the rotation is undefined.
        /// </summary>
        bool IsValid
        {
            get { return Rotation.IsValid; }
        }


        /// <summary>
        /// Inverts this transformation in place.
        /// </summary>
        public void Invert()
        {
            Rotation.Invert();
            Translation = -Rotation.Apply(Translation);
        }


        /// <summary>
        /// Applies this transformation to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec2d Apply(Vec2d point)
        {
            return Rotation.Apply(point) + Translation;
        }


        /// <summary>
        /// Applies this transformation to the given transformation.
        /// </summary>
        /// <param name="other"></param>
        public Orient2d Apply(Orient2d other)
        {
            other.Rotation = Rotation.Apply(other.Rotation);
            other.Translation = Apply(other.Translation);
            return other;
        }


        /// <summary>
        /// Applies the inverse of this transformation to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec2d ApplyInverse(Vec2d point)
        {
            return Rotation.ApplyInverse(point - Translation);
        }


        /// <summary>
        /// Applies the inverse of this transformation to the given transformation.
        /// </summary>
        /// <param name="other"></param>
        public Orient2d ApplyInverse(Orient2d other)
        {
            other.Rotation = Rotation.ApplyInverse(other.Rotation);
            other.Translation = ApplyInverse(other.Translation);
            return other;
        }
    }
}
