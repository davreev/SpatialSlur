
/*
 * Notes
 */

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur
{
    /// <summary>
    /// Represents an angle-preserving affine transformation in 2 dimensions.
    /// </summary>
    public struct Transform2d
    {
        #region Static Members

        /// <summary></summary>
        public static readonly Transform2d Identity = new Transform2d(new Vector2d(1.0), OrthoBasis2d.Identity, Vector2d.Zero);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public static implicit operator Transform2d(OrthoBasis2d rotation)
        {
            return new Transform2d(new Vector2d(1.0), rotation, Vector2d.Zero);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="orient"></param>
        public static implicit operator Transform2d(Orient2d orient)
        {
            return new Transform2d(new Vector2d(1.0), orient.Rotation, orient.Translation);
        }


        /// <summary>
        /// Applies the given transformation to the given point.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector2d operator *(Transform2d transform, Vector2d point)
        {
            return transform.Apply(point);
        }


        /// <summary>
        /// Concatenates the given transformations by applying the first to the second.
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static Transform2d operator *(Transform2d t0, Transform2d t1)
        {
            return t0.Apply(t1);
        }


        /// <summary>
        /// Creates a relative transformation from t0 to t1.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Transform2d CreateFromTo(Transform2d from, Transform2d to)
        {
            return to.Apply(from.Inverse);
        }

        #endregion


        /// <summary></summary>
        public OrthoBasis2d Rotation;
        /// <summary></summary>
        public Vector2d Translation;
        /// <summary></summary>
        public Vector2d Scale;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="rotation"></param>
        /// <param name="translation"></param>
        public Transform2d(Vector2d scale, OrthoBasis2d rotation, Vector2d translation)
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
        public Transform2d(Vector2d scale, Orient2d orientation)
        {
            Scale = scale;
            Rotation = orientation.Rotation;
            Translation = orientation.Translation;
        }


        /// <summary>
        /// 
        /// </summary>
        public Transform2d Inverse
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
        /// 
        /// </summary>
        /// <returns></returns>
        public Transform3d As3d
        {
            get => new Transform3d(Scale.As3d, Rotation.As3d, Translation.As3d);
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
        public Vector2d Apply(Vector2d point)
        {
            return Rotation.Apply(point * Scale) + Translation;
        }


        /// <summary>
        /// Applies this transformation to the given transformation.
        /// </summary>
        /// <param name="other"></param>
        public Transform2d Apply(Transform2d other)
        {
            return new Transform2d
            {
                Rotation = Rotation.Apply(other.Rotation),
                Translation = Apply(other.Translation),
                Scale = other.Scale * Scale
            };
        }


        /// <summary>
        /// Applies the inverse of this transformation to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2d ApplyInverse(Vector2d point)
        {
            return Rotation.ApplyInverse(point - Translation) / Scale;
        }


        /// <summary>
        /// Applies the inverse of this transformation to the given transformation.
        /// </summary>
        /// <param name="other"></param>
        public Transform2d ApplyInverse(Transform2d other)
        {
            return new Transform2d
            {
                Rotation = Rotation.ApplyInverse(other.Rotation),
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
        public bool ApproxEquals(Transform2d other, double epsilon = D.ZeroTolerance)
        {
            return
                Translation.ApproxEquals(other.Translation, epsilon) &&
                Rotation.ApproxEquals(other.Rotation, epsilon) &&
                Scale.ApproxEquals(other.Scale, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Matrix3d ToMatrix()
        {
            var rx = Rotation.X * Scale.X;
            var ry = Rotation.Y * Scale.Y;

            return new Matrix3d(
                rx.X, ry.X, Translation.X,
                rx.Y, ry.Y, Translation.Y,
                0.0, 0.0, 1.0
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="rotation"></param>
        /// <param name="translation"></param>
        public void Deconstruct(out Vector2d scale, out OrthoBasis2d rotation, out Vector2d translation)
        {
            scale = Scale;
            rotation = Rotation;
            translation = Translation;
        }
    }
}
