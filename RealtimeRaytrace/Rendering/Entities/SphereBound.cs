using System;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class SphereBound : Entity
    {
        protected readonly float _radius = 1f;

        public SphereBound(Vector3 position, float radius)
            : base(position, 0, 0, 0)
        {
            _radius = radius;
        }

        public bool IsIntersecting(Ray ray)
        {
            return IsIntersecting(ray.GetStart(), ray.GetDirection());
        }

        protected bool IsIntersecting(Vector3 start, Vector3 direction)
        {
            Vector3 l = GetPosition() - start; //Vector to sphere center (and direction)
            float tc = Vector3.Dot(l, direction); //Length to ray hit-center
            float els = Vector3.Dot(l, l);
            float radiusSquared = _radius * _radius;
            if (tc < 0 && els > radiusSquared)
                return false;
            float dSquared = els - tc * tc; //Squared perpendicular distance from spherecenter to ray (vinkelrätt) 
            if (dSquared > radiusSquared)
                return false;
            return true;
        }

        public override bool IsPointInside(Vector3 point)
        {
            Vector3 l = point - _position;
            return l.Length() <= _radius;
        }
        public float GetRadius()
        {
            return _radius;
        }

        public override string ToString()
        {
            return string.Format("{0}, radius: {1}", _position.ToString(), _radius.ToString());
        }
    }
}

