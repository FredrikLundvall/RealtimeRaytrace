using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RealtimeRaytrace
{
    public interface IRenderer
    {
        void Render(GameTime gameTime);
        Camera MainCamera { get;}
    }
}
