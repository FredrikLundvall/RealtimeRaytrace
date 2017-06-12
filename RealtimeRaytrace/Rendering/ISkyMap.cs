using System;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public interface ISkyMap
    {
        Color GetColorInSky(Ray ray);
    }
}