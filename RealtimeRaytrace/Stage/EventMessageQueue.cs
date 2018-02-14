using System;
using System.Collections.Generic;

namespace RealtimeRaytrace
{
    public class EventMessageQueue : IMessageQueue
    {
        //TODO: Use other implementation for speed purposes (FastPriorityQueue maybe?)
        private EventMessage[] _eventMessageQueue;

        public EventMessageQueue()
        {
            _eventMessageQueue = new EventMessage[500];
        }

        public void AddMessage(EventMessage eventMessage)
        {
            for (int i = 0; i < _eventMessageQueue.Length; i++)
            {
                if (!_eventMessageQueue[i].GetActive())
                {
                    _eventMessageQueue[i] = eventMessage;
                    break;
                }
            }
        }

        public EventMessage PullNextMessage(TimeSpan gameTime)
        {
            //TimeSpan firstMessageTimeToArrive = TimeSpan.MaxValue;
            //int firstMessageIndex = -1;
            for (int i = 0; i < _eventMessageQueue.Length; i++)
            {
                //if (_eventMessageQueue[i].GetActive() && _eventMessageQueue[i].IsBeforeTimeToArrive(firstMessageTimeToArrive))
                if (_eventMessageQueue[i].GetActive() && _eventMessageQueue[i].IsBeforeTimeToArrive(gameTime))
                {
                    //firstMessageTimeToArrive = _eventMessageQueue[i].GetTimeToArrive();
                    //firstMessageIndex = i;
                    EventMessage pulledMessage = _eventMessageQueue[i];
                    _eventMessageQueue[i] = EventMessage.CreateInactiveEventMessage();
                    return pulledMessage;
                }
            }
            //if (firstMessageIndex >= 0 && _eventMessageQueue[firstMessageIndex].IsBeforeTimeToArrive(gameTime))
            //{
            //    EventMessage firstMessage = _eventMessageQueue[firstMessageIndex];
            //    _eventMessageQueue[firstMessageIndex] = new EventMessage();
            //    return firstMessage;
            //}
            //else
            return EventMessage.CreateInactiveEventMessage();
        }

        public int CountActiveMessages()
        {
            int activeMessages = 0;
            for (int i = 0; i < _eventMessageQueue.Length; i++)
            {
                activeMessages += _eventMessageQueue[i].GetActive() ? 1 : 0;
            }
            return activeMessages;
        }
    }
}
