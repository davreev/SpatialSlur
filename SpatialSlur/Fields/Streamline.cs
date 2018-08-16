
/*
 * Notes
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    public static class Streamline
    {
        /// <summary>
        /// Returns a streamline through the given vector field starting at the given point.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="point"></param>
        /// <param name="stepSize"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static IEnumerable<Vector2d> IntegrateFrom(IField2d<Vector2d> field, Vector2d point, double stepSize, IntegrationMode mode = IntegrationMode.Euler)
        {
            switch (mode)
            {
                case IntegrationMode.Euler:
                    return IntegrateFromEuler(field, point, stepSize);
                case IntegrationMode.RK2:
                    return IntegrateFromRK2(field, point, stepSize);
                case IntegrationMode.RK4:
                    return IntegrateFromRK4(field, point, stepSize);
            }

            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        private static IEnumerable<Vector2d> IntegrateFromEuler(IField2d<Vector2d> field, Vector2d point, double stepSize)
        {
            while (true)
            {
                point += field.ValueAt(point) * stepSize;
                yield return point;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static IEnumerable<Vector2d> IntegrateFromRK2(IField2d<Vector2d> field, Vector2d point, double stepSize)
        {
            while (true)
            {
                var v0 = field.ValueAt(point);
                var v1 = field.ValueAt(point + v0 * stepSize);

                point += (v0 + v1) * 0.5 * stepSize;
                yield return point;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static IEnumerable<Vector2d> IntegrateFromRK4(IField2d<Vector2d> field, Vector2d point, double stepSize)
        {
            double dt2 = stepSize * 0.5;
            double dt6 = stepSize / 6.0;

            while (true)
            {
                var v0 = field.ValueAt(point);
                var v1 = field.ValueAt(point + v0 * dt2);
                var v2 = field.ValueAt(point + v1 * dt2);
                var v3 = field.ValueAt(point + v2 * stepSize);

                point += (v0 + 2.0 * v1 + 2.0 * v2 + v3) * dt6;
                yield return point;
            }
        }


        /// <summary>
        /// Returns a streamline through the given vector field starting at the given point.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="point"></param>
        /// <param name="stepSize"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static IEnumerable<Vector3d> IntegrateFrom(IField3d<Vector3d> field, Vector3d point, double stepSize, IntegrationMode mode = IntegrationMode.Euler)
        {
            switch (mode)
            {
                case IntegrationMode.Euler:
                    return IntegrateFromEuler(field, point, stepSize);
                case IntegrationMode.RK2:
                    return IntegrateFromRK2(field, point, stepSize);
                case IntegrationMode.RK4:
                    return IntegrateFromRK4(field, point, stepSize);
            }

            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        private static IEnumerable<Vector3d> IntegrateFromEuler(IField3d<Vector3d> field, Vector3d point, double stepSize)
        {
            yield return point;

            while (true)
            {
                point += field.ValueAt(point) * stepSize;
                yield return point;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static IEnumerable<Vector3d> IntegrateFromRK2(IField3d<Vector3d> field, Vector3d point, double stepSize)
        {
            yield return point;

            while (true)
            {
                var v0 = field.ValueAt(point);
                var v1 = field.ValueAt(point + v0 * stepSize);

                point += (v0 + v1) * 0.5 * stepSize;
                yield return point;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static IEnumerable<Vector3d> IntegrateFromRK4(IField3d<Vector3d> field, Vector3d point, double stepSize)
        {
            double dt2 = stepSize * 0.5;
            double dt6 = stepSize / 6.0;

            yield return point;

            while (true)
            {
                var v0 = field.ValueAt(point);
                var v1 = field.ValueAt(point + v0 * dt2);
                var v2 = field.ValueAt(point + v1 * dt2);
                var v3 = field.ValueAt(point + v2 * stepSize);

                point += (v0 + 2.0 * v1 + 2.0 * v2 + v3) * dt6;
                yield return point;
            }
        }
    }
}
