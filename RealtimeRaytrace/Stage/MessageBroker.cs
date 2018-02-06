using System;
using System.Collections.Generic;


namespace RealtimeRaytrace
{
    public class MessageBroker
    {
        List<IMessageHandler> _messageHandlerList;
        EventMessageQueue _eventMessageQueue;

        public MessageBroker(List<IMessageHandler> messageHandlerList, EventMessageQueue eventMessageQueue)
        {
            _messageHandlerList = messageHandlerList;
            _eventMessageQueue = eventMessageQueue;
        }

        public MessageBroker() : this(new List<IMessageHandler>(), new EventMessageQueue())
        {
        }

        public void UpdateGameTime(TimeSpan gameTime)
        {
            EventMessage eventMessage = _eventMessageQueue.PullNextMessage(gameTime);
            foreach(IMessageHandler messageHandler in _messageHandlerList)
            {
                messageHandler.HandleEvent(eventMessage,gameTime);
            }
        }

        public void AddMessageHandler(IMessageHandler messageHandler)
        {
            _messageHandlerList.Add(messageHandler);
        }
    }
}
