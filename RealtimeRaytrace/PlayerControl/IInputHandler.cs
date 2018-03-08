using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public interface IInputHandler
    {
        void InitiateInput();
        void HandleInput(GameTime gameTime, IMessageSender messageSender);
    }
}
