using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonCode.Windows
{
    public class TextureElement : Element
    {
        Texture2D texture;
        Color color;

        public TextureElement(Texture2D image, string name)
        {
            texture = image;
            color = Color.White;
            ResizeBehavior = ResizeKind.FillRatio;
            IdealDimensions = new Vector2(texture.Bounds.Width, texture.Bounds.Height);
            Name = name;
        }

        public TextureElement(Texture2D image, Coordinate minSize, Coordinate maxSize, Color color, string name, ResizeKind resize)
        {
            texture = image;
            this.color = color;
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
            ResizeBehavior = resize;
            IdealDimensions = new Vector2(texture.Bounds.Width, texture.Bounds.Height);
            Name = name;
        }

        public override void Resize(Rectangle targetSpace)
        {
            Rectangle resultArea = Rectangle.Empty;
            //if the given space is larger than either maximum, restrict that/those dimensions
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

        public override void Move(Coordinate movement)
        {
            targetArea.Location += (Point)movement;
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, targetArea, color);
        }
    }
}
