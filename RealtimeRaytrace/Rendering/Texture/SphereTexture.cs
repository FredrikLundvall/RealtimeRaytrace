using System;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class SphereTexture : ArrayTexture, ITextureMap
    {
        protected SphereTextureType _sphereTextureType;

        public SphereTexture(GraphicsDeviceManager graphicsDeviceManager, string skyTextureFilename, SphereTextureType sphereTextureType) : base(graphicsDeviceManager, skyTextureFilename)
        {
            _sphereTextureType = sphereTextureType;
        }

        public Color GetColorFromDirection(Vector3 direction, bool flipU = false, bool flipV = false)
        {
            double u = 0;
            double v = 0;
            AddressType at = AddressType.Clamping;

            switch (_sphereTextureType)
            {
                case SphereTextureType.FisheyeHorizontal:
                    double theta = Math.Atan2(-direction.Y, direction.X);
                    double phi = Math.Atan2(Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y), direction.Z);
                    double FOV = Math.PI * 2;
                    double width = 1;
                    double r = width * phi / FOV;

                    // Pixel in fisheye space
                    u = 0.5 * width + r * Math.Cos(theta);
                    v = 0.5 * width + r * Math.Sin(theta);
                    at = AddressType.Clamping;
                    break;

                case SphereTextureType.Photo360:
                default:
                    u = (0.5 + Math.Atan2(direction.Z, direction.X) / (Math.PI * 2));
                    v = (0.5 - Math.Asin(direction.Y) / Math.PI);
                    at = AddressType.WrapU_ClampV;
                    break;
            }
            return readTexture((float)u, (float)v, at, flipU, flipV);
        }
    }
}
