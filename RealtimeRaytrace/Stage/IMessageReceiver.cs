using System;

namespace RealtimeRaytrace
{
    public interface IMessageReceiver
    {

        bool IdMatchesReceiver(string receiver);
        void ReceiveMessage(EventMessage eventMessage, TimeSpan gameTime);
    }
}
