using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Double precision 3x3 matrix.
    /// </summary>
    [Serializable]
    public struct Matrix3d
    {
        #region Static

        /// <summary></summary>
        public static readonly Matrix3d Identity = new Matrix3d(1.0);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        public static implicit operator Matrix3d(Matrix4d matrix)
        {
            return new Matrix3d(
                matrix.M00, matrix.M01, matrix.M02,
                matrix.M10, matrix.M11, matrix.M12,
                matrix.M20, matrix.M21, matrix.M22
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public static implicit operator Matrix3d(OrthoBasis3d rotation)
        {
            return rotation.ToMatrix();
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orient"></param>
        public static implicit operator Matrix3d(Orient2d orient)
        {
            return orient.ToMatrix();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        public static implicit operator Matrix3d(Transform2d transform)
        {
            return transform.ToMatrix();
        }


        /// <summary>
        /// Matrix scalar multiplication
        /// </summary>
        /// <returns></returns>
        public static Matrix3d operator *(Matrix3d matrix, double scalar)
        {
            matrix.Scale(scalar);
            return matrix;
        }


        /// <summary>
        /// Matrix scalar multiplication
        /// </summary>
        /// <returns></returns>
        public static Matrix3d operator *(double scalar, Matrix3d matrix)
        {
            matrix.Scale(scalar);
            return matrix;
        }


        /// <summary>
        /// Matrix scalar multiplication
        /// </summary>
        /// <returns></returns>
        public static Matrix3d operator /(Matrix3d matrix, double scalar)
        {
            matrix.Scale(1.0 / scalar);
            return matrix;
        }


        /// <summary>
        /// Matrix vector multiplication
        /// </summary>
        /// <returns></returns>
        public static Vec3d operator *(Matrix3d matrix, Vec3d vector)
        {
            return matrix.Apply(vector);
        }


        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <returns></returns>
        public static Matrix3d operator *(Matrix3d m0, Matrix3d m1)
        {
            m0.Apply(ref m1, ref m1);
            return m1;
        }


        /// <summary>
        /// Matrix addition
        /// </summary>
        /// <returns></returns>
        public static Matrix3d operator +(Matrix3d m0, Matrix3d m1)
        {
            m0.M00 += m1.M00;
            m0.M01 += m1.M01;
            m0.M02 += m1.M02;

            m0.M10 += m1.M10;
            m0.M11 += m1.M11;
            m0.M12 += m1.M12;

            m0.M20 += m1.M20;
            m0.M21 += m1.M21;
            m0.M22 += m1.M22;

            return m0;
        }


        /// <summary>
        /// Matrix subtraction
        /// </summary>
        /// <returns></returns>
        public static Matrix3d operator -(Matrix3d m0, Matrix3d m1)
        {
            m0.M00 -= m1.M00;
            m0.M01 -= m1.M01;
            m0.M02 -= m1.M02;

            m0.M10 -= m1.M10;
            m0.M11 -= m1.M11;
            m0.M12 -= m1.M12;

            m0.M20 -= m1.M20;
            m0.M21 -= m1.M21;
            m0.M22 -= m1.M22;

            return m0;
        }


        /// <summary>
        /// Matrix negation
        /// </summary>
        /// <returns></returns>
        public static Matrix3d operator -(Matrix3d matrix)
        {
            matrix.M00 -= matrix.M00;
            matrix.M01 -= matrix.M01;
            matrix.M02 -= matrix.M02;

            matrix.M10 -= matrix.M10;
            matrix.M11 -= matrix.M11;
            matrix.M12 -= matrix.M12;

            matrix.M20 -= matrix.M20;
            matrix.M21 -= matrix.M21;
            matrix.M22 -= matrix.M22;

            return matrix;
        }


        /*
        /// <summary>
        /// Matrix vector multiplication
        /// </summary>
        /// <returns></returns>
        public static Vec3d Multiply(ref Matrix3d matrix, Vec3d vector)
        {
            return matrix.Apply(vector);
        }


        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <returns></returns>
        public static Matrix3d Multiply(ref Matrix3d m0, ref Matrix3d m1)
        {
            return m0.Apply(m1);
        }
        */
       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row0"></param>
        /// <param name="row1"></param>
        /// <returns></returns>
        public static Matrix3d CreateFromRows(Vec3d row0, Vec3d row1, Vec3d row2)
        {
            return new Matrix3d(
                row0.X, row0.Y, row0.Z,
                row1.X, row1.Y, row1.Z,
                row2.X, row2.Y, row2.Z
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="row0"></param>
        /// <param name="row1"></param>
        /// <returns></returns>
        public static Matrix3d CreateFromColumns(Vec3d column0, Vec3d column1, Vec3d column2)
        {
            return new Matrix3d(
                column0.X, column1.X, column2.X,
                column0.Y, column1.Y, column2.Y,
                column0.Z, column1.Z, column2.Z
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static Matrix3d CreateCovariance(IEnumerable<Vec3d> vectors)
        {
            return CreateCovariance(vectors, vectors.Mean());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static Matrix3d CreateCovariance(IEnumerable<Vec3d> vectors, Vec3d mean)
        {
            var result = new Matrix3d();
            int n = 0;

            // calculate covariance matrix
            foreach (var v in vectors)
            {
                var d = v - mean;
                result.M00 += d.X * d.X;
                result.M01 += d.X * d.Y;
                result.M02 += d.X * d.Z;
                result.M11 += d.Y * d.Y;
                result.M12 += d.Y * d.Z;
                result.M22 += d.Z * d.Z;
                n++;
            }

            //average upper triangular values
            var t = 1.0 / n;
            result.M00 *= t;
            result.M01 *= t;
            result.M02 *= t;
            result.M11 *= t;
            result.M12 *= t;
            result.M22 *= t;

            // set symmetric values
            result.M10 = result.M01;
            result.M20 = result.M02;
            result.M21 = result.M12;

            return result;
        }


        /// <summary>
        /// Returns a numerical approximation of the Jacobian of the given function with respect to the given vector.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="vector"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static Matrix3d CreateJacobian(Func<Vec3d, Vec3d> function, Vec3d vector, double epsilon = SlurMath.ZeroTolerance)
        {
            (var x, var y, var z) = vector;

            var col0 = function(new Vec3d(x + epsilon, y, z)) - function(new Vec3d(x - epsilon, y, z));
            var col1 = function(new Vec3d(x, y + epsilon, z)) - function(new Vec3d(x, y - epsilon, z));
            var col2 = function(new Vec3d(x, y, z + epsilon)) - function(new Vec3d(x, y, z - epsilon));

            return new Matrix3d(col0, col1, col2) / (2.0 * epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        private static double GetDeterminant(double m00, double m01, double m10, double m11)
        {
            return m00 * m11 - m01 * m10;
        }

        #endregion


        /// <summary>Entry at row 0 column 0</summary>
        public double M00;
        /// <summary>Entry at row 0 column 1</summary>
        public double M01;
        /// <summary>Entry at row 0 column 2</summary>
        public double M02;

        /// <summary>Entry at row 1 column 0</summary>
        public double M10;
        /// <summary>Entry at row 1 column 1</summary>
        public double M11;
        /// <summary>Entry at row 1 column 2</summary>
        public double M12;

        /// <summary>Entry at row 2 column 0</summary>
        public double M20;
        /// <summary>Entry at row 2 column 1</summary>
        public double M21;
        /// <summary>Entry at row 2 column 2</summary>
        public double M22;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="diagonal"></param>
        public Matrix3d(double diagonal)
            : this()
        {
            M00 = M11 = M22 = diagonal;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="m00"></param>
        /// <param name="m01"></param>
        /// <param name="m02"></param>
        /// <param name="m10"></param>
        /// <param name="m11"></param>
        /// <param name="m12"></param>
        /// <param name="m20"></param>
        /// <param name="m21"></param>
        /// <param name="m22"></param>
        public Matrix3d(
            double m00, double m01, double m02,
            double m10, double m11, double m12,
            double m20, double m21, double m22
            )
        {
            M00 = m00;
            M01 = m01;
            M02 = m02;

            M10 = m10;
            M11 = m11;
            M12 = m12;

            M20 = m20;
            M21 = m21;
            M22 = m22;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="column0"></param>
        /// <param name="column1"></param>
        /// <param name="column2"></param>
        public Matrix3d(Vec3d column0, Vec3d column1, Vec3d column2)
        {
            M00 = column0.X;
            M01 = column1.X;
            M02 = column2.X;

            M10 = column0.Y;
            M11 = column1.Y;
            M12 = column2.Y;

            M20 = column0.Z;
            M21 = column1.Z;
            M22 = column2.Z;
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Column0
        {
            get { return new Vec3d(M00, M10, M20); }
            set
            {
                M00 = value.X;
                M10 = value.Y;
                M20 = value.Z;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Column1
        {
            get { return new Vec3d(M01, M11, M21); }
            set
            {
                M01 = value.X;
                M11 = value.Y;
                M21 = value.Z;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Column2
        {
            get { return new Vec3d(M02, M12, M22); }
            set
            {
                M02 = value.X;
                M12 = value.Y;
                M22 = value.Z;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Row0
        {
            get { return new Vec3d(M00, M01, M02); }
            set
            {
                M00 = value.X;
                M01 = value.Y;
                M02 = value.Z;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Row1
        {
            get { return new Vec3d(M10, M11, M12); }
            set
            {
                M10 = value.X;
                M11 = value.Y;
                M12 = value.Z;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Row2
        {
            get { return new Vec3d(M20, M21, M22); }
            set
            {
                M20 = value.X;
                M21 = value.Y;
                M22 = value.Z;
            }
        }


        /// <summary>
        /// Returns the transpose of this matrix.
        /// </summary>
        public Matrix3d Transposed
        {
            get
            {
                return new Matrix3d(
                    M00, M10, M20,
                    M01, M11, M21,
                    M02, M12, M22
                    );
            }
        }


        /// <summary>
        /// Returns the indentity if this matrix cannot be inverted.
        /// </summary>
        public Matrix3d Inverse
        {
            get
            {
                Invert(out Matrix3d m);
                return m;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Determinant
        {
            get
            {
               return 
                    M00 * M11 * M22 +
                    M10 * M21 * M02 +
                    M20 * M01 * M12 -
                    M20 * M11 * M02 -
                    M10 * M01 * M22 -
                    M00 * M21 * M12;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Trace
        {
            get { return M00 + M11 + M22; }
        }
        

        /// <summary>
        /// Returns the matrix of minors.
        /// </summary>
        public Matrix3d Minor
        {
            get
            {
                return new Matrix3d(
                    Minor00, Minor01, Minor02,
                    Minor10, Minor11, Minor12,
                    Minor20, Minor21, Minor22
                    );
            }
        }
        

        /// <summary>
        /// Returns the cofactor matrix.
        /// </summary>
        public Matrix3d Cofactor
        {
            get
            {
                return new Matrix3d(
                    Minor00, -Minor01, Minor02,
                    -Minor10, Minor11, -Minor12,
                    Minor20, -Minor21, Minor22
                    );
            }
        }

        
        /// <summary>
        /// Returns the adjugate matrix.
        /// This is defined as the transpose of the cofactor matrix.
        /// </summary>
        public Matrix3d Adjugate
        {
            get
            {
                return new Matrix3d(
                    Minor00, -Minor10, Minor20,
                    -Minor01, Minor11, -Minor21,
                    Minor02, -Minor12, Minor22
                    );
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor00
        {
            get { return GetDeterminant(M11, M12, M21, M22); }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor01
        {
            get { return GetDeterminant(M10, M12, M20, M22); }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor02
        {
            get { return GetDeterminant(M10, M11, M20, M21); }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor10
        {
            get { return GetDeterminant(M01, M02, M21, M22);}
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor11
        {
            get { return GetDeterminant(M00, M02, M20, M22); }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor12
        {
            get { return GetDeterminant(M00, M01, M20, M21); }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor20
        {
            get { return GetDeterminant(M01, M02, M11, M12); }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor21
        {
            get { return GetDeterminant(M00, M02, M10, M12); }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor22
        {
            get { return GetDeterminant(M00, M01, M10, M11); }
        }
        

        /// <summary>
        /// Scales this matrix in place.
        /// </summary>
        /// <param name="factor"></param>
        public void Scale(double factor)
        {
            M00 *= factor;
            M01 *= factor;
            M02 *= factor;

            M10 *= factor;
            M11 *= factor;
            M12 *= factor;

            M20 *= factor;
            M21 *= factor;
            M22 *= factor;
        }


        /// <summary>
        /// Transposes this matrix in place.
        /// </summary>
        public void Transpose()
        {
            CoreUtil.Swap(ref M10, ref M01);
            CoreUtil.Swap(ref M20, ref M02);
            CoreUtil.Swap(ref M21, ref M12);
        }


        /// <summary>
        /// Returns true on success.
        /// </summary>
        public bool Invert(out Matrix3d result)
        {
            // inversion via cofactors
            // https://en.wikipedia.org/wiki/Minor_(linear_algebra)

            var m00 = Minor00;
            var m10 = -Minor01;
            var m20 = Minor02;
            double d = M00 * m00 + M01 * m10 + M02 * m20;

            if (Math.Abs(d) > 0.0)
            {
                d = 1.0f / d;

                result.M00 = m00 * d;
                result.M01 = -Minor10 * d;
                result.M02 = Minor20 * d;

                result.M10 = m10 * d;
                result.M11 = Minor11 * d;
                result.M12 = -Minor21 * d;

                result.M20 = m20 * d;
                result.M21 = -Minor12 * d;
                result.M22 = Minor22 * d;

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
        public Vec3d Apply(Vec3d vector)
        {
            return new Vec3d(
               Vec3d.Dot(Row0, vector),
               Vec3d.Dot(Row1, vector),
               Vec3d.Dot(Row2, vector)
               );
        }


        /// <summary>
        /// Applies this transformation to the given transformation.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Matrix3d Apply(Matrix3d other)
        {
            Apply(ref other, ref other);
            return other;
        }


        /// <summary>
        /// Applies this transformation to the given transformation in place.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public void Apply(ref Matrix3d other)
        {
            Apply(ref other, ref other);
        }


        /// <summary>
        /// Applies this transformation to the given transformation.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public void Apply(ref Matrix3d other, ref Matrix3d result)
        {
            result.Column0 = Apply(other.Column0);
            result.Column1 = Apply(other.Column1);
            result.Column2 = Apply(other.Column2);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool ApproxEquals(ref Matrix3d other, double tolerance = SlurMath.ZeroTolerance)
        {
            return
                SlurMath.ApproxEquals(M00, other.M00, tolerance) &&
                SlurMath.ApproxEquals(M01, other.M01, tolerance) &&
                SlurMath.ApproxEquals(M02, other.M02, tolerance) &&

                SlurMath.ApproxEquals(M10, other.M10, tolerance) &&
                SlurMath.ApproxEquals(M11, other.M11, tolerance) &&
                SlurMath.ApproxEquals(M12, other.M12, tolerance) &&

                SlurMath.ApproxEquals(M20, other.M20, tolerance) &&
                SlurMath.ApproxEquals(M21, other.M21, tolerance) &&
                SlurMath.ApproxEquals(M22, other.M22, tolerance);
        }


        /// <summary>
        /// Result is given in row-major order
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            var result = new double[9];
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
            result[2] = M02;

            result[3] = M10;
            result[4] = M11;
            result[5] = M12;

            result[6] = M20;
            result[7] = M21;
            result[8] = M22;
        }
    }
}
