using System;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public interface ITextureMap
    {
        Color GetColorFromDirection(Vector3 direction, bool flipU = false, bool flipV = false);
    }
}
