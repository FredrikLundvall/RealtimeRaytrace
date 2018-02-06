using System;
using System.Collections.Generic;


namespace RealtimeRaytrace
{
    public struct EventMessage
    {
        readonly private TimeSpan _timeToArrive;
        readonly private string _sender;
        readonly private string _receiver;
        readonly private string _type;
        readonly private string _data;
        private bool _active;


        public EventMessage(TimeSpan timeToArrive,string sender,string receiver,string type,string data)
        {
            _timeToArrive = timeToArrive;
            _sender = sender;
            _receiver = receiver;
            _type = type;
            _data = data;
            _active = true;
        }

        public bool MatchesReceiver(string receiver)
        {
            return (_receiver == receiver);
        }

        public bool IsBeforeTimeToArrive(TimeSpan timeToArrive)
        {
            return (_timeToArrive < timeToArrive);
        }

        public bool GetActive()
        {
            return _active;
        }

        public TimeSpan GetTimeToArrive()
        {
            return _timeToArrive;
        }

        public override string ToString()
        {
            return string.Format("{0};{1};{2};{3};{4};{5}", _timeToArrive.ToString(), _sender, _receiver, _type, _data, _active.ToString());
        }

    }
}
