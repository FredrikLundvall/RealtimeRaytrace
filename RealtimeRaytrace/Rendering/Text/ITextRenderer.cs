using System;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public interface ITextRenderer
    {
        void SetText(int index, string text, Vector2 pos, Color color);
        void RemoveText(int index);
        void RemoveAll();
        void Render(GameTime gameTime);
    }
}
