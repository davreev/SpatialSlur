/*
 * Notes
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.Dynamics;
using SpatialSlur.Collections;


namespace SpatialSlur
{
    using Constd = SlurMath.Constantsd;

    /// <summary>
    /// 
    /// </summary>
    public static partial class Geometry
    {
        /// <summary>
        /// Returns true if a unique plane was found i.e. the given points are not coincident or colinear.
        /// </summary>
        public static bool FitPlane(
            ArrayView<Particle> particles,
            ArrayView<ParticlePosition> positions,
            out Vector3d origin,
            out Vector3d normal,
            double epsilon = Constd.ZeroTolerance)
        {
            // impl refs
            // https://www.geometrictools.com/Documentation/LeastSquaresFitting.pdf
            // http://www.ilikebigbits.com/blog/2017/9/24/fitting-a-plane-to-noisy-points-in-3d

            origin = GetMassCenter(particles, positions);
            Matrix3d covm = CreateCovariance(particles, positions, origin);
            Matrix3d.Decompose.EigenSymmetric(covm, out Matrix3d vecs, out Vector3d vals, epsilon);
            normal = vecs.Column2;

            // If 2nd eigenvalue is 0, then no unique solution (i.e. points are colinear at best)
            return Math.Abs(vals.Y) >= epsilon;
        }


        /// <summary>
        /// Returns true if a unique line was found i.e. the given points are not coincident.
        /// </summary>
        public static bool FitLine(
            ArrayView<Particle> particles,
            ArrayView<ParticlePosition> positions,
            out Vector3d start,
            out Vector3d direction,
            double epsilon = Constd.ZeroTolerance)
        {
            // Impl refs
            // https://www.geometrictools.com/Documentation/LeastSquaresFitting.pdf
            // http://www.ilikebigbits.com/blog/2017/9/24/fitting-a-plane-to-noisy-points-in-3d
            
            Matrix3d covm = CreateCovariance(particles, positions, out start);
            Matrix3d.Decompose.EigenSymmetric(covm, out Matrix3d vecs, out Vector3d vals, epsilon);
            direction = vecs.Column0;

            // If 1st eigenvalue is 0, then no unique solution (i.e. points are coincident)
            return Math.Abs(vals.X) >= epsilon;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool FitSphere(
            ArrayView<Particle> particles,
            ArrayView<ParticlePosition> positions,
            out Vector3d origin,
            out double radius,
            double epsilon = Constd.ZeroTolerance)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool FitCircle(
            ArrayView<Particle> particles,
            ArrayView<ParticlePosition> positions,
            out Vector3d origin,
            out Vector3d normal,
            out double radius,
            double epsilon = Constd.ZeroTolerance)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vector3d GetMassCenter(
            ArrayView<Particle> particles,
            ArrayView<ParticlePosition> positions)
        {
            var sum = Vector3d.Zero;
            double weightSum = 0.0;

            for (int i = 0; i < particles.Count; i++)
            {
                ref var p = ref positions[particles[i].PositionIndex];
                var w = 1.0 / p.InverseMass;
                sum += p.Current * w;
                weightSum += w;
            }

            return sum / weightSum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Matrix3d CreateCovariance(
            ArrayView<Particle> particles,
            ArrayView<ParticlePosition> positions,
            out Vector3d massCenter)
        {
            massCenter = GetMassCenter(particles, positions);
            return CreateCovariance(particles, positions, GetMassCenter(particles, positions));
        }


        /// <summary>
        /// 
        /// </summary>
        public static Matrix3d CreateCovariance(
            ArrayView<Particle> particles,
            ArrayView<ParticlePosition> positions,
            Vector3d massCenter)
        {
            var result = new Matrix3d();
            double weightSum = 0.0;

            for (int i = 0; i < particles.Count; i++)
            {
                ref var p = ref positions[particles[i].PositionIndex];
                var d = p.Current - massCenter;
                var w = 1.0 / p.InverseMass;

                result.M00 += d.X * d.X * w;
                result.M01 += d.X * d.Y * w;
                result.M02 += d.X * d.Z * w;
                result.M11 += d.Y * d.Y * w;
                result.M12 += d.Y * d.Z * w;
                result.M22 += d.Z * d.Z * w;

                weightSum += w;
            }

            var t = 1.0 / weightSum;

            result.M00 *= t;
            result.M11 *= t;
            result.M22 *= t;
            result.M10 = result.M01 *= t;
            result.M20 = result.M02 *= t;
            result.M21 = result.M12 *= t;

            return result;
        }
    }
}
