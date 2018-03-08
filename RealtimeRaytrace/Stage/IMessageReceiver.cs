using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public interface IMessageReceiver
    {
        bool IdMatchesReceiver(string receiver);
        void ReceiveMessage(EventMessage eventMessage, GameTime gameTime);
    }
}
