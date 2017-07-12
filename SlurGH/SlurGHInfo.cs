using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace SpatialSlur.SlurGH
{
    public class SlurGHInfo : GH_AssemblyInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public override string Name
        {
            get { return "SpatialSlur.SlurGH"; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override Bitmap Icon
        {
            get { return null; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override string Description
        {
            get { return ""; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override Guid Id
        {
            get { return new Guid("8546948e-7b1e-4e42-beb9-a924be0b7964"); }
        }


        /// <summary>
        /// 
        /// </summary>
        public override string AuthorName
        {
            get { return "David Reeves"; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override string AuthorContact
        {
            get { return "http://spatialslur.com/"; }
        }
    }
}
