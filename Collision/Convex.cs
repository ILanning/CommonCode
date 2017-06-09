using System;
using Microsoft.Xna.Framework;

namespace CommonCode.Collision
{
    public class Convex : ICollidable
    {
        Vector2[] points;
        public Coordinate Position { get; set; }
        public Rectangle Bounds { get { return simpleCollision; } }

        Rectangle simpleCollision;

        public Convex(Vector2[] boundary) : this(boundary, Coordinate.Zero) { }

        public Convex(Vector2[] boundary, Coordinate position)
        {
            //Find outer edges of shape
            float minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;
            points = boundary;
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].X > maxX)
                    maxX = points[i].X;
                if (points[i].X < minX)
                    minX = points[i].X;
                if (points[i].Y > maxY)
                    maxY = points[i].Y;
                if (points[i].Y < minY)
                    minY = points[i].Y;
            }

            //Normalize to (0, 0)
            Position = new Coordinate((int)minX, (int)minY);
            for (int i = 0; i < points.Length; i++)
                points[i] -= Position;

            //Create rectangle for faster AABB collision if we want it later
            simpleCollision = new Rectangle(0, 0, (int)Math.Ceiling(maxX - minX), (int)Math.Ceiling(maxY - minY));

            Position += position;
        }

        public bool IsColliding(Vector2 point)
        {
            //This algorithm determines how many times a horizontal line drawn from the given point intersects the collision shape.
            //If the number is odd, then the point must be within the shape.  If it is even, then the point is outside.
            int i, j = points.Length - 1;
            bool oddNodes = false;

            for (i = 0; i < points.Length; i++)
            {
                Vector2 adjPoint1 = points[i] + Position;
                Vector2 adjPoint2 = points[j] + Position;
                if ((adjPoint1.Y < point.Y && adjPoint2.Y >= point.Y ||
                     adjPoint2.Y < point.Y && adjPoint1.Y >= point.Y) &&
                    (adjPoint1.X <= point.X || adjPoint2.X <= point.X))
                {
                    if (adjPoint1.X + (point.Y - adjPoint1.Y) / (adjPoint2.Y - adjPoint1.Y) * (adjPoint2.X - adjPoint1.X) < point.X)
                    {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }
            return oddNodes;
        }

        public bool IsColliding(ICollidable secondShape)
        {
            if (secondShape is Convex)
            {
                Convex other = (secondShape as Convex);
             
                for (int i = 0; i < other.points.Length; i++)
                    if (IsColliding(other.points[i] + other.Position))
                        return true;

                for (int i = 0; i < points.Length; i++)
                    if (other.IsColliding(points[i] + Position))
                        return true;

                return false;
            }
            else if (secondShape is AABox)
            {
                AABox other = (AABox)secondShape;

                for (int i = 0; i < points.Length; i++)
                    if (other.IsColliding(points[i]))
                        return true;

                Vector2[] corners = new Vector2[]{new Vector2(other.edges.Left, other.edges.Top), new Vector2(other.edges.Right, other.edges.Top), 
                                                  new Vector2(other.edges.Right, other.edges.Bottom), new Vector2(other.edges.Left, other.edges.Bottom)};
                for (int i = 0; i < 4; i++)
                    if (IsColliding(corners[i]))
                        return true;

                return false;
            }

            throw new NotImplementedException("The collision calculation " + secondShape.GetType().Name + " - Convex has not been implemented.");
        }

        //Bug:   Will return false if both objects occupy the exact same space
        //Note:  Will return false as long as no points from the object being compared to are within the current object. 
        //       Even if a line from the other object is going through the current collision data.

        //REPLACE:  Instead try system where you check the points of the object you want to move against the 
        //line you are colliding with, find the projection vector from the average of the points that are
        //past the line

        //Find line closest to the points inside the SetPiece
        /// <summary>
        /// Takes a list of points and finds the most effecient way to shove those points out of the polygon.
        /// </summary>
        /// <param name="otherObject">Colliding points to be pushed out of the polygon. IsColliding can return such a list.</param>
        /// <returns>Vector2 representing the way to stop collision that moves the offending points the least.</returns>
        public Vector2 FindDisplacementVector(Convex other)
        {
            Vector2 displacementVector = Vector2.Zero;
            int h = points.Length - 1;
            float closestLine = float.MaxValue;
            for (int i = 0; i < points.Length; i++)
            {
                float temp = AverageDistanceFromPointsToLine(points[i] + Position, points[h] + Position, other.points, other.Position);
                if (Math.Abs(temp) < Math.Abs(closestLine))
                {
                    displacementVector = new Vector2(-((points[i].Y + Position.Y) - (points[h].Y + Position.Y)), (points[i].X + Position.X) - (points[h].X + Position.X));
                    closestLine = temp;
                }
                h = i;
            }
            displacementVector.Normalize();
            displacementVector *= closestLine;
            return displacementVector;
        }

        float AverageDistanceFromPointsToLine(Vector2 lineStart, Vector2 lineEnd, Vector2[] points, Vector2 position)
        {
            float[] results = new float[points.Length];

            //The line, where startLine is the origin
            Vector2 line = lineEnd - lineStart;
            float lineLength = line.Length();
            
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 point = points[i] + position;
                //Find where the point would be on the line
                float r = Vector2.Dot(point - lineStart, line) / lineLength;

                //Get the distance to the nearest point on the line segment
                if (r < 0)
                    results[i] = (lineStart - point).Length();
                else if (r > lineLength)
                    results[i] = (lineEnd - point).Length();
                else
                {
                    Vector2 projPoint = r * line + lineStart;
                    results[i] = (projPoint - point).Length();
                }
            }
            float sum = 0;
            for (int j = 0; j < results.Length; j++)
                sum += results[j];
            return sum / results.Length;
        }
    }
}