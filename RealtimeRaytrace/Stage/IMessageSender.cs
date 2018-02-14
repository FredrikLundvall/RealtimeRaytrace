using System;
using System.Collections.Generic;


namespace RealtimeRaytrace
{
    public interface IMessageSender
    {
        void SendMessage(EventMessage eventMessage);
    }
}
