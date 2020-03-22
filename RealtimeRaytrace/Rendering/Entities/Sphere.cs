using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class Sphere : SphereBase
    {
        public Sphere(Vector3 position, float radius = 0.5f, ITextureMap textureMap = null)
            : base(position, radius, textureMap)
        {
        }

        public override Intersection Intersect(Ray ray)
        {
            Intersection sphereIntersection = base.Intersect(ray);
            return sphereIntersection;
        }
    }
}

