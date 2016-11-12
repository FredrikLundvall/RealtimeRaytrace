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
    public class CalculateTests
    {
        [TestMethod]
        public void CoordinateHashOfDifferentTypesWillReturnSameValue()
        {
            Assert.AreEqual<int>(Calculate.CoordinateHash(1, 1, 1), Calculate.CoordinateHash(new Vector3(1, 1, 1)));
            Assert.AreEqual<int>(Calculate.CoordinateHash(1, 1, 1), Calculate.CoordinateHash(new IntVector(1, 1, 1)));
            Assert.AreEqual<int>(Calculate.CoordinateHash(new Vector3(1, 1, 1)), Calculate.CoordinateHash(new IntVector(1, 1, 1)));
        }

        [TestMethod]
        public void CoordinateHashWithNegativeValuesWillReturnDifferentValue()
        {
            Assert.AreNotEqual<int>(Calculate.CoordinateHash(1, 1, 1), Calculate.CoordinateHash(1, 1, -1));
            Assert.AreNotEqual<int>(Calculate.CoordinateHash(1, 1, 1), Calculate.CoordinateHash(1, -1, 1));
            Assert.AreNotEqual<int>(Calculate.CoordinateHash(1, 1, 1), Calculate.CoordinateHash(-1, 1, 1));
        }

    }
}
