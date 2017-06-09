using CommonCode.Drawing;
using Microsoft.Xna.Framework;

namespace CommonCode.Content
{
    /*public struct FrameBuilder
    {
        public Coordinate Location;
        public Coordinate Source;
        public short Delay;

        public FrameBuilder(Coordinate location, Coordinate source, short delay)
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
    }*/

    public class AnimationBuilder
    {
        public string TextureImagePath;
        public bool ShouldLoop;
        public bool TextureFlipped;
        public int FirstFrame;
        public Coordinate frameSize;
        public Frame[] Frames;

        public AnimationBuilder()
        { }

        public AnimationBuilder(string pathName, Frame[] newFrames, bool willLoop, int startFrame, Point SizeOfFrames)
        {
            TextureImagePath = pathName;
            ShouldLoop = willLoop;
            FirstFrame = startFrame;
            Frames = new Frame[newFrames.Length];
            for (int i = 0; i < newFrames.Length; i++)
                Frames[i] = newFrames[i];
            frameSize = SizeOfFrames;
        }
    }
}
