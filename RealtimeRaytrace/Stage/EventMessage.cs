using System;
using System.Collections.Generic;


namespace RealtimeRaytrace
{
    public struct EventMessage : ICommanding
    {
        readonly private TimeSpan _timeToArrive;
        readonly private string _sender;
        readonly private string _receiver;
        readonly private EventMessageType _messageType;
        readonly private float _amount;
        private bool _active;

        public void Execute(ICommandable commandable, float elapsedTotalSeconds)
        {
            switch (_messageType)
            {
                case EventMessageType.MoveDepth:
                    commandable.MoveDepth(_amount, elapsedTotalSeconds);
                    break;
                case EventMessageType.MoveHeight:
                    commandable.MoveHeight(_amount, elapsedTotalSeconds);
                    break;
                case EventMessageType.MoveSide:
                    commandable.MoveSide(_amount, elapsedTotalSeconds);
                    break;
                case EventMessageType.RotatePitch:
                    commandable.RotatePitch(_amount, elapsedTotalSeconds);
                    break;
                case EventMessageType.RotateYaw:
                    commandable.RotateYaw(_amount, elapsedTotalSeconds);
                    break;
            }
        }

        public EventMessage(TimeSpan timeToArrive, string sender, string receiver, EventMessageType messageType = EventMessageType.DoNothing, float amount = 0)
        {
            _timeToArrive = timeToArrive;
            _sender = sender;
            _receiver = receiver;
            _messageType = messageType;
            _amount = amount;
            _active = true;
        }

        public static EventMessage CreateInactiveEventMessage()
        {
            return new EventMessage();
        }

        public bool IsBeforeTimeToArrive(TimeSpan timeToArrive)
        {
            return (_timeToArrive < timeToArrive);
        }

        public bool GetActive()
        {
            return _active;
        }

        public string GetSender()
        {
            return _sender;
        }

        public string GetReceiver()
        {
            return _receiver;
        }

        public TimeSpan GetTimeToArrive()
        {
            return _timeToArrive;
        }

        public EventMessageType GetMessageType()
        {
            return _messageType;
        }

        public float GetAmount()
        {
            return _amount;
        }

        public override string ToString()
        {
            return string.Format("{0};{1};{2};{3};{4};{5}", _timeToArrive.ToString(), _sender, _receiver, _messageType.ToString(), _amount.ToString("F4"), _active.ToString());
        }

    }
}
