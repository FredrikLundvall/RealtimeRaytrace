using System;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class Actor : IMessageHandler
    {
        protected IMessagePoster _messagePoster;
        protected AnimationArchive _animationArchive;
        protected Vector3 _position;
        Vector3 _direction; //Quaternion? Matrix?
        string _id; 

        public Actor(IMessagePoster messagePoster, AnimationArchive animationArchive,  string id)
        {
            _messagePoster = messagePoster;
            _animationArchive = animationArchive;
            _id = id;
        }

        public void HandleEvent(EventMessage eventMessage, TimeSpan gameTime)
        {
            if (!eventMessage.MatchesReceiver(_id))
                return;                 
        }

        public AnimationArchive GetAnimationArchive()
        {
            return _animationArchive;
        } 
    }
}
