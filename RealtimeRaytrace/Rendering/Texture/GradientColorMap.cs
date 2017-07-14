using System;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class GradientColorMap : ITextureMap
    {
        protected readonly Color _startColorU, _endColorU, _startColorV, _endColorV;

        public GradientColorMap(Color StartColorU, Color EndColorU, Color StartColorV, Color EndColorV)
        {
            _startColorU = StartColorU;
            _endColorU = EndColorU;
            _startColorV = StartColorV;
            _endColorV = EndColorV;
        }

        public Color GetColorFromDirection(Vector3 direction, bool flipU = false, bool flipV = false)
        {
            double u = (0.5 + Math.Atan2(direction.Z, direction.X) / (Math.PI * 2));
            double v = (0.5 - Math.Asin(direction.Y) / Math.PI);
            return Color.Lerp(Color.Lerp(_startColorU, _endColorU, (float)u), Color.Lerp(_startColorV, _endColorV, (float)v), 0.5f);
            ;
        }
    }
}
