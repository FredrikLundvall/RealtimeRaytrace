using System;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class LightsourceBase : SphereBase
    {
        float _intensity;

        public LightsourceBase(Vector3 position, float intensity, float radius = 0.5f, ITextureMap textureMap = null)
            : base(position, radius, textureMap)
        {
            _intensity = intensity;
        }

        public float GetIntensity()
        {
            return _intensity;
        }

        public override string ToString()
        {
            return string.Format("{0}, radius: {1}, intensity: {2} ", _position.ToString(), _radius.ToString(), _intensity.ToString());
        }
    }
}
