﻿
/*
 * Notes
 */

using System;
using System.Collections.Generic;

using Constd = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur
{
    /// <summary>
    /// Double precision 3x3 matrix.
    /// </summary>
    [Serializable]
    public struct Matrix3d
    {
        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        public static class Decompose
        {
            /// <summary>
            /// Returns the eigen decomposition of the given matrix A.
            /// Assumes A is symmetric.
            /// </summary>
            public static void EigenSymmetric(Matrix3d A, out Matrix3d Q, out Vector3d lambda, double epsilon = Constd.ZeroTolerance)
            {
                // impl refs
                // https://www2.units.it/ipl/students_area/imm2/files/Numerical_Recipes.pdf (11.1)
                // https://www.mpi-hd.mpg.de/personalhomes/globes/3x3/index.html
                
                Q = Identity;
                DiagonalizeJacobi(ref A, ref Q, epsilon);
                lambda = new Vector3d(A.M00, A.M11, A.M22);

                // sort results by descending absolute eigenvalue
                {
                    if (Math.Abs(lambda.Z) > Math.Abs(lambda.Y))
                    {
                        Utilities.Swap(ref lambda.Y, ref lambda.Z);
                        SwapColumns12(ref Q);
                    }

                    if (Math.Abs(lambda.Y) > Math.Abs(lambda.X))
                    {
                        Utilities.Swap(ref lambda.X, ref lambda.Y);
                        SwapColumns01(ref Q);
                    }

                    if (Math.Abs(lambda.Z) > Math.Abs(lambda.Y))
                    {
                        Utilities.Swap(ref lambda.Y, ref lambda.Z);
                        SwapColumns12(ref Q);
                    }
                }
            }


            /// <summary>
            /// Performs a Jacobi diagonalization of matrix A.
            /// Returns true if A is successfully diagonalized within the specified number of steps.
            /// Also returns the concatenation of applied Jacobi rotations in V.
            /// </summary>
            private static bool DiagonalizeJacobi(ref Matrix3d A, ref Matrix3d V, double epsilon = Constd.ZeroTolerance, int maxSteps = 16)
            {
                // impl refs
                // https://www2.units.it/ipl/students_area/imm2/files/Numerical_Recipes.pdf (11.1)
                // https://en.wikipedia.org/wiki/Jacobi_rotation

                while (maxSteps-- > 0)
                {
                    var a01 = Math.Abs(A.M01);
                    var a02 = Math.Abs(A.M02);
                    var a12 = Math.Abs(A.M12);

                    // Jacobi rotate in plane of max off-diagonal value of A
                    if (a01 > a02 && a01 > a12)
                    {
                        if (a01 <= epsilon) return true;
                        JacobiRotate01(ref A, ref V);
                    }
                    else if (a02 > a12)
                    {
                        if (a02 <= epsilon) return true;
                        JacobiRotate02(ref A, ref V);
                    }
                    else
                    {
                        if (a12 <= epsilon) return true;
                        JacobiRotate12(ref A, ref V);
                    }
                }

                return false;
            }


            /// <summary>
            /// 
            /// </summary>
            private static void JacobiRotate01(ref Matrix3d A, ref Matrix3d V)
            {
                var beta = (A.M11 - A.M00) / (2.0 * A.M01);
                GetJacobiTerms(beta, out var t, out var c, out var s, out var p);

                /*
                    | c  s  0 |
                P = |-s  c  0 |
                    | 0  0  1 |
                */
                
                // A' = Pt A P
                {
                    var a20 = A.M20 - s * (A.M21 + p * A.M20);
                    var a21 = A.M21 + s * (A.M20 - p * A.M21);
                    A.M20 = A.M02 = a20;
                    A.M21 = A.M12 = a21;

                    A.M00 -= t * A.M01;
                    A.M11 += t * A.M01;
                    A.M01 = A.M10 = 0.0; // eliminated off-diagonal elements
                }

                // V' = V P
                {
                    var col0 = V.Column0;
                    var col1 = V.Column1;
                    V.Column0 = c * col0 - s * col1;
                    V.Column1 = s * col0 + c * col1;
                }
            }


            /// <summary>
            /// 
            /// </summary>
            private static void JacobiRotate02(ref Matrix3d A, ref Matrix3d V)
            {
                var beta = (A.M22 - A.M00) / (2.0 * A.M02);
                GetJacobiTerms(beta, out var t, out var c, out var s, out var p);

                /*
                    | c  0  s |
                P = | 0  1  0 |
                    |-s  0  c |
                */

                // A' = Pt A P
                {
                    var a10 = A.M10 - s * (A.M12 + p * A.M10);
                    var a12 = A.M12 + s * (A.M10 - p * A.M12);
                    A.M10 = A.M01 = a10;
                    A.M12 = A.M21 = a12;
                    
                    A.M00 -= t * A.M02;
                    A.M22 += t * A.M02;
                    A.M02 = A.M20 = 0.0; // eliminated off-diagonal elements
                }

                // V' = V P
                {
                    var col0 = V.Column0;
                    var col2 = V.Column2;
                    V.Column0 = c * col0 - s * col2;
                    V.Column2 = s * col0 + c * col2;
                }
            }


            /// <summary>
            /// 
            /// </summary>
            private static void JacobiRotate12(ref Matrix3d A, ref Matrix3d V)
            {
                var beta = (A.M22 - A.M11) / (2.0 * A.M12);
                GetJacobiTerms(beta, out var t, out var c, out var s, out var p);

                /*
                    | 1  0  0 |
                P = | 0  c  s |
                    | 0 -s  c |
                */

                // A' = Pt A P
                {
                    var a01 = A.M01 - s * (A.M02 + p * A.M01);
                    var a02 = A.M02 + s * (A.M01 - p * A.M02);
                    A.M01 = A.M10 = a01;
                    A.M02 = A.M20 = a02;

                    A.M11 -= t * A.M12;
                    A.M22 += t * A.M12;
                    A.M12 = A.M21 = 0.0; // eliminated off-diagonal elements
                }

                // V' = V P
                {
                    var col1 = V.Column1;
                    var col2 = V.Column2;
                    V.Column1 = c * col1 - s * col2;
                    V.Column2 = s * col1 + c * col2;
                }
            }


            /// <summary>
            /// 
            /// </summary>
            private static void GetJacobiTerms(double beta, out double t, out double c, out double s, out double p)
            {
                t = Math.Sign(beta) / (Math.Abs(beta) + Math.Sqrt(beta * beta + 1.0));
                c = 1.0 / Math.Sqrt(t * t + 1.0);
                s = c * t;
                p = s / (1.0 + c);
            }


            /// <summary>
            /// 
            /// </summary>
            private static void SwapColumns01(ref Matrix3d A)
            {
                Utilities.Swap(ref A.M00, ref A.M01);
                Utilities.Swap(ref A.M10, ref A.M11);
                Utilities.Swap(ref A.M20, ref A.M21);
            }


            /// <summary>
            /// 
            /// </summary>
            private static void SwapColumns12(ref Matrix3d A)
            {
                Utilities.Swap(ref A.M01, ref A.M02);
                Utilities.Swap(ref A.M11, ref A.M12);
                Utilities.Swap(ref A.M21, ref A.M22);
            }


#if OBSOLETE
            /// <summary>
            /// Diagonalizes matrix A by iteratively applying Jacobi rotations.
            /// Returns true if A is successfully diagonalized within the specified number of steps.
            /// Also returns the cumulative Jacobi rotation in V.
            /// </summary>
            /// <param name="A"></param>
            /// <param name="V"></param>
            /// <param name="epsilon"></param>
            /// <param name="maxSteps"></param>
            /// <returns></returns>
            private static bool DiagonalizeJacobi(ref Matrix3d A, ref Matrix3d V, double epsilon = D.ZeroTolerance, int maxSteps = 16)
            {
                // impl ref
                // https://www2.units.it/ipl/students_area/imm2/files/Numerical_Recipes.pdf (11.1)

                // TODO optimize - no need to build the full 3x3 rotation matrix at each step
                Matrix3d P;

                while (maxSteps-- > 0)
                {
                    var a01 = Math.Abs(A.M01);
                    var a02 = Math.Abs(A.M02);
                    var a12 = Math.Abs(A.M12);

                    // Create Jacobi rotation P from max off-diagonal value of A
                    if (a01 > a02 && a01 > a12)
                    {
                        if (a01 < epsilon) return true;
                        GetJacobiRotation01(ref A, out P);
                    }
                    else if (a02 > a12)
                    {
                        if (a02 < epsilon) return true;
                        GetJacobiRotation02(ref A, out P);
                    }
                    else
                    {
                        if (a12 < epsilon) return true;
                        GetJacobiRotation12(ref A, out P);
                    }

                    // Apply Jacobi rotation
                    A = P.ApplyTranspose(A.Apply(ref P)); // A' = Pt A P
                    V = V.Apply(ref P); // V' = V P
                }

                return false;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            private static void GetJacobiRotation01(ref Matrix3d A, out Matrix3d P)
            {
                var b = (A.M11 - A.M00) / (2.0 * A.M01);
                GetJacobiTerms(b, out var c, out var s);
                
                P = Identity;
                P.M00 = P.M11 = c;
                P.M01 = s;
                P.M10 = -s;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            private static void GetJacobiRotation02(ref Matrix3d A, out Matrix3d P)
            {
                var b = (A.M22 - A.M00) / (2.0 * A.M02);
                GetJacobiTerms(b, out var c, out var s);

                P = Identity;
                P.M00 = P.M22 = c;
                P.M02 = s;
                P.M20 = -s;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            private static void GetJacobiRotation12(ref Matrix3d A, out Matrix3d P)
            {
                var b = (A.M22 - A.M11) / (2.0 * A.M12);
                GetJacobiTerms(b, out var c, out var s);

                P = Identity;
                P.M11 = P.M22 = c;
                P.M12 = s;
                P.M21 = -s;
            }
#endif


            /// <summary>
            /// 
            /// </summary>
            /// <param name="A"></param>
            /// <param name="Q"></param>
            /// <param name="R"></param>
            public static void QR(ref Matrix3d A, out Matrix3d Q, out Matrix3d R)
            {
                // impl ref
                // http://pages.cs.wisc.edu/~sifakis/papers/SVD_TR1690.pdf
                // http://davidstutz.de/matrix-decompositions/matrix-decompositions/givens

                // R -> result of successive Givens rotations applied to A (3x3 upper triangular)
                // Q -> concatenation of all Givens rotations (3x3 orthogonal)
                
                R = A;
                Q = Identity;

                GivensRotate10(ref R, ref Q);
                GivensRotate20(ref R, ref Q);
                GivensRotate21(ref R, ref Q);
            }


            /// <summary>
            /// 
            /// </summary>
            private static void GivensRotate10(ref Matrix3d A, ref Matrix3d V)
            {                
                GetGivensTerms(A.M00, A.M10, out var c, out var s);

                /*
                    | c -s  0 |
                G = | s  c  0 |
                    | 0  0  1 |
                */
                
                Apply(ref A); // A' = G A
                Apply(ref V); // V' = G V

                void Apply(ref Matrix3d R)
                {
                    var row0 = R.Row0;
                    var row1 = R.Row1;
                    R.Row0 = c * row0 - s * row1;
                    R.Row1 = s * row0 + c * row1;
                }
            }


            /// <summary>
            /// 
            /// </summary>
            private static void GivensRotate20(ref Matrix3d A, ref Matrix3d V)
            {
                GetGivensTerms(A.M00, A.M20, out var c, out var s);

                /*
                    | c  0 -s |
                G = | 0  1  0 |
                    | s  0  c |
                */

                Apply(ref A); // A' = G A
                Apply(ref V); // V' = G V

                void Apply(ref Matrix3d R)
                {
                    var row0 = R.Row0;
                    var row2 = R.Row2;
                    R.Row0 = c * row0 - s * row2;
                    R.Row2 = s * row0 + c * row2;
                }
            }


            /// <summary>
            /// 
            /// </summary>
            private static void GivensRotate21(ref Matrix3d A, ref Matrix3d V)
            {
                GetGivensTerms(A.M11, A.M21, out var c, out var s);

                /*
                    | 1  0  0 |
                G = | 0  c -s |
                    | 0  s  c |
                */

                Apply(ref A); // A' = G A
                Apply(ref V); // V' = G V

                void Apply(ref Matrix3d R)
                {
                    var row1 = R.Row1;
                    var row2 = R.Row2;
                    R.Row1 = c * row1 - s * row2;
                    R.Row2 = s * row1 + c * row2;
                }
            }


            /// <summary>
            /// 
            /// </summary>
            private static void GetGivensTerms(double ajj, double aij, out double c, out double s)
            {
                var t = Math.Sqrt(ajj * ajj + aij * aij);
                c = ajj / t;
                s = -aij / t;
            }


            /// <summary>
            /// Performs a singular value decomposition of the given matrix A.
            /// Returns the rank of A.
            /// Note that this implementation ensures that U and V are proper rotations (i.e. no reflections).
            /// </summary>
            public static int SingularValue(ref Matrix3d A, out Matrix3d U, out Vector3d sigma, out Matrix3d V, double epsilon = Constd.ZeroTolerance)
            {
                // U -> proper rotation (no reflection)
                // sigma -> singular values
                // V -> proper rotation (no reflection)

                // impl ref
                // https://www.math.ucla.edu/~jteran/papers/ITF04.pdf

                var AtA = A.ApplyTranspose(ref A);
                EigenSymmetricPSD(AtA, out V, out sigma, epsilon);

                // handle reflection in V
                if (V.Determinant < 0.0)
                    V.Column2 *= -1.0;

                // U = A V inv(sigma)
                // must handle cases where singular values are zero
                if (sigma.X <= epsilon)
                {
                    // all zero singular values

                    //     | 0  -  - |
                    // Σ = | -  0  - |
                    //     | -  -  0 |

                    sigma.X = sigma.Y = sigma.Z = 0.0;
                    U = Identity;

                    return 0;
                }
                else if (sigma.Y <= epsilon)
                {
                    // one non-zero singular value

                    //     | x  -  - |
                    // Σ = | -  0  - |
                    //     | -  -  0 |

                    sigma.X = Math.Sqrt(sigma.X);
                    sigma.Y = sigma.Z = 0.0;

                    var u0 = A.Apply(V.Column0 / sigma.X);
                    var u1 = u0.Y > 0.0 ? u0.CrossX : u0.CrossY;

                    u1.Unitize();
                    U = new Matrix3d(u0, u1, Vector3d.Cross(u0, u1));

                    return 1;
                }
                else if (sigma.Z <= epsilon)
                {
                    // two non-zero singular values

                    //     | x  -  - |
                    // Σ = | -  y  - |
                    //     | -  -  0 |

                    sigma.X = Math.Sqrt(sigma.X);
                    sigma.Y = Math.Sqrt(sigma.Y);
                    sigma.Z = 0.0;

                    var u0 = A.Apply(V.Column0 / sigma.X);
                    var u1 = A.Apply(V.Column1 / sigma.Y);
                    U = new Matrix3d(u0, u1, Vector3d.Cross(u0, u1));

                    return 2;
                }

                // all non-zero singular values

                //     | x  -  - |
                // Σ = | -  y  - |
                //     | -  -  z |

                sigma.X = Math.Sqrt(sigma.X);
                sigma.Y = Math.Sqrt(sigma.Y);
                sigma.Z = Math.Sqrt(sigma.Z);

                U = new Matrix3d(
                    A.Apply(V.Column0 / sigma.X),
                    A.Apply(V.Column1 / sigma.Y),
                    A.Apply(V.Column2 / sigma.Z));

                // handle reflection in U
                if (U.Determinant < 0.0)
                {
                    U.Column2 *= -1.0;
                    sigma.Z *= -1.0;
                }

                return 3;
            }


            /// <summary>
            /// Returns the eigen decomposition of the given matrix A.
            /// Assumes that A is symmetric and has non-negative eigenvalues (positive semidefinite).
            /// </summary>
            private static void EigenSymmetricPSD(Matrix3d A, out Matrix3d Q, out Vector3d lambda, double epsilon = Constd.ZeroTolerance)
            {
                // impl refs
                // https://www2.units.it/ipl/students_area/imm2/files/Numerical_Recipes.pdf (11.1)
                // https://www.mpi-hd.mpg.de/personalhomes/globes/3x3/index.html

                Q = Identity;
                DiagonalizeJacobi(ref A, ref Q, epsilon);
                lambda = new Vector3d(A.M00, A.M11, A.M22);

                // sort results by descending eigenvalue
                {
                    if (lambda.Z > lambda.Y)
                    {
                        Utilities.Swap(ref lambda.Y, ref lambda.Z);
                        SwapColumns12(ref Q);
                    }

                    if (lambda.Y > lambda.X)
                    {
                        Utilities.Swap(ref lambda.X, ref lambda.Y);
                        SwapColumns01(ref Q);
                    }

                    if (lambda.Z > lambda.Y)
                    {
                        Utilities.Swap(ref lambda.Y, ref lambda.Z);
                        SwapColumns12(ref Q);
                    }
                }
            }


            /// <summary>
            /// Returns the polar decomposition of the given matrix A.
            /// Also returns the rank of A.
            /// </summary>
            /// <param name="A"></param>
            /// <param name="R"></param>
            /// <param name="S"></param>
            public static int Polar(ref Matrix3d A, out Matrix3d R, out Matrix3d S)
            {
                // impl ref
                // https://en.wikipedia.org/wiki/Orthogonal_Procrustes_problem

                // R -> proper rotation (no reflection)
                // S -> deformation

                // Can be reconstructed from SVD of A
                int rank = SingularValue(ref A, out Matrix3d U, out Vector3d sigma, out Matrix3d V);
                
                // S = V sigma Vt
                S = new Matrix3d(
                    V.Apply(sigma * V.Row0),
                    V.Apply(sigma * V.Row1),
                    V.Apply(sigma * V.Row2)
                    );

                // R = U Vt
                R = new Matrix3d(
                    U.Apply(V.Row0),
                    U.Apply(V.Row1),
                    U.Apply(V.Row2)
                    );

                return rank;
            }
        }
        
#endregion


#region Static Members

        /// <summary></summary>
        public static readonly Matrix3d Identity = new Matrix3d(1.0);


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
        public static Vector3d operator *(Matrix3d matrix, Vector3d vector)
        {
            return matrix.Apply(vector);
        }


        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <returns></returns>
        public static Matrix3d operator *(Matrix3d m0, Matrix3d m1)
        {
            return m0.Apply(ref m1);
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
        /// 
        /// </summary>
        /// <param name="m0"></param>
        /// <param name="m1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Matrix3d Lerp(Matrix3d m0, Matrix3d m1, double t)
        {
            return m0.LerpTo(m1, t);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="row0"></param>
        /// <param name="row1"></param>
        /// <param name="row2"></param>
        /// <returns></returns>
        public static Matrix3d CreateFromRows(Vector3d row0, Vector3d row1, Vector3d row2)
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
        /// <param name="column0"></param>
        /// <param name="column1"></param>
        /// <param name="column2"></param>
        /// <returns></returns>
        public static Matrix3d CreateFromColumns(Vector3d column0, Vector3d column1, Vector3d column2)
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
        /// <param name="points"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static Matrix3d CreateCovariance(IEnumerable<Vector3d> points, out Vector3d mean)
        {
            mean = points.Mean();
            return CreateCovariance(points, mean);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static Matrix3d CreateCovariance(IEnumerable<Vector3d> points, Vector3d mean)
        {
            var result = new Matrix3d();
            int n = 0;
            
            foreach (var v in points)
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

            var t = 1.0 / n;
            result.M00 *= t;
            result.M11 *= t;
            result.M22 *= t;
            result.M10 = result.M01 *= t;
            result.M20 = result.M02 *= t;
            result.M21 = result.M12 *= t;

            return result;
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
        /// <param name="diagonal"></param>
        public Matrix3d(Vector3d diagonal)
            : this()
        {
            M00 = diagonal.X;
            M11 = diagonal.Y;
            M22 = diagonal.Z;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="column0"></param>
        /// <param name="column1"></param>
        /// <param name="column2"></param>
        public Matrix3d(Vector3d column0, Vector3d column1, Vector3d column2)
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
            double m20, double m21, double m22)
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
        public Vector3d Column0
        {
            get { return new Vector3d(M00, M10, M20); }
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
        public Vector3d Column1
        {
            get { return new Vector3d(M01, M11, M21); }
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
        public Vector3d Column2
        {
            get { return new Vector3d(M02, M12, M22); }
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
        public Vector3d Row0
        {
            get { return new Vector3d(M00, M01, M02); }
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
        public Vector3d Row1
        {
            get { return new Vector3d(M10, M11, M12); }
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
        public Vector3d Row2
        {
            get { return new Vector3d(M20, M21, M22); }
            set
            {
                M20 = value.X;
                M21 = value.Y;
                M22 = value.Z;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector3d Diagonal
        {
            get => new Vector3d(M00, M11, M22);
            set
            {
                M00 = value.X;
                M11 = value.Y;
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
        /// Returns the inverse of this matrix.
        /// If not invertable, returns the identity matrix.
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
        /// Returns the adjugate matrix i.e. the transpose of the cofactor matrix.
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
        /// 
        /// </summary>
        /// <returns></returns>
        public Matrix4d As4d
        {
            get => new Matrix4d(
                M00, M01, M02, 0.0,
                M10, M11, M12, 0.0,
                M20, M21, M22, 0.0,
                0.0, 0.0, 0.0, 1.0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool IsSymmetric(double epsilon = Constd.ZeroTolerance)
        {
            return
                SlurMath.ApproxEquals(M01, M10, epsilon) &&
                SlurMath.ApproxEquals(M01, M10, epsilon) &&
                SlurMath.ApproxEquals(M12, M21, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double OneNorm()
        {
            return new Vector3d(Column0.ComponentSum, Column1.ComponentSum, Column2.ComponentSum).ComponentMax;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double InfNorm()
        {
            return new Vector3d(Row0.ComponentSum, Row1.ComponentSum, Row2.ComponentSum).ComponentMax;
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
            Utilities.Swap(ref M10, ref M01);
            Utilities.Swap(ref M20, ref M02);
            Utilities.Swap(ref M21, ref M12);
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
            double d = M00 * m00 + M01 * m10 + M02 * m20; // determinant

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
        public Vector3d Apply(Vector3d vector)
        {
            return new Vector3d(
               Vector3d.Dot(Row0, vector),
               Vector3d.Dot(Row1, vector),
               Vector3d.Dot(Row2, vector)
               );
        }


        /// <summary>
        /// Applies this transformation to the given transformation.
        /// </summary>
        /// <param name="other"></param>
        public Matrix3d Apply(Matrix3d other)
        {
            return Apply(ref other);
        }


        /// <summary>
        /// Applies this transformation to the given transformation.
        /// </summary>
        /// <param name="other"></param>
        public Matrix3d Apply(ref Matrix3d other)
        {
            return new Matrix3d(
                Apply(other.Column0),
                Apply(other.Column1),
                Apply(other.Column2));
        }


        /// <summary>
        /// Applies the transpose of this transformation to the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector3d ApplyTranspose(Vector3d vector)
        {
            return new Vector3d(
               Vector3d.Dot(Column0, vector),
               Vector3d.Dot(Column1, vector),
               Vector3d.Dot(Column2, vector)
               );
        }


        /// <summary>
        /// Applies the transpose of this transformation to the given transformation.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Matrix3d ApplyTranspose(Matrix3d other)
        {
            return ApplyTranspose(ref other);
        }


        /// <summary>
        /// Applies the transpose of this transformation to the given transformation.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Matrix3d ApplyTranspose(ref Matrix3d other)
        {
            return new Matrix3d(
                ApplyTranspose(other.Column0),
                ApplyTranspose(other.Column1),
                ApplyTranspose(other.Column2)
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(Matrix3d other, double epsilon = Constd.ZeroTolerance)
        {
            return ApproxEquals(ref other, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(ref Matrix3d other, double epsilon = Constd.ZeroTolerance)
        {
            return
                SlurMath.ApproxEquals(M00, other.M00, epsilon) &&
                SlurMath.ApproxEquals(M01, other.M01, epsilon) &&
                SlurMath.ApproxEquals(M02, other.M02, epsilon) &&

                SlurMath.ApproxEquals(M10, other.M10, epsilon) &&
                SlurMath.ApproxEquals(M11, other.M11, epsilon) &&
                SlurMath.ApproxEquals(M12, other.M12, epsilon) &&

                SlurMath.ApproxEquals(M20, other.M20, epsilon) &&
                SlurMath.ApproxEquals(M21, other.M21, epsilon) &&
                SlurMath.ApproxEquals(M22, other.M22, epsilon);
        }


        /// <summary>
        /// Returns the roots of the characteristic polynomial of this matrix.
        /// These are also the eigenvalues of this matrix.
        /// </summary>
        public bool SolveCharacteristic(out double r0, out double r1, out double r2, double epsilon = Constd.ZeroTolerance)
        {
            // impl ref
            // https://math.stackexchange.com/questions/1721765/compute-the-characteristic-equation-3x3-matrix?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa

            return SlurMath.SolveCubic(-Trace, Minor00 + Minor11 + Minor22, -Determinant, out r0, out r1, out r2, epsilon) > 0;
        }


        /// <summary>
        /// Solves the linear system Ax = b
        /// </summary>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public bool SolveFor(Vector3d b, out Vector3d x)
        {
            // impl ref
            // https://en.wikipedia.org/wiki/Cramer%27s_rule

            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Matrix3d LerpTo(Matrix3d other, double factor)
        {
            return CreateFromRows(
                Row0.LerpTo(other.Row0, factor),
                Row1.LerpTo(other.Row1, factor),
                Row2.LerpTo(other.Row2, factor));
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
