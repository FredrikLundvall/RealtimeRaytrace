using System;
using System.Collections.Generic;

namespace RealtimeRaytrace
{
    public class EventMessageQueue : IMessagePoster
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
            TimeSpan firstMessageTimeToArrive = TimeSpan.MaxValue;
            int firstMessageIndex = -1;
            for (int i = 0; i < _eventMessageQueue.Length; i++)
            {
                if (_eventMessageQueue[i].GetActive() && _eventMessageQueue[i].IsBeforeTimeToArrive(firstMessageTimeToArrive))
                {
                    firstMessageTimeToArrive = _eventMessageQueue[i].GetTimeToArrive();
                    firstMessageIndex = i;
                }
            }
            if (firstMessageIndex >= 0 && _eventMessageQueue[firstMessageIndex].IsBeforeTimeToArrive(gameTime))
            {
                EventMessage firstMessage = _eventMessageQueue[firstMessageIndex];
                _eventMessageQueue[firstMessageIndex] = new EventMessage();
                return firstMessage;
            }
            else
                return new EventMessage();
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
