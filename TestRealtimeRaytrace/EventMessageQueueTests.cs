using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using RealtimeRaytrace;

namespace TestRealtimeRaytrace
{
    [TestClass]
    public class EventMessageQueueTests
    {
        [TestMethod]
        public void PullNextMessage_WhenEmpty_ReturnsInactiveMessage()
        {
            EventMessageQueue mq = new EventMessageQueue();

            Assert.AreEqual<string>(
                new EventMessage().ToString(),
                mq.PullNextMessage(new TimeSpan(9878)).ToString());
        }

        public void PullNextMessage_WhenMessageAfterTime_ReturnsInactiveMessage()
        {
            EventMessageQueue mq = new EventMessageQueue();
            EventMessage message = new EventMessage(new TimeSpan(1256), "Hasse", "Tage", "Roligt", "Lindeman");

            Assert.AreEqual<string>(
                new EventMessage().ToString(),
                mq.PullNextMessage(new TimeSpan(9878)).ToString());
        }

        [TestMethod]
        public void PullNextMessage_WhenOneMessageExists_ReturnsCorrectMessage()
        {
            EventMessageQueue mq = new EventMessageQueue();
            EventMessage message = new EventMessage(new TimeSpan(1256), "Hasse", "Tage", "Roligt", "Lindeman");
            mq.AddMessage(message);

            Assert.AreEqual<string>(
                message.ToString(),
                mq.PullNextMessage(new TimeSpan(1300)).ToString());
        }

        [TestMethod]
        public void PullNextMessage_WhenOneMessageWithrightTimeExists_ReturnsCorrectMessage()
        {
            EventMessageQueue mq = new EventMessageQueue();
            EventMessage message = new EventMessage(new TimeSpan(1256), "Hasse", "Tage", "Roligt", "Lindeman");
            mq.AddMessage(message);

            Assert.AreEqual<string>(
                message.ToString(),
                mq.PullNextMessage(new TimeSpan(1300)).ToString());
        }

        [TestMethod]
        public void PullNextMessage_WhenOneMessageExists_RemovesMessage()
        {
            EventMessageQueue mq = new EventMessageQueue();
            EventMessage message = new EventMessage(new TimeSpan(1256), "Hasse", "Tage", "Roligt", "Lindeman");
            mq.AddMessage(message);

            mq.PullNextMessage(new TimeSpan(1300)).ToString();

            Assert.AreEqual<string>(
                new EventMessage().ToString(),
                mq.PullNextMessage(new TimeSpan(1300)).ToString());
        }

        [TestMethod]
        public void PullNextMessage_WhenMultipleMessagesExists_ReturnsFirstMessage()
        {
            EventMessageQueue mq = new EventMessageQueue();
            mq.AddMessage(new EventMessage(new TimeSpan(9878), "Hasse", "Tage", "Roligt", "Lindeman"));
            mq.AddMessage(new EventMessage(new TimeSpan(3489), "Hasse", "Tage", "Roligt", "Lindeman"));
            EventMessage message = new EventMessage(new TimeSpan(1256), "Hasse", "Tage", "Roligt", "Lindeman");
            mq.AddMessage(message);
            mq.AddMessage(new EventMessage(new TimeSpan(5689), "Hasse", "Tage", "Roligt", "Lindeman"));
            mq.AddMessage(new EventMessage(new TimeSpan(2389), "Hasse", "Tage", "Roligt", "Lindeman"));

            Assert.AreEqual<string>(
                message.ToString(),
                mq.PullNextMessage(new TimeSpan(1300)).ToString());
        }
    }
}

