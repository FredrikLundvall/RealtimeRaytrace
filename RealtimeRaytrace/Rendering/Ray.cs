using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class Ray
    {
        protected
        Vector3 _start = Vector3.Zero;
        Vector3 _vector = Vector3.Forward;

        public Ray(Vector3 start, Vector3 vector)
        {
            _start = start;
            _vector = Vector3.Normalize( vector);
        }

        public Ray(Camera camera) : this(camera.GetPosition(), camera.GetRotatedLookAt()) { }

        public Ray(Camera camera, float x, float y)
            : this(camera.GetPosition(), Vector3.Transform(camera.GetLookAt(), Matrix.CreateFromYawPitchRoll(camera.GetYaw() + (x * 0.001f), camera.GetPitch() + (y * 0.001f), camera.GetRoll())))
        {
            //TODO: Likely a gimbal lock. Spheres get squashed around pitch = (1.57 or -1.57) radians, thats when looking up or down 90 degrees
         
            //if fixing this using quarternions... look at this: http://gamedev.stackexchange.com/questions/30644/how-to-keep-my-quaternion-using-fps-camera-from-tilting-and-messing-up

            //_vector = Vector3.Transform(camera.GetLookAt(), Matrix.CreateFromYawPitchRoll(camera.GetYaw() + (x * 0.001f), camera.GetPitch() + (y * 0.001f), camera.GetRoll()));
        }

        public Vector3 GetStart()
        {
            return _start;
        }

        public Vector3 GetVector()
        {
            return _vector;
        }
    
    }




}
