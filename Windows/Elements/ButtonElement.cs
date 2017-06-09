using CommonCode.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CommonCode.Windows
{
    public class ButtonElement : Element, IInputElement
    {
        public string[] Events { get { return new string[] { "Clicked", "Hovered", "Held", "Released" }; } }

        Button internalButton;
        Coordinate size;

        public event EventHandler Clicked
        {
            add { internalButton.Clicked += value; }
            remove { internalButton.Clicked -= value; }
        }
        public event EventHandler Hovered
        {
            add { internalButton.Hovered += value; }
            remove { internalButton.Hovered -= value; }
        }
        public event EventHandler Held
        {
            add { internalButton.Held += value; }
            remove { internalButton.Held -= value; }
        }
        public event EventHandler Released
        {
            add { internalButton.Released += value; }
            remove { internalButton.Released -= value; }
        }

        public ButtonElement(Button button, Coordinate buttonDimensions, string name, SideTack attachment)
        {
            internalButton = button;
            size = buttonDimensions;
            SideAttachment = attachment;
            Name = name;
        }



        public void HandleInput()
        {
            internalButton.HandleInput();
        }

        public override void Move(Coordinate movement)
        {
            internalButton.Position += movement;
        }

        public override void Resize(Rectangle targetSpace)
        {
            //Buttons do not change in size or shape
            //Reconsider this if collidables are made to be able to stretch
            Rectangle resultArea = new Rectangle();
            resultArea.Width = size.X;
            resultArea.Height = size.Y;
            Coordinate pos = SideStick(targetSpace, resultArea);
            resultArea.Location = (Point)pos;
            internalButton.Position = pos;
            targetArea = resultArea;
        }

        public override void Draw(SpriteBatch sb)
        {
            internalButton.Draw(sb);
        }
    }
}
