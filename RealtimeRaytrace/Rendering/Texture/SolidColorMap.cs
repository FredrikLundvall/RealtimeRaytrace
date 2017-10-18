using System;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class SolidColorMap : ITextureMap
    {
        protected readonly Color _color;

        public SolidColorMap(Color color)
        {
            _color = color;
        }
        public Color GetColorFromDirection(Vector3 direction, bool flipU = false, bool flipV = false, float offsetU = 0, float offsetV = 0, float scaleU = 0, float scaleV = 0)
        {
            return _color;
        }
    }
}
