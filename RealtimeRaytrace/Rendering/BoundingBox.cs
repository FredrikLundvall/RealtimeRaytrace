using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public struct BoundingBox
    {
        Vector3 _min;
        Vector3 _max;

        public BoundingBox(Vector3 min, Vector3 max)
        {
            _min = min;
            _max = max;
        }

        public BoundingBoxIntersection Intersect(Ray r)
        {
            float tmin = float.MinValue;
            float tmax = float.MaxValue;

            if (r.GetDirection().X != 0.0)
            {
                float t1 = (_min.X - r.GetStart().X) / r.GetDirection().X;
                float t2 = (_max.X - r.GetStart().X) / r.GetDirection().X;
                tmin = Math.Max(tmin, Math.Min(t1, t2));
                tmax = Math.Min(tmax, Math.Max(t1, t2));
            }
            else if (r.GetDirection().X <= _min.X || r.GetDirection().X >= _max.X)
            {
                return BoundingBoxIntersection.CreateNullBoundingBoxIntersection();
            }
            if (r.GetDirection().Y != 0.0)
            {
                float t1 = (_min.Y - r.GetStart().Y) / r.GetDirection().Y;
                float t2 = (_max.Y - r.GetStart().Y) / r.GetDirection().Y;
                tmin = Math.Max(tmin, Math.Min(t1, t2));
                tmax = Math.Min(tmax, Math.Max(t1, t2));
            }
            else if (r.GetDirection().Y <= _min.Y || r.GetDirection().Y >= _max.Y)
            {
                return BoundingBoxIntersection.CreateNullBoundingBoxIntersection(); 
            }
            if (r.GetDirection().Z != 0.0)
            {
                float t1 = (_min.Z - r.GetStart().Z) / r.GetDirection().Z;
                float t2 = (_max.Z - r.GetStart().Z) / r.GetDirection().Z;
                tmin = Math.Max(tmin, Math.Min(t1, t2));
                tmax = Math.Min(tmax, Math.Max(t1, t2));
            }
            else if (r.GetDirection().Z <= _min.Z || r.GetDirection().Z >= _max.Z)
            {
                return BoundingBoxIntersection.CreateNullBoundingBoxIntersection();
            }

            if (tmax > tmin && tmax > 0.0)
                return new BoundingBoxIntersection(tmin);
            else
                return BoundingBoxIntersection.CreateNullBoundingBoxIntersection();
        }

    }
}
