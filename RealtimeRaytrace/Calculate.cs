using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    //TODO: Change this to something else
    public class Calculate
    {

        public static int CoordinateHash(Vector3 pos, int voxelSize)
        {
            IntVector iPos = ((IntVector)(pos / voxelSize));

            uint ux = (uint)iPos.X;
            uint uy = (uint)iPos.Y;
            uint uz = (uint)iPos.Z;

            //Move the 8 bits of x, y and z to different positions in one int. then check the negative bit (31)
            return (int)((ux & 255) << 18 | (uy & 255) << 9 | (uz & 255) | ((ux & 2147483648) >> 4) | ((uy & 2147483648) >> 13) | ((uz & 2147483648) >> 22)); 
        }
    }
}
