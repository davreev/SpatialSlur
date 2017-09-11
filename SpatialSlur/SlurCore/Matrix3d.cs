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
    /// Double precision 3x3 matrix.
    /// </summary>
    public struct Matrix3d
    {
        #region Static

        /// <summary></summary>
        public static Matrix3d Identity = new Matrix3d(1.0);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public static implicit operator Matrix3d(Rotation2d rotation)
        {
            var m = Identity;
            m.Column0 = rotation.X;
            m.Column1 = rotation.Y;
            return m;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="orient"></param>
        public static implicit operator Matrix3d(Orient2d orient)
        {
            var m = new Matrix3d();
            m.Column0 = orient.Rotation.X;
            m.Column1 = orient.Rotation.Y;
            m.Column2 = new Vec3d(orient.Translation, 1.0);
            return m;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        public static implicit operator Matrix3d(Transform2d transform)
        {
            var m = new Matrix3d();
            m.Column0 = transform.Rotation.X * transform.Scale.X;
            m.Column1 = transform.Rotation.Y * transform.Scale.Y;
            m.Column2 = new Vec3d(transform.Translation, 1.0);
            return m;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public static implicit operator Matrix3d(Rotation3d rotation)
        {
            var m = Identity;
            m.Column0 = rotation.X;
            m.Column1 = rotation.Y;
            m.Column2 = rotation.Z;
            return m;
        }


        /// <summary>
        /// Matrix scalar multiplication
        /// </summary>
        /// <returns></returns>
        public static Matrix3d operator *(Matrix3d matrix, double scalar)
        {
            matrix.M00 *= scalar;
            matrix.M01 *= scalar;
            matrix.M02 *= scalar;

            matrix.M10 *= scalar;
            matrix.M11 *= scalar;
            matrix.M12 *= scalar;

            matrix.M20 *= scalar;
            matrix.M21 *= scalar;
            matrix.M22 *= scalar;

            return matrix;
        }


        /// <summary>
        /// Matrix vector multiplication
        /// </summary>
        /// <returns></returns>
        public static Vec3d operator *(Matrix3d matrix, Vec3d vector)
        {
            return new Vec3d(
                Vec3d.Dot(vector, matrix.Row0),
                Vec3d.Dot(vector, matrix.Row1),
                Vec3d.Dot(vector, matrix.Row2)
                );
        }


        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <returns></returns>
        public static Matrix3d operator *(Matrix3d m0, Matrix3d m1)
        {
            m1.Column0 = Multiply(ref m0, m1.Column0);
            m1.Column1 = Multiply(ref m0, m1.Column1);
            m1.Column2 = Multiply(ref m0, m1.Column2);
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


        /// <summary>
        /// Matrix vector multiplication
        /// </summary>
        /// <returns></returns>
        public static Vec3d Multiply(ref Matrix3d matrix, Vec3d vector)
        {
            return new Vec3d(
                Vec3d.Dot(vector, matrix.Row0),
                Vec3d.Dot(vector, matrix.Row1),
                Vec3d.Dot(vector, matrix.Row2)
                );
        }


        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <returns></returns>
        public static Matrix3d Multiply(ref Matrix3d m0, ref Matrix3d m1)
        {
            var m2 = new Matrix3d();
            m2.Column0 = Multiply(ref m0, m1.Column0);
            m2.Column1 = Multiply(ref m0, m1.Column1);
            m2.Column2 = Multiply(ref m0, m1.Column2);
            return m2;
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
            :this()
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
        /// 
        /// </summary>
        public Matrix3d Inverse
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
            get { return M00 + M11 + M22; }
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
            var result = new double[9];
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

            result[3] = M10;
            result[4] = M11;
            result[5] = M12;

            result[6] = M20;
            result[7] = M21;
            result[8] = M22;
        }
    }
}
