using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public interface IWorld
    {
        void CreateWorld(GraphicsDeviceManager graphicsDeviceManager, int sizeX, int sizeY, int sizeZ);
        void AddEntity(Entity entity);
        BoundingBoxIntersection Intersect(Ray r);
        Intersection GetClosestIntersection(Ray ray, float distance);
        bool IsIntersecting(Ray r);
    }
}
