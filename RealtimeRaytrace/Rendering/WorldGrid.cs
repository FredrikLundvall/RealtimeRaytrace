using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class WorldGrid
    {
        private readonly Random _rnd = new Random();

        //TODO: Use fixed array for speed... (possibly a list in every position
        //private Entity[, ,] positionEntityArray = new Entity[500, 500, 500];

        //Serves as index for all entities that are indexed by voxel-position
        protected Dictionary<int, Entity> _voxelPositionEntityIndex;

        public WorldGrid()
        {
            _voxelPositionEntityIndex = new Dictionary<int, Entity>();
        }

        public void CreateCubeWorld(int sizeX, int sizeY,int sizeZ)
        {
            int n = 0;

            for (int z = 0; z < sizeZ; z++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    AddEntity(new Sphere(n, new Vector3(x - sizeX / 2, sizeY / 2, z - sizeZ / 2), new Color(255, _rnd.Next(128), _rnd.Next(200))));
                    n++;
                    AddEntity(new Sphere(n, new Vector3(x - sizeX / 2, -sizeY / 2, z - sizeZ / 2), new Color(128, 255, _rnd.Next(200))));
                    n++;
                }
            }

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    AddEntity(new Sphere(n, new Vector3(x - sizeX / 2, y - sizeY / 2, sizeZ / 2), new Color(128, _rnd.Next(200), 255)));
                    n++;
                    AddEntity(new Sphere(n, new Vector3(x - sizeX / 2, y - sizeY / 2, -sizeZ / 2), new Color(255, 128, _rnd.Next(200))));
                    n++;
                }
            }

            for (int z = 0; z < sizeZ; z++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    AddEntity(new Sphere(n, new Vector3(sizeX / 2, y - sizeY / 2, z - sizeZ / 2), new Color(255, 255, _rnd.Next(200))));
                    n++;
                    AddEntity(new Sphere(n, new Vector3(-sizeX / 2, y - sizeY / 2, z - sizeZ / 2), new Color(_rnd.Next(100), _rnd.Next(200), 255)));
                    n++;
                }
            }


            //int levelY = -10;// (_rnd.Next(sizeY) - sizeY / 2);
            //for (int z = 0; z < sizeZ; z++)
            //{
            //    for (int x = 0; x < sizeX; x++)
            //    {
            //        AddEntity(new Sphere(n, new Vector3(x - sizeX / 2, levelY, z - sizeZ / 2), new Color(_rnd.Next(255), _rnd.Next(255), _rnd.Next(255))));
            //        n++;
            //    }
            //}

            //for (int i = 0; i < 100; i++)
            //{
            //    AddEntity(new Sphere(n, new Vector3((_rnd.Next(sizeX) - sizeX / 2), (_rnd.Next(sizeY) - sizeY / 2), (_rnd.Next(sizeZ) - sizeZ / 2)), new Color(_rnd.Next(255), _rnd.Next(255), _rnd.Next(255)),0.15f));
            //    n++;
            //}

            AddEntity(new Sphere(n, new Vector3(0, 0, 0), Color.Red,0.75f));
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
            //Add the entity to the index-by-position-list
            if (entity.GetIsIndexedByPosition())
                addEntityToVoxelPositionIndex(entity);
        }

        public void ReIndexByVoxelPosition(Entity entity, Vector3 newPosition)
        {
            int newHashedVoxelPosition = hashPosition(newPosition);
            int hashedVoxelPosition = hashPosition(entity.GetPosition());

            if (newHashedVoxelPosition == hashedVoxelPosition || !entity.GetIsIndexedByPosition())
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
                throw new Exception(String.Format("Entity not found in position:  ", entity.GetPosition().ToString()));
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

        public virtual ICollection<Entity> GetEntityCollection()
        {
            return _voxelPositionEntityIndex.Values;
        }

    }
}
