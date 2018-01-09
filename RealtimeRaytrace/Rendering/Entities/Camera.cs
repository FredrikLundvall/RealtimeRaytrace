using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    abstract public class Camera : Entity
    {
        public Camera(Vector3 position, float yaw, float pitch, float roll)
            : base(position,yaw,pitch,roll)
        {
        }
        abstract public Ray SpawnRay(float x, float y, double maxDistance);
   }
}
