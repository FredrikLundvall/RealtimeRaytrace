using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class Vector3EqualityComparer : IEqualityComparer<Vector3>
    {

        public bool Equals(Vector3 p1, Vector3 p2)
        {
            if (Math.Truncate(p1.X) == Math.Truncate(p2.X) &
                Math.Truncate(p1.Y) == Math.Truncate(p2.Y) &
                Math.Truncate(p1.Z) == Math.Truncate(p2.Z))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public int GetHashCode(Vector3 ps)
        {
            return Math.Truncate(ps.X).GetHashCode() ^ Math.Truncate(ps.Y).GetHashCode() ^ Math.Truncate(ps.Z).GetHashCode();
        }

    }

}
