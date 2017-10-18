using System;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class SkyHemisphereTexture : ArrayTexture, ISkyMap
    {
        protected double _horizSlide; //0 - 1
        protected HemisphereTextureType _hemisphereTextureType;
        protected bool _flipU;
        protected bool _flipV;

        public SkyHemisphereTexture(GraphicsDeviceManager graphicsDeviceManager, string skyTextureFilename, HemisphereTextureType hemisphereTextureType, bool flipU, bool flipV = false, double horizSlide = 0) : base(graphicsDeviceManager, skyTextureFilename)
        {
            _horizSlide = horizSlide;
            _hemisphereTextureType = hemisphereTextureType;
            _flipU = flipU;
            _flipV = flipV;
        }

        public Color GetColorInSky(Ray ray)
        {
            return GetColorInSky(ray.GetDirection());
        }

        public Color GetColorInSky(Vector3 direction)
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
                    double theta = Math.Atan2(direction.Z, -direction.X);
                    double phi = Math.Atan2(Math.Sqrt(direction.X * direction.X + direction.Z * direction.Z), direction.Y);
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
                    u = 0.5 + Math.Atan2(direction.Z, direction.X) / (Math.PI * 2);
                    //nästan rätt (eller är det vissa texturer som är fel?)...           
                    //float v = (float)( direction.Y > 0 ? Math.Asin(direction.Y) / (Math.PI / 2) : 0);
                    v = (1 - (Math.Asin(direction.Y) / (Math.PI * horizMult) + (0.5 * _horizSlide)));
                    at = AddressType.WrapU_ClampV;
                    break;
            }

            return readTexture((float)u, (float)v, at, _flipU, _flipV);
        }
    }
}
