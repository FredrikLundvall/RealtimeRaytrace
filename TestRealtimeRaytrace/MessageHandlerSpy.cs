using System;
using System.Collections.Generic;


namespace RealtimeRaytrace
{
    public class MessageReceiverSpy : IMessageReceiver
    {
        string _id;
        public string ActionLog;

        public MessageReceiverSpy(string id)
        {
            _id = id;
        }

        public bool IdMatchesReceiver(string receiver)
        {
            return (_id == receiver);
        }

        public void ReceiveMessage(EventMessage eventMessage, TimeSpan gameTime)
        {
            ActionLog += ActionLog + String.Format("HandleEvent:{0};",eventMessage.ToString());
        }
    }
}
