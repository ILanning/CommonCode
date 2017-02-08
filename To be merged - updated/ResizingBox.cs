/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CommonCode.UI
{
    public interface IElement
    {
        Coordinate Position { get; }
        void Move(Coordinate movement);
        void Resize(Rectangle targetSpace);
        void Draw(SpriteBatch sb);
    }

    public class ResizingBox : IElement
    {
        Rectangle targetArea;
        float[] elementSpacing;
        IElement[] elements;
        bool horizontal;
        bool initialized = false;
        public bool Horizontal
        {
            get { return horizontal; }
            set 
            {
                horizontal = value;
                Resize(targetArea);
            }
        }
        public Coordinate Position
        {
            get { return targetArea.Location; }
            set { Move(value - (Coordinate)targetArea.Location); }
        }
        public Coordinate Size
        {
            get { return new Coordinate(targetArea.Width, targetArea.Height); }
        }

        /// <summary>
        /// Creates a new uninitialized ResizingBox.  Resize() must be called at least once before use.
        /// </summary>
        /// <param name="spacing"></param>
        /// <param name="elements"></param>
        /// <param name="horizontal"></param>
        public ResizingBox(float[] spacing, IElement[] elements, bool horizontal)
        {
            if (spacing.Length != elements.Length)
                throw new ArgumentException("The number of elements and spacing indicators must match.");
            //Normalize spacing ratios to be fractions of 1
            float total = 0;
            for (int i = 0; i < spacing.Length; i++)
                total += spacing[i];
            for (int i = 0; i < spacing.Length; i++)
                spacing[i] /= total;

            elementSpacing = spacing;
            this.elements = elements;
            Horizontal = horizontal;
        }

        /// <summary>
        /// Creates a new ResizingBox and initializes it with the given space.
        /// </summary>
        /// <param name="spacing"></param>
        /// <param name="elements"></param>
        /// <param name="targetSpace"></param>
        /// <param name="horizontal"></param>
        public ResizingBox(float[] spacing, IElement[] elements, Rectangle targetSpace, bool horizontal)
        {
            if (spacing.Length != elements.Length)
                throw new ArgumentException("The number of elements and spacing indicators must match.");
            //Normalize spacing ratios to be fractions of 1
            float total = 0;
            for (int i = 0; i < spacing.Length; i++)
                total += spacing[i];
            for (int i = 0; i < spacing.Length; i++)
                spacing[i] /= total;

            elementSpacing = spacing;
            this.elements = elements;
            Horizontal = horizontal;
            targetArea = targetSpace;
            Resize(targetSpace);
        }

        public void Resize(Rectangle targetSpace)
        {
            targetArea = targetSpace;
            //Recalculate spacing based on input
            if (Horizontal)
            {
                int spaceRemaining = targetSpace.Width;
                int adjLocation = targetSpace.X;
                int i = 0;
                for (; i < elements.Length - 1; i++)
                {
                    int width = (int)(targetSpace.Width * elementSpacing[i]);
                    spaceRemaining -= width;
                    elements[i].Resize(new Rectangle(adjLocation, targetSpace.Y, width, targetSpace.Height));
                    adjLocation += width;
                }
                elements[elements.Length - 1].Resize(new Rectangle(adjLocation, targetSpace.Y, spaceRemaining, targetSpace.Height));
            }
            else
            {
                int spaceRemaining = targetSpace.Height;
                int adjLocation = targetSpace.Y;
                int i = 0;
                for (; i < elements.Length - 1; i++)
                {
                    int height = (int)(targetSpace.Height * elementSpacing[i]);
                    spaceRemaining -= height;
                    elements[i].Resize(new Rectangle(targetSpace.X, adjLocation, targetSpace.Width, height));
                    adjLocation += height;
                }
                elements[elements.Length - 1].Resize(new Rectangle(targetSpace.X, adjLocation, targetSpace.Width, spaceRemaining));
            }
            if (!initialized)
                initialized = true;
        }

        public void Move(Coordinate movement)
        {
            targetArea.Location += (Point)movement;
            for (int i = 0; i < elements.Length; i++)
                elements[i].Move(movement);

        }

        public void Draw(SpriteBatch sb)
        {
            if (!initialized)
                throw new InvalidOperationException("This object cannot be drawn until it has be Resized at least once.");
            for (int i = 0; i < elements.Length; i++)
                elements[i].Draw(sb);
        }
    }

    public class FlatColorElement : IElement
    {
        Rectangle targetArea;
        static Texture2D baseTexture;
        public Color color;

        public Coordinate Position { get { return targetArea.Location; } }

        public static void Initialize(Texture2D texture)
        {
            baseTexture = texture;
        }

        public FlatColorElement(Color color)
        {
            this.color = color;
        }

        public void Resize(Rectangle targetSpace)
        {
            targetArea = targetSpace;
        }

        public void Move(Coordinate movement)
        {
            targetArea.Location += (Point)movement;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(baseTexture, targetArea, color);
        }
    }

    public class TextureElement : IElement
    {
        Rectangle targetArea;
        Texture2D texture;
        Color color;
        bool preserveAspect;

        public Coordinate Position { get { return targetArea.Location; } }

        public TextureElement(Texture2D image)
        {
            texture = image;
            color = Color.White;
            preserveAspect = false;
        }

        public TextureElement(Texture2D image, Color color, bool preserveAspectRatio)
        {
            texture = image;
            this.color = color;
            preserveAspect = preserveAspectRatio;
        }

        public void Resize(Rectangle targetSpace)
        {
            if (!preserveAspect)
                targetArea = targetSpace;
            else
            {
                Rectangle finalArea = new Rectangle();
                float textureRatio = (float)texture.Bounds.Width / (float)texture.Bounds.Height;
                float inputRatio = (float)targetSpace.Width / (float)targetSpace.Height;
                if (inputRatio < textureRatio)
                {
                    finalArea.Width = targetSpace.Width;
                    finalArea.Height = (int)(targetSpace.Height * ((float)texture.Bounds.Height / (float)texture.Bounds.Width));
                    finalArea.X = targetSpace.X;
                    finalArea.Y = (targetSpace.Height - finalArea.Height) / 2 + targetSpace.Y;
                }
                else if(inputRatio > textureRatio)
                {
                    finalArea.Width = (int)(targetSpace.Width * textureRatio);
                    finalArea.Height = targetSpace.Height;
                    finalArea.X = (targetSpace.Width - finalArea.Width) / 2 + targetSpace.X;
                    finalArea.Y = targetSpace.Y;
                }
                targetArea = finalArea;
            }
        }

        public void Move(Coordinate movement)
        {
            targetArea.Location += (Point)movement;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, targetArea, color);
        }
    }

    public class RGBElement : IElement
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
        Rectangle targetArea;
        Rectangle rArea;
        Rectangle gArea;
        Rectangle bArea;
        static Texture2D baseTexture;
        bool horizontal = true;

        public Coordinate Position { get { return targetArea.Location; } }

        public static void Initialize(Texture2D texture)
        {
            baseTexture = texture;
        }

        public RGBElement(Color color, bool horizontal)
        {
            this.color = color;
            this.horizontal = horizontal;
        }

        public void Resize(Rectangle targetSpace)
        {
            if (horizontal)
            {
                rArea = new Rectangle(targetSpace.X, targetSpace.Y, (int)(targetSpace.Width * (color.R / 255f)), targetSpace.Height / 3);
                gArea = new Rectangle(targetSpace.X, targetSpace.Y + rArea.Height, (int)(targetSpace.Width * (color.G / 255f)), targetSpace.Height / 3);
                bArea = new Rectangle(targetSpace.X, gArea.Y + gArea.Height, (int)(targetSpace.Width * (color.B / 255f)), targetSpace.Height - gArea.Height - rArea.Height);
            }
            else
            {
                rArea = new Rectangle(targetSpace.X, targetSpace.Y, targetSpace.Width / 3, (int)(targetSpace.Height * (color.R / 255f)));
                gArea = new Rectangle(targetSpace.X + rArea.Width, targetSpace.Y, targetSpace.Width / 3, (int)(targetSpace.Height * (color.G / 255f)));
                bArea = new Rectangle(gArea.X + gArea.Width, targetSpace.Y, targetSpace.Width - gArea.Width - rArea.Width, (int)(targetSpace.Height * (color.B / 255f)));
            }
            targetArea = targetSpace;
        }

        public void Move(Coordinate movement)
        {
            targetArea.Location += (Point)movement;
            rArea.Location += (Point)movement;
            gArea.Location += (Point)movement;
            bArea.Location += (Point)movement;
        }

        public void Draw(SpriteBatch sb)
        { 
            sb.Draw(baseTexture, rArea, new Color(255, 0, 0));
            sb.Draw(baseTexture, gArea, new Color(0, 255, 0));
            sb.Draw(baseTexture, bArea, new Color(0, 0, 255));
        }
    }
}*/
