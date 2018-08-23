
/*
 * Notes
 */

using System;
using System.Collections.Generic;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur
{
    /// <summary>
    /// Double precision 2x2 matrix.
    /// </summary>
    [Serializable]
    public struct Matrix2d
    {
        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        public static class Decompose
        {
            /// <summary>
            /// Returns the eigen decomposition of the given matrix A.
            /// </summary>
            /// <param name="A"></param>
            /// <param name="Q"></param>
            /// <param name="lambda"></param>
            /// <param name="epsilon"></param>
            /// <returns></returns>
            public static bool EigenSymmetric(Matrix2d A, out Matrix2d Q, out Vector2d lambda, double epsilon = D.ZeroTolerance)
            {
                // impl ref
                // https://www.mpi-hd.mpg.de/personalhomes/globes/3x3/index.html
                // http://www.math.harvard.edu/archive/21b_fall_04/exhibits/2dmatrices/

                if (A.SolveCharacteristic(out lambda.X, out lambda.Y, epsilon))
                {
                    if (Math.Abs(A.M10) > epsilon)
                    {
                        Q.M00 = lambda.X - A.M11;
                        Q.M01 = lambda.Y - A.M11;
                        Q.M10 = Q.M11 = A.M10;
                    }
                    else if (Math.Abs(A.M01) > epsilon)
                    {
                        Q.M00 = Q.M01 = A.M01;
                        Q.M10 = lambda.X - A.M00;
                        Q.M11 = lambda.Y - A.M00;
                    }
                    else
                    {
                        Q = Identity;
                    }

                    return true;
                }

                Q = new Matrix2d();
                return false;
            }
        }
        
        #endregion


        #region Static Members

        /// <summary></summary>
        public static readonly Matrix2d Identity = new Matrix2d(1.0);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public static implicit operator Matrix2d(OrthoBasis2d rotation)
        {
            return rotation.ToMatrix();
        }


        /// <summary>
        /// Matrix scalar multiplication
        /// </summary>
        /// <returns></returns>
        public static Matrix2d operator *(Matrix2d matrix, double scalar)
        {
            matrix.Scale(scalar);
            return matrix;
        }


        /// <summary>
        /// Matrix scalar multiplication
        /// </summary>
        /// <returns></returns>
        public static Matrix2d operator *(double scalar, Matrix2d matrix)
        {
            matrix.Scale(scalar);
            return matrix;
        }


        /// <summary>
        /// Matrix scalar multiplication
        /// </summary>
        /// <returns></returns>
        public static Matrix2d operator /(Matrix2d matrix, double scalar)
        {
            matrix.Scale(1.0 / scalar);
            return matrix;
        }


        /// <summary>
        /// Matrix vector multiplication
        /// </summary>
        /// <returns></returns>
        public static Vector2d operator *(Matrix2d matrix, Vector2d vector)
        {
            return matrix.Apply(vector);
        }


        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <returns></returns>
        public static Matrix2d operator *(Matrix2d m0, Matrix2d m1)
        {
            return m0.Apply(m1);
        }


        /// <summary>
        /// Matrix addition
        /// </summary>
        /// <returns></returns>
        public static Matrix2d operator +(Matrix2d m0, Matrix2d m1)
        {
            m0.M00 += m1.M00;
            m0.M01 += m1.M01;

            m0.M10 += m1.M10;
            m0.M11 += m1.M11;

            return m0;
        }


        /// <summary>
        /// Matrix subtraction
        /// </summary>
        /// <returns></returns>
        public static Matrix2d operator -(Matrix2d m0, Matrix2d m1)
        {
            m0.M00 -= m1.M00;
            m0.M01 -= m1.M01;

            m0.M10 -= m1.M10;
            m0.M11 -= m1.M11;

            return m0;
        }


        /// <summary>
        /// Matrix negation
        /// </summary>
        /// <returns></returns>
        public static Matrix2d operator -(Matrix2d matrix)
        {
            matrix.M00 -= matrix.M00;
            matrix.M01 -= matrix.M01;

            matrix.M10 -= matrix.M10;
            matrix.M11 -= matrix.M11;

            return matrix;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="m0"></param>
        /// <param name="m1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Matrix2d Lerp(Matrix2d m0, Matrix2d m1, double t)
        {
            return m0.LerpTo(m1, t);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="row0"></param>
        /// <param name="row1"></param>
        /// <returns></returns>
        public static Matrix2d CreateFromRows(Vector2d row0, Vector2d row1)
        {
            return new Matrix2d(
                row0.X, row0.Y,
                row1.X, row1.Y
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="column0"></param>
        /// <param name="column1"></param>
        /// <returns></returns>
        public static Matrix2d CreateFromColumns(Vector2d column0, Vector2d column1)
        {
            return new Matrix2d(
                column0.X, column1.X,
                column0.Y, column1.Y
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static Matrix2d CreateCovariance(IEnumerable<Vector2d> vectors)
        {
            return CreateCovariance(vectors, vectors.Mean());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static Matrix2d CreateCovariance(IEnumerable<Vector2d> vectors, Vector2d mean)
        {
            var result = new Matrix2d();
            int n = 0;
            
            foreach (var v in vectors)
            {
                var d = v - mean;
                result.M00 += d.X * d.X;
                result.M01 += d.X * d.Y;
                result.M11 += d.Y * d.Y;
                n++;
            }
            
            var t = 1.0 / n;
            result.M00 *= t;
            result.M11 *= t;
            result.M10 =  result.M01 *= t;

            return result;
        }

        
        /// <summary>
        /// Returns a numerical approximation of the Jacobian of the given function with respect to the given vector.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="vector"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static Matrix2d CreateJacobian(Func<Vector2d, Vector2d> function, Vector2d vector, double epsilon = D.ZeroTolerance)
        {
            (var x, var y) = vector;
            
            var col0 = function(new Vector2d(x + epsilon, y)) - function(new Vector2d(x - epsilon, y));
            var col1 = function(new Vector2d(x, y + epsilon)) - function(new Vector2d(x, y - epsilon));
            
            return new Matrix2d(col0, col1) / (2.0 * epsilon);
        }


        /// <summary>
        /// Returns a numerical approximation of the Hessian of the given function with respect to the given vector.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="vector"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static Matrix2d CreateHessian(Func<Vector2d, double> function, Vector2d vector, double epsilon = D.ZeroTolerance)
        {
            return CreateJacobian(p => Geometry.GetGradient(function, vector, epsilon), vector, epsilon);
        }

#endregion


        /// <summary>Entry at row 0 column 0</summary>
        public double M00;
        /// <summary>Entry at row 0 column 1</summary>
        public double M01;

        /// <summary>Entry at row 1 column 0</summary>
        public double M10;
        /// <summary>Entry at row 1 column 1</summary>
        public double M11;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="diagonal"></param>
        public Matrix2d(double diagonal)
            : this()
        {
            M00 = M11 = diagonal;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="m00"></param>
        /// <param name="m01"></param>
        /// <param name="m10"></param>
        /// <param name="m11"></param>
        public Matrix2d(double m00, double m01, double m10, double m11)
        {
            M00 = m00;
            M01 = m01;
            M10 = m10;
            M11 = m11;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="column0"></param>
        /// <param name="column1"></param>
        public Matrix2d(Vector2d column0, Vector2d column1)
        {
            M00 = column0.X;
            M01 = column1.X;
            M10 = column0.Y;
            M11 = column1.Y;
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector2d Column0
        {
            get { return new Vector2d(M00, M10); }
            set
            {
                M00 = value.X;
                M10 = value.Y;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector2d Column1
        {
            get { return new Vector2d(M01, M11); }
            set
            {
                M01 = value.X;
                M11 = value.Y;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector2d Row0
        {
            get { return new Vector2d(M00, M01); }
            set
            {
                M00 = value.X;
                M01 = value.Y;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector2d Row1
        {
            get { return new Vector2d(M10, M11); }
            set
            {
                M10 = value.X;
                M11 = value.Y;
            }
        }


        /// <summary>
        /// Returns the transpose of this matrix.
        /// </summary>
        public Matrix2d Transposed
        {
            get { return new Matrix2d(M00, M10, M01, M11); }
        }


        /// <summary>
        /// Returns the indentity if this matrix cannot be inverted.
        /// </summary>
        public Matrix2d Inverse
        {
            get
            {
                Invert(out Matrix2d m);
                return m;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Determinant
        {
            get { return M00 * M11 - M01 * M10; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Trace
        {
            get { return M00 + M11; }
        }


        /// <summary>
        /// Returns the matrix of minors.
        /// </summary>
        public Matrix2d Minor
        {
            get { return new Matrix2d(Minor00, Minor01, Minor10, Minor11); }
        }


        /// <summary>
        /// Returns the cofactor matrix.
        /// </summary>
        public Matrix2d Cofactor
        {
            get { return new Matrix2d(Minor00, -Minor01, -Minor10, Minor11); }
        }


        /// <summary>
        /// Returns the adjugate matrix i.e. the transpose of the cofactor matrix.
        /// </summary>
        public Matrix2d Adjugate
        {
            get { return new Matrix2d(Minor00, -Minor10, -Minor01, Minor11); }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor00
        {
            get { return M11; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor01
        {
            get { return M10; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor10
        {
            get { return M01; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor11
        {
            get { return M00; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Matrix3d As3d
        {
            get => new Matrix3d(
                M00, M01, 0.0,
                M10, M11, 0.0,
                0.0, 0.0, 0.0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Matrix4d As4d
        {
            get => new Matrix4d(
                M00, M01, 0.0, 0.0,
                M10, M11, 0.0, 0.0,
                0.0, 0.0, 0.0, 0.0,
                0.0, 0.0, 0.0, 0.0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool IsSymmetric(double epsilon = D.ZeroTolerance)
        {
            return SlurMath.ApproxEquals(M01, M10);
        }
        

        /// <summary>
        /// Scales this matrix in place.
        /// </summary>
        /// <param name="factor"></param>
        public void Scale(double factor)
        {
            M00 *= factor;
            M01 *= factor;
            M10 *= factor;
            M11 *= factor;
        }


        /// <summary>
        /// Transposes this matrix in place.
        /// </summary>
        public void Transpose()
        {
            Utilities.Swap(ref M10, ref M01);
        }


        /// <summary>
        /// Returns true on success.
        /// </summary>
        public bool Invert(out Matrix2d result)
        {
            // inversion via cofactors
            // https://en.wikipedia.org/wiki/Minor_(linear_algebra)

            var d = Determinant;

            if (Math.Abs(d) > 0.0)
            {
                d = 1.0 / d;

                result.M00 = Minor00 * d;
                result.M01 = -Minor10 * d;

                result.M10 = -Minor01 * d;
                result.M11 = Minor11 * d;

                return true;
            }

            result = Identity;
            return false;
        }


        /// <summary>
        /// Applies this transformation to the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector2d Apply(Vector2d vector)
        {
            return new Vector2d(
               Vector2d.Dot(Row0, vector),
               Vector2d.Dot(Row1, vector)
               );
        }


        /// <summary>
        /// Applies this transformation to the given transformation.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Matrix2d Apply(Matrix2d other)
        {
            return new Matrix2d(
                Apply(other.Column0),
                Apply(other.Column1)
                );
        }


        /// <summary>
        /// Applies the transpose of this transformation to the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector2d ApplyTranspose(Vector2d vector)
        {
            return new Vector2d(
               Vector2d.Dot(Column0, vector),
               Vector2d.Dot(Column1, vector)
               );
        }


        /// <summary>
        /// Applies the transpose of this transformation to the given transformation.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Matrix2d ApplyTranspose(Matrix2d other)
        {
            return new Matrix2d(
                ApplyTranspose(other.Column0),
                ApplyTranspose(other.Column1)
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(Matrix2d other, double epsilon = D.ZeroTolerance)
        {
            return
                SlurMath.ApproxEquals(M00, other.M00, epsilon) &&
                SlurMath.ApproxEquals(M01, other.M01, epsilon) &&

                SlurMath.ApproxEquals(M10, other.M10, epsilon) &&
                SlurMath.ApproxEquals(M11, other.M11, epsilon);
        }


        /// <summary>
        /// Returns the roots of the characteristic polynomial of this matrix.
        /// These are also the eigenvalues of this matrix.
        /// </summary>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool SolveCharacteristic(out double r0, out double r1, double epsilon = D.ZeroTolerance)
        {
            return SlurMath.SolveQuadratic(1.0, -Trace, Determinant, out r0, out r1, epsilon) > 0;
        }


        /// <summary>
        /// Solves the linear system Ax = b
        /// </summary>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public bool SolveFor(Vector2d b, out Vector2d x)
        {
            // impl ref
            // https://en.wikipedia.org/wiki/Cramer%27s_rule

            var det = Determinant;

            if (Math.Abs(det) > 0.0)
            {
                det = 1.0 / det;
                x.X = (M11 * b.X - M01 * b.Y) * det;
                x.Y = (M00 * b.Y - M10 * b.X) * det;
                return true;
            }
            
            // no unique solution
            x = Vector2d.Zero;
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Matrix2d LerpTo(Matrix2d other, double factor)
        {
            return CreateFromRows(
                Row0.LerpTo(other.Row0, factor),
                Row1.LerpTo(other.Row1, factor));
        }


        /// <summary>
        /// Result is given in row-major order
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            var result = new double[4];
            ToArray(result);
            return result;
        }


        /// <summary>
        /// Result is given in row-major order
        /// </summary>
        /// <param name="result"></param>
        public void ToArray(double[] result)
        {
            result[0] = M00;
            result[1] = M01;

            result[2] = M10;
            result[3] = M11;
        }
    }
}
