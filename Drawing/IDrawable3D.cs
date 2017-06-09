using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonCode.Drawing
{
    public interface IDrawable3D
    {
        float DepthBias { get; }
        Vector3 WorldPosition { get; }
        void Draw(Effect effect, GraphicsDevice graphics);
    }
}
