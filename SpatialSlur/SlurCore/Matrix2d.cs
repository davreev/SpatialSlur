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
    /// Double precision 2x2 matrix.
    /// </summary>
    [Serializable]
    public struct Matrix2d
    {
        #region Static

        /// <summary></summary>
        public static Matrix2d Identity = new Matrix2d(1.0);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        public static implicit operator Matrix2d(Matrix3d matrix)
        {
            return new Matrix2d(
                matrix.M00, matrix.M01,
                matrix.M10, matrix.M11
                );
        }


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
            matrix.M00 *= scalar;
            matrix.M01 *= scalar;

            matrix.M10 *= scalar;
            matrix.M11 *= scalar;

            return matrix;
        }


        /// <summary>
        /// Matrix vector multiplication
        /// </summary>
        /// <returns></returns>
        public static Vec2d operator *(Matrix2d matrix, Vec2d vector)
        {
            return Multiply(ref matrix, vector);
        }


        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <returns></returns>
        public static Matrix2d operator *(Matrix2d m0, Matrix2d m1)
        {
            return Multiply(ref m0, ref m1);
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
        /// Matrix vector multiplication
        /// </summary>
        /// <returns></returns>
        public static Vec2d Multiply(ref Matrix2d matrix, Vec2d vector)
        {
            return new Vec2d(
                Vec2d.Dot(vector, matrix.Row0),
                Vec2d.Dot(vector, matrix.Row1)
                );
        }


        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <returns></returns>
        public static Matrix2d Multiply(ref Matrix2d m0, ref Matrix2d m1)
        {
            return new Matrix2d(
                Multiply(ref m0, m1.Column0),
                Multiply(ref m0, m1.Column1)
                );
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
        public Matrix2d(Vec2d column0, Vec2d column1)
        {
            M00 = column0.X;
            M01 = column1.X;

            M10 = column0.Y;
            M11 = column1.Y;
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec2d Column0
        {
            get { return new Vec2d(M00, M10); }
            set
            {
                M00 = value.X;
                M10 = value.Y;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec2d Column1
        {
            get { return new Vec2d(M01, M11); }
            set
            {
                M01 = value.X;
                M11 = value.Y;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec2d Row0
        {
            get { return new Vec2d(M00, M01); }
            set
            {
                M00 = value.X;
                M01 = value.Y;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec2d Row1
        {
            get { return new Vec2d(M10, M11); }
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
        /// 
        /// </summary>
        public Matrix2d Inverse
        {
            get
            {
                var m = this;
                m.Invert();
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
        /// Transposes this matrix in place.
        /// </summary>
        public void Transpose()
        {
            CoreUtil.Swap(ref M10, ref M01);
        }


        /// <summary>
        /// Inverts this matrix in place
        /// </summary>
        public void Invert()
        {
            var dInv = 1.0 / Determinant;
            var temp = M00;

            M00 = M11 * dInv;
            M01 *= -dInv;
            M10 *= -dInv;
            M11 = temp * dInv;
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
