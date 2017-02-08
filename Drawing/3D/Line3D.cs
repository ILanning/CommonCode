using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonCode.Drawing
{
    public class Line3D
    {
        public VertexPositionColor pointA;
        public VertexPositionColor pointB;

        public Vector3 WorldPosition
        {
            get
            {
                if (Vector3.DistanceSquared(ScreenManager.Globals.Camera.CameraPosition, pointA.Position) <
                    Vector3.DistanceSquared(ScreenManager.Globals.Camera.CameraPosition, pointB.Position))
                    return pointA.Position;
                return pointB.Position;
            }
            set
            {
                pointB.Position += value - pointA.Position;
                pointA.Position = value;
            }
        }

        public Line3D(Vector3 point1, Vector3 point2, Color shade)
        {
            //point1.Y -= 34;
            //point1.Z -= 2.1f;
            //point2.Y -= 34;
            //point2.Z -= 2.1f;
            pointA = new VertexPositionColor(point1, shade);
            pointB = new VertexPositionColor(point2, shade);
        }

        public void Draw(GraphicsDevice graphics)
        {
            graphics.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList,
                           new VertexPositionColor[] { pointA, pointB }, 0, 2, new int[] { 0, 1 }, 0, 1);
        }
    }
}
