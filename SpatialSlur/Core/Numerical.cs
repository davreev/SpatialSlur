
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Constd = SpatialSlur.SlurMath.Constantsd;
using Constf = SpatialSlur.SlurMath.Constantsf;

namespace SpatialSlur
{
    /// <summary>
    /// 
    /// </summary>
    public static class Numerical
    {
        /// <summary>
        /// Returns a numerical approximation of the gradient of the given function with respect to the given vector.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="vector"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static Vector2d GetGradient(Func<Vector2d, double> function, Vector2d vector, double epsilon = Constd.ZeroTolerance)
        {
            (var x, var y) = vector;

            double gx = function(new Vector2d(x + epsilon, y)) - function(new Vector2d(x - epsilon, y));
            double gy = function(new Vector2d(x, y + epsilon)) - function(new Vector2d(x, y - epsilon));

            return new Vector2d(gx, gy) / (2.0 * epsilon);
        }


        /// <summary>
        /// Returns a numerical approximation of the gradient of the given function with respect to the given vector.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="vector"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static Vector3d GetGradient(Func<Vector3d, double> function, Vector3d vector, double epsilon = Constd.ZeroTolerance)
        {
            (var x, var y, var z) = vector;

            double gx = function(new Vector3d(x + epsilon, y, z)) - function(new Vector3d(x - epsilon, y, z));
            double gy = function(new Vector3d(x, y + epsilon, z)) - function(new Vector3d(x, y - epsilon, z));
            double gz = function(new Vector3d(x, y, z + epsilon)) - function(new Vector3d(x, y, z - epsilon));

            return new Vector3d(gx, gy, gz) / (2.0 * epsilon);
        }


        /// <summary>
        /// Returns a numerical approximation of the gradient of the given function with respect to the given vector.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="vector"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static Vector4d GetGradient(Func<Vector4d, double> function, Vector4d vector, double epsilon = Constd.ZeroTolerance)
        {
            (var x, var y, var z, var w) = vector;

            double gx = function(new Vector4d(x + epsilon, y, z, w)) - function(new Vector4d(x - epsilon, y, z, w));
            double gy = function(new Vector4d(x, y + epsilon, z, w)) - function(new Vector4d(x, y - epsilon, z, w));
            double gz = function(new Vector4d(x, y, z + epsilon, w)) - function(new Vector4d(x, y, z - epsilon, w));
            double gw = function(new Vector4d(x, y, z, w + epsilon)) - function(new Vector4d(x, y, z, w - epsilon));

            return new Vector4d(gx, gy, gz, gw) / (2.0 * epsilon);
        }


        /// <summary>
        /// Returns a numerical approximation of the gradient of the given function with respect to the given vector.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="vector"></param>
        /// <param name="result"></param>
        /// <param name="epsilon"></param>
        public static void GetGradient(Func<double[], double> function, double[] vector, double[] result, double epsilon = Constd.ZeroTolerance)
        {
            double d2 = 1.0 / (epsilon * 2.0);

            for (int i = 0; i < vector.Length; i++)
            {
                double t = vector[i];

                vector[i] = t + epsilon;
                double g0 = function(vector);

                vector[i] = t - epsilon;
                double g1 = function(vector);

                result[i] = (g0 - g1) * d2;
                vector[i] = t;
            }
        }


        /// <summary>
        /// Returns a numerical approximation of the Jacobian of the given function with respect to the given vector.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="vector"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static Matrix2d GetJacobian(Func<Vector2d, Vector2d> function, Vector2d vector, double epsilon = Constd.ZeroTolerance)
        {
            (var x, var y) = vector;

            var col0 = function(new Vector2d(x + epsilon, y)) - function(new Vector2d(x - epsilon, y));
            var col1 = function(new Vector2d(x, y + epsilon)) - function(new Vector2d(x, y - epsilon));

            return new Matrix2d(col0, col1) / (2.0 * epsilon);
        }


        /// <summary>
        /// Returns a numerical approximation of the Jacobian of the given function with respect to the given vector.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="vector"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static Matrix3d GetJacobian(Func<Vector3d, Vector3d> function, Vector3d vector, double epsilon = Constd.ZeroTolerance)
        {
            (var x, var y, var z) = vector;

            var col0 = function(new Vector3d(x + epsilon, y, z)) - function(new Vector3d(x - epsilon, y, z));
            var col1 = function(new Vector3d(x, y + epsilon, z)) - function(new Vector3d(x, y - epsilon, z));
            var col2 = function(new Vector3d(x, y, z + epsilon)) - function(new Vector3d(x, y, z - epsilon));

            return new Matrix3d(col0, col1, col2) / (2.0 * epsilon);
        }


