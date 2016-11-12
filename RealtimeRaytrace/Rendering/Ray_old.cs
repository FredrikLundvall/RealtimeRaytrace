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
            _vector = vector;
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

        public delegate bool Plot(int x,int y, int z);

        private static void Swap<T>(ref T lhs, ref T rhs) { T temp; temp = lhs; lhs = rhs; rhs = temp; }

        /// <summary>
        /// Plot the ray from the startpoint for a given distance
        /// if the answer from the callback is true then the function continues
        /// </summary>
        public void PlotRay(float distance, Plot plotCallback)
        {
            PlotLine(_start, _start + _vector * distance, plotCallback); 
        }

        public static void PlotLine(Vector3 p0, Vector3 p1, Plot plotCallback)
        {
            PlotLine((int)Math.Truncate(p0.X), (int)Math.Truncate(p0.Y), (int)Math.Truncate(p0.Z), (int)Math.Truncate(p1.X), (int)Math.Truncate(p1.Y), (int)Math.Truncate(p1.Z), plotCallback);
        }

        public static void PlotLine(int x0, int y0, int z0, int x1, int y1, int z1, Plot plotCallback)
        {
            bool steep_xy = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep_xy)
            {
                Swap<int>(ref x0, ref y0);
                Swap<int>(ref x1, ref y1);
            }

            bool steep_xz = Math.Abs(z1 - z0) > Math.Abs(x1 - x0);
            if (steep_xz)
            {
                Swap<int>(ref x0, ref z0);
                Swap<int>(ref x1, ref z1);
            }

            int dX = Math.Abs(x1 - x0);
            int dY = Math.Abs(y1 - y0);
            int dZ = Math.Abs(z1 - z0);
            int err_xy = (dX / 2);
            int err_xz = err_xy;

            int xstep = (x0 < x1 ? 1 : -1);
            int ystep = (y0 < y1 ? 1 : -1);
            int zstep = (z0 < z1 ? 1 : -1);

            int y = y0;
            int x = x0;
            int z = z0;
            int xend = x1 + xstep;
            while (x != xend)
            {
                int tx = x;
                int ty = y;
                int tz = z;

                if (steep_xz)
                    Swap<int>(ref tx, ref tz);
                if (steep_xy)
                    Swap<int>(ref tx, ref ty);

                if (!plotCallback(tx, ty, tz))
                    return;

                err_xy -= dY;
                err_xz -= dZ;

                if (err_xy < 0)
                {
                    y += ystep;
                    err_xy += dX;
                }

                if (err_xz < 0)
                {
                    z += zstep;
                    err_xz += dX;
                }

                x += xstep;

            }

        }

    
    }




}
