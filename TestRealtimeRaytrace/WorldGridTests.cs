using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using RealtimeRaytrace;

namespace TestRealtimeRaytrace
{
    class WorldGridSpy : WorldGrid
    {
        public string logText = "";

        public override Entity GetEntityByVoxelPosition(int x, int y, int z)
        {
            logText += string.Format("({0},{1},{2});", x, y, z); 
            return null;
        }
    }

    [TestClass]
    public class WorldGridTests
    {
        [TestMethod]
        public void AddedEntityAmongNoOtherEntitiesWillBeFoundByPosition()
        {
            Entity entityInPosition;

            WorldGrid worldGrid = new WorldGrid();
            Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
            Entity origEntity = new Sphere(position, Color.Black);
            worldGrid.AddEntity(origEntity);

            entityInPosition = worldGrid.GetEntityByVoxelPosition((IntVector) position);

            Assert.IsNotNull(entityInPosition, "No entity was found at the position");
            Assert.AreEqual<Vector3>(position, entityInPosition.GetPosition(), "The added and the found entity did not have the same position.");
            Assert.AreEqual<Entity>(origEntity, entityInPosition, "The added and the found entity were not the same.");
        }
        [TestMethod]
        public void AddedEntityAmongNoOtherEntitiesWillNotBeFoundByWrongPosition()
        {
            Entity entityInPosition;

            WorldGrid worldGrid = new WorldGrid();
            Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
            Entity origEntity = new Sphere(position, Color.Black);
            worldGrid.AddEntity(origEntity);

            entityInPosition = worldGrid.GetEntityByVoxelPosition(new IntVector(1,0,0));

            Assert.IsNull(entityInPosition);
        }
        [TestMethod]
        public void AddedEntityAmongNoOtherEntitiesWillBeFoundByFractionalPosition()
        {
            Entity entityInPosition;

            WorldGrid worldGrid = new WorldGrid();
            Vector3 position = new Vector3(1.9f, 0.5f, -3.6f);
            Entity origEntity = new Sphere(position, Color.Black);
            worldGrid.AddEntity(origEntity);

            entityInPosition = worldGrid.GetEntityByVoxelPosition((IntVector)position);

            Assert.IsNotNull(entityInPosition, "No entity was found at the position");
            Assert.AreEqual<Vector3>(position, entityInPosition.GetPosition(), "The added and the found entity did not have the same position.");
            Assert.AreEqual<Entity>(origEntity, entityInPosition, "The added and the found entity were not the same.");
        }
        [TestMethod]
        public void AddedEntityAmongOtherEntitiesWillBeFoundByPosition()
        {
            Entity entityInPosition;

            WorldGrid worldGrid = new WorldGrid();
            Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
            Entity origEntity = new Sphere(position, Color.Black);
            worldGrid.AddEntity(origEntity);

            worldGrid.AddEntity(new Sphere(new Vector3(1.0f, 0.0f, 0.0f), Color.Black));
            worldGrid.AddEntity(new Sphere(new Vector3(0.0f, 1.0f, 0.0f), Color.Black));
            worldGrid.AddEntity(new Sphere(new Vector3(0.0f, 0.0f, 1.0f), Color.Black));

            entityInPosition = worldGrid.GetEntityByVoxelPosition((IntVector)position);

            Assert.IsNotNull(entityInPosition, "No entity was found at the position");
            Assert.AreEqual<Vector3>(position, entityInPosition.GetPosition(), "The added and the found entity did not have the same position.");
            Assert.AreEqual<Entity>(origEntity, entityInPosition, "The added and the found entity were not the same.");
        }

        //[TestMethod]
        //public void PlottedDiagonalRayWillHaveTheRightPositions()
        //{
        //    WorldGridSpy worldGrid = new WorldGridSpy();

        //    RealtimeRaytrace.Ray r = new RealtimeRaytrace.Ray(new Vector3(0, 0, 0), new Vector3(0.577350f, 0.577350f, 0.577350f));

        //    Intersection i = worldGrid.GetClosestIntersection(r, 7);

        //    Assert.AreEqual<string>("(0,0,0);(0,0,1);(0,1,1);(1,1,1);(1,1,2);(1,2,2);(2,2,2);", worldGrid.logText);
        //}

        //[TestMethod]
        //public void PlottedNegativeDiagonalRayWillHaveTheRightPositions()
        //{
        //    WorldGridSpy worldGrid = new WorldGridSpy();

