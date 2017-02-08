using Microsoft.Xna.Framework;

namespace CommonCode.Collision
{
    public interface ICollidable
    {
        Coordinate Position { get; set; }
        bool IsColliding(Vector2 point);
        bool IsColliding(ICollidable second);
    }
}