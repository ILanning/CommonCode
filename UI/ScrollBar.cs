using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CommonCode.Collision;

namespace CommonCode.UI
{
    public class ScrollBar
    {
        public static Texture2D spriteSheet;

        int controlledAreaLength = 0;
        int windowLength = 0;
        int controlledAreaPos = 0;
        Button scrollUpButton;
        public Button backingButton;
        Button scrollDownButton;
        Button sliderButton;
        int sliderSize, buttonSize, backingSize, barWidth;
        public int prevValue;
        Coordinate position;
        Texture2D texture;
        Color color;

        public static void Initialize(Texture2D baseTexture)
        {
            spriteSheet = baseTexture;
        }

        /// <summary>
        /// Creates a new scroll bar.
        /// </summary>
        /// <param name="controlledAreaLength">The length of the scrollable area.</param>
        /// <param name="windowLength">The length of the visible part of the scrollable area, and the length of the scroll bar itself.</param>
        /// <param name="initialPosition">The initial position in the scrollable area.</param>
        /// <param name="position">The position on the screen of the scroll bar itself.</param>
        public ScrollBar(int controlledAreaLength, int windowLength, int initialPosition, Coordinate position)
        {
            controlledAreaPos = initialPosition;
            this.position = position;
            Resize(controlledAreaLength, windowLength);
            Color = Color.White;
        }

        //Call this when the bar or controlled area get resized
        public void Resize(int controlledAreaLength, int windowLength)
        {
            if (this.windowLength == windowLength && this.controlledAreaLength == controlledAreaLength)
                return;
            //Gather some useful values
            barWidth = spriteSheet.Width;
            int copyArea = barWidth * barWidth;
            backingSize = windowLength - barWidth * 2;
            int backingArea = backingSize * barWidth;

            if (windowLength < barWidth * 3)
                throw new ArgumentOutOfRangeException("windowLength", "The window length is smaller than the minimum length of a scroll bar");

            //Load in textures

            Color[] scrollBarButton = new Color[copyArea];
            Color[] spriteSheetColors = new Color[spriteSheet.Width * spriteSheet.Height];
            spriteSheet.GetData<Color>(spriteSheetColors);
            for (int i = 0; i < copyArea; i++)
                scrollBarButton[i] = spriteSheetColors[i + copyArea];

            if (this.windowLength != windowLength)
            {
                //Build out pixel arrays containing the buttons from the scroll bar's base texture
                Color[] scrollUp = new Color[copyArea], backing = new Color[copyArea], scrollDown = new Color[copyArea];
                for (int i = 0; i < copyArea; i++)
                    scrollUp[i] = spriteSheetColors[i];
                for (int i = 0; i < copyArea; i++)
                    backing[i] = spriteSheetColors[i + copyArea * 2];
                for (int i = 0; i < copyArea; i++)
                    scrollDown[i] = spriteSheetColors[i + copyArea * 3];
                Color[] finalBacking = new Color[windowLength * barWidth];

                for (int i = 0; i < copyArea; i++) //Add first button to final texture
                    finalBacking[i] = scrollUp[i];
                int arrayPos = 0;
                int nextArrayPos = 6 * barWidth;
                for (; arrayPos < nextArrayPos; arrayPos++)
                    finalBacking[arrayPos + copyArea] = backing[arrayPos];
                nextArrayPos += backingArea - barWidth * 12;
                for (; arrayPos < nextArrayPos; arrayPos++)
                    finalBacking[arrayPos + copyArea] = backing[6 * barWidth + arrayPos % (barWidth * 12)];//emptySpace[arrayPos % width];
                nextArrayPos += barWidth * 12;
                for (int sourcePos = 0; arrayPos < nextArrayPos; arrayPos++, sourcePos++)
                    finalBacking[arrayPos + copyArea] = backing[barWidth * 6 + sourcePos];
                //for (int i = 0; i < backingArea; i++) //Add back/base bar to final texture, copying as needed            //Change to reflect new texture
                //    finalBacking[i + copyArea] = backing[i % copyArea];
                for (int i = 0; i < copyArea; i++)//Add last button to final texture
                    finalBacking[i + copyArea + backingArea] = scrollDown[i];
                if (texture != null)
                    texture.Dispose();
                texture = new Texture2D(ScreenManager.Globals.Graphics, barWidth, windowLength);
                texture.SetData<Color>(finalBacking);
            }

            //Generate the scrolling bar's texture
            Texture2D sliderTexture;
            sliderSize = (int)Math.Ceiling(backingSize * ((float)windowLength / controlledAreaLength));
            if (sliderSize > backingSize)
                sliderSize = backingSize;
            if (sliderSize < barWidth)
            {
                sliderTexture = new Texture2D(ScreenManager.Globals.Graphics, barWidth, barWidth);
                sliderTexture.SetData<Color>(scrollBarButton);
            }
            else                                                                                                         //Change to reflect new texture
            {
                Color[] finalSlider = new Color[sliderSize * barWidth];
                Color[] emptySpace = new Color[barWidth];
                for (int i = 0; i < barWidth; i++)//Empty space for dynamic sizing
                    emptySpace[i] = scrollBarButton[i + 2 * barWidth];
                int arrayPos = 0;
                int nextArrayPos = 2 * barWidth;
                int emptyLength = sliderSize - 17;

                for (; arrayPos < nextArrayPos; arrayPos++)
                    finalSlider[arrayPos] = scrollBarButton[arrayPos];
                nextArrayPos += emptyLength / 2 * barWidth;
                for (; arrayPos < nextArrayPos; arrayPos++)
                    finalSlider[arrayPos] = emptySpace[arrayPos % barWidth];
                nextArrayPos += 13 * barWidth;
                for (int sourcePos = 5 * barWidth; arrayPos < nextArrayPos; arrayPos++, sourcePos++)
                    finalSlider[arrayPos] = scrollBarButton[sourcePos];
                nextArrayPos += (emptyLength - emptyLength / 2) * barWidth;
                for (; arrayPos < nextArrayPos; arrayPos++)
                    finalSlider[arrayPos] = emptySpace[arrayPos % barWidth];
                nextArrayPos += 2 * barWidth;
                for (int sourcePos = 21 * barWidth; arrayPos < nextArrayPos; arrayPos++, sourcePos++)
                    finalSlider[arrayPos] = scrollBarButton[sourcePos];
                sliderTexture = new Texture2D(ScreenManager.Globals.Graphics, barWidth, sliderSize);
                sliderTexture.SetData<Color>(finalSlider);
            }
            //Set up buttons
            scrollUpButton = new Button(new AABox(new Rectangle(0, 0, barWidth, barWidth)), position: position);
            scrollUpButton.Clicked += onScrollUp;
            backingButton = new Button(new AABox(new Rectangle(0, barWidth, barWidth, backingSize)), position: new Coordinate(0, barWidth));
            backingButton.Clicked += onPressBacking;
            Rectangle sliderBox = new Rectangle(0, 0, sliderTexture.Width, sliderTexture.Height);
            sliderButton = new Button(new AABox(sliderBox), sliderTexture, new Rectangle[] { sliderBox, sliderBox, sliderBox, sliderBox }, new Coordinate());
            sliderButton.Clicked += onDragSlider;
            scrollDownButton = new Button(new AABox(new Rectangle(0, barWidth + backingSize, barWidth, barWidth)), position: new Coordinate(0, windowLength - barWidth));
            scrollDownButton.Clicked += onScrollDown;
            prevValue = backingSize;
            buttonSize = barWidth;
            sliderSize = sliderTexture.Height;

            this.controlledAreaLength = controlledAreaLength;
            this.windowLength = windowLength;
            centerSlider();
            Position = position;
        }

