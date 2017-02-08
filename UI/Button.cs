using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CommonCode.Collision;

namespace CommonCode.UI
{
    public class Button
    {
        public bool Usable
        {
            get { return state != ButtonState.Unusable; }
            set
            {
                if (!value)
                    state = ButtonState.Unusable;
                else if (state == ButtonState.Unusable)
                    state = ButtonState.Inactive;
            }
        }
        public Coordinate Position
        {
            get { return hitBox.Position; }
            set { hitBox.Position = value; }
        }
        public bool IsHovered { get { return state == ButtonState.Hovered || state == ButtonState.Held; } }
        public bool IsHeld { get { return state == ButtonState.Held; } }
        public event EventHandler Clicked;
        public event EventHandler Hovered;
        public event EventHandler Released;
        public event EventHandler Held;
        Texture2D texture;
        ButtonState state = ButtonState.Inactive;
        Rectangle[] textureSamples = new Rectangle[4];
        ICollidable hitBox;

        public Button(ICollidable hitBox, Coordinate position, bool usable) : this(null, null, hitBox, position, usable) { }

        public Button(Texture2D texture, Rectangle[] samples, ICollidable hitBox, Coordinate position, bool usable)
        {
            this.texture = texture;
            textureSamples = samples;
            this.hitBox = hitBox;
            Position = position;
            if (!usable)
                state = ButtonState.Unusable;
        }
        
        public void HandleInput()
        {
            if (state != ButtonState.Unusable)
            {
                if (hitBox.IsColliding(InputManager.MousePosition))
                {
                    if (Hovered != null)
                        Hovered(this, EventArgs.Empty);
                    if (InputManager.IsMouseButtonDown(MouseButtons.LMB))
                    {
                        if (Clicked != null && state != ButtonState.Held)
                            Clicked(this, EventArgs.Empty);
                        if (Held != null)
                            Held(this, EventArgs.Empty);
                        state = ButtonState.Held;
                    }
                    else
                    {
                        if (Released != null && state == ButtonState.Held)
                            Released(this, EventArgs.Empty);
                        state = ButtonState.Hovered;
                    }
                }
                else
                    state = ButtonState.Inactive;
            }
        }

        public void Draw(SpriteBatch sb) 
        { 
            if(texture != null)
                sb.Draw(texture, Position, textureSamples[(int)state], Color.White); 
        }

        enum ButtonState
        {
            /// <summary>
            /// The mouse is not over the button, but the button may be clicked.
            /// </summary>
            Inactive = 0,
            /// <summary>
            /// The mouse is over the button, but not clicking it.
            /// </summary>
            Hovered,
            /// <summary>
            /// The mouse is clicking the button.
            /// </summary>
            Held,
            /// <summary>
            /// The button may not be interacted with.
            /// </summary>
            Unusable
        }
    }
}