using Microsoft.Xna.Framework;
using System.Xml.Serialization;

namespace CommonCode.Content
{
    /*public struct CoordinateBuilder
    {
        public int X;
        public int Y;

        public CoordinateBuilder(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Coordinate(CoordinateBuilder a)
        {
            return new Coordinate(a.X, a.Y);
        }

        public static implicit operator CoordinateBuilder(Coordinate a)
        {
            return new CoordinateBuilder(a.X, a.Y);
        }

        public static implicit operator Point(CoordinateBuilder a)
        {
            return new Point(a.X, a.Y);
        }

        public static implicit operator CoordinateBuilder(Point a)
        {
            return new CoordinateBuilder(a.X, a.Y);
        }
    }*/

    public struct Vector2Builder
    {
        [XmlAttribute]
        public float X;
        [XmlAttribute]
        public float Y;

        public Vector2Builder(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Vector2(Vector2Builder a)
        {
            return new Vector2(a.X, a.Y);
        }

        public static implicit operator Vector2Builder(Vector2 a)
        {
            return new Vector2Builder(a.X, a.Y);
        }
    }

    public struct Vector3Builder
    {
        [XmlAttribute]
        public float X;
        [XmlAttribute]
        public float Y;
        [XmlAttribute]
        public float Z;

        public Vector3Builder(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static implicit operator Vector3(Vector3Builder a)
        {
            return new Vector3(a.X, a.Y, a.Z);
        }

        public static implicit operator Vector3Builder(Vector3 a)
        {
            return new Vector3Builder(a.X, a.Y, a.Z);
        }
    }

    public struct Vector4Builder
    {
        [XmlAttribute]
        public float X;
        [XmlAttribute]
        public float Y;
        [XmlAttribute]
        public float Z;
        [XmlAttribute]
        public float W;

        public Vector4Builder(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static implicit operator Vector4(Vector4Builder a)
        {
            return new Vector4(a.X, a.Y, a.Z, a.W);
        }

        public static implicit operator Vector4Builder(Vector4 a)
        {
            return new Vector4Builder(a.X, a.Y, a.Z, a.W);
        }

        public static implicit operator Quaternion(Vector4Builder a)
        {
            return new Quaternion(a.X, a.Y, a.Z, a.W);
        }

        public static implicit operator Vector4Builder(Quaternion a)
        {
            return new Vector4Builder(a.X, a.Y, a.Z, a.W);
        }
    }
}
