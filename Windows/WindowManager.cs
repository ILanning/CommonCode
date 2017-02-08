using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace CommonCode.Windows
{
    ///TODO: Complete class objectives
    public class WindowManager
    {
        /*Objectives:
         * Isolate input gathering and formatting from the requester
         * Basic interface: Functions are provided that will be called when certain conditions have been met (ie button clicked, key pressed, etc)
         *   Game world does not need to know what those conditions are
         *   Layout information can be loaded from a file
         *     Dummy file I/O class with hardcoded layout data while development is underway
         *   Use reflection to match code function name with file interface element
         * 
         * New UI elements that will be needed:
         *   Text Input Box
         * 
         * UI Modifications:
         *   Make all current elements compatible with ResizingBox
         */
        List<Window> windows = new List<Window>();
        List<Window> windowsToAdd = new List<Window>();
        List<Window> windowsToRemove = new List<Window>();
        public string TitleFont;
        public string MainFont;
        bool dragging;
        bool used;
        bool collided;

        public bool InteractedWith { get { return dragging || used; } }
        public bool CollidedWith { get { return dragging || used || collided; } }
        public bool Locked = false;

        public WindowManager(string titleFont = "Default", string mainFont = "Default")
        {
            TitleFont = titleFont;
            MainFont = mainFont;
        }

        //Takes a list of events and/or delegates
        public void Initialize()
        {
            
        }

        //Loads in assets, builds windows and layouts
        public void LoadContent()
        { 
            
        }

        public void AddWindow(Window window)
        {
            windowsToAdd.Add(window);
        }

        public void DeleteWindow(Window window)
        {
            windowsToRemove.Add(window);
        }

        public void HandleInput()
        {
            used = false;
            collided = false;        
            if (windows.Count > 0)
            {
                if (!dragging && !Locked)
                {
                    for (int i = 0; i < windows.Count; i++)
                    {
                        //the first window that says that its been clicked should get focus
                        if (windows[i].Clicked())
                        {
                            //focussing a window should move it to the top of the list
                            if (i != 0)
                            {
                                windows[0].InFocus = false;
                                Window tempRef = windows[i];
                                windows.RemoveAt(i);
                                windows.Insert(0, tempRef);
                            }
                            windows[0].InFocus = true;
                            collided = true;
                            used = true;
                            break;
                        }
                        if (windows[i].MouseCollided())
                            collided = true;
                    }
                    //Unfocus the top window if the mouse has been used elsewhere
                    if ((InputManager.IsMouseButtonDown(MouseButtons.LMB) || InputManager.IsMouseButtonDown(MouseButtons.MMB) || InputManager.IsMouseButtonDown(MouseButtons.RMB)) && !collided)
                        windows[0].InFocus = false;

                    if (windows[0].InFocus)
                    {
                        //if the window's title bar was clicked, set dragging to true
                        if (windows[0].TitleClicked())
                            dragging = true;
                        //otherwise, the window should be handled
                        else
                            windows[0].HandleInput();
                    }
                }
                else if(!Locked)
                {
                    windows[0].Move((Coordinate)InputManager.MouseMovement);
                    if (InputManager.IsMouseButtonUp(MouseButtons.LMB))
                        dragging = false;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (windowsToAdd.Count > 0)
            {
                foreach (Window window in windowsToAdd)
                    windows.Add(window);
                if (windows.Count == 1)
                    windows[0].InFocus = true;
                windowsToAdd = new List<Window>();
            }
            if (windowsToRemove.Count > 0)
            {
                bool setFocus = false;
                if (windowsToRemove.Contains(windows[0]))
                    setFocus = true;
                foreach (Window window in windowsToRemove)
                    windows.Remove(window);
                windowsToRemove = new List<Window>();
                if (windows.Count > 0 && setFocus)
                    windows[0].InFocus = true;
            }
            for (int i = 0; i < windows.Count; i++)
                windows[i].Update(gameTime);
        }

        public void Draw(SpriteBatch sb)
        {
            //Back to front
            for (int i = windows.Count - 1; i >= 0; i--)
                windows[i].Draw(sb);
        }
    }
}
