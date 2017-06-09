using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;

namespace CommonCode.Drawing
{
    public class Animation
    {
        public Vector2 Position;
        bool paused = false;
        public bool Paused 
        {
            get { return paused; }
            set
            {
                
                paused = value;
                timeToNextFrame = frames[currentFrame].Length;
            }
        }
        Texture2D texture;
        Frame[] frames;
        int startFrame;
        int currentFrame;
        /// <summary>
        /// Time until the frame changes, in milliseconds
        /// </summary>
        int timeToNextFrame;
        /// <summary>
        /// If true, the animation will loop.
        /// </summary>
        bool repeat = true;

        public Animation(Texture2D image, Frame[] frames, Vector2 position) : this(image, frames, 0, false, true, position) { }

        public Animation(Texture2D image, Frame[] frames, int startFrame, Vector2 position) : this(image, frames, startFrame, false, true, position) { }

        public Animation(Texture2D image, Frame[] frames, int startFrame, bool paused, bool repeat, Vector2 position)
        {
            texture = image;
            this.frames = frames;
            this.startFrame = startFrame;
            currentFrame = startFrame;
            this.paused = paused;
            this.repeat = repeat;
            timeToNextFrame = frames[currentFrame].Length;
            Position = position;
        }

        /// <summary>
        /// Returns the animation to the state it was initialized in.
        /// </summary>
        public void Reset()
        {
            timeToNextFrame = 0;
            currentFrame = startFrame;
            paused = false;
        }

        public void Update(GameTime gameTime)
        {
            if (!paused)
            {
                timeToNextFrame -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                while (timeToNextFrame <= 0)
                {                    
                    currentFrame++;
                    if (currentFrame >= frames.Length)
                    {
                        if (!repeat)
                        {
                            currentFrame--;
                            Paused = true;
                            break;
                        }
                        else
                            currentFrame = 0;
                    }
                    timeToNextFrame += frames[currentFrame].Length;
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, Position, frames[currentFrame].Dimensions, Color.White);
        }

        public void Draw(SpriteBatch sb, Vector2 offset)
        {
            sb.Draw(texture, Position + offset, frames[currentFrame].Dimensions, Color.White);
        }
    }

    public struct Frame
    {
        [XmlAttribute]
        /// <summary>
        /// Location and size of the region of the texture that will be drawn for this frame.
        /// </summary>
        public Rectangle Dimensions;
        [XmlAttribute]
        /// <summary>
        /// Length of time that this frame will last, in milliseconds.
        /// </summary>
        public int Length;

        public Frame(Rectangle dimensions, int length)
        {
            Dimensions = dimensions;
            Length = length;
        }

        public Frame(int x, int y, int width, int height, int length)
        {
            Dimensions = new Rectangle(x, y, width, height);
            Length = length;
        }
    }
}
