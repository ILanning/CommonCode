using Microsoft.Xna.Framework;

namespace CommonCode
{
    public abstract class Screen
    {
        public Screen() { }

        public bool isHidden;
        public bool DrawCoveredScreens = false;
        public bool UpdateCoveredScreens = false;
        public bool isContentLoaded = false;
        /// <summary>
        /// Determines the order in which draw functions are called.
        /// </summary>
        public bool Draw3DFirst = false;
        /// <summary>
        /// Flags the screen for disabling. 1 to remove, 2 to delete.
        /// </summary>
        public int RemoveDeleteFlag = 0;

        public virtual void LoadContent() { isContentLoaded = true; }

        public virtual void HandleInput(GameTime gameTime) { }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(GameTime gameTime) { }

        public virtual void UnloadContent() { }

        public virtual void RemoveSelf()
        {
            ScreenManager.DeleteScreen(this);
        }
    }
}
