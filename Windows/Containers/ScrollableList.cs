using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CommonCode.Windows
{
    /// <summary>
    /// A container that gives components as much space as they want.  Will crate scroll bars if the space allotted to it is not large enough.
    /// </summary>
    public class ScrollableList : Element, IContainer
    {
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

        Element[] elements;
        Coordinate trueMin;
        Coordinate containedMin;
        Color? color;
        bool horizontal;

        public ScrollableList(Element[] elements, bool horizontal, Rectangle initialDimensions, Coordinate minSize, Coordinate maxSize, Color color, SideTack attachment)
        {
            if (minSize.X < 0)
                minSize.X = 0;
            if (minSize.Y < 0)
                minSize.Y = 0;
            trueMin = minSize;
            if (maxSize.X < 0)
                maxSize.X = int.MaxValue;
            if (maxSize.Y < 0)
                maxSize.Y = int.MaxValue;
            MaximumSize = maxSize;

            this.elements = elements;
            this.horizontal = horizontal;
            this.color = color;
            SideAttachment = attachment;

            Resize(initialDimensions);
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

        public override void Move(Coordinate movement)
        {
            throw new NotImplementedException();
        }

        public override void Resize(Rectangle targetSpace)
        {
            updateMinimums();
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < elements.Length; i++)
                elements[i].Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
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
