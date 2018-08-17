
/*
 * Notes
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur
{
    /// <summary>
    /// 
    /// </summary>
    public struct Plane3d
    {
        #region Static Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        private static Plane3d CreateXY(double distance)
        {
            return new Plane3d()
            {
                _normal = Vector3d.UnitZ,
                _distance = distance
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        private static Plane3d CreateYZ(double distance)
        {
            return new Plane3d()
            {
                _normal = Vector3d.UnitX,
                _distance = distance
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        private static Plane3d CreateZX(double distance)
        {
            return new Plane3d()
            {
                _normal = Vector3d.UnitY,
                _distance = distance
            };
        }

        #endregion


        private Vector3d _normal;
        private double _distance;


        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="distance"></param>
        public Plane3d(Vector3d normal, double distance)
            : this()
        {
            Normal = normal;
            _distance = distance;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="point"></param>
        public Plane3d(Vector3d normal, Vector3d point)
            : this()
        {
            Normal = normal;
            MakePassThrough(point);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public Plane3d(Vector3d p0, Vector3d p1, Vector3d p2)
            : this()
        {
            Set(p0, p1, p2);
        }

        #endregion


        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Vector3d Normal
        {
            get { return _normal; }
            set
            {
                if (value.Unitize())
                    _normal = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }


        /// <summary>
        /// Returns true if this plane has a non-zero normal.
        /// </summary>
        public bool IsValid
        {
            get { return _normal.SquareLength > 0.0; }
        }

        #endregion


        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public bool Set(Vector3d p0, Vector3d p1, Vector3d p2)
        {
            var n = Vector3d.Cross(p1 - p0, p2 - p1);

            if (!n.Unitize())
                return false;

            _normal = n;
            _distance = Vector3d.Dot(p0, n);

            return true;
        }

        
        /// <summary>
        /// Sets the distance of this plane such that it passes through the given point.
        /// </summary>
        /// <param name="point"></param>
        public void MakePassThrough(Vector3d point)
        {
            _distance = Vector3d.Dot(point, _normal);
        }


        /// <summary>
        /// Returns the closest point on this plane to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector3d ClosestPoint(Vector3d point)
        {
            return point - DistanceTo(point) * _normal;
        }


        /// <summary>
        /// Returns the signed distance from this plane to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double DistanceTo(Vector3d point)
        {
            return Vector3d.Dot(point, _normal) - _distance;
        }


        /// <summary>
        /// Returns the projection of the given point along the given direction onto this plane.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Vector3d ProjectTo(Vector3d point, Vector3d direction)
        {
            return point - direction * (DistanceTo(point) / Vector3d.Dot(direction, _normal));
        }

        #endregion
    }
}
