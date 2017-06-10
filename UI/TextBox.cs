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
        Vector2 screenPosition;
        Vector2 positionOffset;
        float rotation;
        Coordinate dimensions;
        Color color;
        IModifier2D[] modifiers = new IModifier2D[4];
        string text;
        string font;
        StringPositionColor[] lines;
        ScrollBar scrollBar;
        Coordinate scrollBarOffset;
        Rectangle padding;

        public bool IsSelected;
        public event EventHandler Selected;
        public event EventHandler Clicked;
        public event EventHandler Collided;

        /// <summary>
        /// Creates a new text box.
        /// </summary>
        /// <param name="text">The initial text in the text box.</param>
        /// <param name="dimensions">The dimensions of the text box.</param>
        /// <param name="spriteFont">The font for all text within the box.</param>
        /// <param name="padding">The padding to apply to the text.  The scroll bar lies outside of the padding, but within the dimensions.</param>
        /// <param name="color">The text box</param>
        /// <param name="scrollBarDimensions"></param>
        public TextBox(string text, Rectangle dimensions, string spriteFont, Rectangle? padding = null, Color? color = null, 
            Color? scrollBarColor = null, Color? scrollBarDisabledColor = null, Rectangle? scrollBarDimensions = null)
        {
            if (color == null)
                this.color = Color.White;
            else
                this.color = (Color)color;

            if (padding != null)
                this.padding = (Rectangle)padding;
            else
                this.padding = new Rectangle(0, 0, 0, 0);

            this.text = text;
            font = spriteFont;
            Rectangle scrollDim;
            if (scrollBarDimensions != null)
            {
                scrollDim = (Rectangle)scrollBarDimensions;
                scrollBarOffset = new Coordinate(scrollDim.X, scrollDim.Y);
            }
            else
            {
                scrollDim = new Rectangle(0, 0, ScrollBar.spriteSheet.Width, dimensions.Height);
                scrollBarOffset = new Coordinate(dimensions.Width - ScrollBar.spriteSheet.Width, 0);
            }
            this.dimensions = new Coordinate(dimensions.Width - scrollDim.Width, dimensions.Height);
            
            lines = wordWrap(new Vector2(this.dimensions.X - this.padding.Right, this.dimensions.Y - this.padding.Bottom), text, font, this.color);
            if (lines.Length * 20 + 5 > dimensions.Height) //If the text is long enough that a scroll bar is needed
                scrollBar = new ScrollBar(lines.Length * 20 + 5, scrollDim.Height, new Coordinate(scrollDim.X, scrollDim.Y), 0, true, scrollBarColor, scrollBarDisabledColor);
            else
                scrollBar = new ScrollBar(scrollDim.Height, scrollDim.Height, new Coordinate(scrollDim.X, scrollDim.Y), 0, false, scrollBarColor, scrollBarDisabledColor);

            WorldPosition = new Vector2(dimensions.X, dimensions.Y);
        }

        public virtual void HandleInput()
        {
            OnClicked();
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
            sb.GraphicsDevice.ScissorRectangle = new Rectangle((int)(screenPosition.X + positionOffset.X) + padding.X, (int)(screenPosition.Y + positionOffset.Y) + padding.Y, dimensions.X - padding.Right, dimensions.Y - padding.Bottom);

            Coordinate offsetFromScroll = new Coordinate(0, scrollBar.CurrentPos);
            int i = 0, curLine = 1, limit = lines.Length;

            //Finds the earliest line that may be drawn
            for(; curLine < lines.Length; curLine++)
            {
                if (lines[curLine].Position.Y >= scrollBar.CurrentPos)
                {
                    i = curLine - 1;
                    break;
                }
            }
            //Finds the last line that may be 
            int maxHeight = scrollBar.CurrentPos + dimensions.Y - padding.Bottom;
            for (; curLine < lines.Length; curLine++)
            {
                if(lines[curLine].Position.Y > maxHeight)
                {
                    limit = curLine;
                    break;
                }
            }
            for (; i < limit; i++)
                sb.DrawString(ScreenManager.Globals.Fonts[font], lines[i].Text, (Coordinate)(lines[i].Position + positionOffset + screenPosition - offsetFromScroll) + (Coordinate)padding.Location, lines[i].Color);

            sb.End();
            sb.GraphicsDevice.ScissorRectangle = new Rectangle();
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
        }

        /// <summary>
        /// Will parse a string into an array of StringPositionColors that fit in the passed in box.
        /// </summary>
        /// <param name="boxDimensions"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        StringPositionColor[] wordWrap(Vector2 boxDimensions, string text, string font, Color color)
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

                if (wordsParsedThisLine == 0)//If this is true, it got stuck because a single "word" took up more than a whole line.
                {
                    throw new ArgumentException("A word could not be broken up to fit within boxDimensions.", "text");
                }

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
                screenPosition = value;
                scrollBar.Position = (Coordinate)(screenPosition + positionOffset + scrollBarOffset);
            } 
        }

        public Vector2 Offset 
        { 
            get { return positionOffset; } 
            set 
            {                
                positionOffset = value;
                scrollBar.Position = (Coordinate)(screenPosition + positionOffset + scrollBarOffset);
            } 
        }

        public float Rotation { get { return rotation; } set { rotation = value; } }

        public Vector2 Scale { get { return dimensions; } set { dimensions = (Coordinate)value; } }

        public Color Color { get { return color; } set { color = value; } }

        public IModifier2D[] Modifiers { get { return modifiers; } }

        public string Text 
        {
            get { return text; }
            set
            {
                if (value != text)
                {
                    lines = wordWrap(new Vector2(dimensions.X - padding.Right, dimensions.Y - padding.Bottom), value, font, color);
                    bool needsScrollBar = lines.Length * 20 + 5 > dimensions.Y;
                    if (needsScrollBar)
                    {
                        scrollBar.ControlledArea = lines.Length * 20 + 5;
                        scrollBar.Enabled = true;
                    }
                    else
                    {
                        scrollBar.ControlledArea = dimensions.Y;
                        scrollBar.Enabled = false;
                    }
                    text = value;
                }
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
                Collided?.Invoke(this, EventArgs.Empty);
                Selected?.Invoke(this, EventArgs.Empty);
                IsSelected = true;
                return true;
            }
            else
                IsSelected = false;
            return false;
        }

        public bool OnClicked()
        {
            if (OnCollision() && InputManager.IsMouseButtonDown(MouseButtons.LMB))
            {
                Clicked?.Invoke(this, EventArgs.Empty);
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
