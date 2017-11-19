using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Notes
 * http://danceswithcode.net/engineeringnotes/quaternions/quaternions.html
 * http://www.opengl-tutorial.org/intermediate-tutorials/tutorial-17-quaternions/
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Quaternion representation of a 3 dimensional rotation.
    /// Slower to concatenate than quaternions but faster to transform vectors.
    /// See detailed comparison here https://en.wikipedia.org/wiki/Quaternions_and_spatial_rotation
    /// </summary>
    public partial struct Quaterniond
    {
        #region Static

        /// <summary></summary>
        public static readonly Quaterniond Zero = new Quaterniond();

        /// <summary></summary>
        public static readonly Quaterniond Identity = new Quaterniond(0.0, 0.0, 0.0, 1.0);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static implicit operator Quaterniond(Vec4d vector)
        {
            return new Quaterniond(vector.X, vector.Y, vector.Z, vector.W);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="quatern"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vec3d operator *(Quaterniond rotation, Vec3d vector)
        {
            return rotation.Apply(vector);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="quatern"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Quaterniond operator *(Quaterniond r0, Quaterniond r1)
        {
            return r0.Apply(r1);
        }


        /// <summary>
        /// Returns the unit quaternion dot product.
        /// This is the cosine of the angle between the 2 rotations.
        /// </summary>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <returns></returns>
        public static double Dot(Quaterniond r0, Quaterniond r1)
        {
            return r0.X * r1.X + r0.Y * r1.Y + r0.Z * r1.Z + r0.W * r1.W;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static Quaterniond Lerp(Quaterniond r0, Quaterniond r1, double factor)
        {
            return r0.LerpTo(r1, factor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static Quaterniond Nlerp(Quaterniond r0, Quaterniond r1, double factor)
        {
            return r0.NlerpTo(r1, factor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static Quaterniond Slerp(Quaterniond r0, Quaterniond r1, double factor)
        {
            return r0.SlerpTo(r1, factor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <returns></returns>
        public static Quaterniond CreateRelative(Quaterniond r0, Quaterniond r1)
        {
            return r1.Apply(r0.Inverse);
        }


        /// <summary>
        /// http://lolengine.net/blog/2013/09/18/beautiful-maths-quaternion-from-vectors
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Quaterniond CreateFromTo(Vec3d from, Vec3d to)
        {
            //TODO
            // https://stackoverflow.com/questions/1171849/finding-quaternion-representing-the-rotation-from-one-vector-to-another
            //throw new NotImplementedException();
            
            if (!from.Unitize() || !to.Unitize())
                return Identity;

            var ct = Vec3d.Dot(from, to);

            // parallel check
            if (1.0 - Math.Abs(ct) < SlurMath.ZeroTolerance)
            {
                //opposite check
                if(ct < 0.0)
                {
                    var perp = from.X < 1.0 ? from.CrossX() : from.CrossY();
                    var t = 1.0 / perp.Length;
                    return new Quaterniond(perp.X * t, perp.Y * t, perp.Z * t, 0.0);
                }

                return Identity;
            }

            // can assume axis is valid
            var axis = Vec3d.Cross(from, to);
            var q = new Quaterniond(axis.X, axis.Y, axis.Z, ct + 1);
            q.Unitize();

            return q;
        }


        /// <summary>
        /// http://www.opengl-tutorial.org/intermediate-tutorials/tutorial-17-quaternions/
        /// </summary>
        /// <param name="direction"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static Quaterniond CreateLookAt(Vec3d direction, Vec3d up)
        {
            throw new NotImplementedException();
        }

        #endregion


        /// <summary>
        /// First imaginary component = axis.X * sin(angle /2)
        /// </summary>
        public double X;

        /// <summary>
        /// Second imaginary component = axis.Y * sin(angle /2)
        /// 
        /// </summary>
        public double Y;

        /// <summary>
        /// Third imaginary component = axis.Z * sin(angle /2)
        /// </summary>
        public double Z;

        /// <summary>
        /// Real component = cos(angle/2)
        /// </summary>
        public double W;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public Quaterniond(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        public Quaterniond(Vec3d axis, double angle)
            : this()
        {
            Set(axis, angle);
        }


        /// <summary>
        /// The axis and angle of rotation are taken from the direction and length of the given vector respectively.
        /// </summary>
        /// <param name="rotation"></param>
        public Quaterniond(Vec3d rotation)
            : this()
        {
            Set(rotation);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public Quaterniond(AxisAngle3d rotation)
            : this()
        {
            Set(rotation);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public Quaterniond(OrthoBasis3d rotation)
            : this()
        {
            Set(rotation);
        }


        /// <summary>
        /// 
        /// </summary>
        public double SquareLength
        {
            get { return X * X + Y * Y + Z * Z + W * W; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Length
        {
            get { return Math.Sqrt(SquareLength); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Quaterniond Inverse
        {
            get { return Conjugate; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Quaterniond Conjugate
        {
            get { return new Quaterniond(-X, -Y, -Z, W); }
        }


        /// <summary>
        /// Returns a unit length copy of this quaternion.
        /// </summary>
        public Quaterniond Versor
        {
            get
            {
                var q = this;
                return q.Unitize() ? q : Zero;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsZero(double tolerance = SlurMath.ZeroTolerance)
        {
            return
                Math.Abs(X) < tolerance &&
                Math.Abs(Y) < tolerance &&
                Math.Abs(Z) < tolerance &&
                Math.Abs(W) < tolerance;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsUnit(double tolerance = SlurMath.ZeroTolerance)
        {
            return SlurMath.ApproxEquals(SquareLength, 1.0, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="imaginary"></param>
        /// <param name="real"></param>
        public void Set(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }


        /// <summary>
        /// Assumes the given rotation is valid.
        /// </summary>
        /// <param name="rotation"></param>
        public void Set(AxisAngle3d rotation)
        {
            var axis = rotation.Axis;
            var s = Math.Sqrt(0.5 - rotation.CosAngle);

            X = axis.X * s;
            Y = axis.Y * s;
            Z = axis.Z * s;
            W = Math.Sqrt(0.5 + rotation.CosAngle);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        public void Set(Vec3d axis, double angle)
        {
            if (!axis.Unitize())
                return;

            SetImpl(axis, angle);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public void Set(Vec3d rotation)
        {
            var angle = rotation.SquareLength;

            if (angle > 0.0)
            {
                angle = Math.Sqrt(angle);
                SetImpl(rotation / angle, angle);
                return;
            }

            SetIdentity();
        }


        /// <summary>
        /// Assumes axis is unit length
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        private void SetImpl(Vec3d axis, double angle)
        {
            var s = Math.Sin(angle * 0.5);
            X = axis.X * s;
            Y = axis.Y * s;
            Z = axis.Z * s;
            W = Math.Cos(angle * 0.5);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public void Set(OrthoBasis3d rotation)
        {
            Set(ref rotation);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public void Set(ref OrthoBasis3d rotation)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetIdentity()
        {
            X = Y = Z = 0.0;
            W = 1.0;
        }


        /// <summary>
        /// Unitizes the quaternion in place.
        /// </summary>
        /// <returns></returns>
        public bool Unitize()
        {
            double d = SquareLength;
            
            if (d > 0.0)
            {
                d = 1.0 / Math.Sqrt(d);
                X *= d;
                Y *= d;
                Z *= d;
                W *= d;
                return true;
            }

            return false;
        }
        

        /// <summary>
        /// Inverts this quaternion in place.
        /// </summary>
        public void Invert()
        {
            X = -X;
            Y = -Y;
            Z = -Z;
        }


        /// <summary>
        /// Linear interpolation between this quaternion and another.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Quaterniond LerpTo(Quaterniond other, double factor)
        {
            var cos = Vec4d.Dot(this, other);

            if (cos < 0.0)
            {
                return new Quaterniond(
                    X + (-other.X - X) * factor,
                    Y + (-other.Y - Y) * factor,
                    Z + (-other.Z - Z) * factor,
                    W + (-other.W - W) * factor
                    );
            }
            else
            {
                return new Quaterniond(
                    X + (other.X - X) * factor,
                    Y + (other.Y - Y) * factor,
                    Z + (other.Z - Z) * factor,
                    W + (other.W - W) * factor
                    );
            }
        }


        /// <summary>
        /// Normalized linear interpolation between this quaternion and another.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Quaterniond NlerpTo(Quaterniond other, double factor)
        {
            return LerpTo(other, factor).Versor;
        }


        /// <summary>
        /// Spherical interpolation between this quaternion and another.
        /// Assumes both are unit quaternions.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Quaterniond SlerpTo(Quaterniond other, double factor)
        {
            var ct = Dot(this, other);

            // same rotation
            if (1.0 - Math.Abs(ct) < SlurMath.ZeroTolerance)
                return this;

            var t = Math.Acos(ct);
            var st = Math.Sin(t);
            var stInv = 1.0 / st;

            var ta = factor * t;
            var k0 = Math.Sin(t - ta) * stInv;
            var k1 = Math.Sin(ta) * stInv;

            return new Quaterniond()
            {
                X = k0 * X + k1 * other.X,
                Y = k0 * Y + k1 * other.Y,
                Z = k0 * Z + k1 * other.Z,
                W = k0 * W + k1 * other.W
            };
        }


        /// <summary>
        /// Applies this rotation to the given vector.
        /// Assumes this quaternion is unit length.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vec3d Apply(Vec3d vector)
        {
            var x2 = 2.0 * X;
            var y2 = 2.0 * Y;
            var z2 = 2.0 * Z;

            var wx2 = W * x2;
            var wy2 = W * y2;
            var wz2 = W * z2;

            var xx2 = X * x2;
            var xy2 = X * y2;
            var xz2 = X * z2;

            var yy2 = Y * y2;
            var yz2 = Y * z2;
            var zz2 = Z * z2;

            return new Vec3d(
                vector.X * (1.0 - yy2 - zz2) + vector.Y * (xy2 - wz2) + vector.Z * (xz2 + wy2),
                vector.X * (xy2 + wz2) + vector.Y * (1.0 - xx2 - zz2) + vector.Z * (yz2 - wx2),
                vector.X * (xz2 - wy2) + vector.Y * (yz2 + wx2) + vector.Z * (1.0 - xx2 - yy2)
                );
        }


        /// <summary>
        /// Applies this rotation to the given rotation.
        /// Assumes both quaternions are unit length.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Quaterniond Apply(Quaterniond other)
        {
            return new Quaterniond()
            {
                X = W * other.X + X * other.W + Y * other.Z - Z * other.Y,
                Y = W * other.Y - X * other.Z + Y * other.W + Z * other.X,
                Z = W * other.Z + X * other.Y - Y * other.X + Z * other.W,
                W = W * other.W - X * other.X - Y * other.Y - Z * other.Z
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Matrix3d ToMatrix()
        {
            var x2 = 2.0 * X;
            var y2 = 2.0 * Y;
            var z2 = 2.0 * Z;

            var wx2 = W * x2;
            var wy2 = W * y2;
            var wz2 = W * z2;

            var xx2 = X * x2;
            var xy2 = X * y2;
            var xz2 = X * z2;

            var yy2 = Y * y2;
            var yz2 = Y * z2;
            var zz2 = Z * z2;

            return new Matrix3d(
                1.0 - yy2 - zz2, xy2 - wz2, xz2 + wy2,
                xy2 + wz2, 1.0 - xx2 - zz2, yz2 - wx2,
                xz2 - wy2, yz2 + wx2, 1.0 - xx2 - yy2
                );
        }
    }
}
