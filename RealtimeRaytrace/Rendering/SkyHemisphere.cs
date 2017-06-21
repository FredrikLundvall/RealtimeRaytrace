using System;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public enum HemisphereTextureType { Panorama, FisheyeVertical };

    public class SkyHemisphere : TextureBase, ISkyMap
    {
        protected double _horizSlide; //0 - 1
        protected HemisphereTextureType _hemisphereTextureType;

        public SkyHemisphere(GraphicsDeviceManager graphicsDeviceManager, string skyTextureFilename, HemisphereTextureType hemisphereTextureType, bool flipU, bool flipV = false, double horizSlide = 0) : base(graphicsDeviceManager, skyTextureFilename, flipU, flipV)
        {
            _horizSlide = horizSlide;
            _hemisphereTextureType = hemisphereTextureType;
        }

        public Color GetColorInSky(Ray ray)
        {
            double horizMult = 0.5 + _horizSlide * 0.5;
            double u = 0;
            double v = 0;
            AddressType at = AddressType.Clamping;

            switch (_hemisphereTextureType)
            {
                case HemisphereTextureType.FisheyeVertical:
                    //From:http://paulbourke.net/dome/fish2/
                    // Calculate fisheye angle and radius
                    double theta = Math.Atan2(ray.GetVector().Z, -ray.GetVector().X);
                    double phi = Math.Atan2(Math.Sqrt(ray.GetVector().X * ray.GetVector().X + ray.GetVector().Z * ray.GetVector().Z), ray.GetVector().Y);
                    double FOV = Math.PI * (1 + _horizSlide);
                    double width = 1;
                    double r = width * phi / FOV;

                    // Pixel in fisheye space
                    u = 0.5 * width + r * Math.Cos(theta);
                    v = 0.5 * width + r * Math.Sin(theta);
                    at = AddressType.Clamping;
                    break;
                case HemisphereTextureType.Panorama:
                default:
                    u = (0.5 + Math.Atan2(ray.GetVector().Z, ray.GetVector().X) / (Math.PI * 2));
                    //nästan rätt (eller är det vissa texturer som är fel?)...           
                    //float v = (float)( ray.GetVector().Y > 0 ? Math.Asin(ray.GetVector().Y) / (Math.PI / 2) : 0);
                    v = (1 - (Math.Asin(ray.GetVector().Y) / (Math.PI * horizMult) + (0.5 * _horizSlide)));
                    at = AddressType.WrapU_ClampV;
                    break;
            }

            return readTexture((float)u, (float)v, at);
        }
    }
}
