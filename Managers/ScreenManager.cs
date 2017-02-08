using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace CommonCode
{
    /// <summary>
    /// A GameComponent level manager that keeps track of active screens.
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {
        bool isInitialized; 
        List<Screen> screens = new List<Screen>();
        List<Screen> screensToBeAdded;
        private static bool isMouseVisible = false;

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

        public ScreenManager(Game game)
            : base(game)
        {
            StaticGame = game;
            screensToBeAdded = new List<Screen>();
        }

        /// <summary>
        /// Adds the given screen to the top of the screen stack at the beginning of the next frame.
        /// </summary>
        /// <param name="screen">Screen to add.</param>
        public void AddScreen(Screen screen)
        {
            screensToBeAdded.Add(screen);
        }

        /// <summary>
        /// Removes a screen from the draw/update list without deleting it.
        /// </summary>
        /// <param name="screen">Screen to disable.</param>
        public void RemoveScreen(Screen screen)
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
        public void DeleteScreen(Screen screen)
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
        public Screen[] GetScreens()
        {
            return screens.ToArray();
        }

        public override void Initialize()
        {
            Content = new DynamicContentManager(Game, Game.Content.RootDirectory);
            FontManager.Content = Content;
            base.Initialize();
            isInitialized = true;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            SphericalCamera spherical = new SphericalCamera(new Vector3(0, 0, 0), new Vector2(0), 25);
            Globals = new GameGlobals(GraphicsDevice, null, new SpriteBatch(GraphicsDevice), spherical);
            Globals.SetData();
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
            if (GameSettings.RunInBackground || Game.IsActive)
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
                    s.screenManager = this;
                    if (isInitialized && !s.isContentLoaded)
                        s.LoadContent();
                    screens.Insert(0, s);
                }
                if(screensToBeAdded.Count > 0)
                    screensToBeAdded.Clear();
                //Only the topmost screen can recieve user input
                screens[0].HandleInput();
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