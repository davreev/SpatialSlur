using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace SpatialSlur.SlurGH
{
    /// <summary>
    /// 
    /// </summary>
    public class SlurGHInfo : GH_AssemblyInfo
    {
        /// <inheritdoc />
        public override string Name
        {
            get { return "SpatialSlur.SlurGH"; }
        }


        /// <inheritdoc />
        public override Bitmap Icon
        {
            get { return null; }
        }


        /// <inheritdoc />
        public override string Description
        {
            get { return ""; }
        }


        /// <inheritdoc />
        public override Guid Id
        {
            get { return new Guid("8546948e-7b1e-4e42-beb9-a924be0b7964"); }
        }


        /// <inheritdoc />
        public override string AuthorName
        {
            get { return "David Reeves"; }
        }


        /// <inheritdoc />
        public override string AuthorContact
        {
            get { return "http://spatialslur.com/"; }
        }
    }
}
