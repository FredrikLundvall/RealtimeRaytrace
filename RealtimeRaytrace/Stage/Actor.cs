using System;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class Actor : IMessageReceiver
    {
        protected IMessageSender _messageSender;
        //protected AnimationArchive _animationArchive;
        //protected Vector3 _position;
        //Vector3 _direction; //Quaternion? Matrix?
        string _id; 

        public Actor(string id, IMessageSender messageSender)
        {
            _messageSender = messageSender;
            _id = id;
        }

        public bool IdMatchesReceiver(string receiver)
        {
            return (_id == receiver);
        }

        public void ReceiveMessage(EventMessage eventMessage, TimeSpan gameTime)
        {
            _messageSender.SendMessage(new EventMessage(TimeSpan.FromMilliseconds(gameTime.TotalMilliseconds + 3000), _id, eventMessage.GetSender(), "", ""));
        }
    }
}
