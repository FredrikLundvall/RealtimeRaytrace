﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class SphereGroup : Entity
    {
        //List<Sphere> _spheres;

        public SphereGroup(Vector3 position)
            : base(position, 0, 0, 0)
        {
            //_spheres = new List<Sphere>();
        }

        public void AddSphere(Sphere sphere)
        {
            //_spheres.Add(sphere);
            _entityList.Add(sphere);
        }

        public override Intersection Intersect(Ray ray)
        {
            //closest hit
            Intersection closestIntersection = new Intersection(true);
            foreach (Sphere sphere in _entityList)
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
