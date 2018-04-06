using System;
using System.IO;
using System.Text;

using SpatialSlur.SlurCore;

/*
 * Notes 
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public static class GridField3dFactoryExtension
    {
        #region Vec3d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static GridField3d<Vec3d> CreateFromFGA(this GridField3dFactory<Vec3d> factory, string path)
        {
            var content = File.ReadAllText(path, Encoding.ASCII);
            var values = content.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            int nx = int.Parse(values[0]);
            int ny = int.Parse(values[1]);
            int nz = int.Parse(values[2]);

            Vec3d p0 = new Vec3d(
                double.Parse(values[3]),
                double.Parse(values[4]),
                double.Parse(values[5]));

            Vec3d p1 = new Vec3d(
               double.Parse(values[6]),
               double.Parse(values[7]),
               double.Parse(values[8]));

            var result = factory.Create(nx, ny, nz, new Interval3d(p0, p1));
            var vecs = result.Values;
            int index = 0;

            for (int i = 9; i < values.Length; i += 3)
            {
                vecs[index++] = new Vec3d(
                    double.Parse(values[i]),
                    double.Parse(values[i + 1]),
                    double.Parse(values[i + 2]));
            }

            return result;
        }

        #endregion
    }
}
