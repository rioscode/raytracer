﻿using OpenTK.Mathematics;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.AccelerationStructures.BVH {
    /// <summary> A possible split for a BVHNode </summary>
    public class Split {
        /// <summary> Along which direction the split is </summary>
        public Vector3 Direction { get; }
        /// <summary> The left AABB of the split </summary>
        public AABB Left { get; }
        /// <summary> The right AABB of the split </summary>
        public AABB Right { get; }

        /// <summary> The Surface Area Heuristic of the split </summary>
        public float SurfaceAreaHeuristic => Left.SurfaceAreaHeuristic + Right.SurfaceAreaHeuristic;

        /// <summary> Create an empty split </summary>
        public Split() {
            Direction = Vector3.Zero;
            Left = new AABB();
            Right = new AABB();
        }

        /// <summary> Create a new split with AABB's </summary>
        /// <param name="left">The left AABB</param>
        /// <param name="right">The right AABB</param>
        public Split(Vector3 direction, AABB left, AABB right) {
            Direction = direction;
            Left = left;
            Right = right;
        }

        /// <summary> Create a new split with primitives </summary>
        /// <param name="primitivesLeft">The primitives for the left AABB</param>
        /// <param name="primitivesRight">The primitives for the right AABB</param>
        public Split(Vector3 direction, IEnumerable<IAABB> primitivesLeft, IEnumerable<IAABB> primitivesRight) {
            Direction = direction;
            Left = new AABB(primitivesLeft);
            Right = new AABB(primitivesRight);
        }
    }
}
