using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurSketch
{
    //
    public class PointGrid
    {
        private readonly Point3d[] _points;
        private readonly int _nx, _ny;


        /// <summary>
        /// 
        /// </summary>
        public Point3d[] Points
        {
            get { return _points; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return _points.Length; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int CountX
        {
            get { return _nx; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int CountY
        {
            get { return _ny; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        public PointGrid(int nx, int ny)
        {
            if (nx < 2 || ny < 2)
                throw new System.ArgumentException("the grid must have at least 2 points in each dimension");

            _nx = nx;
            _ny = ny;
            _points = new Point3d[_nx * _ny];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public void Set(IList<Point3d> points)
        {
            if (_points.Length != points.Count)
                throw new System.ArgumentException("the number of points provided does not match the grid dimensions");

            for (int i = 0; i < _points.Length; i++)
                _points[i] = points[i];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public Point3d PointAt(int i, int j)
        {
            if (j < 0 || j >= _nx)
                throw new System.ArgumentException("j is out of range");

            if (i < 0 || i >= _ny)
                throw new System.ArgumentException("i is out of range");

            return _points[j + i * _nx];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Point3d Evaluate(Point3d point)
        {
            return Evaluate(point.X, point.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Point3d Evaluate(Point2d point)
        {
            return Evaluate(point.X, point.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public Point3d Evaluate(double u, double v)
        {
            u *= _nx - 1;
            v *= _ny - 1;

            // get whole component
            int j = (int)Math.Floor(u);
            int i = (int)Math.Floor(v);

            // constrain whole numbers to valid range
            j = SlurMath.Clamp(j, 0, _nx - 2);
            i = SlurMath.Clamp(i, 0, _ny - 2);
  
            // get fractional component
            u -= j;
            v -= i;

            int index = j + i * _nx;
            Point3d p0 = _points[index];
            Point3d p1 = _points[index + 1];
            Point3d p2 = _points[index + _nx];
            Point3d p3 = _points[index + _nx + 1];

            p0 = p0.LerpTo(p1, u);
            p2 = p2.LerpTo(p3, u);
            return p0.LerpTo(p2, v);
        }
    }
}
