using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class Sphere : SphereBase
    {
        //List<AntiSphere> _antiSphereList;

        public Sphere(Vector3 position, Color color, float radius = 0.5f, ITextureMap textureMap = null)
            : base(position, color, radius, textureMap)
        {
            //_antiSphereList = new List<AntiSphere>(0);
        }

        public void AddAntiSphere(AntiSphere sphere)
        {
            //_antiSphereList.Add(sphere);
            _entityList.Add(sphere);
        }

        protected void calculateAntiSphereIntersections(Ray ray, out AntiIntersection[] antiSphereIntersectionList, out bool antiSphereIntersectionListIsHit)
        {
            //Local array will help about thread issues
            antiSphereIntersectionList = new AntiIntersection[_entityList.Count];
            antiSphereIntersectionListIsHit = false;
            for (int i = 0; i < _entityList.Count; i++)
            {
                var intersection = (_entityList[i] as AntiSphere).AntiIntersect(ray, this);
                antiSphereIntersectionListIsHit = antiSphereIntersectionListIsHit | intersection.IsHit();
                insertAntiIntersectionSortedToArray(antiSphereIntersectionList, i, intersection);
            }
        }

        protected void insertAntiIntersectionSortedToArray(AntiIntersection[] array, int maxPosition, AntiIntersection item)
        {
            if (maxPosition == 0)
            {
                array[0] = item;
                return;
            }
            //Sort at insert
            for (int i = maxPosition - 1; i >= 0; i--)
            {
                if (item.GetTNear() > array[i].GetTNear() | item.IsNull())
                {
                    array[i + 1] = item;
                    break;
                }
                else
                {
                    array[i + 1] = array[i];
                    if (i == 0)
                        array[0] = item;
                }
            }
        }

        public override Intersection Intersect(Ray ray)
        {
            Intersection sphereIntersection = base.Intersect(ray);
            if (sphereIntersection.IsHit() && _entityList.Count != 0)
                sphereIntersection = getClosestIntersectionFromAntiSpheres(sphereIntersection, ray);
            return sphereIntersection;
        }

        protected Intersection getClosestIntersectionFromAntiSpheres(Intersection sphereIntersection, Ray ray)
        {
            bool insideSphere = sphereIntersection.GetTFirstHit() == sphereIntersection.GetTFar();
            AntiIntersection[] antiSphereIntersectionList;
            bool antiSphereIntersectionListIsHit;
            calculateAntiSphereIntersections(ray, out antiSphereIntersectionList, out antiSphereIntersectionListIsHit);
            if (antiSphereIntersectionListIsHit)
            {
                AntiIntersection farthestAntiIntersection = getFarthestAntiIntersection(sphereIntersection.GetTNear(), sphereIntersection.GetTFar(), antiSphereIntersectionList);
                if (farthestAntiIntersection.IsHit())
                {
                    var intersection = this.Intersect(farthestAntiIntersection.GetPositionFar(), farthestAntiIntersection.GetNormalFarTexture());
                    sphereIntersection = farthestAntiIntersection.CreateIntersection(intersection.GetNormalFirstHitTexture());
                }
                else if (farthestAntiIntersection.IsInfinite())
                    sphereIntersection = new Intersection(true);
            }
            return sphereIntersection;
        }

        private AntiIntersection getFarthestAntiIntersection(float tMin, float tMax, AntiIntersection[] antiSphereIntersectionList)
        {
            AntiIntersection farthestAntiIntersection = AntiIntersection.CreateNullAntiIntersection();
            tMin = (tMin < 0) ? 0 : tMin;
            float tCurrent = tMin;
            //compare the hits in the antilist
            foreach (AntiIntersection antiIntersection in antiSphereIntersectionList)
            {
                //Break here, because the sorting places all the missed intersections in the end
                if (!antiIntersection.IsHit())
                    break;
                if(antiIntersection.isDistanceInside(tCurrent))
                {
                    //Move away the nearest hit to the other side of the AntiSphere
                    tCurrent = antiIntersection.GetTFar();
                    farthestAntiIntersection = antiIntersection;
                }
                if (tCurrent >= tMax)
                {
                    //It went through
                    farthestAntiIntersection = AntiIntersection.CreateInfiniteAntiIntersection();
                    break;
                }
            }
            return farthestAntiIntersection;
        }
    }
}

