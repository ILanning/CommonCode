using Microsoft.Xna.Framework;

namespace CommonCode.Content
{
    public class WrappedModelBuilder : Builder<WrappedModelBuilder>
    {
        public Vector3Builder Scale;
        public bool[] MeshAlpha;
        public string ModelPath;

        public WrappedModelBuilder() { }

        public WrappedModelBuilder(Vector3 scale, bool[] meshTransparent, string modelPath)
        {
            Scale = scale;
            MeshAlpha = meshTransparent;
            ModelPath = modelPath;
        }
    }
}
