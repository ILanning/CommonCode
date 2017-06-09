using System;
using Microsoft.Xna.Framework;

namespace CommonCode.Collision
{
    public class AABox : ICollidable
    {
        public Coordinate Position { get { return edges.Location; } set { edges.Location = (Point)value; } }
        public Rectangle edges;
        public Rectangle Bounds { get { return edges; } }

        public AABox(Rectangle boundary)
        {
            edges = boundary;
        }

        /// <summary>
        /// Creates a deep copy of the AABox.
        /// </summary>
        /// <param name="original">The AABox to make a copy of.</param>
        public AABox(AABox original)
        {
            edges = original.edges;
        }

        public bool IsColliding(Vector2 point)
        {
            if(point.X >= edges.Left && point.X <= edges.Right && point.Y <= edges.Bottom && point.Y >= edges.Top)
                return true;
            return false;
        }

        public bool IsColliding(ICollidable secondShape)
        {
            if (secondShape is AABox)
                return edges.Intersects((secondShape as AABox).edges);
            else if (secondShape is Convex)
                return secondShape.IsColliding(this);
            throw new NotImplementedException("The collision calculation " + secondShape.GetType().Name + " - AABox has not been implemented.");
        }


    }
}
