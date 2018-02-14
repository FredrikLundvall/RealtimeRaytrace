using System;
using System.Collections.Generic;


namespace RealtimeRaytrace
{
    public class MessageDispatcher: IMessageSender
    {
        List<IMessageReceiver> _messageReceiverList;
        IMessageQueue _messageQueue;

        public MessageDispatcher(List<IMessageReceiver> messageReceiverList, IMessageQueue messageQueue)
        {
            _messageReceiverList = messageReceiverList;
            _messageQueue = messageQueue;
        }

        public MessageDispatcher() : this(new List<IMessageReceiver>(), new EventMessageQueue())
        {
        }

        public void SendMessage(EventMessage eventMessage)
        {
            _messageQueue.AddMessage(eventMessage);
        }

        public void HandleMessages(TimeSpan gameTime)
        {
            EventMessage eventMessage = _messageQueue.PullNextMessage(gameTime);
            foreach(IMessageReceiver messageReceiver in _messageReceiverList)
            {
                if(messageReceiver.IdMatchesReceiver(eventMessage.GetReceiver()))
                    messageReceiver.ReceiveMessage(eventMessage,gameTime);
            }
        }

        public void RegisterMessageHandler(IMessageReceiver messageHandler)
        {
            _messageReceiverList.Add(messageHandler);
        }

        public void UnregisterMessageHandler(IMessageReceiver messageHandler)
        {
            _messageReceiverList.Remove(messageHandler);
        }
    }
}
