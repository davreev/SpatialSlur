using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurSketch
{
    public class TwistedBox
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
        public TwistedBox()
        {
            _corners = new Point3d[8];
        }


        /// <summary>
        /// 
        /// </summary>
        public TwistedBox(Point3d p0, Point3d p1, Point3d p2, Point3d p3, Point3d p4, Point3d p5, Point3d p6, Point3d p7)
            : this()
        {
            _corners[0] = p0;
            _corners[1] = p0;
            _corners[2] = p0;
            _corners[3] = p0;
            _corners[4] = p0;
            _corners[5] = p0;
            _corners[6] = p0;
            _corners[7] = p0;
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
            Point3d p0 = _corners[0].LerpTo(_corners[1], u);
            Point3d p1 = _corners[2].LerpTo(_corners[3], u);
            Point3d p2 = _corners[4].LerpTo(_corners[5], u);
            Point3d p3 = _corners[6].LerpTo(_corners[7], u);

            p0 = p0.LerpTo(p1, v);
            p2 = p2.LerpTo(p3, v);

            return p0.LerpTo(p2, w);
        }
    }
}
