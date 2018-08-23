
/*
 * Notes
 */

using System;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur
{
    /// <summary>
    /// Double precision 4x4 matrix.
    /// </summary>
    [Serializable]
    public partial struct Matrix4d
    {
        #region Static Members

        /// <summary></summary>
        public static readonly Matrix4d Identity = new Matrix4d(1.0);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="orient"></param>
        public static implicit operator Matrix4d(Orient3d orient)
        {
            return orient.ToMatrix();
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        public static implicit operator Matrix4d(Transform3d transform)
        {
            return transform.ToMatrix();
        }


        /// <summary>
        /// Matrix scalar multiplication
        /// </summary>
        /// <returns></returns>
        public static Matrix4d operator *(Matrix4d matrix, double scalar)
        {
            matrix.Scale(scalar);
            return matrix;
        }


        /// <summary>
        /// Matrix scalar multiplication
        /// </summary>
        /// <returns></returns>
        public static Matrix4d operator *(double scalar, Matrix4d matrix)
        {
            matrix.Scale(scalar);
            return matrix;
        }


        /// <summary>
        /// Matrix scalar division
        /// </summary>
        /// <returns></returns>
        public static Matrix4d operator /(Matrix4d matrix, double scalar)
        {
            matrix.Scale(1.0 / scalar);
            return matrix;
        }


        /// <summary>
        /// Matrix vector multiplication
        /// </summary>
        /// <returns></returns>
        public static Vector4d operator *(Matrix4d matrix, Vector4d vector)
        {
            return matrix.Apply(vector);
        }


        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <returns></returns>
        public static Matrix4d operator *(Matrix4d m0, Matrix4d m1)
        {
            return m0.Apply(ref m1);
        }


        /// <summary>
        /// Matrix addition
        /// </summary>
        /// <returns></returns>
        public static Matrix4d operator +(Matrix4d m0, Matrix4d m1)
        {
            m0.M00 += m1.M00;
            m0.M01 += m1.M01;
            m0.M02 += m1.M02;
            m0.M03 += m1.M03;

            m0.M10 += m1.M10;
            m0.M11 += m1.M11;
            m0.M12 += m1.M12;
            m0.M13 += m1.M13;

            m0.M20 += m1.M20;
            m0.M21 += m1.M21;
            m0.M22 += m1.M22;
            m0.M23 += m1.M23;

            m0.M30 += m1.M30;
            m0.M31 += m1.M31;
            m0.M32 += m1.M32;
            m0.M33 += m1.M33;

            return m0;
        }


        /// <summary>
        /// Matrix subtraction
        /// </summary>
        /// <returns></returns>
        public static Matrix4d operator -(Matrix4d m0, Matrix4d m1)
        {
            m0.M00 -= m1.M00;
            m0.M01 -= m1.M01;
            m0.M02 -= m1.M02;
            m0.M03 -= m1.M03;

            m0.M10 -= m1.M10;
            m0.M11 -= m1.M11;
            m0.M12 -= m1.M12;
            m0.M13 -= m1.M13;

            m0.M20 -= m1.M20;
            m0.M21 -= m1.M21;
            m0.M22 -= m1.M22;
            m0.M23 -= m1.M23;

            m0.M30 -= m1.M30;
            m0.M31 -= m1.M31;
            m0.M32 -= m1.M32;
            m0.M33 -= m1.M33;

            return m0;
        }


        /// <summary>
        /// Matrix negation
        /// </summary>
        /// <returns></returns>
        public static Matrix4d operator -(Matrix4d matrix)
        {
            matrix.M00 -= matrix.M00;
            matrix.M01 -= matrix.M01;
            matrix.M02 -= matrix.M02;
            matrix.M03 -= matrix.M03;

            matrix.M10 -= matrix.M10;
            matrix.M11 -= matrix.M11;
            matrix.M12 -= matrix.M12;
            matrix.M13 -= matrix.M13;
        
            matrix.M20 -= matrix.M20;
            matrix.M21 -= matrix.M21;
            matrix.M22 -= matrix.M22;
            matrix.M23 -= matrix.M23;

            matrix.M30 -= matrix.M30;
            matrix.M31 -= matrix.M31;
            matrix.M32 -= matrix.M32;
            matrix.M33 -= matrix.M33;

            return matrix;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="m0"></param>
        /// <param name="m1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Matrix4d Lerp(Matrix4d m0, Matrix4d m1, double t)
        {
            return m0.LerpTo(m1, t);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="row0"></param>
        /// <param name="row1"></param>
        /// <param name="row2"></param>
        /// <param name="row3"></param>
        /// <returns></returns>
        public static Matrix4d CreateFromRows(Vector4d row0, Vector4d row1, Vector4d row2, Vector4d row3)
        {
            return new Matrix4d(
                row0.X, row0.Y, row0.Z, row0.W,
                row1.X, row1.Y, row1.Z, row1.W,
                row2.X, row2.Y, row2.Z, row2.W,
                row3.X, row3.Y, row3.Z, row3.W
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="column0"></param>
        /// <param name="column1"></param>
        /// <param name="column2"></param>
        /// <param name="column3"></param>
        /// <returns></returns>
        public static Matrix4d CreateFromColumns(Vector4d column0, Vector4d column1, Vector4d column2, Vector4d column3)
        {
            return new Matrix4d(
                column0.X, column1.X, column2.X, column3.X,
                column0.Y, column1.Y, column2.Y, column3.Y,
                column0.Z, column1.Z, column2.Z, column3.Z,
                column0.W, column1.W, column2.W, column3.W
                );
        }


        /// <summary>
        /// Returns a numerical approximation of the Jacobian of the given function with respect to the given vector.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="vector"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static Matrix4d CreateJacobian(Func<Vector4d, Vector4d> function, Vector4d vector, double epsilon = D.ZeroTolerance)
        {
            (var x, var y, var z, var w) = vector;

            var col0 = function(new Vector4d(x + epsilon, y, z, w)) - function(new Vector4d(x - epsilon, y, z, w));
            var col1 = function(new Vector4d(x, y + epsilon, z, w)) - function(new Vector4d(x, y - epsilon, z, w));
            var col2 = function(new Vector4d(x, y, z + epsilon, w)) - function(new Vector4d(x, y, z - epsilon, w));
            var col3 = function(new Vector4d(x, y, z, w + epsilon)) - function(new Vector4d(x, y, z, w - epsilon));

            return new Matrix4d(col0, col1, col2, col3) / (2.0 * epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        private static double GetDeterminant(
            double m00, double m01, double m02,
            double m10, double m11, double m12,
            double m20, double m21, double m22
            )
        {
            return
                   m00 * m11 * m22 + m10 * m21 * m02 +
                   m20 * m01 * m12 - m20 * m11 * m02 -
                   m10 * m01 * m22 - m00 * m21 * m12;
        }

        #endregion


        /// <summary>Entry at row 0 column 0</summary>
        public double M00;
        /// <summary>Entry at row 0 column 1</summary>
        public double M01;
        /// <summary>Entry at row 0 column 2</summary>
        public double M02;
        /// <summary>Entry at row 0 column 3</summary>
        public double M03;

        /// <summary>Entry at row 1 column 0</summary>
        public double M10;
        /// <summary>Entry at row 1 column 1</summary>
        public double M11;
        /// <summary>Entry at row 1 column 2</summary>
        public double M12;
        /// <summary>Entry at row 1 column 3</summary>
        public double M13;

        /// <summary>Entry at row 2 column 0</summary>
        public double M20;
        /// <summary>Entry at row 2 column 1</summary>
        public double M21;
        /// <summary>Entry at row 2 column 2</summary>
        public double M22;
        /// <summary>Entry at row 2 column 3</summary>
        public double M23;

        /// <summary>Entry at row 3 column 0</summary>
        public double M30;
        /// <summary>Entry at row 3 column 1</summary>
        public double M31;
        /// <summary>Entry at row 3 column 2</summary>
        public double M32;
        /// <summary>Entry at row 3 column 3</summary>
        public double M33;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="diagonal"></param>
        public Matrix4d(double diagonal)
            : this()
        {
            M00 = M11 = M22 = M33 = diagonal;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="m00"></param>
        /// <param name="m01"></param>
        /// <param name="m02"></param>
        /// <param name="m03"></param>
        /// <param name="m10"></param>
        /// <param name="m11"></param>
        /// <param name="m12"></param>
        /// <param name="m13"></param>
        /// <param name="m20"></param>
        /// <param name="m21"></param>
        /// <param name="m22"></param>
        /// <param name="m23"></param>
        /// <param name="m30"></param>
        /// <param name="m31"></param>
        /// <param name="m32"></param>
        /// <param name="m33"></param>
        public Matrix4d(
            double m00, double m01, double m02, double m03,
            double m10, double m11, double m12, double m13,
            double m20, double m21, double m22, double m23,
            double m30, double m31, double m32, double m33
            )
        {
            M00 = m00;
            M01 = m01;
            M02 = m02;
            M03 = m03;

            M10 = m10;
            M11 = m11;
            M12 = m12;
            M13 = m13;

            M20 = m20;
            M21 = m21;
            M22 = m22;
            M23 = m23;

            M30 = m30;
            M31 = m31;
            M32 = m32;
            M33 = m33;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="column0"></param>
        /// <param name="column1"></param>
        /// <param name="column2"></param>
        /// <param name="column3"></param>
        public Matrix4d(Vector4d column0, Vector4d column1, Vector4d column2, Vector4d column3)
        {
            M00 = column0.X;
            M01 = column1.X;
            M02 = column2.X;
            M03 = column3.X;

            M10 = column0.Y;
            M11 = column1.Y;
            M12 = column2.Y;
            M13 = column3.Y;

            M20 = column0.Z;
            M21 = column1.Z;
            M22 = column2.Z;
            M23 = column3.Z;

            M30 = column0.W;
            M31 = column1.W;
            M32 = column2.W;
            M33 = column3.W;
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector4d Column0
        {
            get { return new Vector4d(M00, M10, M20, M30); }
            set
            {
                M00 = value.X;
                M10 = value.Y;
                M20 = value.Z;
                M30 = value.W;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector4d Column1
        {
            get { return new Vector4d(M01, M11, M21, M31); }
            set
            {
                M01 = value.X;
                M11 = value.Y;
                M21 = value.Z;
                M31 = value.W;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector4d Column2
        {
            get { return new Vector4d(M02, M12, M22, M32); }
            set
            {
                M02 = value.X;
                M12 = value.Y;
                M22 = value.Z;
                M32 = value.W;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector4d Column3
        {
            get { return new Vector4d(M03, M13, M23, M33); }
            set
            {
                M03 = value.X;
                M13 = value.Y;
                M23 = value.Z;
                M33 = value.W;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector4d Row0
        {
            get { return new Vector4d(M00, M01, M02, M03); }
            set
            {
                M00 = value.X;
                M01 = value.Y;
                M02 = value.Z;
                M03 = value.W;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector4d Row1
        {
            get { return new Vector4d(M10, M11, M12, M13); }
            set
            {
                M10 = value.X;
                M11 = value.Y;
                M12 = value.Z;
                M13 = value.W;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector4d Row2
        {
            get { return new Vector4d(M20, M21, M22, M23); }
            set
            {
                M20 = value.X;
                M21 = value.Y;
                M22 = value.Z;
                M23 = value.W;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector4d Row3
        {
            get { return new Vector4d(M30, M31, M32, M33); }
            set
            {
                M30 = value.X;
                M31 = value.Y;
                M32 = value.Z;
                M33 = value.W;
            }
        }


        /// <summary>
        /// Returns the transpose of this matrix.
        /// </summary>
        public Matrix4d Transposed
        {
            get
            {
                return new Matrix4d(
                    M00, M10, M20, M30,
                    M01, M11, M21, M31,
                    M02, M12, M22, M32,
                    M03, M13, M23, M33
                    );
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Matrix4d Inverse
        {
            get
            {
                Invert(out Matrix4d m);
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
                var a0 = M20 * M31 - M21 * M30;
                var a1 = M20 * M32 - M22 * M30;
                var a2 = M20 * M33 - M23 * M30;
                var a3 = M21 * M32 - M22 * M31;
                var a4 = M21 * M33 - M23 * M31;
                var a5 = M22 * M33 - M23 * M32;

                var b0 = a5 * M11 - a4 * M12 + a3 * M13;
                var b1 = a2 * M12 - a5 * M10 - a1 * M13;
                var b2 = a4 * M10 - a2 * M11 + a0 * M13;
                var b3 = a1 * M11 - a3 * M10 - a0 * M12;

                return b0 * M00 + b1 * M01 + b2 * M02 + b3 * M03;
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        public double Trace
        {
            get { return M00 + M11 + M22 + M33; }
        }


        /// <summary>
        /// Returns the matrix of minors.
        /// </summary>
        public Matrix4d Minor
        {
            get
            {
                return new Matrix4d(
                    Minor00, Minor01, Minor02, Minor03,
                    Minor10, Minor11, Minor12, Minor13,
                    Minor20, Minor21, Minor22, Minor23,
                    Minor30, Minor31, Minor32, Minor33
                    );
            }
        }
        

        /// <summary>
        /// Returns the cofactor matrix
        /// </summary>
        public Matrix4d Cofactor
        {
            get
            {
                return new Matrix4d(
                    Minor00, -Minor01, Minor02, -Minor03,
                    -Minor10, Minor11, -Minor12, Minor13,
                    Minor20, -Minor21, Minor22, -Minor23,
                    -Minor30, Minor31, -Minor32, Minor33
                    );
            }
        }


        /// <summary>
        /// Returns the adjugate matrix i.e. the transpose of the cofactor matrix.
        /// </summary>
        public Matrix4d Adjugate
        {
            get
            {
                return new Matrix4d(
                    Minor00, -Minor10, Minor20, -Minor30,
                    -Minor01, Minor11, -Minor21, Minor31,
                    Minor02, -Minor12, Minor22, -Minor32,
                    -Minor03, Minor13, -Minor23, Minor33
                    );
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        public double Minor00
        {
            get
            {
                return GetDeterminant(
                    M11, M12, M13, 
                    M21, M22, M23, 
                    M31, M32, M33);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor01
        {
            get
            {
                return GetDeterminant(
                    M10, M12, M13, 
                    M20, M22, M23, 
                    M30, M32, M33);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor02
        {
            get
            {
                return 
                    GetDeterminant(
                        M10, M11, M13, 
                        M20, M21, M23, 
                        M30, M31, M33);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor03
        {
            get
            {
                return GetDeterminant(
                    M10, M11, M12, 
                    M20, M21, M22, 
                    M30, M31, M32);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor10
        {
            get
            {
                return GetDeterminant(
                    M01, M02, M03,
                    M21, M22, M23, 
                    M31, M32, M33);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor11
        {
            get
            {
                return GetDeterminant(
                    M00, M02, M03, 
                    M20, M22, M23,
                    M30, M32, M33);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor12
        {
            get
            {
                return GetDeterminant(
                    M00, M01, M03, 
                    M20, M21, M23, 
                    M30, M31, M33);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor13
        {
            get
            {
                return GetDeterminant(
                    M00, M01, M02, 
                    M20, M21, M22, 
                    M30, M31, M32);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor20
        {
            get
            {
                return GetDeterminant(
                    M01, M02, M03, 
                    M11, M12, M13, 
                    M31, M32, M33);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor21
        {
            get
            {
                return GetDeterminant(
                    M00, M02, M03, 
                    M10, M12, M13, 
                    M30, M32, M33);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor22
        {
            get
            {
                return GetDeterminant(
                    M00, M01, M03, 
                    M10, M11, M13, 
                    M30, M31, M33);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor23
        {
            get
            {
                return GetDeterminant(
                    M00, M01, M02, 
                    M10, M11, M12, 
                    M30, M31, M32);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor30
        {
            get
            {
                return GetDeterminant(
                    M01, M02, M03, 
                    M11, M12, M13, 
                    M21, M22, M23);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor31
        {
            get
            {
                return GetDeterminant(
                    M00, M02, M03, 
                    M10, M12, M13, 
                    M20, M22, M23);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Minor32
        {
            get
            {
                return GetDeterminant(
                    M00, M01, M03, 
                    M10, M11, M13, 
                    M20, M21, M23);
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        public double Minor33
        {
            get
            {
                return GetDeterminant(
                    M00, M01, M02, 
                    M10, M11, M12, 
                    M20, M21, M22);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool IsSymmetric(double epsilon = D.ZeroTolerance)
        {
            return
                SlurMath.ApproxEquals(M01, M10) &&
                SlurMath.ApproxEquals(M02, M20) &&
                SlurMath.ApproxEquals(M03, M30) &&
                SlurMath.ApproxEquals(M12, M21) &&
                SlurMath.ApproxEquals(M13, M31) &&
                SlurMath.ApproxEquals(M23, M32);
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
            M03 *= factor;

            M10 *= factor;
            M11 *= factor;
            M12 *= factor;
            M13 *= factor;

            M20 *= factor;
            M21 *= factor;
            M22 *= factor;
            M23 *= factor;

            M30 *= factor;
            M31 *= factor;
            M32 *= factor;
            M33 *= factor;
        }


        /// <summary>
        /// Transposes this matrix in place.
        /// </summary>
        public void Transpose()
        {
            Utilities.Swap(ref M10, ref M01);
            Utilities.Swap(ref M20, ref M02);
            Utilities.Swap(ref M30, ref M03);
            Utilities.Swap(ref M21, ref M12);
            Utilities.Swap(ref M31, ref M13);
            Utilities.Swap(ref M32, ref M23);
        }


        /// <summary>
        /// Returns true on success
        /// </summary>
        public bool Invert(out Matrix4d result)
        {
            // inversion via cofactors
            // https://en.wikipedia.org/wiki/Minor_(linear_algebra)

            var d = Determinant;

            if(d > 0.0)
            {
                d = 1.0 / d;

                result.M00 = Minor00 * d;
                result.M01 = -Minor10 * d;
                result.M02 = Minor20 * d;
                result.M03 = -Minor30 * d;

                result.M10 = -Minor01 * d;
                result.M11 = Minor11 * d;
                result.M12 = -Minor21 * d;
                result.M13 = Minor31 * d;

                result.M20 = Minor02 * d;
                result.M21 = -Minor12 * d;
                result.M22 = Minor22 * d;
                result.M23 = -Minor32 * d;

                result.M30 = -Minor03 * d;
                result.M31 = Minor13 * d;
                result.M32 = -Minor23 * d;
                result.M33 = Minor33 * d;

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
        public Vector4d Apply(Vector4d vector)
        {
            return new Vector4d(
               Vector4d.Dot(Row0, vector),
               Vector4d.Dot(Row1, vector),
               Vector4d.Dot(Row2, vector),
               Vector4d.Dot(Row3, vector)
               );
        }


        /// <summary>
        /// Applies this transformation to the given transformation in place.
        /// </summary>
        /// <param name="other"></param>
        public Matrix4d Apply(Matrix4d other)
        {
            return Apply(ref other);
        }


        /// <summary>
        /// Applies this transformation to the given transformation in place.
        /// </summary>
        /// <param name="other"></param>
        public Matrix4d Apply(ref Matrix4d other)
        {
            return new Matrix4d(
                Apply(other.Column0),
                Apply(other.Column1),
                Apply(other.Column2),
                Apply(other.Column3)
                );
        }


        /// <summary>
        /// Applies the transpose of this transformation to the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector4d ApplyTranspose(Vector4d vector)
        {
            return new Vector4d(
               Vector4d.Dot(Column0, vector),
               Vector4d.Dot(Column1, vector),
               Vector4d.Dot(Column2, vector),
               Vector4d.Dot(Column3, vector)
               );
        }


        /// <summary>
        /// Applies the transpose of this transformation to the given transformation.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Matrix4d ApplyTranspose(Matrix4d other)
        {
            return ApplyTranspose(ref other);
        }


        /// <summary>
        /// Applies the transpose of this transformation to the given transformation.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Matrix4d ApplyTranspose(ref Matrix4d other)
        {
            return new Matrix4d(
                ApplyTranspose(other.Column0),
                ApplyTranspose(other.Column1),
                ApplyTranspose(other.Column2),
                ApplyTranspose(other.Column3)
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(Matrix4d other, double epsilon = D.ZeroTolerance)
        {
            return ApproxEquals(ref other, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(ref Matrix4d other, double epsilon = D.ZeroTolerance)
        {
            return
                SlurMath.ApproxEquals(M00, other.M00, epsilon) &&
                SlurMath.ApproxEquals(M01, other.M01, epsilon) &&
                SlurMath.ApproxEquals(M02, other.M02, epsilon) &&
                SlurMath.ApproxEquals(M03, other.M03, epsilon) &&

                SlurMath.ApproxEquals(M10, other.M10, epsilon) &&
                SlurMath.ApproxEquals(M11, other.M11, epsilon) &&
                SlurMath.ApproxEquals(M12, other.M12, epsilon) &&
                SlurMath.ApproxEquals(M13, other.M13, epsilon) &&

                SlurMath.ApproxEquals(M20, other.M20, epsilon) &&
                SlurMath.ApproxEquals(M21, other.M21, epsilon) &&
                SlurMath.ApproxEquals(M22, other.M22, epsilon) &&
                SlurMath.ApproxEquals(M23, other.M23, epsilon) &&

                SlurMath.ApproxEquals(M30, other.M30, epsilon) &&
                SlurMath.ApproxEquals(M31, other.M31, epsilon) &&
                SlurMath.ApproxEquals(M32, other.M32, epsilon) &&
                SlurMath.ApproxEquals(M33, other.M33, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Matrix4d LerpTo(Matrix4d other, double factor)
        {
            return CreateFromRows(
                Row0.LerpTo(other.Row0, factor),
                Row1.LerpTo(other.Row1, factor),
                Row2.LerpTo(other.Row2, factor),
                Row3.LerpTo(other.Row3, factor));
        }


        /// <summary>
        /// Result is given in row-major order
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            var result = new double[16];
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
            result[3] = M03;

            result[4] = M10;
            result[5] = M11;
            result[6] = M12;
            result[7] = M13;

            result[8] = M20;
            result[9] = M21;
            result[10] = M22;
            result[11] = M23;

            result[12] = M30;
            result[13] = M31;
            result[14] = M32;
            result[15] = M33;
        }
    }
}
