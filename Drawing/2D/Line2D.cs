using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CommonCode.Drawing
{
    /// <summary>
    /// Draws a line on the screen of a given color and width.
    /// </summary>
    public class Line2D
    {
        public Vector2 Position;
        public float Rotation;
        /// <summary>
        /// X = Length, Y = Width.
        /// </summary>
        public Vector2 Scale;
        public Color Color;

        public Line2D() { }

        /// <summary>
        /// Creates a line from two points.
        /// </summary>
        /// <param name="point1">One of the points to draw from.</param>
        /// <param name="point2">The other of the points to draw from.</param>
        /// <param name="width">The width of the line.</param>
        /// <param name="color">The color of this line.</param>
        public Line2D(Vector2 point1, Vector2 point2, float width, Color color) 
        {
            Position = point1;
            Rotation = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            Scale = new Vector2(Vector2.Distance(point1, point2), width);
            Color = color;
        }

        public Line2D(Vector2 position, float rotation, Vector2 scale, Color color)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Color = color;
        }

        public void Update() { }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(ScreenManager.Globals.White1By1, Position, null, Color, Rotation, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }

        public void Draw(Vector2 screenPosition, SpriteBatch sb, float layerDepth = 0)
        {
            sb.Draw(ScreenManager.Globals.White1By1, Position + screenPosition, null, Color, Rotation, Vector2.Zero, Scale, SpriteEffects.None, layerDepth);
        }

        public static void Draw(Vector2 point1, Vector2 point2, Color color, Vector2 screenPosition, SpriteBatch sb, float width = 1, float layerDepth = 0)
        {
            float rotation = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            Vector2 scale = new Vector2(Vector2.Distance(point1, point2), width);
            sb.Draw(ScreenManager.Globals.White1By1, point1 + screenPosition, null, color, rotation, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
        }
    }
}
