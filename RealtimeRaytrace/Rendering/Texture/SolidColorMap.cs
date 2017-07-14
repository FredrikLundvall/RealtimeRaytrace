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
        public Color GetColorFromDirection(Vector3 direction, bool flipU = false, bool flipV = false)
        {
            return _color;
        }
    }
}
