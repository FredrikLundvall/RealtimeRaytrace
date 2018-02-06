using System;
using System.Collections.Generic;


namespace RealtimeRaytrace
{
    public interface IMessagePoster
    {
        void AddMessage(EventMessage eventMessage);
    }
}