        //    RealtimeRaytrace.Ray r = new RealtimeRaytrace.Ray(new Vector3(0, 0, 0), new Vector3(-0.577350f, -0.577350f, -0.577350f));

        //    Intersection i = worldGrid.GetClosestIntersection(r, 7);

        //    Assert.AreEqual<string>("(0,0,0);(0,0,-1);(0,-1,-1);(-1,-1,-1);(-1,-1,-2);(-1,-2,-2);(-2,-2,-2);", worldGrid.logText);
        //}

        //[TestMethod]
        //public void PlottedSteepYRayWillHaveTheRightPositions()
        //{
        //    WorldGridSpy worldGrid = new WorldGridSpy();

        //    RealtimeRaytrace.Ray r = new RealtimeRaytrace.Ray(new Vector3(0, 0, 0), new Vector3(0.485071f, 0.727607f, 0.485071f));

        //    Intersection i = worldGrid.GetClosestIntersection(r, 8);

        //    Assert.AreEqual<string>("(0,0,0);(0,1,0);(0,1,1);(1,1,1);(1,2,1);(1,2,2);(2,2,2);(2,3,2);", worldGrid.logText);
        //}

        //[TestMethod]
        //public void PlottedSteepZRayWillHaveTheRightPositions()
        //{
        //    WorldGridSpy worldGrid = new WorldGridSpy();

        //    RealtimeRaytrace.Ray r = new RealtimeRaytrace.Ray(new Vector3(0, 0, 0), new Vector3(0.485071f, 0.485071f, 0.727607f));

        //    Intersection i = worldGrid.GetClosestIntersection(r, 8);

        //    Assert.AreEqual<string>("(0,0,0);(0,0,1);(0,1,1);(1,1,1);(1,1,2);(1,2,2);(2,2,2);(2,2,3);", worldGrid.logText);
        //}

        //[TestMethod]
        //public void PlottedSteepYZRayWillHaveTheRightPositions()
        //{
        //    WorldGridSpy worldGrid = new WorldGridSpy();

        //    RealtimeRaytrace.Ray r = new RealtimeRaytrace.Ray(new Vector3(0, 0, 0), new Vector3(0.426401f, 0.639602f, 0.639602f));

        //    Intersection i = worldGrid.GetClosestIntersection(r, 9);

        //    Assert.AreEqual<string>("(0,0,0);(0,0,1);(0,1,1);(1,1,1);(1,1,2);(1,2,2);(2,2,2);(2,2,3);(2,3,3);", worldGrid.logText);
        //}

        //[TestMethod]
        //public void TracingRayFromOrigoWillReturnClosestIntersection()
        //{
        //    WorldGrid worldGrid = new WorldGrid();

        //    Sphere s = new Sphere(0, new Vector3(0, 0, 10), Color.Black, 1f);
        //    worldGrid.AddEntity(s);

        //    Sphere sFar = new Sphere(0, new Vector3(0, 0, 12), Color.White, 1f);
        //    worldGrid.AddEntity(sFar);
 
        //    RealtimeRaytrace.Ray r = new RealtimeRaytrace.Ray(new Vector3(0, 0, 0), new Vector3(0, 0, 1));

        //    Intersection i = worldGrid.GetClosestIntersection(r, 50);

        //    Assert.AreEqual<String>((new Intersection(new Vector3(0, 0, 9), new Vector3(0, 0, -1), 9, s)).ToString(),i.ToString());
        //}

        //[TestMethod]
        //public void TracingRayFromOrigoWithFarestSphereFirstWillReturnClosestIntersection()
        //{
        //    WorldGrid worldGrid = new WorldGrid();

        //    Sphere sFar = new Sphere(0, new Vector3(0, 0, 12), Color.White, 1f);
        //    worldGrid.AddEntity(sFar);

        //    Sphere s = new Sphere(0, new Vector3(0, 0, 10), Color.Black, 1f);
        //    worldGrid.AddEntity(s);

        //    RealtimeRaytrace.Ray r = new RealtimeRaytrace.Ray(new Vector3(0, 0, 0), new Vector3(0, 0, 1));

        //    Intersection i = worldGrid.GetClosestIntersection(r, 50);

        //    Assert.AreEqual<String>((new Intersection(new Vector3(0, 0, 9), new Vector3(0, 0, -1), 9, s)).ToString(), i.ToString());
        //}

        //[TestMethod]
        //public void TracingRayFromPositiveCoordinatesWillReturnClosestIntersection()
        //{
        //    WorldGrid worldGrid = new WorldGrid();

