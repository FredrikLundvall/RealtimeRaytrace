using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class AnimationFrame
    {
        Entity _entity;
        TimeSpan _timeSpan;
        Vector3 _movement;
        Vector3 _rotation; //Quaternion? Matrix?
        float _force;

        public AnimationFrame(Entity entity, TimeSpan timeSpan, Vector3 movement, Vector3 rotation, float force = 1.0f)
        {
            _entity = entity;
            _timeSpan = timeSpan;
            _movement = movement;
            _rotation = rotation;
            _force = force;
        }
    }
}
