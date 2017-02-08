using Microsoft.Xna.Framework;

namespace CommonCode.Content
{
    public class TexturedPlaneBuilder : Builder<TexturedPlaneBuilder>
    {
        public string ImagePath;
        public Vector3Builder Position;
        public Vector4Builder Rotation;
        public Vector2Builder Scale;
        public Vector4Builder Color;

        public TexturedPlaneBuilder() { }

        public TexturedPlaneBuilder(Vector3 position, Quaternion rotation, Vector2 scale, Vector4 color, string imagePath)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Color = color;
            ImagePath = imagePath;
        }
    }
}
