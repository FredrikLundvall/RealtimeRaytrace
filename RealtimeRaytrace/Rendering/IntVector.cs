using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class IntVector
    {
        public int X;
        public int Y;
        public int Z;

        public IntVector(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public IntVector(Vector3 v) : this((int)Math.Truncate(v.X), (int)Math.Truncate(v.Y), (int)Math.Truncate(v.Z)) { }
        public static explicit operator IntVector(Vector3 v)
        {
            return new IntVector(v);
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2})", X.ToString(), Y.ToString(), Z.ToString());
        }
    }
}
