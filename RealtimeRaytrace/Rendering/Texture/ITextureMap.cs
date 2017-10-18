using System;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public interface ITextureMap
    {
        Color GetColorFromDirection(Vector3 direction, bool flipU = false, bool flipV = false, float offsetU = 0, float offsetV = 0, float scaleU = 0, float scaleV = 0);
    }
}
