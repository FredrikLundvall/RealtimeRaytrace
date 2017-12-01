using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class AntiSphere : SphereBase
    {
        public AntiSphere(int index, bool isIndexedByPosition, Vector3 position, Color color, float radius = 0.5f, ITextureMap textureMap = null)
            : base(index, isIndexedByPosition, position, color, radius, textureMap)
        {
        }

        public AntiSphere(int index, Vector3 position, Color color, float radius = 0.5f, ITextureMap textureMap = null)
            : this(index, true, position, color, radius, textureMap)
        {
        }

        public AntiIntersection AntiIntersect(Ray ray, SphereBase parentSphere)
        {
            Vector3 l = GetPosition() - ray.GetStart(); //Vector to sphere center (and direction)
            float tc = Vector3.Dot(l, ray.GetDirection()); //Length to ray hit-center
            float els = Vector3.Dot(l, l);
            float radiusSquared = _radius * _radius;

            if (tc < 0 && els > radiusSquared)
            {
                return AntiIntersection.CreateNullAntiIntersection();
            }
            float dSquared = els - tc * tc; //Squared perpendicular distance from spherecenter to ray (vinkelrätt) 
            if (dSquared > radiusSquared)
            {
                return AntiIntersection.CreateNullAntiIntersection();
            }
            float t1c = (float)Math.Sqrt(radiusSquared - dSquared);
            float tNear, tFar;
            tNear = tc - t1c;
            tFar = tc + t1c;
            Vector3 pNear = ray.GetStart() + Vector3.Multiply(ray.GetDirection(), tNear);
            Vector3 pFar = ray.GetStart() + Vector3.Multiply(ray.GetDirection(), tFar);
            var normNearTexture = Vector3.Normalize(Vector3.Divide(GetPosition() - pNear, _radius));
            var normFarTexture = Vector3.Normalize(Vector3.Divide(GetPosition() - pFar, _radius));
            return new AntiIntersection(pNear, pFar, normNearTexture, normFarTexture, tNear, tFar, this);
        }
    }
}