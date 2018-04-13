using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RealtimeRaytrace
{

    public class SkyBoxTexture : ISkyMap
    {
        protected enum SkyBoxIndex { SkyBoxRight = 0, SkyBoxLeft, SkyBoxUp, SkyBoxDown, SkyBoxFront, SkyBoxBack };

        ArrayTexture[] _skyBoxTexture;

        public SkyBoxTexture(
            GraphicsDeviceManager graphicsDeviceManager, 
            string backFilename, 
            string frontFilename,
            string upFilename, 
            string downFilename,
            string leftFilename, 
            string rightFilename) 
        {
            _skyBoxTexture = new ArrayTexture[6];
            _skyBoxTexture[(int)SkyBoxIndex.SkyBoxLeft] = new ArrayTexture(graphicsDeviceManager, leftFilename);
            _skyBoxTexture[(int)SkyBoxIndex.SkyBoxFront] = new ArrayTexture(graphicsDeviceManager, frontFilename);
            _skyBoxTexture[(int)SkyBoxIndex.SkyBoxUp] = new ArrayTexture(graphicsDeviceManager, upFilename);
            _skyBoxTexture[(int)SkyBoxIndex.SkyBoxDown] = new ArrayTexture(graphicsDeviceManager, downFilename);
            _skyBoxTexture[(int)SkyBoxIndex.SkyBoxRight] = new ArrayTexture(graphicsDeviceManager, rightFilename);
            _skyBoxTexture[(int)SkyBoxIndex.SkyBoxBack] = new ArrayTexture(graphicsDeviceManager, backFilename);
        }

        public SkyBoxTexture(GraphicsDeviceManager graphicsDeviceManager, string multiTextureFilename)
        {
            Texture2D texture = ArrayTexture.LoadTexture(graphicsDeviceManager.GraphicsDevice, multiTextureFilename);
            Color[] textureArray = new Color[texture.Width * texture.Height];
            texture.GetData(textureArray);
            texture.Dispose();
            _skyBoxTexture = new ArrayTexture[6];
            _skyBoxTexture[(int)SkyBoxIndex.SkyBoxLeft] = new ArrayTexture(textureArray, texture.Width, texture.Height, MultiTexture4x3.Keep0x1);
            _skyBoxTexture[(int)SkyBoxIndex.SkyBoxFront] = new ArrayTexture(textureArray, texture.Width, texture.Height, MultiTexture4x3.Keep1x1);
            _skyBoxTexture[(int)SkyBoxIndex.SkyBoxUp] = new ArrayTexture(textureArray, texture.Width, texture.Height, MultiTexture4x3.Keep1x0);
            _skyBoxTexture[(int)SkyBoxIndex.SkyBoxDown] = new ArrayTexture(textureArray, texture.Width, texture.Height, MultiTexture4x3.Keep1x2);
            _skyBoxTexture[(int)SkyBoxIndex.SkyBoxRight] = new ArrayTexture(textureArray, texture.Width, texture.Height, MultiTexture4x3.Keep2x1);
            _skyBoxTexture[(int)SkyBoxIndex.SkyBoxBack] = new ArrayTexture(textureArray, texture.Width, texture.Height, MultiTexture4x3.Keep3x1);
        }

        public Color GetColorInSky(Ray ray)
        {
            return GetColorInSky(ray.GetDirection());
        }

        public Color GetColorInSky(Vector3 direction)
        {
            float u = 0;
            float v = 0;
            SkyBoxIndex index = SkyBoxIndex.SkyBoxRight;

            float x = direction.X;
            float y = -direction.Y;
            float z = -direction.Z;

            float absX = Math.Abs(x);
            float absY = Math.Abs(y);
            float absZ = Math.Abs(z);

            bool isXPositive = x > 0 ? true : false;
            bool isYPositive = y > 0 ? true : false;
            bool isZPositive = z > 0 ? true : false;

            float maxAxis = 1;
            float uc = 0;
            float vc = 0;

            // POSITIVE X
            if (isXPositive && absX >= absY && absX >= absZ)
            {
                // u (0 to 1) goes from +z to -z
                // v (0 to 1) goes from -y to +y
                maxAxis = absX;
                uc = -z;
                vc = y;
                index = SkyBoxIndex.SkyBoxRight;
            }
            // NEGATIVE X
            if (!isXPositive && absX >= absY && absX >= absZ)
            {
                // u (0 to 1) goes from -z to +z
                // v (0 to 1) goes from -y to +y
                maxAxis = absX;
                uc = z;
                vc = y;
                index = SkyBoxIndex.SkyBoxLeft;
            }
            // POSITIVE Y
            if (isYPositive && absY >= absX && absY >= absZ)
            {
                // u (0 to 1) goes from -x to +x
                // v (0 to 1) goes from +z to -z
                maxAxis = absY;
                uc = x;
                vc = -z;
                index = SkyBoxIndex.SkyBoxDown;
            }
            // NEGATIVE Y
            if (!isYPositive && absY >= absX && absY >= absZ)
            {
                // u (0 to 1) goes from -x to +x
                // v (0 to 1) goes from -z to +z
                maxAxis = absY;
                uc = x;
                vc = z;
                index = SkyBoxIndex.SkyBoxUp;
            }
            // POSITIVE Z
            if (isZPositive && absZ >= absX && absZ >= absY)
            {
                // u (0 to 1) goes from -x to +x
                // v (0 to 1) goes from -y to +y
                maxAxis = absZ;
                uc = x;
                vc = y;
                index = SkyBoxIndex.SkyBoxFront;
            }
            // NEGATIVE Z
            if (!isZPositive && absZ >= absX && absZ >= absY)
            {
                // u (0 to 1) goes from +x to -x
                // v (0 to 1) goes from -y to +y
                maxAxis = absZ;
                uc = -x;
                vc = y;
                index = SkyBoxIndex.SkyBoxBack;
            }

            // Convert range from -1 to 1 to 0 to 1
            u = 0.5f * (uc / maxAxis + 1.0f);
            v = 0.5f * (vc / maxAxis + 1.0f);

            return _skyBoxTexture[(int)index].readTexture(u, v, AddressType.Clamping);
        }
    }

}
