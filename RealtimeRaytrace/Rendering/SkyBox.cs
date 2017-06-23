using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RealtimeRaytrace
{

    public class SkyBox : ISkyMap
    {
        protected enum SkyBoxIndex { SkyBoxRight = 0, SkyBoxLeft, SkyBoxUp, SkyBoxDown, SkyBoxFront, SkyBoxBack };

        ArrayTexture[] _skyBoxTexture;

        public SkyBox(
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

        public SkyBox(GraphicsDeviceManager graphicsDeviceManager, string multiTextureFilename)
        {
            Texture2D texture = ArrayTexture.LoadTexture(graphicsDeviceManager.GraphicsDevice, multiTextureFilename);
            Color[] textureArray = new Color[texture.Width * texture.Height];
            texture.GetData(textureArray);
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
            float u = 0;
            float v = 0;
            SkyBoxIndex index = SkyBoxIndex.SkyBoxRight;
            //SkyBoxIndex indexWrapU = SkyBoxIndex.SkyBoxUp;
            //SkyBoxIndex indexWrapV = SkyBoxIndex.SkyBoxBack;

            float x = ray.GetVector().X;
            float y = -ray.GetVector().Y;
            float z = -ray.GetVector().Z;

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
                //indexWrapU = SkyBoxIndex.SkyBoxUp;
                //indexWrapV = SkyBoxIndex.SkyBoxBack;
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
                //indexWrapU = SkyBoxIndex.SkyBoxUp;
                //indexWrapV = SkyBoxIndex.SkyBoxFront;
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
                //indexWrapU = SkyBoxIndex.SkyBoxFront;
                //indexWrapV = SkyBoxIndex.SkyBoxRight;
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
                //indexWrapU = SkyBoxIndex.SkyBoxBack;
                //indexWrapV = SkyBoxIndex.SkyBoxRight;
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
                //indexWrapU = SkyBoxIndex.SkyBoxUp;
                //indexWrapV = SkyBoxIndex.SkyBoxRight;
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
                //indexWrapU = SkyBoxIndex.SkyBoxUp;
                //indexWrapV = SkyBoxIndex.SkyBoxLeft;
            }

            // Convert range from -1 to 1 to 0 to 1
            u = 0.5f * (uc / maxAxis + 1.0f);
            v = 0.5f * (vc / maxAxis + 1.0f);

            //To wrap around the bilinear interpolation 
            //BilinearCoordinates coordinates = _skyBoxTexture[(int)index].calculateBilinearCoordinates(u, v);
            //coordinates = _skyBoxTexture[(int)index].applyAddressTypeOnBilinearCoordinates(coordinates, AddressType.WrapAround);
            //Color UminVminColor = _skyBoxTexture[(int)index].readBilinearCoordinateColor(coordinates, BilinearCoordinatesType.UminVmin);
            //Color UmaxVminColor = _skyBoxTexture[(int)index].readBilinearCoordinateColor(coordinates, BilinearCoordinatesType.UmaxVmin);
            //Color UminVmaxColor = _skyBoxTexture[(int)index].readBilinearCoordinateColor(coordinates, BilinearCoordinatesType.UminVmax);
            //Color UmaxVmaxColor = _skyBoxTexture[(int)index].readBilinearCoordinateColor(coordinates, BilinearCoordinatesType.UmaxVmax);

            //return ArrayTexture.bilinearInterpolation(UminVminColor, UmaxVminColor, UminVmaxColor, UmaxVmaxColor, coordinates.ucoef, coordinates.vcoef);

            return _skyBoxTexture[(int)index].readTexture(u, v, AddressType.Clamping);
            //return new Color(0, 0, 0);
        }
    }

}
