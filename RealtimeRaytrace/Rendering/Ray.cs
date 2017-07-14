using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public struct Ray
    {
        private
        Vector3 _start;
        Vector3 _direction;

        public Ray(Vector3 start, Vector3 direction)
        {
            _start = start;
            _direction = direction;
        }

        public Vector3 GetStart()
        {
            return _start;
        }

        public Vector3 GetDirection()
        {
            return _direction;
        }
    
    }




}
