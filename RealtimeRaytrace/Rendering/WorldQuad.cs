﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class WorldQuad : IWorld
    {
        private readonly Random _rnd = new Random();
        private GraphicsDeviceManager _graphicsDeviceManager;
        int _sizeX;
        int _sizeY;
        int _sizeZ;
        SphereBound _sphereBound;
        public Sphere _testSphereMove;

        protected List<Entity> _entityList;
        protected List<Entity> _lightsourceEntityList;

        public WorldQuad()
        {
            _entityList = new List<Entity>();
            _lightsourceEntityList = new List<Entity>();
        }

        public void CreateWorld(GraphicsDeviceManager graphicsDeviceManager, int sizeX, int sizeY, int sizeZ)
        {
            _sizeX = sizeX;
            _sizeY = sizeY;
            _sizeZ = sizeZ;
            Vector3 worldsize = new Vector3(sizeX /2, sizeY/2, sizeZ/2);

            _sphereBound = new SphereBound(new Vector3(0, 0, 0), worldsize.Length());

            _graphicsDeviceManager = graphicsDeviceManager;

            //SphereGroup group = new SphereGroup(new Vector3(0, 0, sizeZ / 2));
            //group.AddSphere(new Sphere(new Vector3(-0.1f, -0.1f, -0.1f), Color.Red, 0.5f));
            //group.AddSphere(new Sphere(new Vector3(0.1f, 0.1f, 0.1f), Color.White, 0.5f));
            //AddEntity(group);

            ////ITextureMap texture = new SphereTexture(_graphicsDeviceManager, @"Content\golfball.jpg", SphereTextureType.Photo360);
            ////var texture = new GradientColorMap(Color.Purple, Color.Orange, Color.Red, Color.Green);

            ITextureMap texture = new SphereTexture(_graphicsDeviceManager, @"Content\earth.png", SphereTextureType.Photo360);
            ITextureMap texture2 = new SphereTexture(_graphicsDeviceManager, @"Content\golfball.jpg", SphereTextureType.Photo360);
            ITextureMap texture3 = new GradientColorMap(Color.Purple, Color.Orange, Color.Red, Color.Green);


            //Sphere sphere = new Sphere(new Vector3(0, 0, 0), Color.Blue, 430.3f, texture);
            //AddEntity(sphere);

            Sphere sphere = new Sphere(new Vector3(0, 0, 0), 430.3f, texture);
            sphere.AddAntiSphere(new AntiSphere(new Vector3(0, 330f, -330.0f), 39f, texture));
            AddEntity(sphere);

            //SphereGroup group = new SphereGroup(new Vector3(0, 0, 0));
            //group.AddSphere(new Sphere(new Vector3(0, 0, 0), Color.Blue, 430.3f, texture));
            //group.AddSphere(new Sphere(new Vector3(0, 0, -430.3f), Color.Red, 320f, texture));
            //AddEntity(group);

            //_testSphereMove = new Sphere(new Vector3(-sizeX / 2, 2, sizeZ / 2), Color.Red, 0.5f, null);
            //AddEntity(_testSphereMove);
        }

        public void AddEntity(Entity entity)
        {
            _entityList.Add(entity);
        }

        public Entity GetEntity(int index)
        {
            return _entityList[index];
        }

        public int EntityCount()
        {
            return _entityList.Count;
        }

        public void AddLightsourceEntity(Entity entity)
        {
            _lightsourceEntityList.Add(entity);
        }

        public Entity GetLightsourceEntity(int index)
        {
            return _lightsourceEntityList[index];
        }

        public int LightsourceEntityCount()
        {
            return _lightsourceEntityList.Count;
        }

        public BoundingBoxIntersection Intersect(Ray r)
        {
            if (_sphereBound.IsIntersecting(r)) 
                return new BoundingBoxIntersection(1.0f);
            else
                return BoundingBoxIntersection.CreateNullBoundingBoxIntersection();
        }

        public bool IsIntersecting(Ray r)
        {
            return _sphereBound.IsIntersecting(r);
        }

        public Intersection GetClosestIntersection(Ray ray, float distance)
        {
            return GetClosestIntersectionByCheckingAllEntities(ray, distance);
        }

        //Raytrace all spheres (for bug testing)
        protected Intersection GetClosestIntersectionByCheckingAllEntities(Ray ray, float distance)
        {
            //nearest hit 
            Intersection closestIntersection = new Intersection(true);

            if(_sphereBound.IsIntersecting(ray))
                foreach (Entity entity in _entityList)
                {
                    Intersection intersection = entity.Intersect(ray);
                    if ((!intersection.IsNull()) && (intersection.GetTFirstHit() > 0) && intersection.GetTFirstHit() < closestIntersection.GetTFirstHit())
                    {
                        closestIntersection = intersection;
                    }
                }

            return closestIntersection;
        }

    }
}
