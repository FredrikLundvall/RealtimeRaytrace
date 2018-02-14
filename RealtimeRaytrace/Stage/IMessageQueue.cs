using System;
using System.Collections.Generic;


namespace RealtimeRaytrace
{
    public interface IMessageQueue
    {
        void AddMessage(EventMessage eventMessage);
        EventMessage PullNextMessage(TimeSpan gameTime);
    }
}
