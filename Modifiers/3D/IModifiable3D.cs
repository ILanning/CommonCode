using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonCode
{
    /// <summary>
    /// An interface that should be applied to all objects in 3D space for modifier compatability.
    /// </summary>
    public interface IModifiable3D
    {
        Vector3 WorldPosition { get; set; }
        Quaternion Rotation { get; set; }
        Vector3 Scale { get; set; }
        Color Color { get; set; }

        IModifier3D[] Modifiers { get; }
        void AddModifier(IModifier3D modifier);
        void ClearModifiers();
        void Draw(BasicEffect effect, GraphicsDevice graphics);
    }
}
