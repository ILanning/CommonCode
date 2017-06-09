using CommonCode.Collision;
using CommonCode.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CommonCode.Windows
{
    public delegate object DataProvider();
    public delegate void UpdateData();
    public delegate void InputResponse(object sender, EventArgs e);
    //Represents a single ingame window.
    public class Window
    {
        /*New Plan:
         * Window takes layout info, object on creation
         * Layout info states type of object (string)
         * Layout info also contains function names (strings) and information on what elements to bind them to
         * Use reflection to cast Object to stated type
         * Store gathered type for later maybe
         * Use reflection to convert function names to calls
         * Bind functions to their elements
         * Elements expose information gathering delegates
         * Game objects expose GetData() functions
         * Buttons can take void functions that may be bound to events
         * Lists can take object[], int[], string[], etc. functions for gathering info about the requested object
         * 
         * In this way, the game world doesn't have to interact with the window UI, and windows don't need to be hardcoded
         * 
         * Required Window Functions:
         * public Window(WindowManager, ContentManager, Rectangle, Layout, Object)
         * 
         * Required Element Functions:
         * 
         */

        //Functionality:
        //  Ability to minimize

        //~14-16 pixels for title bar height
        public bool InFocus 
        {
            get { return state == WindowState.Focussed; }
            set 
            {
                if (state != WindowState.Disabled)
                {
                    if (value)
                        state = WindowState.Focussed;
                    else
                        state = WindowState.Unfocussed;
                }
            }
        }
        public bool Disabled
        {
            get { return state == WindowState.Disabled; }
            set 
            {
                if (value)
                    state = WindowState.Disabled;
                else
                    state = WindowState.Unfocussed;
            }
        }
        public string Title { get; set; }
        public Coordinate Position 
        {
            get { return dimensions.Location; }
            set { Move(value - dimensions.Location ); }
        }

        static Texture2D uiComponents;
        Color[] borderColors;

        Coordinate minSize = new Coordinate(0);
        Coordinate maxSize = new Coordinate(int.MaxValue);
        Rectangle dimensions;
        public int TopPad, BotPad, TitlePad, LeftPad, RightPad;
        Element mainContainer;
        Button closeButton;
        WindowState state;
        IInputElement[] inputs;
        IDataDisplay[] displays;
        WindowManager manager;

        public Window(WindowManager manager, DynamicContentManager content, Rectangle dimensions, Color[] borderColors, IContainer layout, string title = "Window", bool closable = true, bool growForPadding = false)
        {
            //Reserve space for title bar (? x 17) + edges (4 x 2)
            this.manager = manager;
            if (!(layout is Element))
                throw new ArgumentException();
            mainContainer = (Element)layout;
            this.borderColors = borderColors;
            if(uiComponents == null)
                uiComponents = content.Load<Texture2D>(".//UI//UI Elements.png");
            Title = title;
            TopPad = 2;
            BotPad = 2;
            TitlePad = 15;
            LeftPad = 2;
            RightPad = 2;
            this.dimensions = dimensions;
            if (growForPadding)
            {
                dimensions.Width +=  LeftPad + RightPad;
                dimensions.Height += TitlePad + TopPad + BotPad;
            }
            if (closable)
            {
                closeButton = new Button(new AABox(new Rectangle(0, 0, 13, 13)), uiComponents, 
                    new Rectangle[] { new Rectangle(13, 0, 13, 13), new Rectangle(13, 0, 13, 13), new Rectangle(13, 0, 13, 13), new Rectangle(13, 0, 13, 13) },
                     new Coordinate(0, 0));
                closeButton.Released += CloseWindow;
            }

            layout.BuildNames();
            buildBindables();
            Resize(new Coordinate(dimensions.Width, dimensions.Height));
        }

        /// <summary>
        /// Finds all input elements within this window's element tree.
        /// </summary>
        private void buildBindables()
        {
            List<IInputElement> foundInputs = new List<IInputElement>();
            List<IDataDisplay> foundDisplays = new List<IDataDisplay>();
            List<IContainer> containerElements = new List<IContainer>();
            containerElements.Add((IContainer)mainContainer);

            while (containerElements.Count > 0)
            {
                Element[] elementList = containerElements[0].GetElements();
                foreach (Element element in elementList)
                {
                    if (element is IContainer)
                        containerElements.Add((IContainer)element);
                    if (element is IInputElement)
                        foundInputs.Add((IInputElement)element);
                    if (element is IDataDisplay)
                        foundDisplays.Add((IDataDisplay)element);
                }
                containerElements.RemoveAt(0);
            }

            inputs = foundInputs.ToArray();
            displays = foundDisplays.ToArray();
        }
        
        /// <summary>
        /// Binds game world data to an in-window display.  Returns false on failure.
        /// </summary>
        /// <param name="target">The full name of the display.</param>
        /// <param name="provider">The function that will provide data on call.</param>
        public bool BindData(string target, DataProvider provider)
        {
            for (int i = 0; i < displays.Length; i++)
                if (displays[i].FullName == target)
                    return displays[i].BindData(provider);
            return false;
        }

        /// <summary>
        /// Provides a function to call when the data has been updated.  Returns null on failure.
        /// </summary>
        public UpdateData GetUpdater(string target)
        {
            for (int i = 0; i < displays.Length; i++)
                if (displays[i].FullName == target)
                    return displays[i].UpdateData;
            return null;
        }

        /// <summary>
        /// Bind a function to an in-window trigger (ie. a button).  Returns false on failure.
        /// </summary>
        /// <param name="target">The full name of the element that will provide the input.</param>
        /// <param name="eventName">The name of the event to bind the function to.</param>
        /// <param name="response">The function that will be called.</param>
        public bool BindInput(string target, string eventName, Delegate response)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i].FullName == target)
                {
                    EventInfo requestedEvent = inputs[i].GetType().GetEvent(eventName);
                    if (requestedEvent == null || requestedEvent.EventHandlerType != response.GetType())
                        return false;
                    requestedEvent.AddEventHandler(inputs[i], (Delegate)response);
                    return true;
                }
            }
            return false;
        }

        public void Close()
        {
            CloseWindow(null, null);
        }

        public void CloseWindow(object sender, EventArgs e)
        {
            manager.DeleteWindow(this);
        }

        /// <summary>
        /// Returns true if the mouse is over the window.
        /// </summary>
        public bool MouseCollided()
        {
            return dimensions.Contains(InputManager.MousePosition);
        }

        /// <summary>
        /// Returns true if the left mouse button is depressed over the window.
        /// </summary>
        public bool Clicked()
        {
            return InputManager.IsMouseButtonTriggered(MouseButtons.LMB) && dimensions.Contains(InputManager.MousePosition);
        }

        /// <summary>
        /// Returns true if the left mouse button is depressed over the title bar.
        /// </summary>
        public bool TitleClicked()
        {
            return InputManager.IsMouseButtonTriggered(MouseButtons.LMB) && new Rectangle(dimensions.X, dimensions.Y, dimensions.Width - 13 - RightPad, TopPad + TitlePad).Contains(InputManager.MousePosition);
        }

        public void Move(Coordinate movement)
        {
            dimensions.Location += (Point)movement;
            mainContainer.Move(movement);
            if(closeButton != null)
                closeButton.Position += movement;
        }

        public void Resize(Coordinate newSize)
        {
            if (newSize.X > maxSize.X)
                newSize.X = maxSize.X;
            if (newSize.Y > maxSize.Y)
                newSize.Y = maxSize.Y;
            if (newSize.X < minSize.X)
                newSize.X = minSize.X;
            if (newSize.Y < minSize.Y)
                newSize.Y = minSize.Y;
            dimensions.Width = newSize.X;
            dimensions.Height = newSize.Y;

            mainContainer.Resize(new Rectangle(dimensions.X + LeftPad, dimensions.Y + TopPad + TitlePad, dimensions.Width - RightPad - LeftPad, dimensions.Height - TopPad - TitlePad - BotPad));
            if (closeButton != null)
                closeButton.Position = new Coordinate(dimensions.Right - RightPad - 13, dimensions.Y + TopPad);
        }

        public void HandleInput()
        {
            if (closeButton != null)
                closeButton.HandleInput();
            foreach (IInputElement element in inputs)
                element.HandleInput();
        }

        public void Update(GameTime gameTime)
        {
            mainContainer.Update(gameTime);
        }

        public void Draw(SpriteBatch sb)
        {

            Texture2D pixel = ScreenManager.Globals.White1By1;

            sb.Begin(rasterizerState: ScreenManager.Globals.ClipState);
            sb.GraphicsDevice.ScissorRectangle = dimensions;
            //Flat Background
            sb.Draw(pixel, dimensions, Color.Black);
            //Elements
            mainContainer.Draw(sb);
            if (closeButton != null)
            {
                sb.Draw(pixel, new Rectangle((int)closeButton.Position.X, (int)closeButton.Position.Y, 13, 13), borderColors[(int)state]);
                closeButton.Draw(sb);
            }
            //Borders
            sb.Draw(pixel, new Rectangle(dimensions.X, dimensions.Y, dimensions.Width, 1), borderColors[(int)state]); //Top
            sb.Draw(pixel, new Rectangle(dimensions.X, dimensions.Bottom - 1, dimensions.Width, 1), borderColors[(int)state]); //Bottom
            sb.Draw(pixel, new Rectangle(dimensions.X, dimensions.Y + TopPad + TitlePad - 1, dimensions.Width, 1), borderColors[(int)state]); //Title Bar Divider
            sb.Draw(pixel, new Rectangle(dimensions.X, dimensions.Y, 1, dimensions.Height), borderColors[(int)state]); //Left
            sb.Draw(pixel, new Rectangle(dimensions.Right - 1, dimensions.Y, 1, dimensions.Height), borderColors[(int)state]); //Right

            sb.End();
            sb.Begin(rasterizerState: ScreenManager.Globals.ClipState);

            sb.GraphicsDevice.ScissorRectangle = new Rectangle(dimensions.X + LeftPad, dimensions.Y, dimensions.Width - RightPad - 13, TopPad + TitlePad);
            Coordinate titlePos = new Coordinate((dimensions.Width - (int)ScreenManager.Globals.Fonts[manager.TitleFont].MeasureString(Title).X) / 2 + dimensions.Location.X + LeftPad, dimensions.Location.Y);
            sb.DrawString(ScreenManager.Globals.Fonts[manager.TitleFont], Title, titlePos, borderColors[(int)state]);

            sb.End();
        }

        enum WindowState
        { 
            Unfocussed,
            Focussed,
            Disabled
        }
    }
}
