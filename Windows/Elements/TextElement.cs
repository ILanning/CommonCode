using CommonCode.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonCode.Windows
{
    public class TextElement : Element
    {
        Rectangle lastTargetSpace;
        StringFontPositionColor text;
        Vector2 scale;

        public string Text
        {
            get { return text.Text; }
            set
            {
                if (value != text.Text)
                {
                    text.Text = value;
                    Resize(lastTargetSpace);
                }
            }
        }
        //In the future, maybe have this automatically switch font sizes

        public TextElement(string text, string font, Color color, string elementName, ResizeKind resize)
        {
            this.text = new StringFontPositionColor(text, font, Vector2.Zero, color);
            ResizeBehavior = resize;
            if (resize != ResizeKind.FillSpace)
            {
                MaximumSize = (Coordinate)ScreenManager.Globals.Fonts[font].MeasureString(text);
                MinimumSize = MaximumSize;
            }
            else
                MaximumSize = new Coordinate(int.MaxValue);
            Name = elementName;
        }

        public TextElement(string text, string font, Color color, Coordinate minSize, Coordinate maxSize, ResizeKind resize)
        {
            this.text = new StringFontPositionColor(text, font, Vector2.Zero, color);
            if (maxSize == null)
                maxSize = new Coordinate(int.MaxValue);
            if (maxSize.X <= 0)
                maxSize.X = int.MaxValue;
            if (maxSize.Y <= 0)
                maxSize.Y = int.MaxValue;
            MaximumSize = maxSize;
            ResizeBehavior = resize;
        }

        public override void Move(Coordinate movement)
        {
            targetArea.Location += (Point)movement;
        }

        public override void Resize(Rectangle targetSpace)
        {
            lastTargetSpace = targetSpace;
            Rectangle resultArea = Rectangle.Empty;
            Coordinate adjSpace = new Coordinate();
            adjSpace.X = targetSpace.Width > MaximumSize.X ? MaximumSize.X : targetSpace.Width;
            adjSpace.Y = targetSpace.Height > MaximumSize.Y ? MaximumSize.Y : targetSpace.Height;
            Vector2 textSize = ScreenManager.Globals.Fonts[text.Font].MeasureString(text.Text);
            switch (ResizeBehavior)
            {
                case ResizeKind.FillSpace:
                    //Allow text to be distorted
                    scale.X = adjSpace.X / textSize.X;
                    scale.Y = adjSpace.Y / textSize.Y;
                    resultArea.Width = adjSpace.X;
                    resultArea.Height = adjSpace.Y;
                    break;
                case ResizeKind.FillRatio:
                    //Need to know the ratio between the desired and avaialble space in both dimensions
                    //take the largest of the two and use it as the scale factor for both
                    float scaleFactor = adjSpace.X /textSize.X;
                    if (scaleFactor > adjSpace.Y /textSize.Y)
                        scaleFactor = adjSpace.Y / textSize.Y;
                    scale = new Vector2(scaleFactor);
                    resultArea.Width = (int)(adjSpace.X * scaleFactor);
                    resultArea.Height = (int)(adjSpace.Y * scaleFactor);
                    break;
            }
            resultArea.Location = (Point)SideStick(targetSpace, resultArea);
            targetArea = resultArea;
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.DrawString(ScreenManager.Globals.Fonts[text.Font], text.Text, (Coordinate)targetArea.Location, text.Color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
    }
}
