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
    /// Represents a proper rigid transformation in 3 dimensions.
    /// https://en.wikipedia.org/wiki/Rigid_transformation
    /// </summary>
    [Serializable]
    public struct Orient3d
    {
        #region Static

        /// <summary></summary>
        public static readonly Orient3d Identity = new Orient3d(Rotate3d.Identity, Vec3d.Zero);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="orient"></param>
        public static implicit operator Orient3d(Orient2d orient)
        {
            return new Orient3d(orient.Rotation, orient.Translation);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotate"></param>
        public static implicit operator Orient3d(Rotate3d rotate)
        {
            return new Orient3d(rotate, Vec3d.Zero);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="orient"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vec3d operator *(Orient3d orient, Vec3d point)
        {
            return orient.Apply(point);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static Orient3d operator *(Orient3d t0, Orient3d t1)
        {
            return t0.Apply(t1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="orient"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vec3d Multiply(ref Orient3d orient, Vec3d point)
        {
            return orient.Apply(point);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        public static Orient3d Multiply(ref Orient3d t0, ref Orient3d t1)
        {
            return t0.Apply(t1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Orient3d CreateFrom3Points(Vec3d p0, Vec3d p1, Vec3d p2)
        {
            return new Orient3d(p0, p1 - p0, p2 - p1);
        }


        /// <summary>
        /// Creates a relative transformation from t0 to t1.
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static Orient3d CreateRelative(Orient3d t0, Orient3d t1)
        {
            return CreateRelative(ref t0, ref t1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static Orient3d CreateRelative(ref Orient3d t0, ref Orient3d t1)
        {
            return t1.Apply(t0.Inverse);
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eye"></param>
        /// <param name="target"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static Orient3d CreateLookAt(Vec3d eye, Vec3d target, Vec3d up)
        {
            var rotate = new Rotate3d(target - eye, up);
            rotate.SwapZX();

            var orient = new Orient3d(rotate, eye);
            orient.Invert();

            return orient;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Orient3d CreateRotationAboutPoint(AxisAngle3d rotation, Vec3d point)
        {
            throw new NotImplementedException();
        }

        #endregion


        /// <summary></summary>
        public Rotate3d Rotation;
        /// <summary></summary>
        public Vec3d Translation;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="translation"></param>
        public Orient3d(Rotate3d rotation, Vec3d translation)
        {
            Rotation = rotation;
            Translation = translation;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Orient3d(Vec3d origin, Vec3d x, Vec3d y)
        {
            Rotation = new Rotate3d(x, y);
            Translation = origin;
        }
  

        /// <summary>
        /// 
        /// </summary>
        public Orient3d Inverse
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
            Translation = Rotation.Apply(-Translation);
        }


        /// <summary>
        /// Applies this transformation to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d Apply(Vec3d point)
        {
            return Rotation.Apply(point) + Translation;
        }


        /// <summary>
        /// Applies this transformation to the given transformation.
        /// </summary>
        /// <param name="other"></param>
        public Orient3d Apply(Orient3d other)
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
        public Vec3d ApplyInverse(Vec3d point)
        {
            return Rotation.ApplyInverse(point - Translation);
        }


        /// <summary>
        /// Applies the inverse of this transformation to the given transformation.
        /// </summary>
        /// <param name="other"></param>
        public Orient3d ApplyInverse(Orient3d other)
        {
            other.Rotation = Rotation.ApplyInverse(other.Rotation);
            other.Translation = ApplyInverse(other.Translation);
            return other;
        }
    }
}
