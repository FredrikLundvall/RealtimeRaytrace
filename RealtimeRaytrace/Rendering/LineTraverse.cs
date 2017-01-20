using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
//using System.Threading.Tasks;

namespace RealtimeRaytrace
{
    public class LineTraverse
    {
        /// <summary>
        /// The plot3d function delegate
        /// </summary>
        /// <param name="p">The point being plotted</param>
        /// <returns>True to continue, false to stop the algorithm</returns>
        public delegate bool Plot3d(Vector3 p);

        private static void Swap<T>(ref T lhs, ref T rhs) { T temp; temp = lhs; lhs = rhs; rhs = temp; }

        //Now a 3-d version (if the answer from the callback is true then the function continues)
        /// <summary>
        /// Plot the line from (x0, y0, z0) to (x1, y1, z1)
        /// </summary>
        /// <param name="p0">The start point</param>
        /// <param name="p1">The end point</param>
        /// <param name="plotCallback">The plotting function (if this returns false, the algorithm stops early)</param>
        public static void Line3d(Vector3 p0, Vector3 p1, Plot3d plotCallback)
        {
            bool steep_xy = Math.Abs(p1.Y - p0.Y) > Math.Abs(p1.X - p0.X);
            if (steep_xy)
            {
                Swap<float>(ref p0.X, ref p0.Y);
                Swap<float>(ref p1.X, ref p1.Y);
            }

            bool steep_xz = Math.Abs(p1.Z - p0.Z) > Math.Abs(p1.X - p0.X);
            if (steep_xz)
            {
                Swap<float>(ref p0.X, ref p0.Z);
                Swap<float>(ref p1.X, ref p1.Z);
            }

            float dX = Math.Abs(p1.X - p0.X);
            float dY = Math.Abs(p1.Y - p0.Y);
            float dZ = Math.Abs(p1.Z - p0.Z);
            float err_xy = (dX / 2);
            float err_xz = err_xy;

            float xstep = (p0.X < p1.X ? 1 : -1);
            float ystep = (p0.Y < p1.Y ? 1 : -1);
            float zstep = (p0.Z < p1.Z ? 1 : -1);

            Vector3 p = p0;
            float xend = p1.X + xstep;
            while (p.X != xend)
            {
                Vector3 tp = p;

                if (steep_xz)
                    Swap<float>(ref tp.X, ref tp.Z);
                if (steep_xy)
                    Swap<float>(ref tp.X, ref tp.Y);

                if (!plotCallback(tp))
                    return;

                err_xy -= dY;
                err_xz -= dZ;

                if (err_xy < 0)
                {
                    p.Y += ystep;
                    err_xy += dX;
                }

                if (err_xz < 0)
                {
                    p.Z += zstep;
                    err_xz += dX;
                }

                p.X += xstep;

            }

        }

    }
}
