using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public struct RayColor
    {
        private readonly Color _color;
        private readonly float _rayAmount;

        public RayColor(Color color, float rayAmount)
        {
            _color = color;
            _rayAmount = rayAmount;
        }

        public Color GetColor()
        {
            return _color;
        }

        public float GetRayAmount()
        {
            return _rayAmount;
        }
    }
}
