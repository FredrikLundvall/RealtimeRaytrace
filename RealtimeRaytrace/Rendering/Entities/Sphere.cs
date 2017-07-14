using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class Sphere : SphereBase
    {
        private static object _MyLock = new object();

        List<AntiSphereAntiIntersection> _antiSphereIntersectionList;
        bool _antiSphereIntersectionListIsHit;

        public Sphere(int index, bool isIndexedByPosition, Vector3 position, Color color, float radius = 0.5f, ITextureMap textureMap = null)
            : base(index, isIndexedByPosition, position, color, radius, textureMap)
        {
            _antiSphereIntersectionList = new List<AntiSphereAntiIntersection>();
            _antiSphereIntersectionListIsHit = false;
        }

        public Sphere(int index, Vector3 position, Color color, float radius = 0.5f, ITextureMap textureMap = null)
            : this(index, true, position, color, radius, textureMap)
        {
        }

        public void AddAntiSphere(AntiSphere sphere)
        {
            _antiSphereIntersectionList.Add(new AntiSphereAntiIntersection(sphere));
        }

        public void CalculateAntiSphereIntersections(Ray ray)
        {
            _antiSphereIntersectionListIsHit = false;
            //träffarna för alla i antilist
            foreach (AntiSphereAntiIntersection sphereBaseIntersection in _antiSphereIntersectionList)
            {
                var intersection = sphereBaseIntersection.GetAntiSphere().AntiIntersect(ray,this);
                _antiSphereIntersectionListIsHit = _antiSphereIntersectionListIsHit | intersection.IsHit();

                sphereBaseIntersection.SetAntiIntersection(intersection);
            }
            //sortera så att de närmaste träffarna kommer först
            //TODO: om låsningen ska vara kvar måste sorteringen göras annorlunda så att inte GC triggas
            _antiSphereIntersectionList.Sort((x, y) => x.GetAntiIntersection().GetTNear().CompareTo(y.GetAntiIntersection().GetTNear()));
        }

        public override Intersection Intersect(Ray ray)
        {
            Intersection closestIntersection = base.Intersect(ray);
            //TODO: Kolla om det är bättre att räkna ut denna på nytt i varje tråd och slippa lock (alltså spara inte några resultat)
            lock (_MyLock)
            {
                CalculateAntiSphereIntersections(ray);
                if (_antiSphereIntersectionListIsHit)
                {
                    //compare the hits in the antilist
                    float tMax = closestIntersection.GetTFar();
                    float tFar = closestIntersection.GetTFirstHit();
                    foreach (AntiSphereAntiIntersection _antiSphereIntersection in _antiSphereIntersectionList)
                    {
                        //Take the next if this wasn't a hit (should possibly break here, because the sorting place all the missed antiSpheres in the end)
                        if (!_antiSphereIntersection.GetAntiIntersection().IsHit())
                            continue;

                        var antiIntersection = _antiSphereIntersection.GetAntiIntersection();
                        //Stop if this antilist-hit starts further away than the saved "nearest" hit
                        if (antiIntersection.GetTNear() > tFar)
                            break;

                        if (antiIntersection.GetTNear() < tFar && antiIntersection.GetTFar() > tFar)
                        {
                            //Move away the nearest hit to the other side of the AntiSphere
                            tFar = antiIntersection.GetTFar();
                            //Check where the normal hits the parent sphere (this one) and use for the texture
                            var intersection = this.Intersect(antiIntersection.GetPositionFar(),antiIntersection.GetNormalFarTexture());
                            closestIntersection = antiIntersection.CreateIntersection(intersection.GetNormalFirstHitTexture());
                        }

                        if (tFar > tMax)
                        {
                            //Stop here, because the nearest hit is further away than the other side
                            closestIntersection = new Intersection(true);
                            break;
                        }
                    }
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

