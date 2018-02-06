using System;
using System.Collections.Generic;


namespace RealtimeRaytrace
{
    public class MessageHandlerSpy : IMessageHandler
    {
        string _id;
        public string ActionLog;

        public MessageHandlerSpy(string id)
        {
            _id = id;
        }

        public void HandleEvent(EventMessage eventMessage, TimeSpan gameTime)
        {
            if (!eventMessage.MatchesReceiver(_id))
                return;
            ActionLog += ActionLog + String.Format("HandleEvent:{0};",eventMessage.ToString());
        }
    }
}
