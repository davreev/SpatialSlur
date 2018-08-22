
/*
 * Notes
 */

using System;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur
{
    /// <summary>
    /// Represents a proper rigid transformation in 3 dimensions.
    /// https://en.wikipedia.org/wiki/Rigid_transformation
    /// </summary>
    [Serializable]
    public struct Orient3d
    {
        #region Static Members

        /// <summary></summary>
        public static readonly Orient3d Identity = new Orient3d(OrthoBasis3d.Identity, Vector3d.Zero);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public static implicit operator Orient3d(OrthoBasis3d rotation)
        {
            return new Orient3d(rotation, Vector3d.Zero);
        }


        /// <summary>
        /// Applies the given transformation to the given point.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3d operator *(Orient3d transform, Vector3d point)
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
            return t0.Apply(ref t1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Orient3d CreateFromPoints(Vector3d p0, Vector3d p1, Vector3d p2)
        {
            return new Orient3d(OrthoBasis3d.CreateFromXY(p1 - p0, p2 - p0), p0);
        }


        /// <summary>
        /// Creates a relative transformation from t0 to t1.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Orient3d CreateFromTo(Orient3d from, Orient3d to)
        {
            return CreateFromTo(ref from, ref to);
        }


        /// <summary>
        /// Creates a relative transformation from t0 to t1.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Orient3d CreateFromTo(ref Orient3d from, ref Orient3d to)
        {
            var inv = from.Inverse;
            return to.Apply(ref inv);
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eye"></param>
        /// <param name="target"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static Orient3d CreateLookAt(Vector3d eye, Vector3d target, Vector3d up)
        {
            return new Orient3d(OrthoBasis3d.CreateLookAt(target - eye, up), eye);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Orient3d CreateRotationAboutPoint(AxisAngle3d rotation, Vector3d point)
        {
            throw new NotImplementedException();
        }

        #endregion


        /// <summary></summary>
        public OrthoBasis3d Rotation;
        /// <summary></summary>
        public Vector3d Translation;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="translation"></param>
        public Orient3d(OrthoBasis3d rotation, Vector3d translation)
        {
            Rotation = rotation;
            Translation = translation;
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
        public Vector3d Apply(Vector3d point)
        {
            return Rotation.Apply(point) + Translation;
        }


        /// <summary>
        /// Applies the inverse of this transformation to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector3d ApplyInverse(Vector3d point)
        {
            return Rotation.ApplyInverse(point - Translation);
        }


        /// <summary>
        /// Applies this transformation to the given transformation.
        /// </summary>
        /// <param name="other"></param>
        public Orient3d Apply(Orient3d other)
        {
            return Apply(ref other);
        }


        /// <summary>
        /// Applies this transformation to the given transformation.
        /// </summary>
        /// <param name="other"></param>
        public Orient3d Apply(ref Orient3d other)
        {
            return new Orient3d
            {
                Rotation = Rotation.Apply(ref other.Rotation),
                Translation = Apply(other.Translation)
            };
        }


        /// <summary>
        /// Applies the inverse of this transformation to the given transformation.
        /// </summary>
        /// <param name="other"></param>
        public Orient3d ApplyInverse(Orient3d other)
        {
            return ApplyInverse(ref other);
        }


        /// <summary>
        /// Applies the inverse of this transformation to the given transformation.
        /// </summary>
        /// <param name="other"></param>
        public Orient3d ApplyInverse(ref Orient3d other)
        {
            return new Orient3d
            {
                Rotation = Rotation.ApplyInverse(ref other.Rotation),
                Translation = ApplyInverse(other.Translation)
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(Orient3d other, double epsilon = D.ZeroTolerance)
        {
            return ApproxEquals(ref other, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(ref Orient3d other, double epsilon = D.ZeroTolerance)
        {
            return
                Translation.ApproxEquals(other.Translation, epsilon) &&
                Rotation.ApproxEquals(ref other.Rotation, epsilon);
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
        /// <param name="rotation"></param>
        /// <param name="translation"></param>
        public void Deconstruct(out OrthoBasis3d rotation, out Vector3d translation)
        {
            rotation = Rotation;
            translation = Translation;
        }
    }
}
