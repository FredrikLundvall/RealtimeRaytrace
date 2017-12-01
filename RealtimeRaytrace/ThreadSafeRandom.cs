using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealtimeRaytrace
{


    public static class ThreadSafeRandom
    {
        private static Random _global = new Random();
        [ThreadStatic]
        private static Random _local;
        //Check what this does to the performance
        public static int Next()
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return inst.Next();
        }

        public static int Next(int min, int max)
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return inst.Next(min, max);
        }
    }
}
