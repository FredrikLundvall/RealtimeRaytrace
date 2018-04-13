using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace RealtimeRaytrace
{
    public enum MultiTexture4x3 {
        Keep0x0, Keep1x0, Keep2x0, Keep3x0,
        Keep0x1, Keep1x1, Keep2x1, Keep3x1,
        Keep0x2, Keep1x2, Keep2x2, Keep3x2
    }

    public enum AddressType {Clamping, WrapAround, WrapU_ClampV, WrapV_ClampU }

    public enum BilinearCoordinatesType { UminVmin, UmaxVmin, UminVmax, UmaxVmax }

    public struct BilinearCoordinates
    {
        public int umin;
        public int vmin;
        public int umax;
        public int vmax;
        public float ucoef;
        public float vcoef;
    }

    public class ArrayTexture
    {
        protected readonly Color[] _textureArray;
        protected readonly int _width;
        protected readonly int _height;
        protected readonly bool _loadToMemory;
        protected readonly string _textureFilenameArray;

        public static Texture2D LoadTexture(GraphicsDevice graphicsDevice, string textureFilename)
        {
            return Texture2D.FromStream(graphicsDevice, new FileStream(textureFilename, FileMode.Open));
        }

        public static void SaveArray(Color[] textureArray, string textureFilename)
        {
            //File.WriteAllBytes(textureFilename, textureArray);
            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(textureFilename)))
            {
                foreach (Color color in textureArray)
                {
                    writer.Write(color.A);
                    writer.Write(color.R);
                    writer.Write(color.G);
                    writer.Write(color.B);
                }
            }
        }

        public static int mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }

        static public Color bilinearInterpolation(Color uminVmin, Color umaxVmin, Color uminVmax, Color umaxVmax, float ucoef, float vcoef)
        {
            // What follows is a bilinear interpolation
            // along two coordinates u and v.
            return Color.Lerp(Color.Lerp(uminVmin, umaxVmin, ucoef), Color.Lerp(uminVmax, umaxVmax, ucoef), vcoef);
        }

        public ArrayTexture(GraphicsDeviceManager graphicsDeviceManager, string textureFilename, bool loadToMemory = true)
        {
            _loadToMemory = loadToMemory;
            Texture2D texture = LoadTexture(graphicsDeviceManager.GraphicsDevice,textureFilename);
            _width = texture.Width;
            _height = texture.Height;

            Color[] textureArray = new Color[texture.Width * texture.Height];
            texture.GetData(textureArray);
            texture.Dispose();

            if (_loadToMemory)
            {
                _textureArray = textureArray;
            }
            else
            {
                _textureFilenameArray = textureFilename + ".arr";
                SaveArray(textureArray, _textureFilenameArray);
            }
        }

        public ArrayTexture(Color[] textureArray,int totWidth, int totHeight, MultiTexture4x3 multiTexture)
        {
            _width = totWidth / 4;
            _height = totHeight / 3;
            _textureArray = GetImageData(textureArray, totWidth, multiTexture);
        }

        protected Color[] GetImageData(Color[] colorData, int totWidth, MultiTexture4x3 multiTexture)
        {
            Rectangle rectangle;
            switch (multiTexture)
            {
                default:
                case MultiTexture4x3.Keep0x0:
                    rectangle = new Rectangle(0, 0, _width, _height);
                    break;
                case MultiTexture4x3.Keep1x0:
                    rectangle = new Rectangle(_width, 0, _width, _height);
                    break;
                case MultiTexture4x3.Keep2x0:
                    rectangle = new Rectangle(_width * 2, 0, _width, _height);
                    break;
                case MultiTexture4x3.Keep3x0:
                    rectangle = new Rectangle(_width * 3, 0, _width, _height);
                    break;
                case MultiTexture4x3.Keep0x1:
                    rectangle = new Rectangle(0, _height, _width, _height);
                    break;
                case MultiTexture4x3.Keep1x1:
                    rectangle = new Rectangle(_width, _height, _width, _height);
                    break;
                case MultiTexture4x3.Keep2x1:
                    rectangle = new Rectangle(_width * 2, _height, _width, _height);
                    break;
                case MultiTexture4x3.Keep3x1:
                    rectangle = new Rectangle(_width * 3, _height, _width, _height);
                    break;
                case MultiTexture4x3.Keep0x2:
                    rectangle = new Rectangle(0, _height * 2, _width, _height);
                    break;
                case MultiTexture4x3.Keep1x2:
                    rectangle = new Rectangle(_width, _height * 2, _width, _height);
                    break;
                case MultiTexture4x3.Keep2x2:
                    rectangle = new Rectangle(_width * 2, _height * 2, _width, _height);
                    break;
                case MultiTexture4x3.Keep3x2:
                    rectangle = new Rectangle(_width * 3, _height * 2, _width, _height);
                    break;
            }

            Color[] color = new Color[rectangle.Width * rectangle.Height];
            for (int x = 0; x < rectangle.Width; x++)
                for (int y = 0; y < rectangle.Height; y++)
                    color[x + y * rectangle.Width] = colorData[x + rectangle.X + (y + rectangle.Y) * totWidth];
            return color;
        }

        public BilinearCoordinates calculateBilinearCoordinates(float u, float v, bool flipU = false, bool flipV = false)
        {
            u = Math.Abs(u);
            v = Math.Abs(v);
            u = flipU ? 1 - u : u;
            v = flipV ? 1 - v : v;

            BilinearCoordinates coordinates = new BilinearCoordinates();

            coordinates.umin = (int)(_width * u);
            coordinates.vmin = (int)(_height * v);
            coordinates.umax = (int)(_width * u) + 1;//this could end up on a different texture, when using multiple textures
            coordinates.vmax = (int)(_height * v) + 1;//this could end up on a different texture. Three textures is max what should be blended from, if wrapping is to be supported for multiple textures
            coordinates.ucoef = Math.Abs(_width * u - coordinates.umin);
            coordinates.vcoef = Math.Abs(_height * v - coordinates.vmin);

            return coordinates;
        }

        public BilinearCoordinates applyAddressTypeOnBilinearCoordinates(BilinearCoordinates coordinates, AddressType addressType)
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

        public Color readTexture(float u, float v, AddressType addressType, bool flipU = false, bool flipV = false)
        {
            BilinearCoordinates coordinates = calculateBilinearCoordinates(u, v, flipU, flipV);

            coordinates = applyAddressTypeOnBilinearCoordinates(coordinates, addressType);

            return bilinearInterpolation(
                readBilinearCoordinateColor(coordinates, BilinearCoordinatesType.UminVmin),
                readBilinearCoordinateColor(coordinates, BilinearCoordinatesType.UmaxVmin),
                readBilinearCoordinateColor(coordinates, BilinearCoordinatesType.UminVmax),
                readBilinearCoordinateColor(coordinates, BilinearCoordinatesType.UmaxVmax),
                coordinates.ucoef,
                coordinates.vcoef
                );
        }

        public Color readBilinearCoordinateColor(BilinearCoordinates coordinates, BilinearCoordinatesType coordinateType)
        {
            int pos;
            switch (coordinateType)
            {
                default:
                case BilinearCoordinatesType.UminVmin:
                    pos = coordinates.umin + _width * coordinates.vmin;
                    break;
                case BilinearCoordinatesType.UmaxVmin:
                    pos = coordinates.umax + _width * coordinates.vmin;
                    break;
                case BilinearCoordinatesType.UminVmax:
                    pos = coordinates.umin + _width * coordinates.vmax;
                    break;
                case BilinearCoordinatesType.UmaxVmax:
                    pos = coordinates.umax + _width * coordinates.vmax;
                    break;
            }
            if (_loadToMemory)
                return _textureArray[pos];
            else
            {
                using (BinaryReader reader = new BinaryReader(File.Open(_textureFilenameArray, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    reader.BaseStream.Seek(pos * 4, SeekOrigin.Begin);
                    Color color = new Color();
                    color.A = reader.ReadByte();
                    color.R = reader.ReadByte();
                    color.G = reader.ReadByte();
                    color.B = reader.ReadByte();
                    return color;
                }
            }

        }

    }
}
