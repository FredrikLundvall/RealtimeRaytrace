using System;

namespace RealtimeRaytrace
{
    public interface IMessageHandler
    {
        void HandleEvent(EventMessage eventMessage, TimeSpan gameTime);
    }
}
