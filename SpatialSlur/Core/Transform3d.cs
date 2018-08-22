
/*
 * Notes
 */

using System;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur
{
    /// <summary>
    /// Represents an angle-preserving affine transformation in 3 dimensions.
    /// </summary>
    public partial struct Transform3d
    {
        #region Static Members

        /// <summary></summary>
        public static readonly Transform3d Identity = new Transform3d(new Vector3d(1.0), OrthoBasis3d.Identity, Vector3d.Zero);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public static implicit operator Transform3d(OrthoBasis3d rotation)
        {
            return new Transform3d(new Vector3d(1.0), rotation, Vector3d.Zero);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="orient"></param>
        public static implicit operator Transform3d(Orient3d orient)
        {
            return new Transform3d(new Vector3d(1.0), orient.Rotation, orient.Translation);
        }


        /// <summary>
        /// Applies the given transformation to the given point.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3d operator *(Transform3d transform, Vector3d point)
        {
            return transform.Apply(point);
        }


        /// <summary>
        /// Concatenates the given transformations by applying the first to the second.
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static Transform3d operator *(Transform3d t0, Transform3d t1)
        {
            return t0.Apply(ref t1);
        }


        /// <summary>
        /// Creates relative transformation from t0 to t1.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Transform3d CreateFromTo(Transform3d from, Transform3d to)
        {
            return CreateFromTo(ref from, ref to);
        }


        /// <summary>
        /// Creates relative transformation from t0 to t1.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Transform3d CreateFromTo(ref Transform3d from, ref Transform3d to)
        {
            var inv = from.Inverse;
            return to.Apply(ref inv);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Transform3d CreateReflection(Vector3d point, Vector3d normal)
        {
            throw new NotImplementedException();
        }

        #endregion


        /// <summary></summary>
        public OrthoBasis3d Rotation;
        /// <summary></summary>
        public Vector3d Translation;
        /// <summary></summary>
        public Vector3d Scale;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="rotation"></param>
        /// <param name="translation"></param>
        public Transform3d(Vector3d scale, OrthoBasis3d rotation, Vector3d translation)
        {
            Scale = scale;
            Rotation = rotation;
            Translation = translation;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="orientation"></param>
        public Transform3d(Vector3d scale, Orient3d orientation)
        {
            Scale = scale;
            Rotation = orientation.Rotation;
            Translation = orientation.Translation;
        }


        /// <summary>
        /// 
        /// </summary>
        public Transform3d Inverse
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
            Scale = 1.0 / Scale;
            Rotation.Invert();
            Translation = Rotation.Apply(-Translation) * Scale;
        }


        /// <summary>
        /// Applies this transformation to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector3d Apply(Vector3d point)
        {
            return Rotation.Apply(point * Scale) + Translation;
        }


        /// <summary>
        /// Applies this transformation to the given transformation in place.
        /// </summary>
        /// <param name="other"></param>
        public Transform3d Apply(Transform3d other)
        {
            return Apply(ref other);
        }


        /// <summary>
        /// Applies this transformation to the given transformation in place.
        /// </summary>
        /// <param name="other"></param>
        public Transform3d Apply(ref Transform3d other)
        {
            return new Transform3d
            {
                Rotation = Rotation.Apply(ref other.Rotation),
                Translation = Apply(other.Translation),
                Scale = other.Scale * Scale
            };
        }


        /// <summary>
        /// Applies the inverse of this transformation to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector3d ApplyInverse(Vector3d point)
        {
            return Rotation.ApplyInverse(point - Translation) / Scale;
        }


        /// <summary>
        /// Applies the inverse of this transformation to the given transformation in place.
        /// </summary>
        /// <param name="other"></param>
        public Transform3d ApplyInverse(Transform3d other)
        {
            return ApplyInverse(ref other);
        }


        /// <summary>
        /// Applies the inverse of this transformation to the given transformation in place.
        /// </summary>
        /// <param name="other"></param>
        public Transform3d ApplyInverse(ref Transform3d other)
        {
            return new Transform3d
            {
                Rotation = Rotation.ApplyInverse(ref other.Rotation),
                Translation = ApplyInverse(other.Translation),
                Scale = other.Scale / Scale
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(Transform3d other, double epsilon = D.ZeroTolerance)
        {
            return ApproxEquals(ref other, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(ref Transform3d other, double epsilon = D.ZeroTolerance)
        {
            return
                Translation.ApproxEquals(other.Translation, epsilon) &&
                Rotation.ApproxEquals(ref other.Rotation, epsilon) &&
                Scale.ApproxEquals(other.Scale, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Matrix4d ToMatrix()
        {
            var rx = Rotation.X * Scale.X;
            var ry = Rotation.Y * Scale.Y;
            var rz = Rotation.Z * Scale.Z;

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
        /// <param name="scale"></param>
        /// <param name="rotation"></param>
        /// <param name="translation"></param>
        public void Deconstruct(out Vector3d scale, out OrthoBasis3d rotation, out Vector3d translation)
        {
            scale = Scale;
            rotation = Rotation;
            translation = Translation;
        }
    }
}
