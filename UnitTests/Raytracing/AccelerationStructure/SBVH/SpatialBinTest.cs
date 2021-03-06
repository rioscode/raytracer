﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK.Mathematics;
using PathTracer.Pathtracing.AccelerationStructures.SBVH;
using PathTracer.Utilities;

namespace UnitTests.Raytracing.AccelerationStructure.SBVH {
    [TestClass]
    public class SpatialBinTest {
        [TestMethod]
        public void Constructor() {
            SpatialBin bin = new SpatialBin(Vector3.UnitX, 0f, 1f);
            Assert.IsTrue(bin != null);
        }

        [TestMethod]
        public void SplitPlanes() {
            for (int i = 0; i < 100; i++) {
                Vector3 direction = Utils.Random.UnitVector();
                float start = (float)Utils.Random.NextDouble();
                float end = (float)Utils.Random.NextDouble();
                SpatialBin bin = new SpatialBin(direction, start, end);
                Assert.AreEqual(direction * start, bin.SplitPlaneLeft.Position);
                Assert.AreEqual(direction, bin.SplitPlaneLeft.Normal);
                Assert.AreEqual(direction * end, bin.SplitPlaneRight.Position);
                Assert.AreEqual(-direction, bin.SplitPlaneRight.Normal);
            }
        }

        [TestMethod]
        public void BoundingBox() {
            Vector3 direction = Vector3.UnitX;
            float start = -1f;
            float end = 1f;
            SpatialBin bin = new SpatialBin(direction, start, end);
            for (int i = 0; i < 1000; i++) {
                bin.AABB.Add(Utils.Random.Sphere(0f, 1f));
                bin.ClipAndAdd(Utils.Random.Primitive(1f, 3f));
            }
            Vector3[] bounds = bin.AABB.Bounds;
            Assert.AreEqual(-1f, bounds[0].X);
            Assert.AreEqual(1f, bounds[1].X);
        }
    }
}
