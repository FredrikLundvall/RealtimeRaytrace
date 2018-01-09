using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealtimeRaytrace
{
    public interface IInputHandler
    {
        void InitiateInput();
        void HandleInput(Queue<IPlayerCommand> playerCommandQueue);
    }
}
