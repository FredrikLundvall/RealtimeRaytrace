using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class Sphere : Entity
    {
        Color _color = Color.White;
        readonly float _radius = 1f;

        public Sphere(int index, bool isIndexedByPosition, Vector3 position, Color color, float radius = 0.5f)
            : base(index, isIndexedByPosition, position)
        {
            _color = color;
            _radius = radius;
        }

        public Sphere(int index, Vector3 position, Color color, float radius = 0.5f)
            : this(index, true, position, color, radius)
        {
        }

        public Intersection Intersect(Ray ray)
        {
            Vector3 el = GetPosition() - ray.GetStart();
            float d = Vector3.Dot(el, ray.GetVector() );
            float els = Vector3.Dot(el, el);
            float rs = _radius * _radius; 

            if (d < 0 && els > rs)
            {
                return null;
            }
            float ms = els - d * d;
            if (ms > rs)
            {
                return null;
            }
            float q = (float)Math.Sqrt(rs - ms);
            float t;
            if (els > rs)
            {
                t = d - q;
            }
            else
            {
                t = d + q;
            }
            Vector3 p = ray.GetStart() + Vector3.Multiply(ray.GetVector(), t);
            return new Intersection(p, Vector3.Normalize( Vector3.Divide(p - GetPosition(),_radius) ), t, this);
        }

        public Color GetColor()
        {
            return _color;
        }

        public float GetRadius()
        {
            return _radius;
        }

        public override string ToString()
        {
            return string.Format("{0}, color: {1}, radius: {2}",this._color.ToString(), _color.ToString(), _radius.ToString());
        }
    }
}

