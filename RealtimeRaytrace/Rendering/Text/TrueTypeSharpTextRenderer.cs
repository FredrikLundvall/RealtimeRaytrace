using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrueTypeSharp;
using System.IO;

namespace RealtimeRaytrace
{
    public class TrueTypeSharpTextRenderer : ITextRenderer, IDisposable
    {
        protected Dictionary<int,TextBlock> _textBlockList;
        protected Dictionary<Char, Texture2D> _textureForChar;
        protected Dictionary<char, Vector2> _offsetForChar;
        protected GraphicsDeviceManager _graphicsDeviceManager;
        SpriteBatch _spriteBatch;
        TrueTypeFont _font;
        protected float _scale;

        public TrueTypeSharpTextRenderer(GraphicsDeviceManager graphicsDeviceManager, string fontFileName, int pixelSize)
        {
            _textBlockList = new Dictionary<int,TextBlock>();
            _textureForChar = new Dictionary<char, Texture2D>(100);
            _offsetForChar = new Dictionary<char, Vector2>(100);
            _graphicsDeviceManager = graphicsDeviceManager;
            _spriteBatch = new SpriteBatch(_graphicsDeviceManager.GraphicsDevice);
            _font = new TrueTypeFont(new FileStream(fontFileName, FileMode.Open));
            _scale = _font.GetScaleForPixelHeight(pixelSize);
            foreach (var letter in "ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖabcdefghijklmnopqrstuvwxyzåäö0123456789 ()+-.,!?\"%[]{}\\/*^'<>&=_")
            {
                createTextureForChar(letter);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_spriteBatch != null)
                {
                    _spriteBatch.Dispose();
                    _spriteBatch = null;
                }
            }
        }

        protected void createTextureForChar(Char letter)
        {
            int width, height, xOffset, yOffset;
            uint index;
            if(letter == ' ')
                index = _font.FindGlyphIndex('-');
            else
                index = _font.FindGlyphIndex(letter);
            byte[] data = _font.GetGlyphBitmap(index, _scale, _scale, out width, out height, out xOffset, out yOffset);

            Color[] colorData = new Color[width * height];
            Texture2D texture = new Texture2D(_graphicsDeviceManager.GraphicsDevice, width, height);

            if (letter != ' ')
                for (int i = 0; i < colorData.Length; i++)
                    colorData[i] = new Color(data[i], data[i], data[i], data[i]);
            texture.SetData(colorData);
            _textureForChar[letter] =  texture;
            _offsetForChar[letter] = new Vector2(xOffset, yOffset);
        }

        protected Texture2D getTextureForChar(Char letter)
        {
            if (! _textureForChar.ContainsKey(letter))
                createTextureForChar(letter);
            return _textureForChar[letter];
        }

        protected Vector2 getOffsetForChar(Char letter)
        {
            if (!_offsetForChar.ContainsKey(letter))
                createTextureForChar(letter);
            return _offsetForChar[letter];
        }

        public void SetText(int index, string text, Vector2 pos, Color color)
        {
            _textBlockList[index] = new TextBlock(text, pos, color);
        }

        public void RemoveText(int index)
        {
            _textBlockList.Remove(index);
        }

        public void RemoveAll()
        {
            _textBlockList.Clear();
        }

        public void Render(GameTime gameTime)
        {
            _spriteBatch.Begin();
            foreach(TextBlock textBlock in _textBlockList.Values)
            {
                drawTextblock(textBlock);
            }
            _spriteBatch.End();
        }
        protected void drawTextblock(TextBlock textBlock)
        {
            float curX = textBlock.Pos.X;
            foreach (var letter in textBlock.Text)
            {
                Texture2D texture = getTextureForChar(letter);
                Vector2 offset = getOffsetForChar(letter);
                Vector2 curPos;
                curPos.X = curX + offset.X;
                curPos.Y = textBlock.Pos.Y + offset.Y;
                _spriteBatch.Draw(texture, curPos, textBlock.TextColor);
                curX += texture.Width;
            }
        }
    }
}
