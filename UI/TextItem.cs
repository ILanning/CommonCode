using Microsoft.Xna.Framework;

namespace CommonCode.UI
{
    public class TextItem
    {
        public string Text { get; set; }
        protected Rectangle dimensions;

        public TextItem()
        {
            Text = "null";
        }

        public TextItem(Screen screen, Vector2 pos, string text)
        {
            Text = text;
            dimensions = new Rectangle((int)pos.X, (int)pos.Y, 
                (int)ScreenManager.Globals.Fonts["Default"].MeasureString(text).X, 
                (int)ScreenManager.Globals.Fonts["Default"].MeasureString(text).Y);
        }

        public virtual void Draw(Screen screen, Vector2 position)
        {
            Color color = Color.White;
            ScreenManager.Globals.sb.DrawString(ScreenManager.Globals.Fonts["Default"], Text, new Vector2(dimensions.X, dimensions.Y), color);
        }
    }
}
