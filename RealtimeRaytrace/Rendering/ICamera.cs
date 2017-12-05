using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeRaytrace
{
    public interface ICamera
    {
        Ray SpawnRay(float x, float y, double maxDistance);
   }
}
