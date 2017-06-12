using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrueTypeSharp;
using System.IO;

namespace RealtimeRaytrace
{
    public class TrueTypeSharpTextRenderer : ITextRenderer
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

        protected void createTextureForChar(Char letter)
        {
            int width, height, xOffset, yOffset;
            uint index;
            if(letter == ' ')
                index = _font.FindGlyphIndex('-');
            else
                index = _font.FindGlyphIndex(letter);
            byte[] data = _font.GetGlyphBitmap(index, _scale, _scale, out width, out height, out xOffset, out yOffset);
            //_texture = new Texture2D(_graphics.GraphicsDevice, width, height, mipmap: false, format: SurfaceFormat.Alpha8);
            //_texture.SetData(data);

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

        //protected void DrawTexture(GameTime gameTime, Texture2D texture, Vector2 pos)
        //{
        //    BasicEffect basicEffect = new BasicEffect(_graphicsDeviceManager.GraphicsDevice);
        //    basicEffect.View = _basicEffect.View;
        //    basicEffect.Projection = _basicEffect.Projection;
        //    basicEffect.VertexColorEnabled = false;
        //    basicEffect.TextureEnabled = true;
        //    basicEffect.Texture = texture;
        //    VertexPositionTexture[] vert = new VertexPositionTexture[4];
        //    vert[0].Position = new Vector3(0, 0, 1);
        //    vert[1].Position = new Vector3(100, 0, 1);
        //    vert[2].Position = new Vector3(0, 100, 1);
        //    vert[3].Position = new Vector3(100, 100, 1);

        //    vert[0].TextureCoordinate = new Vector2(0, 0);
        //    vert[1].TextureCoordinate = new Vector2(1, 0);
        //    vert[2].TextureCoordinate = new Vector2(0, 1);
        //    vert[3].TextureCoordinate = new Vector2(1, 1);

        //    short[] ind = new short[6];
        //    ind[0] = 0;
        //    ind[1] = 2;
        //    ind[2] = 1;
        //    ind[3] = 1;
        //    ind[4] = 2;
        //    ind[5] = 3;
        //    //_graphicsDeviceManager.GraphicsDevice.Clear(Color.CornflowerBlue);
        //    foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
        //    {
        //        pass.Apply();
        //        _graphicsDeviceManager.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, vert, 0, vert.Length, ind, 0, ind.Length / 3);
        //    }
        //}

    }
}
