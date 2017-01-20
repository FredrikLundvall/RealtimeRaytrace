using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class Intersection
    {
        private
        readonly Vector3 _position;
        readonly Vector3 _normal;
        readonly float _t;//Avstånd till träffen
        readonly Sphere _sphere;

        public Intersection()
        {
            _position = Vector3.Zero;
            _normal = Vector3.Zero;
            _t = float.MaxValue;
            _sphere = null;
            //_sphere = new Sphere(Vector3.Zero, Color.Black);
        }

        public Intersection(Vector3 position, Vector3 normal, float t, Sphere sphere)
        {
            _position = position;
            _normal = normal;
            _t = t;
            _sphere = sphere;
        }

        public float GetT()
        {
            return _t;
        }

        public Vector3 GetPosition()
        {
            return _position;
        }

        public Vector3 GetNormal()
        {
            return _normal;
        }

        public Sphere GetSphere()
        {
            return _sphere;
        }

        public bool IsHit()
        {
            return _sphere != null;
            //return _t < float.MaxValue;
        }

        public override string ToString()
        {
            return string.Format("pos: {0}, normal: {1}, distance: {2}, sphere: {3}", _position.ToString(), _normal.ToString(), _t.ToString(), (_sphere != null) ? _sphere.ToString() : "null");
        }

    }
}
