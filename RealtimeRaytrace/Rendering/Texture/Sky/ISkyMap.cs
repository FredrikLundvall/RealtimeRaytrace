using System;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public interface ISkyMap
    {
        Color GetColorInSky(Ray ray);
        Color GetColorInSky(Vector3 direction);
    }
}