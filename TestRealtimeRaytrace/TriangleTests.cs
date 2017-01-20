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
    public class TriangleTests
    {
        private Triangle CreateTriangle()
        {
            return new Triangle(new Vector3(10f, 200f, 3000f), new Vector3(-20f, -300f, -4000f), new Vector3(-30f, -400f, -5000f));
        }
        private Triangle CreateAllPositiveVectorTriangle()
        {
            return new Triangle(new Vector3(10f, 200f, 3000f), new Vector3(20f, 300f, 4000f), new Vector3(30f, 400f, 5000f));
        }
        private Triangle CreateAllNegativeVectorTriangle()
        {
            return new Triangle(new Vector3(-10f, -200f, -3000f), new Vector3(-20f, -300f, -4000f), new Vector3(-30f, -400f, -5000f));
        }
        private Triangle CreateMixPositiveNegativeVector1Triangle()
        {
            return new Triangle(new Vector3(10f, 200f, 3000f), new Vector3(-20f, -300f, -4000f), new Vector3(-30f, -400f, -5000f));
        }
        private Triangle CreateMixPositiveNegativeVector2Triangle()
        {
            return new Triangle(new Vector3(-10f, -200f, -3000f), new Vector3(20f, 300f, 4000f), new Vector3(-30f, -400f, -5000f));
        }
        private Triangle CreateMixPositiveNegativeVector3Triangle()
        {
            return new Triangle(new Vector3(-10f, -200f, -3000f), new Vector3(-20f, -300f, -4000f), new Vector3(30f, 400f, 5000f));
        }
        private Triangle CreateMixPositiveNegativeVector4Triangle()
        {
            return new Triangle(new Vector3(-10f, -200f, -3000f), new Vector3(20f, 300f, 4000f), new Vector3(30f, 400f, 5000f));
        }
        private Triangle CreateMixPositiveNegativeVector5Triangle()
        {
            return new Triangle(new Vector3(10f, 200f, 3000f), new Vector3(-20f, -300f, -4000f), new Vector3(30f, 400f, 5000f));
        }
        private Triangle CreateMixPositiveNegativeVector6Triangle()
        {
            return new Triangle(new Vector3(10f, 200f, 3000f), new Vector3(20f, 300f, 4000f), new Vector3(-30f, -400f, -5000f));
        }

        [TestMethod]
        public void GetSizeXOfDifferentTrianglesWillReturnTheRightSize()
        {
            Assert.AreEqual<float>(20, CreateAllPositiveVectorTriangle().GetSizeX(), "All positive vertices triangle");
            Assert.AreEqual<float>(20, CreateAllNegativeVectorTriangle().GetSizeX(), "All negative vertices triangle");
            Assert.AreEqual<float>(40, CreateMixPositiveNegativeVector1Triangle().GetSizeX(), "Mix negative and postive (1) vertices triangle");
            Assert.AreEqual<float>(50, CreateMixPositiveNegativeVector2Triangle().GetSizeX(), "Mix negative and postive (2) vertices triangle");
            Assert.AreEqual<float>(50, CreateMixPositiveNegativeVector3Triangle().GetSizeX(), "Mix negative and postive (3) vertices triangle");
            Assert.AreEqual<float>(40, CreateMixPositiveNegativeVector4Triangle().GetSizeX(), "Mix negative and postive (4) vertices triangle");
            Assert.AreEqual<float>(50, CreateMixPositiveNegativeVector5Triangle().GetSizeX(), "Mix negative and postive (5) vertices triangle");
            Assert.AreEqual<float>(50, CreateMixPositiveNegativeVector6Triangle().GetSizeX(), "Mix negative and postive (6) vertices triangle");
        }

        [TestMethod]
        public void GetSizeYDifferentTrianglesWillReturnTheRightSize()
        {
            Assert.AreEqual<float>(200, CreateAllPositiveVectorTriangle().GetSizeY(), "All positive vertices triangle");
            Assert.AreEqual<float>(200, CreateAllNegativeVectorTriangle().GetSizeY(), "All negative vertices triangle");
            Assert.AreEqual<float>(600, CreateMixPositiveNegativeVector1Triangle().GetSizeY(), "Mix negative and postive (1) vertices triangle");
            Assert.AreEqual<float>(700, CreateMixPositiveNegativeVector2Triangle().GetSizeY(), "Mix negative and postive (2) vertices triangle");
            Assert.AreEqual<float>(700, CreateMixPositiveNegativeVector3Triangle().GetSizeY(), "Mix negative and postive (3) vertices triangle");
            Assert.AreEqual<float>(600, CreateMixPositiveNegativeVector4Triangle().GetSizeY(), "Mix negative and postive (4) vertices triangle");
            Assert.AreEqual<float>(700, CreateMixPositiveNegativeVector5Triangle().GetSizeY(), "Mix negative and postive (5) vertices triangle");
            Assert.AreEqual<float>(700, CreateMixPositiveNegativeVector6Triangle().GetSizeY(), "Mix negative and postive (6) vertices triangle");
        }
        [TestMethod]
        public void EqualCheckOfTwoSameTrianglesWillBeEqual()
        {
            Assert.AreEqual<Triangle>(CreateTriangle(), CreateTriangle());
        }
        [TestMethod]
        public void EqualCheckOfTwoDifferentTrianglesWillNotBeEqual()
        {
            Triangle t = CreateTriangle();
            Assert.AreNotEqual<Triangle>(t, new Triangle(t.GetP1() + new Vector3(-1f, -1f, -1f), t.GetP2(), t.GetP3() + new Vector3(1f, 1f, 1f)));
        }
        [TestMethod]
        public void EqualCheckOfTwoTrianglesWithSameVerticesInDifferentOrderWillBeEqual()
        {
            Triangle t = CreateTriangle();
            Assert.AreEqual<Triangle>(t, new Triangle(t.GetP2(), t.GetP3(), t.GetP1()));
        }
        [TestMethod]
        public void FlippingHorizontalBaselineYOfPositiveVerticesTriangleWillSwapYBetweenTwoPoints()
        {
            Triangle triangle;
            triangle = new Triangle(new Vector3(10f, 200f, 3000f), new Vector3(20f, 300f, 4000f), new Vector3(30f, 200f, 5000f));
            triangle.FlipHorizontalBaselineY();
            Assert.AreEqual<Triangle>(new Triangle(new Vector3(10f, 300f, 3000f), new Vector3(20f, 200f, 4000f), new Vector3(30f, 300f, 5000f)), triangle);
        }
        [TestMethod]
        public void FlippingHorizontalBaselineYOfMixedPositiveAndNegativeVerticesTriangleWillSwapYBetweenTwoPoints()
        {
            Triangle triangle;
            triangle = new Triangle(new Vector3(-10f, -200f, -3000f), new Vector3(20f, 300f, 4000f), new Vector3(-30f, -200f, -5000f));
            triangle.FlipHorizontalBaselineY();
            Assert.AreEqual<Triangle>(new Triangle(new Vector3(-10f, 300f, -3000f), new Vector3(20f, -200f, 4000f), new Vector3(-30f, 300f, -5000f)), triangle);
        }
        [TestMethod]
        public void MovingTriangleNoDistanceWillKeepTheSameVertices()
        {
            Triangle triangle = CreateTriangle();
            triangle.Move(new Vector3(0f, 0f, 0f));
            Assert.AreEqual<Triangle>(CreateTriangle(), triangle);
        }
        [TestMethod]
        public void MovingTrianglePositiveDistanceWillMoveVerticesTheSameDistance()
        {
            Triangle triangle = CreateTriangle();
            Vector3 v = new Vector3(10f, 10f, 10f);
            Triangle triangleExpected = new Triangle(
                new Vector3(triangle.GetP1().X + v.X, triangle.GetP1().Y + v.Y, triangle.GetP1().Z + v.Z),
                new Vector3(triangle.GetP2().X + v.X, triangle.GetP2().Y + v.Y, triangle.GetP2().Z + v.Z),
                new Vector3(triangle.GetP3().X + v.X, triangle.GetP3().Y + v.Y, triangle.GetP3().Z + v.Z)
                );
            triangle.Move(v);
            Assert.AreEqual<Triangle>(triangleExpected, triangle);
        }
        [TestMethod]
        public void MovingTriangleNegativeDistanceWillMoveVerticesTheSameDistance()
        {
            Triangle triangle = CreateTriangle();
            Vector3 v = new Vector3(-10f, -10f, -10f);
            Triangle triangleExpected = new Triangle(
                new Vector3(triangle.GetP1().X + v.X, triangle.GetP1().Y + v.Y, triangle.GetP1().Z + v.Z),
                new Vector3(triangle.GetP2().X + v.X, triangle.GetP2().Y + v.Y, triangle.GetP2().Z + v.Z),
                new Vector3(triangle.GetP3().X + v.X, triangle.GetP3().Y + v.Y, triangle.GetP3().Z + v.Z)
                );
            triangle.Move(v);
            Assert.AreEqual<Triangle>(triangleExpected, triangle);
        }
    }
}
