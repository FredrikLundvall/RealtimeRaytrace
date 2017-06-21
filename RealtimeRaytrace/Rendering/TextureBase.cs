using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace RealtimeRaytrace
{
    public enum AddressType {Clamping, WrapAround, WrapU_ClampV, WrapV_ClampU }
    public struct BilinearCoordinates
    {

        public int umin;
        public int vmin;
        public int umax;
        public int vmax;
        public float ucoef;
        public float vcoef;
    }

    public abstract class TextureBase
    {
        protected readonly Color[] _textureArray;
        protected readonly int _width;
        protected readonly int _height;
        protected bool _flipU;
        protected bool _flipV;

        public TextureBase(GraphicsDeviceManager graphicsDeviceManager, string textureFilename, bool flipU = false, bool flipV = false)
        {
            Texture2D texture = Texture2D.FromStream(graphicsDeviceManager.GraphicsDevice, new FileStream(textureFilename, FileMode.Open));
            _textureArray = new Color[texture.Width * texture.Height];
            texture.GetData(_textureArray);
            _width = texture.Width;
            _height = texture.Height;
            _flipU = flipU;
            _flipV = flipV;
        }

        protected BilinearCoordinates calculateBilinearCoordinates(float u, float v)
        {
            u = Math.Abs(u);
            v = Math.Abs(v);
            u = _flipU ? 1 - u : u;
            v = _flipV ? 1 - v : v;

            BilinearCoordinates coordinates = new BilinearCoordinates();

            coordinates.umin = (int)(_width * u);
            coordinates.vmin = (int)(_height * v);
            coordinates.umax = (int)(_width * u) + 1;//dessa kan hamna på en annan texture, tre textures är max vad som kan behöva blandas från
            coordinates.vmax = (int)(_height * v) + 1;//dessa kan hamna på en annan texture
            coordinates.ucoef = Math.Abs(_width * u - coordinates.umin);
            coordinates.vcoef = Math.Abs(_height * v - coordinates.vmin);

            return coordinates;
        }

        protected BilinearCoordinates applyAddressTypeOnBilinearCoordinates(BilinearCoordinates coordinates, AddressType addressType)
        {
            // The texture is being addressed on [0,1]
            // There should be an addressing type in order to 
            // determine how we should access texels when
            // the coordinates are beyond those boundaries.
            // Clamping is done by bringing anything below zero to the coordinate zero and everything beyond one, to one.
            // WrapAround is using modulus to wrap the coordinates off the end to the other side of the texture
            switch (addressType)
            {
                case AddressType.WrapV_ClampU:
                    coordinates.vmin = mod(coordinates.vmin, _width);
                    coordinates.vmax = mod(coordinates.vmax, _width);
                    coordinates.umin = Math.Min(Math.Max(coordinates.umin, 0), _height - 1);
                    coordinates.umax = Math.Min(Math.Max(coordinates.umax, 0), _height - 1);
                    break;
                case AddressType.WrapU_ClampV:
                    coordinates.umin = mod(coordinates.umin, _width);
                    coordinates.umax = mod(coordinates.umax, _width);
                    coordinates.vmin = Math.Min(Math.Max(coordinates.vmin, 0), _height - 1);
                    coordinates.vmax = Math.Min(Math.Max(coordinates.vmax, 0), _height - 1);
                    break;
                case AddressType.WrapAround:
                    coordinates.umin = mod(coordinates.umin, _width);
                    coordinates.umax = mod(coordinates.umax, _width);
                    coordinates.vmin = mod(coordinates.vmin, _height);
                    coordinates.vmax = mod(coordinates.vmax, _height);
                    break;
                case AddressType.Clamping:
                default:
                    coordinates.umin = Math.Min(Math.Max(coordinates.umin, 0), _width - 1);
                    coordinates.umax = Math.Min(Math.Max(coordinates.umax, 0), _width - 1);
                    coordinates.vmin = Math.Min(Math.Max(coordinates.vmin, 0), _height - 1);
                    coordinates.vmax = Math.Min(Math.Max(coordinates.vmax, 0), _height - 1);
                    break;
            }
            return coordinates;
        }

        protected Color readTexture(float u, float v, AddressType addressType)
        {
            BilinearCoordinates coordinates = calculateBilinearCoordinates(u, v);

            coordinates = applyAddressTypeOnBilinearCoordinates(coordinates, addressType);

            return bilinearInterpolation(
                _textureArray[coordinates.umin + _width * coordinates.vmin],
                _textureArray[coordinates.umax + _width * coordinates.vmin],
                _textureArray[coordinates.umin + _width * coordinates.vmax],
                _textureArray[coordinates.umax + _width * coordinates.vmax],
                coordinates.ucoef,
                coordinates.vcoef
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
