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
    public class SphereTests
    {
        [TestMethod]
        public void IntersectingSphereAndRayFromFrontWillReturnValidIntersection()
        {
            Sphere s = new Sphere(0,new Vector3(0, 0, 0), Color.Black, 1f);

            Assert.AreEqual<String>((new Intersection(new Vector3(0, 0, -1f), new Vector3(0, 0, -1), 100f, s)).ToString(), s.Intersect(new RealtimeRaytrace.Ray(new Vector3(0, 0, -101), new Vector3(0, 0, 1))).ToString() );
        }

        [TestMethod]
        public void IntersectingSphereAndRayFromLeftWillReturnValidIntersection()
        {
            Sphere s = new Sphere(0,new Vector3(0, 0, 0), Color.Black,1f);

            Assert.AreEqual<String>((new Intersection(new Vector3(-1, 0, 0), new Vector3(-1, 0, 0), 9, s)).ToString(), s.Intersect(new RealtimeRaytrace.Ray(new Vector3(-10, 0, 0), new Vector3(1, 0, 0))).ToString());
        }

        [TestMethod]
        public void NotIntersectingSphereAndRayWillReturnNull()
        {
            Sphere s = new Sphere(0,new Vector3(10, 0, 0), Color.Black);

            Assert.AreEqual<Intersection>(null, s.Intersect(new RealtimeRaytrace.Ray(new Vector3(0, 0, -101), new Vector3(0, 0, 1))));
        }

        [TestMethod]
        public void IntersectingSphereAndRayFromDiagonalWillReturnValidIntersection()
        {
            Sphere s = new Sphere(0, new Vector3(0, 0, 0), Color.Black);

            Assert.AreEqual<String>((new Intersection(new Vector3(0.2886553f, 0.2886553f, 0.2886553f), Vector3.Normalize(new Vector3(1, 1, 1)), 16.82054f, s)).ToString(), s.Intersect(new RealtimeRaytrace.Ray(new Vector3(10, 10, 10), new Vector3(-1, -1, -1))).ToString());
        }

        [TestMethod]
        public void IntersectingSphereAndRayFromDiagonalNegativeToPositiveWillReturnValidIntersection()
        {
            Sphere s = new Sphere(0, new Vector3(-5, -5, -5), Color.Black);

            Assert.AreEqual<String>((new Intersection(new Vector3(-4.711345f, -4.711345f, -4.711345f), Vector3.Normalize(new Vector3(1, 1, 1)), 16.82054f, s)).ToString(), s.Intersect(new RealtimeRaytrace.Ray(new Vector3(5, 5, 5), new Vector3(-1, -1, -1))).ToString());
        }

    }
}
