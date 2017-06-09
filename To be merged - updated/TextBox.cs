using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonCode.UI
{
    /// <summary>
    /// A 2D box that displays text, with a scroll bar if needed.
    /// </summary>
    public class TextBox : IModifiable2D, IClickable
    {
        private Vector2 screenPosition;
        private Vector2 positionOffset;
        private float rotation;
        private Vector2 dimensions;
        private Color color;
        private IModifier2D[] modifiers = new IModifier2D[4];
        private string text;
        private string font;
        private StringPositionColor[] lines;
        private bool scrollBarUsable;
        private ScrollBar scrollBar;

        public bool IsSelected;
        public event EventHandler Selected;
        public event EventHandler Clicked;
        public event EventHandler Collided;

        public TextBox(string text, Rectangle dimensions, string spriteFont, Color? color = null, Rectangle? scrollBarDimensions = null)
        {
            if (color == null)
                this.color = Color.White;
            else
                this.color = (Color)color;
            this.text = text;
            font = spriteFont;
            screenPosition = new Vector2(dimensions.X, dimensions.Y);
            Rectangle scrollDim;
            if (scrollBarDimensions != null)
                scrollDim = (Rectangle)scrollBarDimensions;
            else
                scrollDim = new Rectangle(dimensions.X + dimensions.Width - (int)(ScrollBar.spriteSheet.Width * 0.5f), dimensions.Y - (int)(ScrollBar.spriteSheet.Width * 0.5f), ScrollBar.spriteSheet.Width - 2, dimensions.Height);
            this.dimensions = new Vector2(dimensions.Width - scrollDim.Width, dimensions.Height);
            lines = WordWrap(this.dimensions, text, font, this.color);
            if (lines.Length * 20 + 5 > dimensions.Height) //If the text is long enough that a scroll bar is needed
            {
                scrollBarUsable = true;
                scrollBar = new ScrollBar(lines.Length * 20 + 5, scrollDim.Height, 0, new Coordinate(scrollDim.X, scrollDim.Y));
                scrollBar.Color = this.color;
            }
            else
            {
                scrollBarUsable = false;
                scrollBar = new ScrollBar(dimensions.Height + 1, scrollDim.Height, 0, new Coordinate(scrollDim.X, scrollDim.Y));
                scrollBar.Color = new Color(this.color.R / 2, this.color.G / 2, this.color.B / 2, this.color.A);
            }
        }

        public virtual void HandleInput()
        {
            OnClicked();
            if (scrollBarUsable)
                scrollBar.HandleInput();
        }

        public virtual void Update()
        {
            if (rotation > MathHelper.TwoPi)
                rotation -= MathHelper.TwoPi;
            else if (rotation < 0)
                rotation += MathHelper.TwoPi;

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

        public virtual void Draw(SpriteBatch sb)
        {
            scrollBar.Draw(sb);
            sb.End();
            RasterizerState scissorState = new RasterizerState();
            scissorState.ScissorTestEnable = true;
            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicWrap, DepthStencilState.Default, scissorState);
            sb.GraphicsDevice.ScissorRectangle = new Rectangle((int)(screenPosition.X + positionOffset.X), (int)(screenPosition.Y + positionOffset.Y), (int)dimensions.X, (int)dimensions.Y);
            Vector2 offsetFromScroll = Vector2.Zero;
            if (scrollBarUsable)
                offsetFromScroll.Y = scrollBar.CurrentPos;
            for (int i = 0; i < lines.Length; i++)
                sb.DrawString(ScreenManager.Globals.Fonts["Default"], lines[i].Text, lines[i].Position + positionOffset + screenPosition - offsetFromScroll, lines[i].Color);
            sb.End();
            sb.GraphicsDevice.ScissorRectangle = new Rectangle();
            //sb.GraphicsDevice.RasterizerState.ScissorTestEnable = false;
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
        }

        /// <summary>
        /// Will parse a string into an array of StringPositionColors that fit in the passed in box.
        /// </summary>
        /// <param name="boxDimensions"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private StringPositionColor[] WordWrap(Vector2 boxDimensions, string text, string font, Color color)
        {
            List<StringPositionColor> tempLineList = new List<StringPositionColor>();
            int yDist = 0;
            string remainingText = text;
            bool endOfText = false;
            List<char> seperators = new List<Char>(new char[] { ' ', '/', '\n', '-', ',', ';', ':', '?', ')' });

            if (text == "")
            {
                return new StringPositionColor[] {};
            }
            do
            {
                string currentLine = "";
                string previousLine = "";
                int lineEnd = 0;
                int wordsParsedThisLine = 0;
                bool lineTooLong = false;

                do
                {
                    previousLine = currentLine;

                    List<int> seperatorLocations = new List<int>(seperators.Count);
                    while (seperatorLocations.Count < seperators.Count)
                        seperatorLocations.Add(0);
                    for (int i = 0; i < seperatorLocations.Count; i++)
                    {
                        seperatorLocations[i] = remainingText.IndexOf(seperators[i], lineEnd);
                        if (seperatorLocations[i] == -1)
                        {
                            seperatorLocations.RemoveAt(i);
                            seperators.RemoveAt(i);
                            i--;
                        }
                    }

                    int firstSeperatorLocation = 0;
                    string word;
                    char firstSeperator = 'a';
                    if (seperators.Count == 0)
                        endOfText = true;
                    else
                    {
                        firstSeperatorLocation = int.MaxValue;
                        for (int i = 0; i < seperators.Count; i++)
                        {
                            if (seperatorLocations[i] < firstSeperatorLocation)
                            {
                                firstSeperator = seperators[i];
                                firstSeperatorLocation = seperatorLocations[i];
                            }
                        }
                        /*if (firstSeperator == '\n')
                        {
                            word = remainingText.Substring(lineEnd, (firstSeperatorLocation + 1) - lineEnd);
                            currentLine = currentLine.Insert(currentLine.Length, word);
                            lineEnd = firstSeperatorLocation + 1;
                            wordsParsedThisLine++;
                            break;
                        }*/
                    }
                    //Step through
                    //Redesign:  Find first seperator, bail and push line if \n is found.

                    if (!endOfText)
                        word = remainingText.Substring(lineEnd, (firstSeperatorLocation + 1) - lineEnd);
                    else
                        word = remainingText.Substring(lineEnd);

                    if (word.Length != 0)
                        wordsParsedThisLine++;
                    currentLine = currentLine.Insert(currentLine.Length, word);
                    lineTooLong = ScreenManager.Globals.Fonts[font].MeasureString(currentLine).X > boxDimensions.X;
                    if (!lineTooLong)
                    {
                        lineEnd = firstSeperatorLocation + 1;
                        if (firstSeperator == '\n' || endOfText)
                        {
                            previousLine = currentLine;
                            break;
                        }
                    }
                } while (!lineTooLong && !endOfText);

                if (wordsParsedThisLine == 0)//If this, it got stuck because a single "word" took up more than a whole line.
                {
                    //TODO: write a function that can break up a word
                    throw new ArgumentException("A word could not be broken up to fit within boxDimensions.", "text");
                }
                //if (endOfText && ScreenManager.DrawData.Fonts[font].MeasureString(currentLine).X < boxDimensions.X)
                //    previousLine = currentLine;

                tempLineList.Add(new StringPositionColor(previousLine.Trim(), new Vector2(0, yDist), color));
                yDist += 20;
                remainingText = remainingText.Remove(0, previousLine.Length);
            } while (!endOfText);
            if (remainingText.Length != 0)
                tempLineList.Add(new StringPositionColor(remainingText.Trim(), new Vector2(0, yDist), color));

            return tempLineList.ToArray();
        }

        #region IModifiable2D Members

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

        #region Properties

        public Vector2 WorldPosition 
        { 
            get { return screenPosition; } 
            set
            {
                scrollBar.Position += (Coordinate)(value - screenPosition);
                screenPosition = value; 
            } 
        }

        public Vector2 Offset 
        { 
            get { return positionOffset; } 
            set 
            {
                scrollBar.Position += (Coordinate)(value - positionOffset);
                positionOffset = value; 
            } 
        }

        public float Rotation { get { return rotation; } set { rotation = value; } }

        public Vector2 Scale { get { return dimensions; } set { dimensions = value; } }

        public Color Color { get { return color; } set { color = value; } }

        public IModifier2D[] Modifiers { get { return modifiers; } }

        public string Text 
        {
            get { return text; }
            set
            {
                lines = WordWrap(dimensions, value, font, color);
                //scrollBar.Resize((int)(lines.Length * 20 + 5 > dimensions.Y ? lines.Length * 20 + 5 : dimensions.Y + 5), (int)dimensions.Y);
                text = value;
            }
        }

        #endregion

        #region IClickable Members

        public virtual bool OnCollision()
        {
            if (InputManager.MousePosition.Y > screenPosition.Y + Offset.Y &&
                InputManager.MousePosition.X > screenPosition.X + Offset.X &&
               (InputManager.MousePosition.Y < screenPosition.Y + Offset.Y + dimensions.Y &&
                InputManager.MousePosition.X < screenPosition.X + Offset.X + dimensions.X))
            {
                if (Collided != null)
                    Collided(this, EventArgs.Empty);
                if (Selected != null)
                    Selected(this, EventArgs.Empty);
                IsSelected = true;
                return true;
            }
            else
                IsSelected = false;
            return false;
        }

        public bool OnClicked()
        {
            if (this.OnCollision() && InputManager.IsMouseButtonDown(MouseButtons.LMB))
            {
                if (Clicked != null)
                    Clicked(this, EventArgs.Empty);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Triggers any behaviors that occur on collision.
        /// </summary>
        public virtual void OnSelectItem() { }

        #endregion
    }
}
