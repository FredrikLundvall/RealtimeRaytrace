using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using RealtimeRaytrace;

namespace TestRealtimeRaytrace
{
    [TestClass]
    public class MessageBrokerTests
    {
        [TestMethod]
        public void UpdateGameTime_WhenMessageIsDue_MessageIsPulledFromQueue()
        {
            EventMessageQueue messageQueue = new EventMessageQueue();
            EventMessage mes = new EventMessage(new TimeSpan(878), "Hasse", "Tage", "Roligt", "Lindeman");
            messageQueue.AddMessage(mes);
            messageQueue.AddMessage(new EventMessage(new TimeSpan(3489), "Hasse", "Tage", "Roligt", "Lindeman"));
            List<IMessageHandler> messageHandlerList = new List<IMessageHandler>();
            MessageHandlerSpy messageHandlerSpy = new MessageHandlerSpy("Tage");
            messageHandlerList.Add(messageHandlerSpy);
            MessageBroker broker = new MessageBroker(messageHandlerList, messageQueue);

            broker.UpdateGameTime(new TimeSpan(1300));

            Assert.AreEqual<int>(1, messageQueue.CountActiveMessages());
        }

        [TestMethod]
        public void UpdateGameTime_WhenMessageIsNotDue_MessageIsNotPulledFromQueue()
        {
            EventMessageQueue messageQueue = new EventMessageQueue();
            EventMessage mes = new EventMessage(new TimeSpan(878), "Hasse", "Tage", "Roligt", "Lindeman");
            messageQueue.AddMessage(mes);
            messageQueue.AddMessage(new EventMessage(new TimeSpan(3489), "Hasse", "Tage", "Roligt", "Lindeman"));
            List<IMessageHandler> messageHandlerList = new List<IMessageHandler>();
            MessageHandlerSpy messageHandlerSpy = new MessageHandlerSpy("Tage");
            messageHandlerList.Add(messageHandlerSpy);
            MessageBroker broker = new MessageBroker(messageHandlerList, messageQueue);

            broker.UpdateGameTime(new TimeSpan(130));

            Assert.AreEqual<int>(2, messageQueue.CountActiveMessages());
        }

        [TestMethod]
        public void UpdateGameTime_WhenReceiverIsFound_MessageIsSentForhandle()
        {
            EventMessageQueue messageQueue = new EventMessageQueue();
            EventMessage mes = new EventMessage(new TimeSpan(878), "Hasse", "Tage", "Roligt", "Lindeman");
            messageQueue.AddMessage(mes);
            messageQueue.AddMessage(new EventMessage(new TimeSpan(3489), "Hasse", "Tage", "Roligt", "Lindeman"));
            List<IMessageHandler> messageHandlerList = new List<IMessageHandler>();
            MessageHandlerSpy messageHandlerSpy = new MessageHandlerSpy("Tage");
            messageHandlerList.Add(messageHandlerSpy);
            MessageBroker broker = new MessageBroker(messageHandlerList, messageQueue);

            broker.UpdateGameTime(new TimeSpan(1300));

            Assert.AreEqual<string>(String.Format("HandleEvent:{0};", mes.ToString()), messageHandlerSpy.ActionLog);
        }

        [TestMethod]
        public void UpdateGameTime_WhenReceiverIsNotFound_MessageIsNotSentForhandle()
        {
            EventMessageQueue messageQueue = new EventMessageQueue();
            EventMessage mes = new EventMessage(new TimeSpan(878), "Hasse", "Tage", "Roligt", "Lindeman");
            messageQueue.AddMessage(mes);
            messageQueue.AddMessage(new EventMessage(new TimeSpan(3489), "Hasse", "Tage", "Roligt", "Lindeman"));
            List<IMessageHandler> messageHandlerList = new List<IMessageHandler>();
            MessageHandlerSpy messageHandlerSpy = new MessageHandlerSpy("Hasse");
            messageHandlerList.Add(messageHandlerSpy);
            MessageBroker broker = new MessageBroker(messageHandlerList, messageQueue);

            broker.UpdateGameTime(new TimeSpan(1300));

            Assert.IsNull(messageHandlerSpy.ActionLog);
        }
    }
}