        //    Sphere s = new Sphere(0, new Vector3(4, 7, 10), Color.Black, 1f);
        //    worldGrid.AddEntity(s);

        //    Sphere sFar = new Sphere(0, new Vector3(4, 7, 12), Color.White, 1f);
        //    worldGrid.AddEntity(sFar);

        //    RealtimeRaytrace.Ray r = new RealtimeRaytrace.Ray(new Vector3(4, 7, 0), new Vector3(0, 0, 1));

        //    Intersection i = worldGrid.GetClosestIntersection(r, 50);

        //    Assert.AreEqual<String>((new Intersection(new Vector3(4, 7, 9), new Vector3(0, 0, -1), 9, s)).ToString(), i.ToString());
        //}


        //[TestMethod]
        //public void TracingRayFromPositiveCoordinatesLookingDownWillReturnClosestIntersection()
        //{
        //    WorldGrid worldGrid = new WorldGrid();

        //    Sphere s = new Sphere(0, new Vector3(4, 0, 10), Color.Black, 1f);
        //    worldGrid.AddEntity(s);

        //    Sphere sFar = new Sphere(0, new Vector3(4, -7, 10), Color.White, 1f);
        //    worldGrid.AddEntity(sFar);

        //    RealtimeRaytrace.Ray r = new RealtimeRaytrace.Ray(new Vector3(4, 7, 10), new Vector3(0, -1, 0));

        //    Intersection i = worldGrid.GetClosestIntersection(r, 50);

        //    Assert.AreEqual<String>((new Intersection(new Vector3(4, 1, 10), new Vector3(0, 1, 0), 6, s)).ToString(), i.ToString());
        //}

        //[TestMethod]
        //public void TracingRayFromNegativeCoordinatesWillReturnClosestIntersection()
        //{
        //    WorldGrid worldGrid = new WorldGrid();

        //    Sphere s = new Sphere(0, new Vector3(-4, -7, -10), Color.Black, 1f);
        //    worldGrid.AddEntity(s);

        //    Sphere sFar = new Sphere(0, new Vector3(-4, -7, -12), Color.White, 1f);
        //    worldGrid.AddEntity(sFar);

        //    RealtimeRaytrace.Ray r = new RealtimeRaytrace.Ray(new Vector3(-4, -7, 0), new Vector3(0, 0, -1));

        //    Intersection i = worldGrid.GetClosestIntersection(r, 50);

        //    Assert.AreEqual<String>((new Intersection(new Vector3(-4, -7, -9), new Vector3(0, 0, 1), 9, s)).ToString(), i.ToString());
        //}

        //[TestMethod]
        //public void TracingRayFromNegativeToPositiveCoordinatesWillReturnClosestIntersection()
        //{
        //    WorldGrid worldGrid = new WorldGrid();

        //    Sphere s = new Sphere(0, new Vector3(-4, -7, -10), Color.Black, 1f);
        //    worldGrid.AddEntity(s);

        //    Sphere sFar = new Sphere(0, new Vector3(-4, -7, -12), Color.White, 1f);
        //    worldGrid.AddEntity(sFar);

        //    RealtimeRaytrace.Ray r = new RealtimeRaytrace.Ray(new Vector3(-4, -7, 10), new Vector3(0, 0, -1));

        //    Intersection i = worldGrid.GetClosestIntersection(r, 50);

        //    Assert.AreEqual<String>((new Intersection(new Vector3(-4, -7, -9), new Vector3(0, 0, 1), 19, s)).ToString(), i.ToString());
        //}

        //[TestMethod]
        //public void TracingRayFromPositiveToNegativeCoordinatesWillReturnClosestIntersection()
        //{
        //    WorldGrid worldGrid = new WorldGrid();

        //    Sphere s = new Sphere(0, new Vector3(-4, -7, 10), Color.Black, 1f);
        //    worldGrid.AddEntity(s);

        //    Sphere sFar = new Sphere(0, new Vector3(-4, -7, 12), Color.White, 1f);
        //    worldGrid.AddEntity(sFar);

        //    RealtimeRaytrace.Ray r = new RealtimeRaytrace.Ray(new Vector3(-4, -7, -10), new Vector3(0, 0, 1));

        //    Intersection i = worldGrid.GetClosestIntersection(r, 50);

        //    Assert.AreEqual<String>((new Intersection(new Vector3(-4, -7, 9), new Vector3(0, 0, -1), 19, s)).ToString(), i.ToString());
        //}

    }
}
