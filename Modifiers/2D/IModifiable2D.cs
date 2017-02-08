using Microsoft.Xna.Framework;

namespace CommonCode
{
    /// <summary>
    /// An interface that should be applied to all objects in 2D space for modifier compatability.
    /// </summary>
    public interface IModifiable2D
    {
        Vector2 WorldPosition { get; set; }
        float Rotation { get; set; }
        Vector2 Scale { get; set; }
        Color Color { get; set; }

        IModifier2D[] Modifiers { get; }
        void AddModifier(IModifier2D modifier);
        void ClearModifiers();
    }
}
