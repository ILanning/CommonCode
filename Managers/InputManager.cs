using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonCode
{
    public class InputManager : GameComponent
    {
        /// <summary>
        /// State of the keyboard at the beginning of this frame.
        /// </summary>
        private static KeyboardState currentKeys;
        /// <summary>
        /// State of the keyboard at the beginning of the last frame.
        /// </summary>
        private static KeyboardState prevKeys;
        /// <summary>
        /// Used for eating keyboard input.
        /// </summary>
        private static List<Keys> pressedKeys;

        /// <summary>
        /// State of the mouse at the beginning of this frame.
        /// </summary>
        private static MouseState currentMouse;
        /// <summary>
        /// State of the mouse at the beginning of the last frame.
        /// </summary>
        private static MouseState prevMouse;

        /// <summary>
        /// The current keybindings associated with the input manager.
        /// </summary>
        public static KeybindManager Keybinds;

        public InputManager(Game game) : base(game) { }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            prevKeys = currentKeys;
            currentKeys = Keyboard.GetState();
            prevMouse = currentMouse;
            currentMouse = Mouse.GetState();

            MouseMovement = new Vector2(currentMouse.X - prevMouse.X, currentMouse.Y - prevMouse.Y);
            pressedKeys = new List<Keys>(currentKeys.GetPressedKeys());
        }

        #region Keybind Functions

        /// <summary>
        /// Checks if a keybound command was pressed this frame.
        /// </summary>
        /// <param name="command">The name of the command in the keybind manager to check.</param>
        /// <returns>Boolean representing button status.</returns>
        public static bool IsBoundTriggered(string command)
        {
            Keys key = Keybinds[command];
            if (key == Keys.F20)
                return IsMouseButtonTriggered(MouseButtons.LMB);
            else if (key == Keys.F21)
                return IsMouseButtonTriggered(MouseButtons.MMB);
            else if (key == Keys.F22)
                return IsMouseButtonTriggered(MouseButtons.RMB);
            else if (key == Keys.F23)
                return IsMouseButtonTriggered(MouseButtons.X1);
            else if (key == Keys.F24)
                return IsMouseButtonTriggered(MouseButtons.X2);
            else
                return IsKeyTriggered(key);
        }
        /// <summary>
        /// Checks if a keybound command was released this frame.
        /// </summary>
        /// <param name="command">The name of the command in the keybind manager to check.</param>
        /// <returns>Boolean representing button status.</returns>
        public static bool IsBoundReleased(string command)
        {
            Keys key = Keybinds[command];
            if (key == Keys.F20)
                return IsMouseButtonReleased(MouseButtons.LMB);
            else if (key == Keys.F21)
                return IsMouseButtonReleased(MouseButtons.MMB);
            else if (key == Keys.F22)
                return IsMouseButtonReleased(MouseButtons.RMB);
            else if (key == Keys.F23)
                return IsMouseButtonReleased(MouseButtons.X1);
            else if (key == Keys.F24)
                return IsMouseButtonReleased(MouseButtons.X2);
            else
                return IsKeyReleased(key);
        }
        /// <summary>
        /// Checks if a keybound command is pressed.
        /// </summary>
        /// <param name="command">The name of the command in the keybind manager to check.</param>
        /// <returns>Boolean representing button status.</returns>
        public static bool IsBoundDown(string command)
        {
            Keys key = Keybinds[command];
            if (key == Keys.F20)
                return IsMouseButtonDown(MouseButtons.LMB);
            else if (key == Keys.F21)
                return IsMouseButtonDown(MouseButtons.MMB);
            else if (key == Keys.F22)
                return IsMouseButtonDown(MouseButtons.RMB);
            else if (key == Keys.F23)
                return IsMouseButtonDown(MouseButtons.X1);
            else if (key == Keys.F24)
                return IsMouseButtonDown(MouseButtons.X2);
            else
                return IsKeyDown(key);
        }
        /// <summary>
        /// Checks if a keybound command is currently released.
        /// </summary>
        /// <param name="command">The name of the command in the keybind manager to check.</param>
        /// <returns>Boolean representing button status.</returns>
        public static bool IsBoundUp(string command)
        {
            Keys key = Keybinds[command];
            if (key == Keys.F20)
                return IsMouseButtonUp(MouseButtons.LMB);
            else if (key == Keys.F21)
                return IsMouseButtonUp(MouseButtons.MMB);
            else if (key == Keys.F22)
                return IsMouseButtonUp(MouseButtons.RMB);
            else if (key == Keys.F23)
                return IsMouseButtonUp(MouseButtons.X1);
            else if (key == Keys.F24)
                return IsMouseButtonUp(MouseButtons.X2);
            else
                return IsKeyUp(key);
        }

        #endregion

        #region Mouse Functions

        public static Vector2 MouseMovement { get; private set; }
        public static Vector2 MouseStoredPosition = Vector2.Zero;
        //public static bool CenterMouse = false;

        public static Coordinate MousePosition
        {
            get { return currentMouse.Position; }
            set { Mouse.SetPosition(value.X, value.Y); }
        }
        
        public static bool HasScrolledUp
        {
            get { return ScrollWheelDistance() > 0; }
        }
        public static bool HasScrolledDown
        {
            get { return ScrollWheelDistance() < 0; }
        }
        public static float ScrollWheelDistance()
        {
            return currentMouse.ScrollWheelValue - prevMouse.ScrollWheelValue;
        }

        public static bool IsMouseButtonTriggered(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LMB:
                    return currentMouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released;
                case MouseButtons.MMB:
                    return currentMouse.MiddleButton == ButtonState.Pressed && prevMouse.MiddleButton == ButtonState.Released;
                case MouseButtons.RMB:
                    return currentMouse.RightButton == ButtonState.Pressed && prevMouse.RightButton == ButtonState.Released;
                case MouseButtons.X1:
                    return currentMouse.XButton1 == ButtonState.Pressed && prevMouse.XButton1 == ButtonState.Released;
                case MouseButtons.X2:
                    return currentMouse.XButton2 == ButtonState.Pressed && prevMouse.XButton2 == ButtonState.Released;
                default:
                    throw new ArgumentException("The given button is not recognized.");
            }
        }
        public static bool IsMouseButtonReleased(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LMB:
                    return currentMouse.LeftButton == ButtonState.Released && prevMouse.LeftButton == ButtonState.Pressed;
                case MouseButtons.MMB:
                    return currentMouse.MiddleButton == ButtonState.Released && prevMouse.MiddleButton == ButtonState.Pressed;
                case MouseButtons.RMB:
                    return currentMouse.RightButton == ButtonState.Released && prevMouse.RightButton == ButtonState.Pressed;
                case MouseButtons.X1:
                    return currentMouse.XButton1 == ButtonState.Released && prevMouse.XButton1 == ButtonState.Pressed;
                case MouseButtons.X2:
                    return currentMouse.XButton2 == ButtonState.Released && prevMouse.XButton2 == ButtonState.Pressed;
                default:
                    throw new ArgumentException("The given button is not recognized.");
            }
        }
        public static bool IsMouseButtonDown(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LMB:
                    return currentMouse.LeftButton == ButtonState.Pressed;
                case MouseButtons.MMB:
                    return currentMouse.MiddleButton == ButtonState.Pressed;
                case MouseButtons.RMB:
                    return currentMouse.RightButton == ButtonState.Pressed;
                case MouseButtons.X1:
                    return currentMouse.XButton1 == ButtonState.Pressed;
                case MouseButtons.X2:
                    return currentMouse.XButton2 == ButtonState.Pressed;
                default:
                    throw new ArgumentException("The given button is not recognized.");
            }
        }
        public static bool IsMouseButtonUp(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LMB:
                    return currentMouse.LeftButton == ButtonState.Released;
                case MouseButtons.MMB:
                    return currentMouse.MiddleButton == ButtonState.Released;
                case MouseButtons.RMB:
                    return currentMouse.RightButton == ButtonState.Released;
                case MouseButtons.X1:
                    return currentMouse.XButton1 == ButtonState.Released;
                case MouseButtons.X2:
                    return currentMouse.XButton2 == ButtonState.Released;
                default:
                    throw new ArgumentException("The given button is not recognized.");
            }
        }

        /*These functions should work with some sort of 3D collision engine in the future.  For now, they are left out.*
        *public static bool MouseCollidesWithMapObject(MapObject mapObject)
        {
            Vector3 nearsource = new Vector3((float)currentMouseState.X, (float)currentMouseState.Y, 0f);
            Vector3 farsource = new Vector3((float)currentMouseState.X, (float)currentMouseState.Y, 1f);

            Matrix world = Matrix.CreateTranslation(0, 0, 0);

            Vector3 nearPoint = game.GraphicsDevice.Viewport.Unproject(nearsource, ScreenManager.Camera.Projection, ScreenManager.Camera.View, world);

            Vector3 farPoint = game.GraphicsDevice.Viewport.Unproject(farsource, ScreenManager.Camera.Projection, ScreenManager.Camera.View, world);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            CollisionRay ray = new CollisionRay(nearPoint, direction);
            return ray.CollisionWith(mapObject.CollisionData);
        }

        public static float? MouseDistanceToMapObject(CollisionBox box)
        {
            Vector3 nearsource = new Vector3((float)currentMouseState.X, (float)currentMouseState.Y, 0f);
            Vector3 farsource = new Vector3((float)currentMouseState.X, (float)currentMouseState.Y, 1f);

            Matrix world = Matrix.CreateTranslation(0, 0, 0);

            Vector3 nearPoint = game.GraphicsDevice.Viewport.Unproject(nearsource, ScreenManager.Camera.Projection, ScreenManager.Camera.View, world);
            Vector3 farPoint = game.GraphicsDevice.Viewport.Unproject(farsource, ScreenManager.Camera.Projection, ScreenManager.Camera.View, world);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            Ray ray = new Ray(nearPoint, direction);
            return ray.Intersects(box.ToBoundingBox());
        }*/
        #endregion

        #region Keyboard Functions
        /// <summary>
        /// Returns an array of all currently pressed keys.
        /// </summary>
        public static Keys[] KeysDown
        {
            get { return currentKeys.GetPressedKeys(); }
        }

        /// <summary>
        /// Returns an array of all keys that were pressed this frame, but not the last.
        /// </summary>
        public static Keys[] KeysTriggered
        {
            get
            {
                List<Keys> returnKeys = new List<Keys>();
                Keys[] keys = KeysDown;
                foreach (var key in currentKeys.GetPressedKeys())
                {
                    if (prevKeys.GetPressedKeys().Contains<Keys>(key))
                        continue;
                    returnKeys.Add(key);
                }
                return returnKeys.ToArray();
            }
        }
        public static bool IsAnyKeyDown
        {
            get { return KeysDown.Length > 0; }
        }
        public static bool WasKeyDown(Keys key)
        {
            return prevKeys.IsKeyDown(key);
        }

        public static bool IsKeyTriggered(Keys key) 
        {
            return (currentKeys.IsKeyDown(key) && prevKeys.IsKeyUp(key)); 
        }
        public static bool IsKeyReleased(Keys key) { return (currentKeys.IsKeyUp(key) && prevKeys.IsKeyDown(key)); }
        public static bool IsKeyDown(Keys key) { return currentKeys.IsKeyDown(key); }
        public static bool IsKeyUp(Keys key) { return currentKeys.IsKeyUp(key); }

        #endregion

        #region Gamepad Functions



        #endregion
    }

    public enum MouseButtons
    {
        LMB = 0,
        MMB,
        RMB,
        X1,
        X2
    }
}
