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
        //Kolla om detta påverkar performance
        public static int Next()
        {
            //return 0;
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
            //return 0;
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
    //public class ThreadSafeRandom
    //{
    //    private static readonly Random _global = new Random();
    //    [ThreadStatic]
    //    private static Random _local;

    //    public ThreadSafeRandom()
    //    {
    //        if (_local == null)
    //        {
    //            int seed;
    //            lock (_global)
    //            {
    //                seed = _global.Next();
    //            }
    //            _local = new Random(seed);
    //        }
    //    }
    //    public int Next()
    //    {
    //        return _local.Next();
    //    }
    //    public int Next(int max)
    //    {
    //        return _local.Next(max);
    //    }
    //    public int Next(int min,int max)
    //    {
    //        return _local.Next(min,max);
    //    }
    //}
}
