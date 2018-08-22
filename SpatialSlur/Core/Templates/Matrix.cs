
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Drawing;

using SpatialSlur.Collections;

using static System.Threading.Tasks.Parallel;

using D = SpatialSlur.SlurMath.Constantsd;
using F = SpatialSlur.SlurMath.Constantsf;

namespace SpatialSlur
{
	/// <summary>
	/// Contains common operations for n-dimensional matrices
	/// </summary>
	public static partial class Matrix
	{
        /// <summary>
	    /// Contains parallel implementations
	    /// </summary>
        public static partial class Parallel
        {

		#region Vector2d

		/// <summary>
        /// 
        /// </summary>
        public static void Abs(ReadOnlyArrayView<Vector2d> matrix, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, matrix.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = Vector2d.Abs(matrix[i]);
            });
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Max(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = Vector2d.Max(m0[i], m1[i]);
            });
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Min(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = Vector2d.Min(m0[i], m1[i]);
            });
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Add(ReadOnlyArrayView<Vector2d> matrix, double scalar, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, matrix.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = matrix[i] + new Vector2d(scalar);
            });
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Add(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i] + m1[i];
            });
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Subtract(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i] - m1[i];
            });
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Multiply(ReadOnlyArrayView<Vector2d> matrix, double scalar, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, matrix.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = matrix[i] * scalar;
            });
        }


		/// <summary>
        /// Note that the given vector is assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void Multiply(ReadOnlyArrayView<Vector2d> matrix, ReadOnlyArrayView<double> vector, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, matrix.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = matrix[i] * vector[i];
            });
        }


		/// <summary>
        ///
        /// </summary>
        public static void MultiplyPointwise(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i] * m1[i];
            });
        }


		/// <summary>
        /// Note that the given vector is assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void Divide(ReadOnlyArrayView<Vector2d> matrix, ReadOnlyArrayView<double> vector, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, matrix.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = matrix[i] / vector[i];
            });
        }


		/// <summary>
        /// 
        /// </summary>
        public static void DividePointwise(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i] / m1[i];
            });
        }


		/// <summary>
        /// result = m0 + m1 * t
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, double t, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i] + m1[i] * t;
            });
        }


		/// <summary>
        /// result = m0 + m1 * t
		/// Note that t is assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, ReadOnlyArrayView<double> t, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i] + m1[i] * t[i];
            });
        }


		/// <summary>
        /// result = m0 * t0 + m1 * t1
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<Vector2d> m0, double t0, ReadOnlyArrayView<Vector2d> m1, double t1, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i] * t0 + m1[i] * t1;
            });
        }


		/// <summary>
        /// result = m0 * t0 + m1 * t1
		/// Note that t0 and t1 are assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<double> t0, ReadOnlyArrayView<Vector2d> m1, ReadOnlyArrayView<double> t1, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i] * t0[i] + m1[i] * t1[i];
            });
        }


		/// <summary>
        /// result = m0 + (m1 - m2) * t
        /// </summary>
        public static void AddScaledDelta(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, ReadOnlyArrayView<Vector2d> m2, double t, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i] + (m1[i] - m2[i]) * t;
            });
        }


		/// <summary>
        /// result = m0 + (m1 - m2) * t
		/// Note that t is assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void AddScaledDelta(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, ReadOnlyArrayView<Vector2d> m2, ReadOnlyArrayView<double> t, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i] + (m1[i] - m2[i]) * t[i];
            });
        }


		/// <summary>
        ///
        /// </summary>
        public static void Lerp(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, double t, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i].LerpTo(m1[i], t);
            });
        }


		/// <summary>
        /// result = m0 + (m1 - m0) * t
        /// Note that t is assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void LerpColumns(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, ReadOnlyArrayView<double> t, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i].LerpTo(m1[i], t[i]);
            });
        }


		/// <summary>
        /// 
        /// </summary>
        public static void UnitizeColumns(ReadOnlyArrayView<Vector2d> matrix, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, matrix.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = matrix[i].Unit;
            });
        }


		/// <summary>
        ///
        /// </summary>
        public static void NormalizeColumns(ReadOnlyArrayView<Vector2d> matrix, Interval2d interval, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, matrix.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = interval.Normalize(matrix[i]);
            });
        }


		/// <summary>
        ///
        /// </summary>
        public static void EvaluateColumns(ReadOnlyArrayView<Vector2d> matrix, Interval2d interval, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, matrix.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = interval.Evaluate(matrix[i]);
            });
        }


		/// <summary>
        ///
        /// </summary>
        public static void RemapColumns(ReadOnlyArrayView<Vector2d> matrix, Interval2d from, Interval2d to, ArrayView<Vector2d> result)
        {
            ForEach(new UniformPartitioner(0, matrix.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = Interval2d.Remap(matrix[i], from, to);
            });
        }

		#endregion


		#region Vector3d

		/// <summary>
        /// 
        /// </summary>
        public static void Abs(ReadOnlyArrayView<Vector3d> matrix, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, matrix.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = Vector3d.Abs(matrix[i]);
            });
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Max(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = Vector3d.Max(m0[i], m1[i]);
            });
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Min(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = Vector3d.Min(m0[i], m1[i]);
            });
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Add(ReadOnlyArrayView<Vector3d> matrix, double scalar, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, matrix.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = matrix[i] + new Vector3d(scalar);
            });
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Add(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i] + m1[i];
            });
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Subtract(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i] - m1[i];
            });
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Multiply(ReadOnlyArrayView<Vector3d> matrix, double scalar, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, matrix.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = matrix[i] * scalar;
            });
        }


		/// <summary>
        /// Note that the given vector is assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void Multiply(ReadOnlyArrayView<Vector3d> matrix, ReadOnlyArrayView<double> vector, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, matrix.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = matrix[i] * vector[i];
            });
        }


		/// <summary>
        ///
        /// </summary>
        public static void MultiplyPointwise(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i] * m1[i];
            });
        }


		/// <summary>
        /// Note that the given vector is assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void Divide(ReadOnlyArrayView<Vector3d> matrix, ReadOnlyArrayView<double> vector, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, matrix.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = matrix[i] / vector[i];
            });
        }


		/// <summary>
        /// 
        /// </summary>
        public static void DividePointwise(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i] / m1[i];
            });
        }


		/// <summary>
        /// result = m0 + m1 * t
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, double t, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i] + m1[i] * t;
            });
        }


		/// <summary>
        /// result = m0 + m1 * t
		/// Note that t is assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, ReadOnlyArrayView<double> t, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i] + m1[i] * t[i];
            });
        }


		/// <summary>
        /// result = m0 * t0 + m1 * t1
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<Vector3d> m0, double t0, ReadOnlyArrayView<Vector3d> m1, double t1, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i] * t0 + m1[i] * t1;
            });
        }


		/// <summary>
        /// result = m0 * t0 + m1 * t1
		/// Note that t0 and t1 are assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<double> t0, ReadOnlyArrayView<Vector3d> m1, ReadOnlyArrayView<double> t1, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i] * t0[i] + m1[i] * t1[i];
            });
        }


		/// <summary>
        /// result = m0 + (m1 - m2) * t
        /// </summary>
        public static void AddScaledDelta(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, ReadOnlyArrayView<Vector3d> m2, double t, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i] + (m1[i] - m2[i]) * t;
            });
        }


		/// <summary>
        /// result = m0 + (m1 - m2) * t
		/// Note that t is assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void AddScaledDelta(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, ReadOnlyArrayView<Vector3d> m2, ReadOnlyArrayView<double> t, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i] + (m1[i] - m2[i]) * t[i];
            });
        }


		/// <summary>
        ///
        /// </summary>
        public static void Lerp(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, double t, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i].LerpTo(m1[i], t);
            });
        }


		/// <summary>
        /// result = m0 + (m1 - m0) * t
        /// Note that t is assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void LerpColumns(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, ReadOnlyArrayView<double> t, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, m0.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = m0[i].LerpTo(m1[i], t[i]);
            });
        }


		/// <summary>
        /// 
        /// </summary>
        public static void UnitizeColumns(ReadOnlyArrayView<Vector3d> matrix, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, matrix.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = matrix[i].Unit;
            });
        }


		/// <summary>
        ///
        /// </summary>
        public static void NormalizeColumns(ReadOnlyArrayView<Vector3d> matrix, Interval3d interval, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, matrix.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = interval.Normalize(matrix[i]);
            });
        }


		/// <summary>
        ///
        /// </summary>
        public static void EvaluateColumns(ReadOnlyArrayView<Vector3d> matrix, Interval3d interval, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, matrix.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = interval.Evaluate(matrix[i]);
            });
        }


		/// <summary>
        ///
        /// </summary>
        public static void RemapColumns(ReadOnlyArrayView<Vector3d> matrix, Interval3d from, Interval3d to, ArrayView<Vector3d> result)
        {
            ForEach(new UniformPartitioner(0, matrix.Count), p =>
            {
                for (int i = p.From; i < p.To; i++)
                    result[i] = Interval3d.Remap(matrix[i], from, to);
            });
        }

		#endregion

        }


		#region Vector2d

		/// <summary>
        /// 
        /// </summary>
        public static Vector2d ColumnMax(ReadOnlyArrayView<Vector2d> matrix)
        {
			var result = matrix[0];

            for (int i = 1; i < matrix.Count; i++)
                result = Vector2d.Max(matrix[i], result);

            return result;
        }


		/// <summary>
        /// 
        /// </summary>
        public static Vector2d ColumnMin(ReadOnlyArrayView<Vector2d> matrix)
        {
			var result = matrix[0];

            for (int i = 1; i < matrix.Count; i++)
                result = Vector2d.Min(matrix[i], result);

            return result;
        }


		/// <summary>
        /// 
        /// </summary>
        public static Vector2d ColumnSum(ReadOnlyArrayView<Vector2d> matrix)
        {
			var sum = Vector2d.Zero;

            for (int i = 0; i < matrix.Count; i++)
                sum += matrix[i];

            return sum;
        }


		/// <summary>
        /// 
        /// </summary>
        public static Vector2d ColumnMean(ReadOnlyArrayView<Vector2d> matrix)
        {
			return ColumnSum(matrix) / matrix.Count;
		}


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, double epsilon = D.ZeroTolerance)
        {
            for (int i = 0; i < m0.Count; i++)
                if (!m0[i].ApproxEquals(m1[i], epsilon)) return false;

            return true;
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Abs(ReadOnlyArrayView<Vector2d> matrix, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < matrix.Count; i++)
                result[i] = Vector2d.Abs(matrix[i]);
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Max(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = Vector2d.Max(m0[i], m1[i]);
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Min(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = Vector2d.Min(m0[i], m1[i]);
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Add(ReadOnlyArrayView<Vector2d> matrix, double scalar, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < matrix.Count; i++)
                result[i] = matrix[i] + new Vector2d(scalar);
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Add(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i] + m1[i];
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Subtract(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i] - m1[i];
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Multiply(ReadOnlyArrayView<Vector2d> matrix, double scalar, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < matrix.Count; i++)
                result[i] = matrix[i] * scalar;
        }


		/// <summary>
        /// Note that the given vector is assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void Multiply(ReadOnlyArrayView<Vector2d> matrix, ReadOnlyArrayView<double> vector, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < matrix.Count; i++)
                result[i] = matrix[i] * vector[i];
        }


		/// <summary>
        /// 
        /// </summary>
        public static void MultiplyPointwise(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i] * m1[i];
        }


		/// <summary>
        /// Note that the given vector is assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void Divide(ReadOnlyArrayView<Vector2d> matrix, ReadOnlyArrayView<double> vector, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < matrix.Count; i++)
                result[i] = matrix[i] / vector[i];
        }


		/// <summary>
        /// 
        /// </summary>
        public static void DividePointwise(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i] / m1[i];
        }


		/// <summary>
        /// result = m0 + m1 * t
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, double t, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i] + m1[i] * t;
        }


		/// <summary>
        /// result = m0 + m1 * t
		/// Note that t is assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, ReadOnlyArrayView<double> t, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i] + m1[i] * t[i];
        }


		/// <summary>
        /// result = m0 * t0 + m1 * t1
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<Vector2d> m0, double t0, ReadOnlyArrayView<Vector2d> m1, double t1, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i] * t0 + m1[i] * t1;
        }


		/// <summary>
        /// result = m0 * t0 + m1 * t1
		/// Note that t0 and t1 are assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<double> t0, ReadOnlyArrayView<Vector2d> m1, ReadOnlyArrayView<double> t1, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i] * t0[i] + m1[i] * t1[i];
        }


		/// <summary>
        /// result = m0 + (m1 - m2) * t
        /// </summary>
        public static void AddScaledDelta(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, ReadOnlyArrayView<Vector2d> m2, double t, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i] + (m1[i] - m2[i]) * t;
        }


		/// <summary>
        /// result = m0 + (m1 - m2) * t
		/// Note that t is assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void AddScaledDelta(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, ReadOnlyArrayView<Vector2d> m2, ReadOnlyArrayView<double> t, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i] + (m1[i] - m2[i]) * t[i];
        }


		/// <summary>
        ///
        /// </summary>
        public static void Lerp(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, double t, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i].LerpTo(m1[i], t);
        }


		/// <summary>
        /// result = m0 + (m1 - m0) * t
        /// Note that t is assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void LerpColumns(ReadOnlyArrayView<Vector2d> m0, ReadOnlyArrayView<Vector2d> m1, ReadOnlyArrayView<double> t, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i].LerpTo(m1[i], t[i]);
        }


		/// <summary>
        /// 
        /// </summary>
        public static void UnitizeColumns(ReadOnlyArrayView<Vector2d> matrix, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < matrix.Count; i++)
                result[i] = matrix[i].Unit;
        }


		/// <summary>
        ///
        /// </summary>
        public static void NormalizeColumns(ReadOnlyArrayView<Vector2d> matrix, Interval2d interval, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < matrix.Count; i++)
                result[i] = interval.Normalize(matrix[i]);
        }


		/// <summary>
        ///
        /// </summary>
        public static void EvaluateColumns(ReadOnlyArrayView<Vector2d> matrix, Interval2d interval, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < matrix.Count; i++)
                result[i] = interval.Evaluate(matrix[i]);
        }


		/// <summary>
        ///
        /// </summary>
        public static void RemapColumns(ReadOnlyArrayView<Vector2d> matrix, Interval2d from, Interval2d to, ArrayView<Vector2d> result)
        {
            for (int i = 0; i < matrix.Count; i++)
                result[i] = Interval2d.Remap(matrix[i], from, to);
        }

		#endregion


		#region Vector3d

		/// <summary>
        /// 
        /// </summary>
        public static Vector3d ColumnMax(ReadOnlyArrayView<Vector3d> matrix)
        {
			var result = matrix[0];

            for (int i = 1; i < matrix.Count; i++)
                result = Vector3d.Max(matrix[i], result);

            return result;
        }


		/// <summary>
        /// 
        /// </summary>
        public static Vector3d ColumnMin(ReadOnlyArrayView<Vector3d> matrix)
        {
			var result = matrix[0];

            for (int i = 1; i < matrix.Count; i++)
                result = Vector3d.Min(matrix[i], result);

            return result;
        }


		/// <summary>
        /// 
        /// </summary>
        public static Vector3d ColumnSum(ReadOnlyArrayView<Vector3d> matrix)
        {
			var sum = Vector3d.Zero;

            for (int i = 0; i < matrix.Count; i++)
                sum += matrix[i];

            return sum;
        }


		/// <summary>
        /// 
        /// </summary>
        public static Vector3d ColumnMean(ReadOnlyArrayView<Vector3d> matrix)
        {
			return ColumnSum(matrix) / matrix.Count;
		}


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, double epsilon = D.ZeroTolerance)
        {
            for (int i = 0; i < m0.Count; i++)
                if (!m0[i].ApproxEquals(m1[i], epsilon)) return false;

            return true;
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Abs(ReadOnlyArrayView<Vector3d> matrix, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < matrix.Count; i++)
                result[i] = Vector3d.Abs(matrix[i]);
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Max(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = Vector3d.Max(m0[i], m1[i]);
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Min(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = Vector3d.Min(m0[i], m1[i]);
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Add(ReadOnlyArrayView<Vector3d> matrix, double scalar, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < matrix.Count; i++)
                result[i] = matrix[i] + new Vector3d(scalar);
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Add(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i] + m1[i];
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Subtract(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i] - m1[i];
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Multiply(ReadOnlyArrayView<Vector3d> matrix, double scalar, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < matrix.Count; i++)
                result[i] = matrix[i] * scalar;
        }


		/// <summary>
        /// Note that the given vector is assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void Multiply(ReadOnlyArrayView<Vector3d> matrix, ReadOnlyArrayView<double> vector, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < matrix.Count; i++)
                result[i] = matrix[i] * vector[i];
        }


		/// <summary>
        /// 
        /// </summary>
        public static void MultiplyPointwise(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i] * m1[i];
        }


		/// <summary>
        /// Note that the given vector is assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void Divide(ReadOnlyArrayView<Vector3d> matrix, ReadOnlyArrayView<double> vector, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < matrix.Count; i++)
                result[i] = matrix[i] / vector[i];
        }


		/// <summary>
        /// 
        /// </summary>
        public static void DividePointwise(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i] / m1[i];
        }


		/// <summary>
        /// result = m0 + m1 * t
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, double t, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i] + m1[i] * t;
        }


		/// <summary>
        /// result = m0 + m1 * t
		/// Note that t is assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, ReadOnlyArrayView<double> t, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i] + m1[i] * t[i];
        }


		/// <summary>
        /// result = m0 * t0 + m1 * t1
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<Vector3d> m0, double t0, ReadOnlyArrayView<Vector3d> m1, double t1, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i] * t0 + m1[i] * t1;
        }


		/// <summary>
        /// result = m0 * t0 + m1 * t1
		/// Note that t0 and t1 are assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<double> t0, ReadOnlyArrayView<Vector3d> m1, ReadOnlyArrayView<double> t1, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i] * t0[i] + m1[i] * t1[i];
        }


		/// <summary>
        /// result = m0 + (m1 - m2) * t
        /// </summary>
        public static void AddScaledDelta(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, ReadOnlyArrayView<Vector3d> m2, double t, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i] + (m1[i] - m2[i]) * t;
        }


		/// <summary>
        /// result = m0 + (m1 - m2) * t
		/// Note that t is assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void AddScaledDelta(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, ReadOnlyArrayView<Vector3d> m2, ReadOnlyArrayView<double> t, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i] + (m1[i] - m2[i]) * t[i];
        }


		/// <summary>
        ///
        /// </summary>
        public static void Lerp(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, double t, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i].LerpTo(m1[i], t);
        }


		/// <summary>
        /// result = m0 + (m1 - m0) * t
        /// Note that t is assumed to represent the diagonal elements of a square matrix.
        /// </summary>
        public static void LerpColumns(ReadOnlyArrayView<Vector3d> m0, ReadOnlyArrayView<Vector3d> m1, ReadOnlyArrayView<double> t, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < m0.Count; i++)
                result[i] = m0[i].LerpTo(m1[i], t[i]);
        }


		/// <summary>
        /// 
        /// </summary>
        public static void UnitizeColumns(ReadOnlyArrayView<Vector3d> matrix, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < matrix.Count; i++)
                result[i] = matrix[i].Unit;
        }


		/// <summary>
        ///
        /// </summary>
        public static void NormalizeColumns(ReadOnlyArrayView<Vector3d> matrix, Interval3d interval, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < matrix.Count; i++)
                result[i] = interval.Normalize(matrix[i]);
        }


		/// <summary>
        ///
        /// </summary>
        public static void EvaluateColumns(ReadOnlyArrayView<Vector3d> matrix, Interval3d interval, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < matrix.Count; i++)
                result[i] = interval.Evaluate(matrix[i]);
        }


		/// <summary>
        ///
        /// </summary>
        public static void RemapColumns(ReadOnlyArrayView<Vector3d> matrix, Interval3d from, Interval3d to, ArrayView<Vector3d> result)
        {
            for (int i = 0; i < matrix.Count; i++)
                result[i] = Interval3d.Remap(matrix[i], from, to);
        }

		#endregion

	}
}