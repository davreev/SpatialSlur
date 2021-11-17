/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.Collections;

using static System.Threading.Tasks.Parallel;
using static SpatialSlur.Collections.DynamicArray;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// Based on energy described in http://www.cs.cmu.edu/~kmcrane/Projects/DiscreteDevelopable/paper.pdf
    /// </summary>
    [Serializable]
    public class Developablize : Impl.PositionConstraint
    {
        #region Nested types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public struct Element
        {
            /// <summary>
            /// 
            /// </summary>
            public static Element Default = new Element()
            {
                First = -1,
                Count = 0,
                Weight = 1.0
            };

            /// <summary>Index of the first particle used by this element</summary>
            public int First;

            /// <summary>Number of particles used by this element</summary>
            public int Count;

            /// <summary>Relative influence of this element</summary>
            public double Weight;
        }

        #endregion

        private DynamicArray<Element> _elements = new DynamicArray<Element>();


        /// <inheritdoc />
        public override void Calculate(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        {
            base.Calculate(positions, rotations);
            var elements = _elements;

            if (Parallel)
                ForEach(new UniformPartitioner(0, elements.Count), Calculate);
            else
                Calculate((0, elements.Count));

            void Calculate((int, int) range)
            {
                // TODO

            }
        }


#if false
        /// <inheritdoc />
        public void Calculate(ReadOnlyArrayView<Body> bodies)
        {
            int n = _indices.Count;

            // check if sufficient number of bodies
            if (n < 4 || !GeometryUtil.FitPlaneToPoints(GetFaceNormals(bodies), out Vec3d p, out Vec3d z))
            {
                _delta = Vec3d.Zero;
                _apply = false;
                return;
            }

            p = bodies[_indices[0]].Position.Current;
            var p0 = bodies[_indices[1]].Position.Current - p;
            var sum = Vec3d.Zero;

            // TODO update based on Debug~/Developablize_Constraint_0.gh

            for(int i = 2; i < n; i++)
            {
                var p1 = bodies[_indices[i]].Position.Current;
                sum += Vec3d.Project(p0 - p, Vec3d.Cross(z, p1 - p0)); // TODO handle potential divide by zero here
                p0 = p1;
            }

            _delta = sum / (n - 1);
            _apply = true;

            // TODO test distributing the reverse among neighbours
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        /// <returns></returns>
        private IEnumerable<Vec3d> GetFaceNormals(ReadOnlyArrayView<Body> bodies)
        {
            var p = bodies[_indices[0]].Position.Current;
            var d0 = bodies[_indices[1]].Position.Current - p;

            for (int i = 2; i < _indices.Count; i++)
            {
                var d1 = bodies[_indices[i]].Position.Current - p;
                yield return Vec3d.Cross(d1, d0);
                d0 = d1;
            }
        }
#endif
    }
}
