using CommonCode.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CommonCode.UI
{
    public class Button : IModifiable2D
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
        public bool IsHovered { get { return state == ButtonState.Hovered || state == ButtonState.Held; } }
        public bool IsHeld { get { return state == ButtonState.Held; } }

        Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                hitBox.Position = (Coordinate)value;
            }
        }

        public event EventHandler Clicked;
        public event EventHandler Hovered;
        public event EventHandler Released;
        public event EventHandler Held;

        Texture2D texture;
        ButtonState state = ButtonState.Inactive;
        Rectangle[] textureSamples;
        //string text;
        //string font;
        //Coordinate textTopLeft;
        public StringFontPositionColor? Text;

        ICollidable hitBox;

        /// <summary>
        /// Creates a new button.  Texture or collider (or both) must not be null.
        /// </summary>
        /// <param name="hitBox">The collision zone associated with this button</param>
        /// <param name="position">The position of this button</param>
        /// <param name="color">The color of this button</param>
        /// <param name="texture">The texture associated with this button</param>
        /// <param name="samples">Samples on the texture coresponding to the states of the button (0:Inactive, 1:Hovered, 2:Held, 3:Unusable)</param>
        /// <param name="usable">Whether or not the button may be intereacted with</param>
        public Button(ICollidable collider = null, Texture2D texture = null, Rectangle[] samples = null, Coordinate? position = null, Color? color = null, bool usable = true, StringFontPositionColor? text = null)
        {
            hitBox = collider;

            if (texture != null)
            {
                this.texture = texture;

                if (hitBox == null)
                {
                    //If a hitbox wasn't given, make one from whatever we've got
                    if (samples != null && samples.Length > 0)
                        hitBox = new AABox(new Rectangle(0, 0, samples[0].Width, samples[0].Height));
                    else
                        hitBox = new AABox(new Rectangle(0, 0, texture.Width, texture.Height));
                }

                if (samples == null || samples.Length == 0)
                {
                    //Fill out samples array with the default texture
                    Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
                    textureSamples = new Rectangle[] { rect, rect, rect, rect };
                }
                else if(samples.Length < 4)
                {
                    //Fill samples array with what was given, and put the inactive state in the rest as a default
                    textureSamples = new Rectangle[4];
                    int i = 0;
                    for (; i < samples.Length; i++)
                        textureSamples[i] = samples[i];
                    for (; i < 4; i++)
                        textureSamples[i] = samples[0];
                }
                else
                    textureSamples = samples;
            }
            else if (hitBox == null)
                throw new ArgumentNullException("collider", "No basis for collision zone creation: Either the texture or the collision area must not be null");

            if (position == null)
                Position = Coordinate.Zero;
            else
                Position = (Coordinate)position;

            if (color == null)
                Color = Color.White;
            else
                Color = (Color)color;

            if (!usable)
                state = ButtonState.Unusable;

            Text = text;
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

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < modifiers.Length; i++)
            {
                if (modifiers[i] != null)
                {
                    modifiers[i].Update();
                    if (modifiers[i].RemoveIfComplete && !modifiers[i].Active)
                    {
                        modifiers[i].Remove();
                        i--;
                    }
                }
            }
        }

        public void Draw(SpriteBatch sb) 
        { 
            if(texture != null)
                sb.Draw(texture, (Coordinate)position, textureSamples[(int)state], Color);
            if (Text != null)
            {
                StringFontPositionColor text = (StringFontPositionColor)Text;
                sb.DrawString(ScreenManager.Globals.Fonts[text.Font], text.Text, position + text.Position, text.Color);
            }
        }

        #region Modifiable2D
        IModifier2D[] modifiers = new IModifier2D[4];

        public IModifier2D[] Modifiers { get { return modifiers; } }
        public Vector2 WorldPosition { get { return Position; } set { Position = value; } }
        public float Rotation
        {
            get { return 0; }
            set { throw new NotSupportedException("The button class may not be rotated"); }
        }
        public Vector2 Scale
        {
            get { return Vector2.One; }
            set { throw new NotSupportedException("The button class may not be scaled"); }
        }
        public Color Color { get; set; }

        public void AddModifier(IModifier2D modifier)
        {
            modifier.owner = this;
            for (int i = 0; i <= modifiers.Length; i++)
            {
                if (i == modifiers.Length)
                {
                    IModifier2D[] newModifiersArray = new IModifier2D[modifiers.Length + 4];
                    for (int h = 0; h < modifiers.Length; h++)
                    {
                        newModifiersArray[h] = modifiers[h];
                    }
                    newModifiersArray[modifiers.Length] = modifier;
                    modifiers = newModifiersArray;
                }
                if (modifiers[i] == null)
                {
                    modifiers[i] = modifier;
                    break;
                }
            }
        }

        public void ClearModifiers()
        {
            modifiers = new IModifier2D[4];
        }

        #endregion

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