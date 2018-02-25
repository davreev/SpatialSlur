using System;

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
        public static readonly Orient3d Identity = new Orient3d(OrthoBasis3d.Identity, Vec3d.Zero);


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
        /// <param name="rotation"></param>
        public static implicit operator Orient3d(OrthoBasis3d rotation)
        {
            return new Orient3d(rotation, Vec3d.Zero);
        }


        /// <summary>
        /// Applies the given transformation to the given point.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vec3d operator *(Orient3d transform, Vec3d point)
        {
            return transform.Apply(point);
        }


        /// <summary>
        /// Concatenates the given transformations by applying the first to the second.
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static Orient3d operator *(Orient3d t0, Orient3d t1)
        {
            t0.Apply(ref t1);
            return t1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Orient3d CreateFromPoints(Vec3d p0, Vec3d p1, Vec3d p2)
        {
            return new Orient3d(p0, p1 - p0, p2 - p1);
        }


        /// <summary>
        /// Creates a relative transformation from t0 to t1.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Orient3d CreateFromTo(Orient3d from, Orient3d to)
        {
            return to.Apply(from.Inverse);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Orient3d CreateFromTo(ref Orient3d from, ref Orient3d to)
        {
            return to.Apply(from.Inverse);
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
            var rot = new OrthoBasis3d(target - eye, up);
            rot.SwapZX();

            var orient = new Orient3d(rot, eye);
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
        public OrthoBasis3d Rotation;
        /// <summary></summary>
        public Vec3d Translation;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="translation"></param>
        public Orient3d(OrthoBasis3d rotation, Vec3d translation)
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
            Rotation = new OrthoBasis3d(x, y);
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
        /// Applies the inverse of this transformation to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d ApplyInverse(Vec3d point)
        {
            return Rotation.ApplyInverse(point - Translation);
        }


        /// <summary>
        /// Applies this transformation to the given transformation.
        /// </summary>
        /// <param name="other"></param>
        public Orient3d Apply(Orient3d other)
        {
            Apply(ref other);
            return other;
        }


        /// <summary>
        /// Applies this transformation to the given transformation in place.
        /// </summary>
        /// <param name="other"></param>
        public void Apply(ref Orient3d other)
        {
            Rotation.Apply(ref other.Rotation);
            other.Translation = Apply(other.Translation);
        }

        
        /// <summary>
        /// Applies the inverse of this transformation to the given transformation.
        /// </summary>
        /// <param name="other"></param>
        public Orient3d ApplyInverse(Orient3d other)
        {
            ApplyInverse(ref other);
            return other;
        }


        /// <summary>
        /// Applies the inverse of this transformation to the given transformation in place.
        /// </summary>
        /// <param name="other"></param>
        public void ApplyInverse(ref Orient3d other)
        {
            Rotation.ApplyInverse(ref other.Rotation);
            other.Translation = ApplyInverse(other.Translation);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool ApproxEquals(ref Orient3d other, double tolerance = SlurMath.ZeroTolerance)
        {
            return
                Translation.ApproxEquals(other.Translation, tolerance) &&
                Rotation.ApproxEquals(ref other.Rotation, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Matrix4d ToMatrix()
        {
            var rx = Rotation.X;
            var ry = Rotation.Y;
            var rz = Rotation.Z;

            return new Matrix4d(
                rx.X, ry.X, rz.X, Translation.X,
                rx.Y, ry.Y, rz.Y, Translation.Y,
                rx.Z, ry.Z, rz.Z, Translation.Z,
                0.0, 0.0, 0.0, 1.0
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Deconstruct(out OrthoBasis3d rotation, out Vec3d translation)
        {
            rotation = Rotation;
            translation = Translation;
        }
    }
}
