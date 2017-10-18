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

        public Color GetColorFromDirection(Vector3 direction, bool flipU = false, bool flipV = false, float offsetU = 0, float offsetV = 0, float scaleU = 0, float scaleV = 0)
        {
            double angle = Math.Atan2(direction.Z, direction.X) / Math.PI;
            double u = angle < 0 ? angle + 1 : 1 - angle;
            double v = 0.5 - Math.Asin(direction.Y) / Math.PI;
            return Color.Lerp(Color.Lerp(_startColorU, _endColorU, (float)u), Color.Lerp(_startColorV, _endColorV, (float)v),0.5f);
        }
    }
}
