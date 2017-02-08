using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonCode.Windows
{
    /// <summary>
    /// Creates a flat plane of a single color.  Doubles as a blank spacer if the color's alpha is zero.
    /// </summary>
    public class FlatColorElement : Element
    {
        public Color color;

        public FlatColorElement(Color color, string name)
        {
            this.color = color;
            SideAttachment = SideTack.Center;
            ResizeBehavior = ResizeKind.FillSpace;
            Name = name;
        }

        public FlatColorElement(Coordinate minSize, Coordinate maxSize, Color color, string name, SideTack attachment)
        {
            if (minSize != null)
            {
                if (minSize.X < 0)
                    minSize.X = 0;
                if (minSize.Y < 0)
                    minSize.Y = 0;
                MinimumSize = minSize;
            }
            if (maxSize != null)
            {
                if (maxSize.X <= 0)
                    maxSize.X = int.MaxValue;
                if (maxSize.Y <= 0)
                    maxSize.Y = int.MaxValue;
                MaximumSize = maxSize;
            }
            SideAttachment = attachment;
            ResizeBehavior = ResizeKind.FillSpace;
            this.color = color;
            Name = name;
        }

        public FlatColorElement(Coordinate minSize, Coordinate maxSize, Vector2 proportions, SideTack attachment, ResizeKind resize, Color color)
        {
            if (minSize != null)
            {
                if (minSize.X < 0)
                    minSize.X = 0;
                if (minSize.Y < 0)
                    minSize.Y = 0;
                MinimumSize = minSize;
            }
            if (maxSize != null)
            {
                if (maxSize.X <= 0)
                    maxSize.X = int.MaxValue;
                if (maxSize.Y <= 0)
                    maxSize.Y = int.MaxValue;
                MaximumSize = maxSize;
            }
            IdealDimensions = proportions;
            SideAttachment = attachment;
            ResizeBehavior = resize;
            this.color = color;
        }

        public override void Move(Coordinate movement)
        {
            targetArea.Location += (Point)movement;
        }

        public override void Resize(Rectangle targetSpace)
        {
            Rectangle resultArea = Rectangle.Empty;
            Coordinate adjSpace = Coordinate.Zero;
            adjSpace.X = targetSpace.Width <= MaximumSize.X ? targetSpace.Width : MaximumSize.X;
            adjSpace.Y = targetSpace.Height <= MaximumSize.Y ? targetSpace.Height : MaximumSize.Y;
            switch (ResizeBehavior)
            {
                case ResizeKind.FillSpace:
                    resultArea.Width = adjSpace.X;
                    resultArea.Height = adjSpace.Y;
                    break;
                case ResizeKind.FillRatio:
                    float intendedRatio = IdealDimensions.X / IdealDimensions.Y;
                    float inputRatio = (float)adjSpace.X / (float)adjSpace.Y;
                    if (inputRatio < intendedRatio)
                    {
                        resultArea.Width = adjSpace.X;
                        resultArea.Height = (int)(adjSpace.Y * (IdealDimensions.Y / IdealDimensions.X));
                    }
                    else if (inputRatio > intendedRatio)
                    {
                        resultArea.Width = (int)(adjSpace.X * intendedRatio);
                        resultArea.Height = adjSpace.Y;
                    }
                    break;
            }
            resultArea.Location = (Point)SideStick(targetSpace, resultArea);
            targetArea = resultArea;
        }

        public override void Draw(SpriteBatch sb)
        {
            if(color.A != 0)
                sb.Draw(ScreenManager.Globals.White1By1, targetArea, color);
        }
    }
}
