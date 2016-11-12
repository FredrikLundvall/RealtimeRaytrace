using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealtimeRaytrace.Rendering
{
    public class RayTraversal
    {
        public int VoxelX;
        public int VoxelY;
        public int VoxelZ;
        public int StepX;
        public int StepY;
        public int StepZ;
        public float TMaxX;
        public float TMaxY;
        public float TMaxZ;
        public float TDeltaX;
        public float TDeltaY;
        public float TDeltaZ;

        public RayTraversal(int voxelX, int voxelY, int voxelZ, int stepX, int stepY, int stepZ)
        {
            VoxelX = voxelX;
            VoxelY = voxelY;
            VoxelZ = voxelZ;
            StepX = stepX;
            StepY = stepY;
            StepZ = stepZ;
        }

    }
}
