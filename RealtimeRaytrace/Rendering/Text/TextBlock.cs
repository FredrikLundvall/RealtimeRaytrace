using System;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public struct TextBlock
    {
        readonly string _text;
        readonly Vector2 _pos;
        readonly Color _textColor;
        public TextBlock(string text, Vector2 pos, Color textColor)
        {
            _text = text;
            _pos = pos;
            _textColor = textColor;
        }

        public string Text
        {
            get { return _text; }
        }

        public Vector2 Pos
        {
            get { return _pos; }
        }

        public Color TextColor
        {
            get { return _textColor; }
        }
    }
}
