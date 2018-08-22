
/*
 * Notes
 */

using System;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur
{
    /// <summary>
    /// Represents a proper rigid transformation in 2 dimensions.
    /// https://en.wikipedia.org/wiki/Rigid_transformation
    /// </summary>
    public struct Orient2d
    {
        #region Static Members

        /// <summary></summary>
        public static readonly Orient2d Identity = new Orient2d(OrthoBasis2d.Identity, Vector2d.Zero);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public static implicit operator Orient2d(OrthoBasis2d rotation)
        {
            return new Orient2d(rotation, Vector2d.Zero);
        }


        /// <summary>
        /// Applies the given transformation to the given point.
        /// 
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector2d operator *(Orient2d transform, Vector2d point)
        {
            return transform.Apply(point);
        }


        /// <summary>
        /// Concatenates the given transformations by applying the first to the second.
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
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        public static Orient2d CreateFromPoints(Vector2d p0, Vector2d p1)
        {
            return new Orient2d(p0, p1 - p0);
        }


        /// <summary>
        /// Creates a relative transformation from t0 to t1.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Orient2d CreateFromTo(Orient2d from, Orient2d to)
        {
            return to.Apply(from.Inverse);
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Orient3d CreateRotationAtPoint(AxisAngle3d rotation, Vector3d point)
        {
            throw new NotImplementedException();
        }

        #endregion


        /// <summary></summary>
        public OrthoBasis2d Rotation;
        /// <summary></summary>
        public Vector2d Translation;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="translation"></param>
        public Orient2d(OrthoBasis2d rotation, Vector2d translation)
        {
            Rotation = rotation;
            Translation = translation;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="x"></param>
        public Orient2d(Vector2d origin, Vector2d x)
        {
            Rotation = new OrthoBasis2d(x);
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
        /// 
        /// </summary>
        /// <returns></returns>
        public Orient3d As3d
        {
            get => new Orient3d(Rotation.As3d, Translation.As3d);
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
        public Vector2d Apply(Vector2d point)
        {
            return Rotation.Apply(point) + Translation;
        }


        /// <summary>
        /// Applies the inverse of this transformation to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2d ApplyInverse(Vector2d point)
        {
            return Rotation.ApplyInverse(point - Translation);
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
        /// Applies the inverse of this transformation to the given transformation.
        /// </summary>
        /// <param name="other"></param>
        public Orient2d ApplyInverse(Orient2d other)
        {
            other.Rotation = Rotation.ApplyInverse(other.Rotation);
            other.Translation = ApplyInverse(other.Translation);
            return other;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(Orient2d other, double epsilon = D.ZeroTolerance)
        {
            return
                Translation.ApproxEquals(other.Translation, epsilon) &&
                Rotation.ApproxEquals(other.Rotation, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Matrix3d ToMatrix()
        {
            var rx = Rotation.X;
            var ry = Rotation.Y;

            return new Matrix3d(
                rx.X, ry.X, Translation.X,
                rx.Y, ry.Y, Translation.Y,
                0.0, 0.0, 1.0
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="translation"></param>
        public void Deconstruct(out OrthoBasis2d rotation, out Vector2d translation)
        {
            rotation = Rotation;
            translation = Translation;
        }
    }
}
