using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class Sphere : SphereBase
    {
        private static object _MyLock = new object();

        List<AntiSphere> _antiSphereList;

        public Sphere(int index, bool isIndexedByPosition, Vector3 position, Color color, float radius = 0.5f, ITextureMap textureMap = null)
            : base(index, isIndexedByPosition, position, color, radius, textureMap)
        {
            _antiSphereList = new List<AntiSphere>();
        }

        public Sphere(int index, Vector3 position, Color color, float radius = 0.5f, ITextureMap textureMap = null)
            : this(index, true, position, color, radius, textureMap)
        {
        }

        public void AddAntiSphere(AntiSphere sphere)
        {
            _antiSphereList.Add(sphere);
        }

        public override Intersection Intersect(Ray ray)
        {
            Intersection closestIntersection = base.Intersect(ray);
            if (!closestIntersection.IsHit())
                return closestIntersection;

            bool insideSphere = closestIntersection.GetTFirstHit() == closestIntersection.GetTFar();
            AntiIntersection farthestAntiIntersection = new AntiIntersection(true);
            //compare the hits of the antiSpheres
            float tMax = closestIntersection.GetTFar();
            float tFar = closestIntersection.GetTFirstHit();
            int i = 0;
            while (i < _antiSphereList.Count)
            {
                var antiIntersection = _antiSphereList[i].AntiIntersect(ray, this);

                //Take the next if this wasn't a hit
                if (!antiIntersection.IsHit())
                {
                    i++;
                    continue;
                }

                //Take the next if this antilist-hit starts further away than the saved "nearest" hit
                if (antiIntersection.GetTNear() > tFar)
                {
                    i++;
                    continue;
                }

                if (antiIntersection.GetTNear() < tFar && antiIntersection.GetTFar() > tFar)
                {
                    //Move away the nearest hit to the other side of the AntiSphere
                    tFar = antiIntersection.GetTFar();
                    farthestAntiIntersection = antiIntersection;
                    if (i == 0)
                    {
                        i++;
                        continue;
                    }
                    else
                    {
                        //restart the check (it isn't in any sorted order)
                        i = 0;
                        continue;
                    }
                }
                else
                {
                    i++;
                    continue;
                }
            }

            //TODO: Maybe when inside the sphere, the intersection with antispheres should be as a normal closest intersection check
            //Not sure if more is needed here, because rendering from inside the sphere, is not really a supported feature...

            if (tFar > tMax && !insideSphere)
            {
                //The nearest hit is further away than the other side
                closestIntersection = new Intersection(true);
            }
            else if(farthestAntiIntersection.IsHit())
            {
                if (insideSphere)
                {
                    //bool insideAntiSphere = farthestAntiIntersection.GetTNear() < 0;

                    //Check where the normal hits the parent sphere (this one) and use for the texture
                    var intersection = this.Intersect(farthestAntiIntersection.GetPositionNear(), farthestAntiIntersection.GetNormalNearTexture());
                    closestIntersection = farthestAntiIntersection.CreateIntersection(intersection.GetNormalFirstHitTexture());
                }
                else
                {
                    //Check where the normal hits the parent sphere (this one) and use for the texture
                    var intersection = this.Intersect(farthestAntiIntersection.GetPositionFar(), farthestAntiIntersection.GetNormalFarTexture());
                    closestIntersection = farthestAntiIntersection.CreateIntersection(intersection.GetNormalFirstHitTexture());
                }
            }
            return closestIntersection;
        }

        public override string ToString()
        {
            return string.Format("{0}, color: {1}, radius: {2}",this._color, _color, GetRadius());
        }
    }
}

