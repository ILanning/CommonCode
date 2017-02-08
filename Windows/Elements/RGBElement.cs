using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CommonCode.Windows
{
    public class RGBElement : Element
    {
        public Color DisplayedValue
        {
            get { return color; }
            set
            {
                color = value;
                Resize(targetArea);
            }
        }
        Color color;
        Rectangle rArea;
        Rectangle gArea;
        Rectangle bArea;
        bool horizontal = true;

        public RGBElement(Color color, bool horizontal, string name, SideTack attachment)
        {
            this.color = color;
            this.horizontal = horizontal;
            ResizeBehavior = ResizeKind.FillSpace;
            Name = name;
            SideAttachment = attachment;
        }

        public override void Move(Coordinate movement)
        {
            targetArea.Location += (Point)movement;
            rArea.Location += (Point)movement;
            gArea.Location += (Point)movement;
            bArea.Location += (Point)movement;
        }

        public override void Resize(Rectangle targetSpace)
        {
            Rectangle resultArea = Rectangle.Empty;
            switch (ResizeBehavior)
            {
                case ResizeKind.FillRatio:
                    throw new NotImplementedException("FillRatio resize behavior has not been implemented for RGBElements.");
                    //break;
                case ResizeKind.FillSpace:
                    resultArea = targetSpace;
                    break;
            }
            if (horizontal)
            {
                rArea = new Rectangle(resultArea.X, resultArea.Y, (int)(resultArea.Width * (color.R / 255f)), resultArea.Height / 3);
                gArea = new Rectangle(resultArea.X, resultArea.Y + rArea.Height, (int)(resultArea.Width * (color.G / 255f)), resultArea.Height / 3);
                bArea = new Rectangle(resultArea.X, gArea.Y + gArea.Height, (int)(resultArea.Width * (color.B / 255f)), resultArea.Height - gArea.Height - rArea.Height);
            }
            else
            {
                rArea = new Rectangle(resultArea.X, resultArea.Y, resultArea.Width / 3, (int)(resultArea.Height * (color.R / 255f)));
                gArea = new Rectangle(resultArea.X + rArea.Width, resultArea.Y, resultArea.Width / 3, (int)(resultArea.Height * (color.G / 255f)));
                bArea = new Rectangle(gArea.X + gArea.Width, resultArea.Y, resultArea.Width - gArea.Width - rArea.Width, (int)(resultArea.Height * (color.B / 255f)));
            }
            resultArea.Location = (Point)SideStick(targetSpace, resultArea);
            targetArea = resultArea;
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(ScreenManager.Globals.White1By1, rArea, new Color(255, 0, 0));
            sb.Draw(ScreenManager.Globals.White1By1, gArea, new Color(0, 255, 0));
            sb.Draw(ScreenManager.Globals.White1By1, bArea, new Color(0, 0, 255));
        }
    }
}
