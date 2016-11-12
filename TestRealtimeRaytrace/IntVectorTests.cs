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
    class IntVectorTests
    {
        [TestMethod]
        public void CastFromVector3WillRoundAwayFromZero()
        {
            Assert.AreEqual<IntVector>(new IntVector(1, 2, 3), (IntVector)new Vector3(0.9f, 1.5f, 3.4f));
            Assert.AreEqual<IntVector>(new IntVector(-1, -2, -3), (IntVector)new Vector3(-0.9f, -1.5f, -3.4f));
        }
    }
}
