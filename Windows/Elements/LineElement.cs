using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CommonCode.Windows
{
    public class LineElement : Element
    {
        public Color Color { get; set; }
        bool horizontal;
        static Texture2D baseTexture;

        public static void Initialize(Texture2D texture)
        {
            baseTexture = texture;
        }

        public LineElement(bool horizontal, int width, SideTack attachment, Color color)
        {
            if (width <= 0)
                throw new ArgumentException("The line must have a width.", "width");
            MaximumSize = new Coordinate(width, 0);
            SideAttachment = attachment;
            ResizeBehavior = ResizeKind.FillSpace;
            Color = color;
            this.horizontal = horizontal;
        }

        public LineElement(bool horizontal, Vector2 proportions, Color color, string name, SideTack attachment)
        {
            Coordinate size = Coordinate.Zero;
            IdealDimensions = proportions;
            SideAttachment = attachment;
            ResizeBehavior = ResizeKind.FillRatio;
            Color = color;
            this.horizontal = horizontal;
            Name = name;
        }

        public override void Move(Coordinate movement)
        {
            targetArea.Location += (Point)movement;
        }

        public override void Resize(Rectangle targetSpace)
        {
            //Resources: minSize, maxSize, idealProportions
            //Gather intended proportions
            Rectangle resultArea = Rectangle.Empty;
            switch (ResizeBehavior)
            {
                case ResizeKind.FillRatio:
                    //Line changes both width and length to match original proportions
                    if (horizontal)
                    {
                        resultArea.Width = targetSpace.Width;
                        resultArea.Height = (int)(targetSpace.Width * (idealDimensions.X / idealDimensions.Y));
                    }
                    else
                    {
                        resultArea.Height = targetSpace.Height;
                        resultArea.Width = (int)(targetSpace.Height * (idealDimensions.Y / idealDimensions.X));
                    }
                    break;
                case ResizeKind.FillSpace:
                    //Line changes length but not width
                    if (horizontal)
                    {
                        resultArea.Width = targetSpace.Width;
                        resultArea.Height = MaximumSize.X;
                    }
                    else
                    {
                        resultArea.Width = MaximumSize.X;
                        resultArea.Height = targetSpace.Height;
                    }
                    break;
            }
            //Send to SideStick() for setting in place
            resultArea.Location = (Point)SideStick(targetSpace, resultArea);
            //Actually set targetArea
            targetArea = resultArea;
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(baseTexture, targetArea, Color);
        }
    }
}
