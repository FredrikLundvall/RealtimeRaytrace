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
    public class BoundingBoxTests
    {
        [TestMethod]
        public void IntersectingBoundingBoxAndRayFromFrontWillReturnValidIntersection()
        {
            RealtimeRaytrace.BoundingBox b = new RealtimeRaytrace.BoundingBox(new Vector3(-10, -10, -10), new Vector3(10, 10, 10));

            Assert.AreEqual<String>(
                new BoundingBoxIntersection(100f).ToString(),
                b.Intersect(new RealtimeRaytrace.Ray(new Vector3(0, 0, 110), new Vector3(0, 0, -1))).ToString());
        }
    }
}
