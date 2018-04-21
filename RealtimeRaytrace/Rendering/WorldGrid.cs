using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class WorldGrid : IWorld
    {
        private readonly Random _rnd = new Random();
        private GraphicsDeviceManager _graphicsDeviceManager;
        int _sizeX;
        int _sizeY;
        int _sizeZ;
        RealtimeRaytrace.BoundingBox _boundingBox;
        public Sphere _testSphereMove;
        protected List<Entity> _lightsourceEntityList;

        //TODO: Use fixed array for speed... (possibly a list in every position
        //private Entity[, ,] positionEntityArray = new Entity[500, 500, 500];

        //Serves as index for all entities that are indexed by voxel-position
        protected Dictionary<int, Entity> _voxelPositionEntityIndex;

        public WorldGrid()
        {
            _voxelPositionEntityIndex = new Dictionary<int, Entity>();
        }

        public void CreateWorld(GraphicsDeviceManager graphicsDeviceManager,int sizeX, int sizeY,int sizeZ)
        {
            _sizeX = sizeX;
            _sizeY = sizeY;
            _sizeZ = sizeZ;
            _boundingBox = new RealtimeRaytrace.BoundingBox(new Vector3(-sizeX / 2 - 0.5f, -sizeY / 2 - 0.5f, -sizeZ / 2 - 0.5f), new Vector3(sizeX / 2 + 0.5f, sizeY / 2 + 0.5f, sizeZ / 2 + 0.5f));

            _graphicsDeviceManager = graphicsDeviceManager;
            //for (int z = 0; z < sizeZ; z++)
            //{
            //    for (int x = 0; x < sizeX; x++)
            //    {
            //        AddEntity(new Sphere(new Vector3(x - sizeX / 2, sizeY / 2, z - sizeZ / 2), new Color(255, _rnd.Next(128), _rnd.Next(200))));
            //        AddEntity(new Sphere(new Vector3(x - sizeX / 2, -sizeY / 2, z - sizeZ / 2), new Color(128, 255, _rnd.Next(200))));
            //    }
            //}

            //for (int y = 0; y < sizeY; y++)
            //{
            //    for (int x = 0; x < sizeX; x++)
            //    {
            //        AddEntity(new Sphere(new Vector3(x - sizeX / 2, y - sizeY / 2, sizeZ / 2), new Color(128, _rnd.Next(200), 255)));
            //        AddEntity(new Sphere(new Vector3(x - sizeX / 2, y - sizeY / 2, -sizeZ / 2), new Color(255, 128, _rnd.Next(200))));
            //    }
            //}

            //for (int z = 0; z < sizeZ; z++)
            //{
            //    for (int y = 0; y < sizeY; y++)
            //    {
            //        AddEntity(new Sphere(new Vector3(sizeX / 2, y - sizeY / 2, z - sizeZ / 2), new Color(255, 255, _rnd.Next(200))));
            //        AddEntity(new Sphere(new Vector3(-sizeX / 2, y - sizeY / 2, z - sizeZ / 2), new Color(_rnd.Next(100), _rnd.Next(200), 255)));
            //    }
            //}


            //int levelY = -10;// (_rnd.Next(sizeY) - sizeY / 2);
            //for (int z = 0; z < sizeZ; z++)
            //{
            //    for (int x = 0; x < sizeX; x++)
            //    {
            //        AddEntity(new Sphere(new Vector3(x - sizeX / 2, levelY, z - sizeZ / 2), new Color(_rnd.Next(255), _rnd.Next(255), _rnd.Next(255))));
            //    }
            //}

            //for (int i = 0; i < 100; i++)
            //{
            //    AddEntity(new Sphere(new Vector3((_rnd.Next(sizeX) - sizeX / 2), (_rnd.Next(sizeY) - sizeY / 2), (_rnd.Next(sizeZ) - sizeZ / 2)), new Color(_rnd.Next(255), _rnd.Next(255), _rnd.Next(255)),0.15f));
            //}


            //SphereGroup group = new SphereGroup(n, new Vector3(0, 0, sizeZ / 2));
            //group.AddSphere(new Sphere(new Vector3(-0.1f, -0.1f, -0.1f), Color.Red, 0.5f));
            //group.AddSphere(new Sphere(new Vector3(0.1f, 0.1f, 0.1f), Color.White, 0.5f));
            //AddEntity(group);

            ////ITextureMap texture = new SphereTexture(_graphicsDeviceManager, @"Content\golfball.jpg", SphereTextureType.Photo360);
            //ITextureMap texture = new SphereTexture(_graphicsDeviceManager, @"Content\earth.jpg", SphereTextureType.Photo360);
            ////var texture = new GradientColorMap(Color.Purple, Color.Orange, Color.Red, Color.Green);

            //Sphere sphere = new Sphere(new Vector3(0.0f, 0.0f, 0.0f), Color.Blue, 0.3f, texture);
            //sphere.AddAntiSphere(new AntiSphere(new Vector3(2.4f, 0f, 0f), Color.Blue, 2.3f, texture));
            //sphere.AddAntiSphere(new AntiSphere(new Vector3(-2.4f, 0f, 0f), Color.Blue, 2.3f, texture));
            //sphere.AddAntiSphere(new AntiSphere(new Vector3(0f, 2.4f, 0f), Color.Blue, 2.3f, texture));
            //sphere.AddAntiSphere(new AntiSphere(new Vector3(0f, -2.4f, 0f), Color.Blue, 2.3f, texture));
            //sphere.AddAntiSphere(new AntiSphere(new Vector3(0f, 0f, 2.4f), Color.Blue, 2.3f, texture));
            //sphere.AddAntiSphere(new AntiSphere(new Vector3(0f, 0f, -2.4f), Color.Blue, 2.3f, texture));
            //AddEntity(sphere);

            _testSphereMove = new Sphere(new Vector3(-sizeX / 2, 2, sizeZ / 2), 0.5f, new SolidColorMap(Color.Red));
            AddEntity(_testSphereMove);
        }

        private int hashPosition(Vector3 pos)
        {
            return hashPosition(pos.X,pos.Y,pos.Z);
        }

        private int hashPosition(float x, float y, float z)
        {
            return hashPosition((int)x, (int)y, (int)z);
        }

        private int hashPosition(int x, int y, int z)
        {
            uint ux = (uint)(x);
            uint uy = (uint)(y);
            uint uz = (uint)(z);

            //Move the 8 bits of x, y and z to different positions in one int. then check the negative bit (31)
            return (int)((ux & 255) << 18 | (uy & 255) << 9 | (uz & 255) | ((ux & 2147483648) >> 5) | ((uy & 2147483648) >> 14) | ((uz & 2147483648) >> 23));
        }

        private void addEntityToVoxelPositionIndex(Entity entity)
        {
            Entity entityInVoxelPosition;
            int hashedVoxelPosition = hashPosition(entity.GetPosition());
            //Check if the position has any entity allready
            if (!_voxelPositionEntityIndex.TryGetValue(hashedVoxelPosition, out entityInVoxelPosition))
            {
                _voxelPositionEntityIndex.Add(hashedVoxelPosition, entity);
            }
            //TODO: What to do when a position is taken?
            //else
            //    positionEntityIndex.Add(entity.GetHashedPosition(), entity);
        }

        public void AddEntity(Entity entity)
        {
            addEntityToVoxelPositionIndex(entity);
        }

        public Entity GetEntity(int index)
        {
            return _voxelPositionEntityIndex[index];
        }

        public int EntityCount()
        {
            return _voxelPositionEntityIndex.Count;
        }

        public void ReIndexByVoxelPosition(Entity entity, Vector3 newPosition)
        {
            int newHashedVoxelPosition = hashPosition(newPosition);
            int hashedVoxelPosition = hashPosition(entity.GetPosition());

            if (newHashedVoxelPosition == hashedVoxelPosition)
            {
                entity.SetPosition(newPosition);
                return;
            }

            Entity entityInVoxelPosition;
            //Check if the entity is found by position
            if (_voxelPositionEntityIndex.TryGetValue(hashedVoxelPosition, out entityInVoxelPosition))
            {
                _voxelPositionEntityIndex.Remove(hashedVoxelPosition);

                //Change position to the new one
                entity.SetPosition(newPosition);
                //Add the entity
                addEntityToVoxelPositionIndex(entity);
            }
            else
                throw new Exception(String.Format("Entity not found in position: {0}", entity.GetPosition().ToString()));
        }

        public Entity GetEntityByVoxelPosition(IntVector position)
        {
            return GetEntityByVoxelPosition(position.X, position.Y, position.Z);
        }

        public virtual Entity GetEntityByVoxelPosition(int x, int y, int z)
        {
            //Return entity at the position
            Entity entityInPosition;
            _voxelPositionEntityIndex.TryGetValue(hashPosition(x,y,z), out entityInPosition);

            return entityInPosition;
        }

        public bool IsIntersecting(Ray r)
        {
            return _boundingBox.Intersect(r).IsHit();
        }

        public BoundingBoxIntersection Intersect(Ray r)
        {
            return _boundingBox.Intersect(r);
        }

        public Intersection GetClosestIntersection(Ray ray, float distance)
        {
            return getClosestIntersectionUsingVoxelTraverse(ray, distance);
        }

        protected Intersection getClosestIntersectionUsingVoxelTraverse(Ray ray, float distance)
        {
            //nearest hit 
            Intersection closestIntersection = new Intersection(true);
            //TODO: Speed up by making a structure of the empty grids?

            //Check the bounding box of the grid
            BoundingBoxIntersection intersectionBox = Intersect(ray);
            if (!intersectionBox.IsHit())
                return closestIntersection;

            Vector3 uPosition = ray.GetStart();
            uPosition = uPosition + ray.GetDirection() * intersectionBox.GetTFirstHit();
            Vector3 vDirection = ray.GetDirection();

            float scaledX = uPosition.X + 0.5f;
            float scaledY = uPosition.Y + 0.5f;
            float scaledZ = uPosition.Z + 0.5f;

            int voxelX = (int)Math.Floor(scaledX);
            int voxelY = (int)Math.Floor(scaledY);
            int voxelZ = (int)Math.Floor(scaledZ);

            int stepX = Math.Sign(vDirection.X);
            int stepY = Math.Sign(vDirection.Y);
            int stepZ = Math.Sign(vDirection.Z);

            int cellBoundaryX = voxelX + (stepX > 0 ? 1 : 0);
            int cellBoundaryY = voxelY + (stepY > 0 ? 1 : 0);
            int cellBoundaryZ = voxelZ + (stepZ > 0 ? 1 : 0);

            float tMaxX = (cellBoundaryX - scaledX) / vDirection.X;
            float tMaxY = (cellBoundaryY - scaledY) / vDirection.Y;
            float tMaxZ = (cellBoundaryZ - scaledZ) / vDirection.Z;

            if (Single.IsNaN(tMaxX) || Single.IsNegativeInfinity(tMaxX)) tMaxX = Single.PositiveInfinity;
            if (Single.IsNaN(tMaxY) || Single.IsNegativeInfinity(tMaxY)) tMaxY = Single.PositiveInfinity;
            if (Single.IsNaN(tMaxZ) || Single.IsNegativeInfinity(tMaxZ)) tMaxZ = Single.PositiveInfinity;

            float tDeltaX = stepX / vDirection.X;
            float tDeltaY = stepY / vDirection.Y;
            float tDeltaZ = stepZ / vDirection.Z;

            if (Single.IsNaN(tDeltaX) || Single.IsNegativeInfinity(tDeltaX)) tDeltaX = Single.PositiveInfinity;
            if (Single.IsNaN(tDeltaY) || Single.IsNegativeInfinity(tDeltaY)) tDeltaY = Single.PositiveInfinity;
            if (Single.IsNaN(tDeltaZ) || Single.IsNegativeInfinity(tDeltaZ)) tDeltaZ = Single.PositiveInfinity;

            int step = 0;
            do
            {
                Entity entityInPosition = GetEntityByVoxelPosition(voxelX, voxelY, voxelZ);
                if (entityInPosition != null)
                {
                    Intersection intersection = entityInPosition.Intersect(ray);
                    if ((!intersection.IsNull()) && (intersection.GetTFirstHit() > 0) && intersection.GetTFirstHit() < closestIntersection.GetTFirstHit())
                    {
                        closestIntersection = intersection;
                        break;
                    }
                }

                if (tMaxX < tMaxY && tMaxX < tMaxZ)
                {
                    voxelX += stepX;
                    tMaxX += tDeltaX;
                }
                else if (tMaxY < tMaxZ)
                {
                    voxelY += stepY;
                    tMaxY += tDeltaY;
                }
                else
                {
                    voxelZ += stepZ;
                    tMaxZ += tDeltaZ;
                }

                step++;
            } while (step < distance);

            return closestIntersection;
        }

        //Raytrace all spheres (for bug testing)
        protected Intersection GetClosestIntersectionByCheckingAllEntities(Ray ray, float distance)
        {
            //nearest hit 
            Intersection closestIntersection = new Intersection(true);

            foreach (Entity entityInPosition in _voxelPositionEntityIndex.Values)
            {
                Intersection intersection = entityInPosition.Intersect(ray);
                if ((!intersection.IsNull()) && (intersection.GetTFirstHit() > 0) && intersection.GetTFirstHit() < closestIntersection.GetTFirstHit())
                {
                    closestIntersection = intersection;
                }
            }

            return closestIntersection;
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

    }
}
