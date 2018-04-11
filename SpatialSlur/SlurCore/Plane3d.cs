
/*
 * Notes
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    public struct Plane3d
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        private static Plane3d CreateXY(double distance)
        {
            return new Plane3d()
            {
                _normal = Vec3d.UnitZ,
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
                _normal = Vec3d.UnitX,
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
                _normal = Vec3d.UnitY,
                _distance = distance
            };
        }

        #endregion


        private Vec3d _normal;
        private double _distance;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="distance"></param>
        public Plane3d(Vec3d normal, double distance)
            : this()
        {
            Normal = normal;
            _distance = distance;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public Plane3d(Vec3d normal, Vec3d point)
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
        public Plane3d(Vec3d p0, Vec3d p1, Vec3d p2)
            : this()
        {
            Set(p0, p1, p2);
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Normal
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
        /// Returns true if this plane has been successfully initialized.
        /// </summary>
        public bool IsValid
        {
            get { return _normal.SquareLength > 0.0; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public bool Set(Vec3d p0, Vec3d p1, Vec3d p2)
        {
            var n = Vec3d.Cross(p1 - p0, p2 - p1);

            if (!n.Unitize())
                return false;

            _normal = n;
            _distance = Vec3d.Dot(p0, n);

            return true;
        }

        
        /// <summary>
        /// Sets the distance of this plane such that it passes through the given point.
        /// </summary>
        /// <param name="point"></param>
        public void MakePassThrough(Vec3d point)
        {
            _distance = Vec3d.Dot(point, _normal);
        }


        /// <summary>
        /// Returns the closest point on this plane to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d ClosestPoint(Vec3d point)
        {
            return point - DistanceTo(point) * _normal;
        }


        /// <summary>
        /// Returns the signed distance from this plane to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double DistanceTo(Vec3d point)
        {
            return Vec3d.Dot(point, _normal) - _distance;
        }


        /// <summary>
        /// Returns the projection of the given point along the given direction onto this plane.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Vec3d ProjectTo(Vec3d point, Vec3d direction)
        {
            return point - direction * (DistanceTo(point) / Vec3d.Dot(direction, _normal));
        }
    }
}
