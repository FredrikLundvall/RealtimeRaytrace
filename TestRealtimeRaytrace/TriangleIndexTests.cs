using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using RealtimeRaytrace;

namespace TestRealtimeRaytrace
{
    [TestClass]
    public class TriangleIndexTests
    {
        private Triangle CreateTriangleWithDuplicateVertices()
        {
            return new Triangle(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f));
        }
        private Triangle CreateTriangle()
        {
            return new Triangle(new Vector3(10f, 200f, 3000f), new Vector3(-20f, -300f, -4000f), new Vector3(-30f, -400f, -5000f));
        }
        private Triangle CreateOtherTriangle()
        {
            return new Triangle(new Vector3(-10f, -200f, -3000f), new Vector3(20f, 300f, 4000f), new Vector3(30f, 400f, 5000f));
        }
        private Triangle CreateTriangleOutsideBoundaries()
        {
            return new Triangle(new Vector3(10000f, 10000f, 10000f), new Vector3(20000f, 20000f, 20000f), new Vector3(30000f, 30000f, 30000f));
        }
        [TestMethod]
        public void AddingOneTriangleAddsThreeVerticesAndThreeIndices()
        {
            TriangleIndex ti;

            ti = new TriangleIndex(-1000f, -1000f, 1000f, 1000f);
            ti.AddTriangle(CreateTriangle());
            Assert.AreEqual<int>(3, ti.GetIndices().Length);
            Assert.AreEqual<int>(3, ti.GetVerticesPositionColor().Length);
        }

        [TestMethod]
        public void AddingOneTriangleWithDuplicateVerticesAddsOneVerticeAndThreeIndices()
        {
            TriangleIndex ti;
            ti = new TriangleIndex(-1000f, -1000f, 1000f, 1000f);
            ti.AddTriangle(CreateTriangleWithDuplicateVertices());
            Assert.AreEqual<int>(3, ti.GetIndices().Length);
            Assert.AreEqual<int>(1, ti.GetVerticesPositionColor().Length);
        }

        [TestMethod]
        public void AddingTwoDifferentTrianglesAddsSixVerticesAndSixIndices()
        {
            TriangleIndex ti;

            ti = new TriangleIndex(-1000f, -1000f, 1000f, 1000f);
            ti.AddTriangle(CreateTriangle());
            ti.AddTriangle(CreateOtherTriangle());
            Assert.AreEqual<int>(6, ti.GetIndices().Length);
            Assert.AreEqual<int>(6, ti.GetVerticesPositionColor().Length);
        }

        [TestMethod]
        public void AddingTwoDuplicateTrianglesAddsThreeVerticesAndThreeIndices()
        {
            TriangleIndex ti;
            ti = new TriangleIndex(-1000f, -1000f, 1000f, 1000f);
            ti.AddTriangle(CreateTriangle());
            ti.AddTriangle(CreateTriangle());
            Assert.AreEqual<int>(3, ti.GetIndices().Length);
            Assert.AreEqual<int>(3, ti.GetVerticesPositionColor().Length);
        }

        [TestMethod]
        public void AddingTwoTrianglesWithSameVerticesInDifferentOrderAddsThreeVerticesAndThreeIndices()
        {
            TriangleIndex ti;
            ti = new TriangleIndex(-1000f, -1000f, 1000f, 1000f);
            Triangle t = CreateTriangle();
            ti.AddTriangle(t);
            ti.AddTriangle(new Triangle( t.GetP2(), t.GetP3(), t.GetP1() ));
            Assert.AreEqual<int>(3, ti.GetIndices().Length);
            Assert.AreEqual<int>(3, ti.GetVerticesPositionColor().Length);
        }

        [TestMethod]
        public void AddingOneTriangleOutsideBounderiesAddsNoVerticesAndNoIndices()
        {
            TriangleIndex ti;
           ti = new TriangleIndex(-1000f, -1000f, 1000f, 1000f);
            ti.AddTriangle(CreateTriangleOutsideBoundaries());
            Assert.AreEqual<int>(0, ti.GetIndices().Length);
            Assert.AreEqual<int>(0, ti.GetVerticesPositionColor().Length);
        }
    }
}
