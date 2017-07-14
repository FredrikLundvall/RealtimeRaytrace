using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class Camera : Entity
    {
        //Matrix _orientation;
        readonly float _maxFov = (float)Math.PI * 2.0f;
        float _fov = (float)Math.PI/2.0f;
        float _correctionAmount = 0.5f;

        public Camera(int index, Vector3 position, float yaw, float pitch, float roll, float fov = (float)Math.PI / 2.2f)
            : base(index, false, position,yaw,pitch,roll)
        {
            if (fov > (_maxFov + 0.01f))
                throw new ArgumentException("FOV cannot exceed: " + _maxFov.ToString());
            _fov = fov;
            _correctionAmount = 1.0f;
        }

        public Ray SpawnRay(float x, float y, double maxDistance)
        {
            Vector3 direction = Vector3.Forward;

            if (x == 0 & y == 0)
                direction = Vector3.Forward;
            else
            {
                //Fixed the gimbal lock. Spheres got squashed around pitch = (1.57 or -1.57) radians, thats when looking up or down 90 degrees    
                //Now the ray is calculated as pitchAngle = distance to (x,y)/maxdistance * fov/2
                //and a rotationAngle = angle to (x,y) 

                double distance = Math.Sqrt(x * x + y * y);
                float pitchAngle = 0;
                //if (y > 0)
                //{
                //Fisheye (all rays have the same angle between them)
                pitchAngle = GetLinearPitchAngle(distance, maxDistance, GetFov());
                //}
                //else
                //{
                //    //Fisheye removal using sinus (the rays near the center has larger angles between them than the ones far from center)
                //    //pitchAngle = GetSinusPitchAngle(distance, maxDistance, camera.GetFov(), camera.GetCorrectionAmount());
                //    pitchAngle = GetLogPitchAngle(distance, maxDistance, camera.GetFov());
                //}

                float rotationAngle = (float)Math.Atan2(x, y);
                direction = Vector3.Transform(Vector3.Forward, Quaternion.CreateFromAxisAngle(Vector3.Backward, rotationAngle) * Quaternion.CreateFromAxisAngle(Vector3.Right, pitchAngle));
            }
            return new Ray(GetPosition(), Vector3.Transform(direction, Quaternion.CreateFromYawPitchRoll(GetYaw(), GetPitch(), GetRoll())) );
        }

        private float GetLinearPitchAngle(double distance, double maxDistance, float fov)
        {
            return (float)((distance / maxDistance) * (fov / 2.0));
        }

        //private float GetSinusPitchAngle(double distance, double maxDistance, float fov, float correctionAmount)
        //{
        //    //To make the fisheye effect a little less annoying, make the angle-steps more high near the centre.
        //    //Sin can be used with values:
        //    //Sin(0) = 0 an Sin(pi/2) = 1
        //    //Where the pitchAngle is calculated as: pitchAngle = (float)(((Math.Sin((distance / maxDistance) * (anglePartOfMaxFov * Math.PI / 2.0)) + Math.Sin((distance / maxDistance) * ((1 - anglePartOfMaxFov) * Math.PI / 2.0)))) * (camera.GetFov() / 2.0));
        //    //return (float)(((Math.Sin((distance / maxDistance) * (correctionAmount * Math.PI / 2.0)) + Math.Sin((distance / maxDistance) * ((1 - correctionAmount) * Math.PI / 2.0)))) * (Fov / 2.0));
        //    return (float)(((Math.Sin((distance / maxDistance) * Math.PI / 2.0))) * (fov / 2.0));
        //}

        //private float GetRootPitchAngle(double distance, double maxDistance, float fov)
        //{
        //    //To make the fisheye effect a little less annoying, make the angle-steps more high near the centre.
        //    double correctionSteepness = 1 / 1.2;
        //    return (float)(Math.Pow((distance / maxDistance), correctionSteepness) * (fov / 2.0));
        //}

        //private float GetLogPitchAngle(double distance, double maxDistance, float fov)
        //{
        //    //To make the fisheye effect a little less annoying, make the angle-steps more high near the centre.
        //    double logBase = Math.PI;
        //    return (float)(Math.Log(distance / maxDistance + 1) * (1.0 / Math.Log(2.0, logBase)) * (fov / 2.0));
        //}

        public float GetFov()
        {
            return _fov;
        }

        public float GetMaxFov()
        {
            return _maxFov;
        }

        public float GetCorrectionAmount()
        {
            return _correctionAmount;
        }

    }
}
