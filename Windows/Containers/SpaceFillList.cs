using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CommonCode.Windows
{
    /// <summary>
    /// A container that fills the available space to it.
    /// </summary>
    public class SpaceFillList : Element, IContainer
    {
        /// <summary>
        /// If true, the list will distort the given size ratios to allot elements the space they want.
        /// </summary>
        public bool ObeyMinimums
        {
            get { return obeyMin; }
            set
            {
                obeyMin = value;
                Resize(targetArea);
            }
        }

        public bool Horizontal
        {
            get { return horizontal; }
            set
            {
                horizontal = value;
                Resize(targetArea);
            }
        }
        public override Coordinate MinimumSize
        {
            get
            {
                Coordinate result = Coordinate.Zero;
                result.X = trueMin.X > containedMin.X ? trueMin.X : containedMin.X;
                result.Y = trueMin.Y > containedMin.Y ? trueMin.Y : containedMin.Y;
                return result;
            }
        }
        private Coordinate trueMin;
        private Coordinate containedMin = Coordinate.Zero;
        float[] elementSpacing;
        Element[] elements;
        bool horizontal;
        bool obeyMin;
        bool initialized = false;
        Color? color = null;

        /// <summary>
        /// Creates a new uninitialized SpaceFillList.  Resize() must be called at least once before use.
        /// </summary>
        public SpaceFillList(float[] spacing, Element[] elements, bool horizontal, string name)
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
            ObeyMinimums = false;
            Name = name;
        }

        /// <summary>
        /// Creates a new SpaceFillList and initializes it with the given space.
        /// </summary>
        public SpaceFillList(float[] spacing, Element[] elements, Rectangle targetSpace, Coordinate minimumSize, bool horizontal, bool obeyMinimums, string name)
        {
            if (spacing.Length != elements.Length)
                throw new ArgumentException("The number of elements and spacing indicators must match.");
            //Normalize spacing ratios to be fractions of 1
            float total = 0;
            for (int i = 0; i < spacing.Length; i++)
                total += spacing[i];
            for (int i = 0; i < spacing.Length; i++)
                spacing[i] /= total;

            if (minimumSize.X < 0)
                minimumSize.X = 0;
            if (minimumSize.Y < 0)
                minimumSize.Y = 0;

            elementSpacing = spacing;
            this.elements = elements;
            Horizontal = horizontal;
            targetArea = targetSpace;
            ObeyMinimums = obeyMinimums;
            trueMin = minimumSize;
            Name = name;

            Resize(targetSpace);
        }

        /// <summary>
        /// Creates a new SpaceFillList and initializes it with the given space.
        /// </summary>
        public SpaceFillList(float[] spacing, Element[] elements, Rectangle targetSpace, Coordinate minimumSize, bool horizontal, bool obeyMinimums, Color bgColor, string name)
        {
            if (spacing.Length != elements.Length)
                throw new ArgumentException("The number of elements and spacing indicators must match.");
            //Normalize spacing ratios to be fractions of 1
            float total = 0;
            for (int i = 0; i < spacing.Length; i++)
                total += spacing[i];
            for (int i = 0; i < spacing.Length; i++)
                spacing[i] /= total;

            if (minimumSize.X < 0)
                minimumSize.X = 0;
            if (minimumSize.Y < 0)
                minimumSize.Y = 0;

            elementSpacing = spacing;
            this.elements = elements;
            Horizontal = horizontal;
            targetArea = targetSpace;
            ObeyMinimums = obeyMinimums;
            trueMin = minimumSize;
            color = bgColor;
            Name = name;

            Resize(targetSpace);
        }

        public Element[] GetElements()
        {
            return elements;
        }

        public void BuildNames()
        {
            FullName = Name;
            foreach (Element element in elements)
                element.BuildNames(FullName);
        }

        public override void BuildNames(string containerName)
        {
            base.BuildNames(containerName);
            foreach (Element element in elements)
                element.BuildNames(FullName);
        }

        public override void Resize(Rectangle targetSpace)
        {
            updateMinimums();
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

        public override void Move(Coordinate movement)
        {
            targetArea.Location += (Point)movement;
            for (int i = 0; i < elements.Length; i++)
                elements[i].Move(movement);

        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < elements.Length; i++)
                elements[i].Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (!initialized)
                throw new InvalidOperationException("This object cannot be drawn until it has be Resized at least once.");
            if (color != null)
                sb.Draw(ScreenManager.Globals.White1By1, targetArea, color.Value);
            for (int i = 0; i < elements.Length; i++)
                elements[i].Draw(sb);
        }

        private void updateMinimums()
        {
            containedMin = Coordinate.Zero;
            for (int i = 0; i < elements.Length; i++)
            {
                if (horizontal)
                {
                    containedMin.X += elements[i].MinimumSize.X;
                    if (elements[i].MinimumSize.Y > containedMin.Y)
                        containedMin.Y = elements[i].MinimumSize.Y;
                }
                else
                {
                    containedMin.Y += elements[i].MinimumSize.Y;
                    if (elements[i].MinimumSize.X > containedMin.X)
                        containedMin.X = elements[i].MinimumSize.X;
                }
            }
        }
    }
}
