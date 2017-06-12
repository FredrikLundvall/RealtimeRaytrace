using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace RealtimeRaytrace
{
    public enum AddressType {Clamping, WrapAround, WrapU_ClampV }

    public abstract class TextureBase
    {
        protected readonly Color[] _skyTextureArray;
        protected readonly int _width;
        protected readonly int _height;

        public TextureBase(GraphicsDeviceManager graphicsDeviceManager, string skyTextureFilename)
        {
            Texture2D skyTexture = Texture2D.FromStream(graphicsDeviceManager.GraphicsDevice, new FileStream(skyTextureFilename, FileMode.Open));
            _skyTextureArray = new Color[skyTexture.Width * skyTexture.Height];
            skyTexture.GetData(_skyTextureArray);
            _width = skyTexture.Width;
            _height = skyTexture.Height;
        }

        protected Color readTexture(float u, float v, AddressType addresType)
        {
            u = Math.Abs(u);
            v = Math.Abs(v);
            int umin = (int)(_width * u);
            int vmin = (int)(_height * v);
            int umax = (int)(_width * u) + 1;
            int vmax = (int)(_height * v) + 1;
            float ucoef = Math.Abs(_width * u - umin);
            float vcoef = Math.Abs(_height * v - vmin);

            // The texture is being addressed on [0,1]
            // There should be an addressing type in order to 
            // determine how we should access texels when
            // the coordinates are beyond those boundaries.

            // Clamping is done by bringing anything below zero to the coordinate zero and everything beyond one, to one.
            // WrapAround is using modulus to wrap the coordinates off the end to the other side of the texture
            switch(addresType)
            {
                case AddressType.WrapU_ClampV:
                    umin = mod(umin, _width);
                    umax = mod(umax, _width);
                    vmin = Math.Min(Math.Max(vmin, 0), _height - 1);
                    vmax = Math.Min(Math.Max(vmax, 0), _height - 1);
                    break;
                case AddressType.WrapAround:
                    umin = mod(umin, _width);
                    umax = mod(umax, _width);
                    vmin = mod(vmin, _height);
                    vmax = mod(vmax, _height);
                    break;
                case AddressType.Clamping:
                default:
                    umin = Math.Min(Math.Max(umin, 0), _width - 1);
                    umax = Math.Min(Math.Max(umax, 0), _width - 1);
                    vmin = Math.Min(Math.Max(vmin, 0), _height - 1);
                    vmax = Math.Min(Math.Max(vmax, 0), _height - 1);
                    break;
            }

            umin = Math.Min(Math.Max(umin, 0), _width - 1);
            umax = Math.Min(Math.Max(umax, 0), _width - 1);
            vmin = Math.Min(Math.Max(vmin, 0), _height - 1);
            vmax = Math.Min(Math.Max(vmax, 0), _height - 1);

            return bilinearInterpolation(
                _skyTextureArray[umin + _width * vmin],
                _skyTextureArray[umax + _width * vmin],
                _skyTextureArray[umin + _width * vmax],
                _skyTextureArray[umax + _width * vmax],
                ucoef,
                vcoef
                );
        }

        protected Color bilinearInterpolation(Color uminVmin, Color umaxVmin, Color uminVmax, Color umaxVmax, float ucoef, float vcoef)
        {
            // What follows is a bilinear interpolation
            // along two coordinates u and v.
            return Color.Lerp(Color.Lerp(uminVmin, umaxVmin, ucoef), Color.Lerp(uminVmax, umaxVmax, ucoef), vcoef);
        }

        protected int mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }
    }
}
