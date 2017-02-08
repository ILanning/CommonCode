using Microsoft.Xna.Framework;

namespace CommonCode.Content
{
    public struct FrameBuilder
    {
        public PointBuilder Location;
        public PointBuilder Source;
        public short Delay;

        public FrameBuilder(Point location, Point source, short delay)
        {
            Location = location;
            Source = source;
            Delay = delay;
        }

        public static implicit operator Frame(FrameBuilder a)
        {
            return new Frame(a.Location, a.Source, a.Delay);
        }

        public static implicit operator FrameBuilder(Frame a)
        {
            return new FrameBuilder(a.Location, a.Source, a.Delay);
        }
    }
    
    public struct Frame
    {
        public Point Location;
        public Point Source;
        public short Delay;

        public Frame(Point location, Point source, short delay)
        {
            Location = location;
            Source = source;
            Delay = delay;
        }
    }

    public class AnimationBuilder
    {
        public string TextureImagePath;
        public bool ShouldLoop;
        public bool TextureFlipped;
        public int FirstFrame;
        public PointBuilder frameSize;
        public FrameBuilder[] Frames;

        public AnimationBuilder()
        { }

        public AnimationBuilder(string pathName, Frame[] newFrames, bool willLoop, int startFrame, Point SizeOfFrames)
        {
            TextureImagePath = pathName;
            ShouldLoop = willLoop;
            FirstFrame = startFrame;
            Frames = new FrameBuilder[newFrames.Length];
            for (int i = 0; i < newFrames.Length; i++)
                Frames[i] = newFrames[i];
            frameSize = SizeOfFrames;
        }
    }
}
