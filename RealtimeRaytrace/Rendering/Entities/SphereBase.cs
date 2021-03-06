﻿using System;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class SphereBase : Entity
    {
        protected Color _color = Color.White;
        protected readonly float _radius = 1f;
        protected readonly ITextureMap _textureMap;

        public SphereBase(int index, bool isIndexedByPosition, Vector3 position, Color color, float radius = 0.5f, ITextureMap textureMap = null)
            : base(index, isIndexedByPosition, position, 0, 0, 0)
        {
            _color = color;
            _radius = radius;
            _textureMap = textureMap;
            if (_textureMap == null)
                _textureMap = new SolidColorMap(_color);
        }

        public SphereBase(int index, Vector3 position, Color color, float radius = 0.5f)
            : this(index, true, position, color, radius)
        {
        }

        //TODO: A overloaded version can send a any parent groups position and rotation as a matrix (to support moving and rotating SphereGroups and AntiSpheres)
        public override Intersection Intersect(Ray ray)
        {
            return Intersect(ray.GetStart(), ray.GetDirection());
        }

        protected override Intersection Intersect(Vector3 start, Vector3 direction)
        {
            Vector3 l = GetPosition() - start; //Vector to sphere center (and direction)
            float tc = Vector3.Dot(l, direction); //Length to ray hit-center
            float els = Vector3.Dot(l, l);
            float radiusSquared = _radius * _radius;
            if (tc < 0 && els > radiusSquared)
                return new Intersection(true);
            float dSquared = els - tc * tc; //Squared perpendicular distance from spherecenter to ray (vinkelrätt) 
            if (dSquared > radiusSquared)
                return new Intersection(true);
            float t1c = (float)Math.Sqrt(radiusSquared - dSquared);
            float tFirstHit, tFar;
            if (els > radiusSquared)
            {
                tFirstHit = tc - t1c;
                tFar = tc + t1c;
            }
            else
            {
                //ray starts inside the sphere
                tFirstHit = tc + t1c;
                tFar = tc + t1c;
            }
            Vector3 pNear = start + Vector3.Multiply(direction, tFirstHit);
            Vector3 normalNear = Vector3.Normalize(Vector3.Divide(pNear - GetPosition(), _radius));
            return new Intersection(pNear, normalNear, normalNear, tFirstHit, tc - t1c, tFar, this);
        }

        public override bool IsPointInside(Vector3 point)
        {
            Vector3 l = point - _position;
            return l.Length() <= _radius;
        }

        public Color GetColor()
        {
            return _color;
        }

        public float GetRadius()
        {
            return _radius;
        }

        public ITextureMap GetTextureMap()
        {
            return _textureMap;
        }

        public override string ToString()
        {
            return string.Format("{0}, color: {1}, radius: {2}",_position.ToString(), _color.ToString(), _radius.ToString());
        }
    }
}