        void onPressBacking(object sender, EventArgs e)
        {
            //teleport to mousePos - (barLength/2)
            float mousePosOnBar = InputManager.MousePosition.Y - (Position.Y + buttonSize) - sliderSize / 2f;
            float barToControlled = controlledAreaLength / (float)backingSize;
            controlledAreaPos = (int)(mousePosOnBar * barToControlled);
            if (controlledAreaPos < 0)
                controlledAreaPos = 0;
            else if (controlledAreaPos > controlledAreaLength - windowLength)
                controlledAreaPos = controlledAreaLength - windowLength;
            centerSlider();
        }

        void centerSlider()
        {
            float controlledToBar = (float)backingSize / controlledAreaLength;
            sliderButton.Position = new Coordinate(0, (int)(controlledAreaPos * controlledToBar) + buttonSize) + position;
        }

        void onDragSlider(object sender, EventArgs e)
        {
            if (sliderButton.IsHeld)
            {
                sliderButton.Position -= new Coordinate(0, (int)InputManager.MouseMovement.Y);
                float barToControlled = controlledAreaLength / (float)backingSize;
                controlledAreaPos = (int)Math.Round(sliderButton.Position.Y * barToControlled);
                if (controlledAreaPos < 0)
                {
                    controlledAreaPos = 0;
                    sliderButton.Position = Coordinate.Zero;
                }
                else if (controlledAreaPos > controlledAreaLength - windowLength || sliderButton.Position.Y >= backingSize - sliderSize)
                {
                    controlledAreaPos = controlledAreaLength - windowLength;
                    sliderButton.Position = new Coordinate(0, backingSize - sliderSize);
                }
            }
        }

        void onScrollUp(object sender, EventArgs e)
        {
            controlledAreaPos -= 3;
            if (controlledAreaPos < 0)
                controlledAreaPos = 0;
            centerSlider();
        }

        void onScrollDown(object sender, EventArgs e)
        {
            controlledAreaPos += 3;
            if (controlledAreaPos > controlledAreaLength - windowLength)
                controlledAreaPos = controlledAreaLength - windowLength;
            centerSlider();
        }

        public void HandleInput()
        {
            scrollUpButton.HandleInput();
            scrollDownButton.HandleInput();
            if (InputManager.HasScrolledUp)
                for (int i = 0; i < InputManager.ScrollWheelDistance() / 10; i++)
                    onScrollUp(null, new EventArgs());
            if (InputManager.HasScrolledDown)
                for (int i = 0; i > InputManager.ScrollWheelDistance() / 10; i--)
                    onScrollDown(null, new EventArgs());
            sliderButton.HandleInput();
            if (!sliderButton.IsHeld)
                backingButton.HandleInput();
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, color);
            sliderButton.Draw(sb);
        }

        #region Properties
        //Current position of the controlled area
        public int CurrentPos
        {
            get { return controlledAreaPos; }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > controlledAreaLength - windowLength)
                    value = controlledAreaLength - windowLength;
                controlledAreaPos = value;
                centerSlider();
            }
        }

        public int Width { get { return barWidth; } }

        public Coordinate Position
        {
            get { return position; }
            set
            {
                Coordinate diff = value - position;
                scrollUpButton.Position = scrollUpButton.Position + diff;
                scrollDownButton.Position = scrollDownButton.Position + diff;
                sliderButton.Position = sliderButton.Position + diff;// + new Coordinate(0, barWidth);
                backingButton.Position = backingButton.Position + diff;
                position = value;
            }
        }

        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                //sliderButton.Color = value;
            }
        }
        #endregion
    }
}
