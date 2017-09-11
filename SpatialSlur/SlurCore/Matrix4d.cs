using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurCore
{
	/// <summary>
    /// Double precision 4x4 matrix.
    /// </summary>
    public struct Matrix4d
    {
        #region Static

        /// <summary></summary>
        public static Matrix4d Identity = new Matrix4d(1.0);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        public static implicit operator Matrix4d(Matrix3d matrix)
        {
            var m = Identity;
            m.Column0 = matrix.Column0;
            m.Column1 = matrix.Column1;
            m.Column2 = matrix.Column2;
            return m;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public static implicit operator Matrix4d(Rotation3d rotation)
        {
            var m = Identity;
            m.Column0 = rotation.X;
            m.Column1 = rotation.Y;
            m.Column2 = rotation.Z;
            return m;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="orient"></param>
        public static implicit operator Matrix4d(Orient3d orient)
        {
            var m = new Matrix4d();
            m.Column0 = orient.Rotation.X;
            m.Column1 = orient.Rotation.Y;
            m.Column2 = orient.Rotation.Z;
            m.Column3 = new Vec4d(orient.Translation, 1.0);
            return m;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        public static implicit operator Matrix4d(Transform3d transform)
        {
            var m = new Matrix4d();
            m.Column0 = transform.Rotation.X * transform.Scale.X;
            m.Column1 = transform.Rotation.Y * transform.Scale.Y;
            m.Column2 = transform.Rotation.Z * transform.Scale.Z;
            m.Column3 = new Vec4d(transform.Translation, 1.0);
            return m;
        }


        /// <summary>
        /// Matrix scalar multiplication
        /// </summary>
        /// <returns></returns>
        public static Matrix4d operator *(Matrix4d matrix, double scalar)
        {
            matrix.M00 *= scalar;
            matrix.M01 *= scalar;
            matrix.M02 *= scalar;
            matrix.M03 *= scalar;

            matrix.M10 *= scalar;
            matrix.M11 *= scalar;
            matrix.M12 *= scalar;
            matrix.M13 *= scalar;

            matrix.M20 *= scalar;
            matrix.M21 *= scalar;
            matrix.M22 *= scalar;
            matrix.M23 *= scalar;

            matrix.M30 *= scalar;
            matrix.M31 *= scalar;
            matrix.M32 *= scalar;
            matrix.M33 *= scalar;

            return matrix;
        }


        /// <summary>
        /// Matrix vector multiplication
        /// </summary>
        /// <returns></returns>
        public static Vec4d operator *(Matrix4d matrix, Vec4d vector)
        {
            return new Vec4d(
                Vec4d.Dot(vector, matrix.Row0),
                Vec4d.Dot(vector, matrix.Row1),
                Vec4d.Dot(vector, matrix.Row2),
                Vec4d.Dot(vector, matrix.Row3)
                );
        }


        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <returns></returns>
        public static Matrix4d operator *(Matrix4d m0, Matrix4d m1)
        {
            m1.Column0 = Multiply(ref m0, m1.Column0);
            m1.Column1 = Multiply(ref m0, m1.Column1);
            m1.Column2 = Multiply(ref m0, m1.Column2);
            m1.Column3 = Multiply(ref m0, m1.Column3);
            return m1;
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
        /// Matrix vector multiplication
        /// </summary>
        /// <returns></returns>
        public static Vec4d Multiply(ref Matrix4d matrix, Vec4d vector)
        {
            return new Vec4d(
                Vec4d.Dot(vector, matrix.Row0),
                Vec4d.Dot(vector, matrix.Row1),
                Vec4d.Dot(vector, matrix.Row2),
                Vec4d.Dot(vector, matrix.Row3)
                );
        }


        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <returns></returns>
        public static Matrix4d Multiply(ref Matrix4d m0, ref Matrix4d m1)
        {
            var m2 = new Matrix4d();
            m2.Column0 = Multiply(ref m0, m1.Column0);
            m2.Column1 = Multiply(ref m0, m1.Column1);
            m2.Column2 = Multiply(ref m0, m1.Column2);
            m2.Column3 = Multiply(ref m0, m1.Column3);
            return m2;
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
        public Vec4d Column0
        {
            get { return new Vec4d(M00, M10, M20, M30); }
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
        public Vec4d Column1
        {
            get { return new Vec4d(M01, M11, M21, M31); }
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
        public Vec4d Column2
        {
            get { return new Vec4d(M02, M12, M22, M32); }
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
        public Vec4d Column3
        {
            get { return new Vec4d(M03, M13, M23, M33); }
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
        public Vec4d Row0
        {
            get { return new Vec4d(M00, M01, M02, M03); }
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
        public Vec4d Row1
        {
            get { return new Vec4d(M10, M11, M12, M13); }
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
        public Vec4d Row2
        {
            get { return new Vec4d(M20, M21, M22, M23); }
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
        public Vec4d Row3
        {
            get { return new Vec4d(M30, M31, M32, M33); }
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
                var result = this;
                result.Invert();
                return result;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Determinant
        {
            get
            {
                throw new NotImplementedException();
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
        /// Transposes this matrix in place.
        /// </summary>
        public void Transpose()
        {
            CoreUtil.Swap(ref M10, ref M01);
            CoreUtil.Swap(ref M20, ref M02);
            CoreUtil.Swap(ref M30, ref M03);
            CoreUtil.Swap(ref M21, ref M12);
            CoreUtil.Swap(ref M31, ref M13);
            CoreUtil.Swap(ref M32, ref M23);
        }


        /// <summary>
        /// Inverts this matrix in place
        /// </summary>
        public void Invert()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            var result = new double[16];
            ToArray(result);
            return result;
        }


        /// <summary>
        /// 
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
