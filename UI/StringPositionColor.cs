using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonCode.UI
{
    public struct StringPositionColor
    {
        public string Text;
        public Vector2 Position;
        public Color Color;

        public StringPositionColor(string text, Vector2 position, Color color)
        {
            Text = text;
            Position = position;
            Color = color;
        }

        public void Draw(SpriteBatch sb, SpriteFont font)
        {
            sb.DrawString(font, Text, Position, Color);
        }
    }

    public struct StringFontPositionColor
    { 
        public string Text;
        public string Font;
        public Vector2 Position;
        public Color Color;

        public StringFontPositionColor(string text, string font, Vector2 position, Color color)
        {
            Text = text;
            Font = font;
            Position = position;
            Color = color;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.DrawString(ScreenManager.Globals.Fonts[Font], Text, Position, Color);
        }
    }
}
