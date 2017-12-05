using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class OrthogonalCamera : Camera
    {
        public OrthogonalCamera(int index, Vector3 position, float yaw, float pitch, float roll)
            : base(index, position, yaw, pitch, roll)
        {
        }

        override public Ray SpawnRay(float x, float y, double maxDistance)
        {
            Vector3 direction = Vector3.Forward;
            return new Ray(GetPosition() + new Vector3(-x / 8,y / 8,0), Vector3.Transform(direction, Quaternion.CreateFromYawPitchRoll(GetYaw(), GetPitch(), GetRoll())));
        }
    }
}
