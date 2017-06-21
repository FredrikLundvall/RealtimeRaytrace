using System;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public enum SphereTextureType { Photo360, FisheyeHorizontal, Box };

    public class SkySphere : TextureBase, ISkyMap
    {
        SphereTextureType _sphereTextureType;

        public SkySphere(GraphicsDeviceManager graphicsDeviceManager, string skyTextureFilename, SphereTextureType sphereTextureType, bool flipU = false, bool flipV = false) : base(graphicsDeviceManager, skyTextureFilename, flipU, flipV)
        {
            _sphereTextureType = sphereTextureType;
        }

        public Color GetColorInSky(Ray ray)
        {
            //float u = (float)(Math.Asin(ray.GetVector().X) / Math.PI + 0.5);
            //float v = (float)(Math.Asin(ray.GetVector().Y) / Math.PI + 0.5);
            double u = 0;
            double v = 0;
            AddressType at = AddressType.Clamping;

            switch (_sphereTextureType)
            {
                //Vector3 v = currentVertice; // unit vector from edge of sphere, -1, -1, -1 to 1, 1, 1
                //float r = Mathf.Atan2(Mathf.Sqrt(v.x * v.x + v.y * v.y), v.z) / (Mathf.PI * 2.0f);
                //float phi = Mathf.Atan2(v.y, v.x);
                //textureCoordinates.x = (r * Mathf.Cos(phi)) + 0.5f;
                //textureCoordinates.y = (r * Mathf.Sin(phi)) + 0.5f;

                case SphereTextureType.FisheyeHorizontal:
                    double theta = Math.Atan2(-ray.GetVector().Y, ray.GetVector().X);
                    double phi = Math.Atan2(Math.Sqrt(ray.GetVector().X * ray.GetVector().X + ray.GetVector().Y * ray.GetVector().Y), ray.GetVector().Z);
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
                    u = (0.5 + Math.Atan2(ray.GetVector().Z, ray.GetVector().X) / (Math.PI * 2));
                    v = (0.5 - Math.Asin(ray.GetVector().Y) / Math.PI);
                    at = AddressType.WrapU_ClampV;
                    break;
            }
            return readTexture((float)u, (float)v,at);
        }
    }
}