        /// <summary>
        /// Returns a numerical approximation of the Jacobian of the given function with respect to the given vector.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="vector"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static Matrix4d GetJacobian(Func<Vector4d, Vector4d> function, Vector4d vector, double epsilon = Constd.ZeroTolerance)
        {
            (var x, var y, var z, var w) = vector;

            var col0 = function(new Vector4d(x + epsilon, y, z, w)) - function(new Vector4d(x - epsilon, y, z, w));
            var col1 = function(new Vector4d(x, y + epsilon, z, w)) - function(new Vector4d(x, y - epsilon, z, w));
            var col2 = function(new Vector4d(x, y, z + epsilon, w)) - function(new Vector4d(x, y, z - epsilon, w));
            var col3 = function(new Vector4d(x, y, z, w + epsilon)) - function(new Vector4d(x, y, z, w - epsilon));

            return new Matrix4d(col0, col1, col2, col3) / (2.0 * epsilon);
        }


        /// <summary>
        /// Returns a numerical approximation of the Hessian of the given function with respect to the given vector.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="vector"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static Matrix2d GetHessian(Func<Vector2d, double> function, Vector2d vector, double epsilon = Constd.ZeroTolerance)
        {
            return GetJacobian(p => GetGradient(function, vector, epsilon), vector, epsilon);
        }


        /// <summary>
        /// Returns a numerical approximation of the Hessian of the given function with respect to the given vector.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="vector"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static Matrix3d GetHessian(Func<Vector3d, double> function, Vector3d vector, double epsilon = Constd.ZeroTolerance)
        {
            return GetJacobian(p => GetGradient(function, vector, epsilon), vector, epsilon);
        }


        /// <summary>
        /// Returns a numerical approximation of the Hessian of the given function with respect to the given vector.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="vector"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static Matrix4d GetHessian(Func<Vector4d, double> function, Vector4d vector, double epsilon = Constd.ZeroTolerance)
        {
            return GetJacobian(p => GetGradient(function, vector, epsilon), vector, epsilon);
        }


        /// <summary>
        /// Approximates the signed distance to the nearest root of an evaluated function.
        /// </summary>
        public static double DistanceToRoot(double value, Vector2d gradient)
        {
            var m = gradient.SquareLength;

            if (m > 0.0)
                return value / Math.Sqrt(m);

            return value < 0.0 ? double.NegativeInfinity : double.PositiveInfinity;
        }


        /// <summary>
        /// Approximates the signed distance to the nearest root of an evaluated function.
        /// </summary>
        public static double DistanceToRoot(double value, Vector3d gradient)
        {
            var m = gradient.SquareLength;

            if (m > 0.0)
                return value / Math.Sqrt(m);

            return value < 0.0 ? double.NegativeInfinity : double.PositiveInfinity;
        }
        

        /// <summary>
        /// Finds the nearest root of the given function via Newton's method.
        /// </summary>
        /// <returns></returns>
        public static Vector2d FindRoot(Func<Vector2d, double> function, Func<Vector2d, Vector2d> gradient, Vector2d point, double epsilon = Constd.ZeroTolerance, int maxSteps = 100)
        {
            while(maxSteps-- > 0)
            {
                var d = function(point);
                if (d < epsilon) return point;

                var g = gradient(point);
                if (Math.Abs(g.X) > 0.0) point.X -= d / g.X;
                if (Math.Abs(g.Y) > 0.0) point.Y -= d / g.Y;
            }

            return point;
        }


        /// <summary>
        /// Finds the nearest root of the given function via Newton's method.
        /// </summary>
        /// <returns></returns>
        public static Vector3d FindRoot(Func<Vector3d, double> function, Func<Vector3d, Vector3d> gradient, Vector3d point, double epsilon = Constd.ZeroTolerance, int maxSteps = 100)
        {
            while (maxSteps-- > 0)
            {
                var d = function(point);
                if (d < epsilon) return point;

                var g = gradient(point);
                if (Math.Abs(g.X) > 0.0) point.X -= d / g.X;
                if (Math.Abs(g.Y) > 0.0) point.Y -= d / g.Y;
                if (Math.Abs(g.Z) > 0.0) point.Z -= d / g.Z;
            }

            return point;
        }


        /// <summary>
        /// Finds the nearest root of the given function via Newton's method.
        /// </summary>
        /// <returns></returns>
        public static Vector4d FindRoot(Func<Vector4d, double> function, Func<Vector4d, Vector4d> gradient, Vector4d point, double epsilon = Constd.ZeroTolerance, int maxSteps = 100)
        {
            while (maxSteps-- > 0)
            {
                var d = function(point);
                if (d < epsilon) return point;

                var g = gradient(point);
                if (Math.Abs(g.X) > 0.0) point.X -= d / g.X;
                if (Math.Abs(g.Y) > 0.0) point.Y -= d / g.Y;
                if (Math.Abs(g.Z) > 0.0) point.Z -= d / g.Z;
                if (Math.Abs(g.W) > 0.0) point.W -= d / g.W;
            }

            return point;
        }
    }
}
