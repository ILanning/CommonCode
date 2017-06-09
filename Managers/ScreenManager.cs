using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace CommonCode
{
    /// <summary>
    /// A singleton GameComponent level manager that keeps track of active screens.
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {
        static bool isInitialized = false; 
        static List<Screen> screens = new List<Screen>();
        static List<Screen> screensToBeAdded = new List<Screen>();
        static bool isMouseVisible = false;

        public static GameGlobals Globals;
        public static Game StaticGame;
        public static DynamicContentManager Content;
        public static bool IsMouseVisible
        {
            get { return isMouseVisible; }
            set
            {
                isMouseVisible = value;
                StaticGame.IsMouseVisible = value;
            }
        }

        public ScreenManager(Game game) : base(game) { StaticGame = game; }

        /// <summary>
        /// Adds the given screen to the top of the screen stack at the beginning of the next frame.
        /// </summary>
        /// <param name="screen">Screen to add.</param>
        public static void AddScreen(Screen screen)
        {
            screensToBeAdded.Add(screen);
        }

        /// <summary>
        /// Removes a screen from the draw/update list without deleting it.
        /// </summary>
        /// <param name="screen">Screen to disable.</param>
        public static void RemoveScreen(Screen screen)
        {
            for (int i = 0; i < screens.Count; i++)
                if (screens[i] == screen)
                {
                    screens[i].RemoveDeleteFlag = 1;
                    break;
                }
        }

        /// <summary>
        /// Removes the given screen and unloads its assets.
        /// </summary>
        /// <param name="screen">Screen to destroy.</param>
        public static void DeleteScreen(Screen screen)
        {
            for (int i = 0; i < screens.Count; i++)
                if (screens[i] == screen)
                {
                    screens[i].RemoveDeleteFlag = 2;
                    break;
                }
        }

        /// <summary>
        /// Returns an array of all screens.
        /// </summary>
        /// <returns></returns>
        public static Screen[] GetScreens()
        {
            return screens.ToArray();
        }

        public override void Initialize()
        {
            Content = new DynamicContentManager(Game, Game.Content.RootDirectory);
            FontManager.Content = Content;
            SphericalCamera spherical = new SphericalCamera(Vector3.Zero, Vector2.Zero, 25);
            Globals = new GameGlobals(GraphicsDevice, null, new SpriteBatch(GraphicsDevice), spherical);
            Globals.SetData();
            base.Initialize();
            isInitialized = true;
        }

        public void LoadContent2()
        {
            foreach (Screen s in screensToBeAdded)
            {
                screens.Insert(0, s);
            }
            screensToBeAdded.Clear();
            base.LoadContent();
            foreach (Screen screen in screens)
                if (!screen.isContentLoaded)
                    screen.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            foreach (Screen screen in screens)
                screen.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (bool.Parse(GameSettings.Settings["RunInBackground"]) || Game.IsActive)
            {
                //Add and remove new screens only before the update loop begins, 
                //to prevent confusion and cut down on error checking
                for (int i = 0; i < screens.Count; i++)
                {
                    if (screens[i].RemoveDeleteFlag != 0)
                    {
                        if (screens[i].RemoveDeleteFlag == 2 && isInitialized)
                            screens[i].UnloadContent();
                        screens.RemoveAt(i--);
                    }
                }
                foreach (Screen s in screensToBeAdded)
                {
                    if (isInitialized && !s.isContentLoaded)
                        s.LoadContent();
                    screens.Insert(0, s);
                }
                if(screensToBeAdded.Count > 0)
                    screensToBeAdded.Clear();
                //Only the topmost screen can recieve user input
                screens[0].HandleInput(gameTime);
                //Any screen can be updated, so long as the screens above it allow it
                for (int i = 0; i < screens.Count; i++)
                {
                    screens[i].Update(gameTime);
                    if (!screens[i].UpdateCoveredScreens)
                        break;
                }
            }
            if (!isMouseVisible)
            {
                if (InputManager.MousePosition.X < 4 || InputManager.MousePosition.X > Game.Window.ClientBounds.Width - 4
                || InputManager.MousePosition.Y < 4 || InputManager.MousePosition.Y > Game.Window.ClientBounds.Height - 4)
                {
                    if (Game.IsMouseVisible != true)
                        Game.IsMouseVisible = true;
                }
                else if (Game.IsMouseVisible != isMouseVisible)
                    Game.IsMouseVisible = isMouseVisible;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //Find the screens that need to be drawn
            int startScreen = 0;
            for (; startScreen < screens.Count-1; startScreen++)
            {
                if (!screens[startScreen].DrawCoveredScreens)
                    break;
            }
            //Draw said screens, starting from the farthest back and working towards the front
            for (; startScreen >= 0; startScreen--)
                screens[startScreen].Draw(gameTime);
        }        
    }
}