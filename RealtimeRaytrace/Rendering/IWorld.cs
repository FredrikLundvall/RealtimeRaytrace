using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public interface IWorld
    {
        void CreateWorld(GraphicsDeviceManager graphicsDeviceManager, int sizeX, int sizeY, int sizeZ);
        void AddEntity(Entity entity);
        Entity GetEntity(int index);
        int EntityCount();
        BoundingBoxIntersection Intersect(Ray r);
        Intersection GetClosestIntersection(Ray ray, float distance);
        bool IsIntersecting(Ray r);
        void AddLightsourceEntity(Entity entity);
        Entity GetLightsourceEntity(int index);
        int LightsourceEntityCount();
    }
}
