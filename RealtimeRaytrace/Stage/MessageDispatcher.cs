using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

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

        public void HandleMessages(GameTime gameTime)
        {
            EventMessage eventMessage;
            do {
                eventMessage = _messageQueue.PullNextMessage(gameTime.TotalGameTime);
                foreach (IMessageReceiver messageReceiver in _messageReceiverList)
                {
                    if (messageReceiver.IdMatchesReceiver(eventMessage.GetReceiver()))
                        messageReceiver.ReceiveMessage(eventMessage, gameTime);
                }
            } while (eventMessage.GetActive() == true);
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
