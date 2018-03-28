
/*
 * Notes
 * 
 * impl refs
 * http://www.cs.ucr.edu/~vbz/resources/quatut.pdf
 * http://danceswithcode.net/engineeringnotes/quaternions/quaternions.html
 * http://www.opengl-tutorial.org/intermediate-tutorials/tutorial-17-quaternions/
 */

using System;

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Quaternion representation of a 3 dimensional rotation.
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
        public static Quaterniond CreateFromTo(Quaterniond r0, Quaterniond r1)
        {
            return r1.Apply(r0.Inverse);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Quaterniond CreateFromTo(Vec3d from, Vec3d to)
        {
            // impl refs
            // https://stackoverflow.com/questions/1171849/finding-quaternion-representing-the-rotation-from-one-vector-to-another
            // http://lolengine.net/blog/2013/09/18/beautiful-maths-quaternion-from-vectors

            if (!from.Unitize() || !to.Unitize())
                return Identity;

            var ca = Vec3d.Dot(from, to);

            // parallel check
            if (SlurMath.ApproxEquals(Math.Abs(ca), 1.0))
            {
                //opposite check
                if(ca < 0.0)
                {
                    var perp = from.X < 1.0 ? from.CrossX() : from.CrossY();
                    var t = 1.0 / perp.Length;
                    return new Quaterniond(perp.X * t, perp.Y * t, perp.Z * t, 0.0);
                }

                return Identity;
            }

            // can assume axis is valid
            var axis = Vec3d.Cross(from, to);
            var q = new Quaterniond(axis.X, axis.Y, axis.Z, ca + 1);
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
            Set(ref rotation);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public Quaterniond(OrthoBasis3d rotation)
            : this()
        {
            Set(ref rotation);
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
        public Quaterniond Unit
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
            return SquareLength < tolerance;
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
        /// 
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        public bool Set(Vec3d axis, double angle)
        {
            if (!axis.Unitize())
                return false;

            SetImpl(axis, angle);
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public bool Set(Vec3d rotation)
        {
            var angle = rotation.SquareLength;

            if (angle > 0.0)
            {
                angle = Math.Sqrt(angle);
                SetImpl(rotation / angle, angle);
                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public void Set(ref AxisAngle3d rotation)
        {
            SetImpl(rotation.Axis, rotation.Angle);
        }


        /// <summary>
        /// Assumes the given axis is unit length.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        private void SetImpl(Vec3d axis, double angle)
        {
            angle *= 0.5;
            var sa = Math.Sin(angle);
            X = axis.X * sa;
            Y = axis.Y * sa;
            Z = axis.Z * sa;
            W = Math.Cos(angle);
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public void Set(ref OrthoBasis3d rotation)
        {
            // impl ref 
            // http://www.cs.ucr.edu/~vbz/resources/quatut.pdf

            (var x, var y, var z) = rotation;
            double trace = x.X + y.Y + z.Z;
            
            if (trace > 0.0)
            {
                double s = Math.Sqrt(trace + 1.0);
                W = 0.5 * s;

                s = 0.5 / s;
                X = (y.Z - z.Y) * s;
                Y = (z.X - x.Z) * s;
                Z = (x.Y - y.X) * s;
            }
            else if (Math.Abs(x.X) > Math.Abs(y.Y) && Math.Abs(x.X) > Math.Abs(z.Z))
            {
                double s = Math.Sqrt(1.0 + x.X - y.Y - z.Z);
                X = 0.5 * s;

                s = 0.5 / s;
                Y = (x.Y + y.X) * s;
                Z = (z.X + x.Z) * s;
                W = (y.Z - z.Y) * s;
            }
            else if (Math.Abs(y.Y) > Math.Abs(z.Z))
            {
                double s = Math.Sqrt(1.0 - x.X + y.Y - z.Z);
                Y = 0.5 * s;

                s = 0.5 / s;
                X = (x.Y + y.X) * s;
                Z = (y.Z + z.Y) * s;
                W = (z.X - x.Z) * s;
            }
            else
            {
                double s = Math.Sqrt(1.0 - x.X - y.Y + z.Z);
                Z = 0.5 * s;

                s = 0.5 / s;
                X = (z.X + x.Z) * s;
                Y = (y.Z + z.Y) * s;
                W = (x.Y - y.X) * s;
            }
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
        /// Negates this quaternion in place.
        /// </summary>
        public void Negate()
        {
            X = -X;
            Y = -Y;
            Z = -Z;
            W = -W;
        }


        /// <summary>
        /// Linear interpolation between this quaternion and another.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Quaterniond LerpTo(Quaterniond other, double factor)
        {
            var ca = Vec4d.Dot(this, other);

            if (ca < 0.0)
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
            return LerpTo(other, factor).Unit;
        }


        /// <summary>
        /// Spherical linear interpolation between this quaternion and another.
        /// Assumes both are unit quaternions.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Quaterniond SlerpTo(Quaterniond other, double factor)
        {
            var ca = Dot(this, other);

            // TODO 
            // handle antiparallel case
            if (Math.Abs(ca) > 1.0 - SlurMath.ZeroTolerance)
                return this;

            // ensures interpolation takes shortest path
            if(ca < 0.0)
            {
                other.Negate();
                ca = -ca;
            }

            var a = Math.Acos(ca); // no need to clamp, already checked above
            var sa = Math.Sin(a);

            var saInv = 1.0 / sa; 
            var af = a * factor;

            var t0 = Math.Sin(a - af) * saInv;
            var t1 = Math.Sin(af) * saInv;

            return new Quaterniond(
                t0 * X + t1 * other.X,
                t0 * Y + t1 * other.Y,
                t0 * Z + t1 * other.Z,
                t0 * W + t1 * other.W
            );
        }


        /// <summary>
        /// Applies this rotation to the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vec3d Apply(Vec3d vector)
        {
            var a = SquareLength;

            if (a > 0.0)
            {
                var b = 2.0 / a;

                a = X * b;
                var wx2 = W * a;
                var xx2 = X * a;

                a = Y * b;
                var wy2 = W * a;
                var xy2 = X * a;
                var yy2 = Y * a;

                a = Z * b;
                var wz2 = W * a;
                var xz2 = X * a;
                var yz2 = Y * a;
                var zz2 = Z * a;

                return new Vec3d(
                    vector.X * (1.0 - yy2 - zz2) + vector.Y * (xy2 - wz2) + vector.Z * (xz2 + wy2),
                    vector.X * (xy2 + wz2) + vector.Y * (1.0 - xx2 - zz2) + vector.Z * (yz2 - wx2),
                    vector.X * (xz2 - wy2) + vector.Y * (yz2 + wx2) + vector.Z * (1.0 - xx2 - yy2)
                    );
            }

            return new Vec3d();
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
        /// <param name="other"></param>
        /// <returns></returns>
        public bool ApproxEquals(Quaterniond other, double tolerance = SlurMath.ZeroTolerance)
        {
            return
                SlurMath.ApproxEquals(X, other.X, tolerance) &&
                SlurMath.ApproxEquals(Y, other.Y, tolerance) &&
                SlurMath.ApproxEquals(Z, other.Z, tolerance) &&
                SlurMath.ApproxEquals(W, other.W, tolerance);
        }


        /// <summary>
        /// Assumes this quaternion is unit length.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        public Vec3d ToAxisAngle()
        {
            var s2 = 1.0 - W * W; // pythag's identity

            if (s2 > 0.0)
            {
                var t = 2.0 * Math.Acos(W) / Math.Sqrt(s2);
                return new Vec3d(X * t, Y * t, Z * t);
            }

            return Vec3d.Zero;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Matrix3d ToMatrix()
        {
            // impl ref 
            // http://www.cs.ucr.edu/~vbz/resources/quatut.pdf

            var a = SquareLength;

            if (a > 0.0)
            {
                var b = 2.0 / a;

                a = X * b;
                var wx2 = W * a;
                var xx2 = X * a;

                a = Y * b;
                var wy2 = W * a;
                var xy2 = X * a;
                var yy2 = Y * a;

                a = Z * b;
                var wz2 = W * a;
                var xz2 = X * a;
                var yz2 = Y * a;
                var zz2 = Z * a;

                return new Matrix3d(
                    1.0 - yy2 - zz2, xy2 - wz2, xz2 + wy2,
                    xy2 + wz2, 1.0 - xx2 - zz2, yz2 - wx2,
                    xz2 - wy2, yz2 + wx2, 1.0 - xx2 - yy2
                    );
            }

            return new Matrix3d();
        }
        

        /*
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public Matrix3d ToMatrix()
        {
            // impl ref 
            // http://www.cs.ucr.edu/~vbz/resources/quatut.pdf

            // assumes the quaternion is unit length

            var xx = X * X;
            var xy = X * Y;
            var xz = X * Z;
            var xw = X * W;
            var yy = Y * Y;
            var yz = Y * Z;
            var yw = Y * W;
            var zz = Z * Z;
            var zw = Z * W;

            return new Matrix3d
            (
                 1.0 - 2.0 * (yy + zz), 2.0 * (xy - zw), 2.0 * (xz + yw),
                 2.0 * (xy + zw), 1.0 - 2.0 * (xx + zz), 2.0 * (yz - xw),
                 2.0 * (xz - yw), 2.0 * (yz + xw), 1.0 - 2.0 * (xx + yy)
            );
        }
        */


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Deconstruct(out double x, out double y, out double z, out double w)
        {
            x = X;
            y = Y;
            z = Z;
            w = W;
        }
    }
}
