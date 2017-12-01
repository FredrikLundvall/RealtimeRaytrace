using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class SphereGroup : Entity
    {
        List<Sphere> _spheres;

        public SphereGroup(int index, bool isIndexedByPosition, Vector3 position)
            : base(index, isIndexedByPosition, position, 0, 0, 0)
        {
            _spheres = new List<Sphere>();
        }

        public SphereGroup(int index, Vector3 position)
            : this(index, true, position)
        {
        }

        public void AddSphere(Sphere sphere)
        {
            _spheres.Add(sphere);
        }

        public override Intersection Intersect(Ray ray)
        {
            //closest hit
            Intersection closestIntersection = new Intersection(true);
            foreach (Sphere sphere in _spheres)
            {
                Intersection intersection = sphere.Intersect(ray);
                if ((!intersection.IsNull()) && intersection.GetTFirstHit() < closestIntersection.GetTFirstHit())
                {
                    closestIntersection = intersection;
                }
            }
            return closestIntersection;
        }

    }
}
