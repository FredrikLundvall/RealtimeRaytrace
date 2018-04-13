using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using RealtimeRaytrace;

namespace TestRealtimeRaytrace
{
    [TestClass]
    public class MessageDispatcherTests
    {
        [TestMethod]
        public void UpdateGameTime_WhenMessageIsDue_MessageIsPulledFromQueue()
        {
            EventMessageQueue messageQueue = new EventMessageQueue();
            EventMessage mes = new EventMessage(new TimeSpan(878), "Hasse", "Tage", EventMessageType.DoNothing, 0.0f);
            messageQueue.AddMessage(mes);
            messageQueue.AddMessage(new EventMessage(new TimeSpan(3489), "Hasse", "Tage", EventMessageType.DoNothing, 0.0f));
            List<IMessageReceiver> messageReceiverList = new List<IMessageReceiver>();
            MessageReceiverSpy messageReceiverSpy = new MessageReceiverSpy("Tage");
            messageReceiverList.Add(messageReceiverSpy);
            MessageDispatcher broker = new MessageDispatcher(messageReceiverList, messageQueue);

            broker.HandleMessages(new GameTime(new TimeSpan(1300), new TimeSpan(1300)));

            Assert.AreEqual<int>(1, messageQueue.CountActiveMessages());
        }

        [TestMethod]
        public void UpdateGameTime_WhenMessageIsNotDue_MessageIsNotPulledFromQueue()
        {
            EventMessageQueue messageQueue = new EventMessageQueue();
            EventMessage mes = new EventMessage(new TimeSpan(878), "Hasse", "Tage", EventMessageType.DoNothing, 0.0f);
            messageQueue.AddMessage(mes);
            messageQueue.AddMessage(new EventMessage(new TimeSpan(3489), "Hasse", "Tage", EventMessageType.DoNothing, 0.0f));
            List<IMessageReceiver> messageReceiverList = new List<IMessageReceiver>();
            MessageReceiverSpy messageReceiverSpy = new MessageReceiverSpy("Tage");
            messageReceiverList.Add(messageReceiverSpy);
            MessageDispatcher broker = new MessageDispatcher(messageReceiverList, messageQueue);

            broker.HandleMessages(new GameTime(new TimeSpan(130), new TimeSpan(130)));

            Assert.AreEqual<int>(2, messageQueue.CountActiveMessages());
        }

        [TestMethod]
        public void UpdateGameTime_WhenReceiverIsFound_MessageIsSentForhandle()
        {
            EventMessageQueue messageQueue = new EventMessageQueue();
            EventMessage mes = new EventMessage(new TimeSpan(878), "Hasse", "Tage", EventMessageType.DoNothing, 0.0f);
            messageQueue.AddMessage(mes);
            messageQueue.AddMessage(new EventMessage(new TimeSpan(3489), "Hasse", "Tage", EventMessageType.DoNothing, 0.0f));
            List<IMessageReceiver> messageReceiverList = new List<IMessageReceiver>();
            MessageReceiverSpy messageReceiverSpy = new MessageReceiverSpy("Tage");
            messageReceiverList.Add(messageReceiverSpy);
            MessageDispatcher broker = new MessageDispatcher(messageReceiverList, messageQueue);

            broker.HandleMessages(new GameTime(new TimeSpan(1300),new TimeSpan(1300)));

            Assert.AreEqual<string>(String.Format("HandleEvent:{0};", mes.ToString()), messageReceiverSpy.ActionLog);
        }

        [TestMethod]
        public void UpdateGameTime_WhenReceiverIsNotFound_MessageIsNotSentForhandle()
        {
            EventMessageQueue messageQueue = new EventMessageQueue();
            EventMessage mes = new EventMessage(new TimeSpan(878), "Hasse", "Tage", EventMessageType.DoNothing, 0.0f);
            messageQueue.AddMessage(mes);
            messageQueue.AddMessage(new EventMessage(new TimeSpan(3489), "Hasse", "Tage", EventMessageType.DoNothing, 0.0f));
            List<IMessageReceiver> messageReceiverList = new List<IMessageReceiver>();
            MessageReceiverSpy messageReceiverSpy = new MessageReceiverSpy("Hasse");
            messageReceiverList.Add(messageReceiverSpy);
            MessageDispatcher broker = new MessageDispatcher(messageReceiverList, messageQueue);

            broker.HandleMessages(new GameTime(new TimeSpan(1300), new TimeSpan(1300)));

            Assert.IsNull(messageReceiverSpy.ActionLog);
        }
    }
}
