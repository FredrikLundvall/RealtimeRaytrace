using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace RealtimeRaytrace
{
    public class SkySphere : TextureBase, ISkyMap
    {
        public SkySphere(GraphicsDeviceManager graphicsDeviceManager, string skyTextureFilename) : base(graphicsDeviceManager, skyTextureFilename) { }

        public Color GetColorInSky(Ray ray)
        {
            //float u = (float)(Math.Asin(ray.GetVector().X) / Math.PI + 0.5);
            //float v = (float)(Math.Asin(ray.GetVector().Y) / Math.PI + 0.5);
            float u = (float)(0.5 - Math.Atan2(ray.GetVector().Z,ray.GetVector().X)/(Math.PI*2) );
            float v = (float)(0.5 - 2.0 * Math.Asin(ray.GetVector().Y) /(Math.PI * 2));
            return readTexture(u,v,AddressType.WrapU_ClampV);
        }
    }
}
