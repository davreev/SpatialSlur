using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using SpatialSlur.SlurCore;


namespace SpatialSlur.SlurSketch
{
    public class PointLattice
    {
        private readonly Point3d[] _points;
        private readonly int _nx, _ny, _nz, _nxy;


        /// <summary>
        /// 
        /// </summary>
        public IList<Point3d> Points
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
        public int CountZ
        {
            get { return _nz; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int CountXY
        {
            get { return _nxy; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        public PointLattice(int nx, int ny, int nz)
        {
            if (nx < 2 || ny < 2 || nz < 2)
                throw new System.ArgumentException("the lattice must have at least 2 points in each dimension");

            _nx = nx;
            _ny = ny;
            _nz = nz;
            _nxy = _nx * _ny;
            _points = new Point3d[_nxy * _nz];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public Point3d PointAt(int i, int j, int k)
        {
            if (k < 0 || k >= _nx || j < 0 || j >= _ny || i < 0 || i >= _nz)
                throw new System.IndexOutOfRangeException();

            return _points[k + j * _nx + i * _nxy];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Point3d Evaluate(Point3d point)
        {
            return Evaluate(point.X, point.Y, point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public Point3d Evaluate(double u, double v, double w)
        {
            u *= _nx - 1; 
            v *= _ny - 1;
            w *= _nz - 1;

            // get whole component
            int k = (int)Math.Floor(u);
            int j = (int)Math.Floor(v);
            int i = (int)Math.Floor(w);

            // constrain whole numbers to valid range
            k = SlurMath.Clamp(k, 0, _nx - 2);
            j = SlurMath.Clamp(j, 0, _ny - 2);
            i = SlurMath.Clamp(i, 0, _nz - 2);
  
            // get fractional component
            u -= k;
            v -= j;
            w -= i;

            int index = k + j * _nx + i * _nxy;
            Point3d p0 = _points[index];
            Point3d p1 = _points[index + 1];
            Point3d p2 = _points[index + _nx];
            Point3d p3 = _points[index + _nx + 1];
            Point3d p4 = _points[index + _nxy];
            Point3d p5 = _points[index + 1 + _nxy];
            Point3d p6 = _points[index + _nx + _nxy];
            Point3d p7 = _points[index + _nx + 1 + _nxy];

            p0 = p0.LerpTo(p1, u);
            p2 = p2.LerpTo(p3, u);
            p4 = p4.LerpTo(p5, u);
            p6 = p6.LerpTo(p7, u);

            p0 = p0.LerpTo(p2, v);
            p4 = p4.LerpTo(p6, v);
            return p0.LerpTo(p4, w);
        }
    }
}
