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
    public class TwistedQuad
    {
        private readonly Point3d[] _corners;

        /// <summary>
        /// 
        /// </summary>
        public Point3d[] Corners
        {
            get { return _corners; }
        }


        /// <summary>
        /// 
        /// </summary>
        public TwistedQuad()
        {
            _corners = new Point3d[4];
        }


        /// <summary>
        /// 
        /// </summary>
        public TwistedQuad(Point3d p0, Point3d p1, Point3d p2, Point3d p3)
            : this()
        {
            _corners[0] = p0;
            _corners[1] = p1;
            _corners[2] = p2;
            _corners[3] = p3;
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
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public Point3d Evaluate(double u, double v)
        {
            Point3d p0 = _corners[0].LerpTo(_corners[1], u);
            Point3d p1 = _corners[2].LerpTo(_corners[3], u);
            return p0.LerpTo(p1, v);
        }
    }
}
