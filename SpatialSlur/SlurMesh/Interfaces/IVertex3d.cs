using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public interface IVertex3d
    {
        /// <summary></summary>
        Vec3d Position { get; set; }

        /// <summary></summary>
        Vec3d Normal { get; set; }

        /// <summary></summary>
        Vec2d Texture { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public static class IVertex3dStatic<V>
        where V : IVertex3d
    {
        public static readonly Func<V, Vec3d> GetPosition = v => v.Position;
        public static readonly Action<V, Vec3d> SetPosition = (v, p) => v.Position = p;

        public static readonly Func<V, Vec3d> GetNormal = v => v.Normal;
        public static readonly Action<V, Vec3d> SetNormal = (v, n) => v.Normal = n;

        public static readonly Func<V, Vec2d> GetTexture = v => v.Texture;
        public static readonly Action<V, Vec2d> SetTexture = (v, t) => v.Texture = t;
    }
}
