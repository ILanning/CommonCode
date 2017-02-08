using Microsoft.Xna.Framework;

namespace CommonCode
{
    /// <summary>
    /// Integer couterpart to Vector2.
    /// </summary>
    public struct Coordinate
    {
        public int X;
        public int Y;

        public Coordinate(int value)
        {
            X = value;
            Y = value;
        }

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        #region Math Operators

        public static bool operator==(Coordinate a, Coordinate b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator==(Coordinate a, Vector2 b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator!=(Coordinate a, Coordinate b)
        {
            return !(a == b);
        }

        public static bool operator!=(Coordinate a, Vector2 b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj != null)
                if (GetType() == obj.GetType())
                    return this == (obj as Coordinate?);
                else if (typeof(Vector2) == obj.GetType())
                    return this == (obj as Vector2?);
            return false;
        }

        public override int GetHashCode()
        {
            int result = 1;
            result *= 37 * X;
            result *= 37 * Y;
            return result;
        }

        public override string ToString()
        {
            return "(" + X.ToString() + ", " + Y.ToString() + ")";
        }

        public static Coordinate operator+(Coordinate a, Coordinate b)
        {
            return new Coordinate(a.X + b.X, a.Y + b.Y);
        }

        public static Coordinate operator+(Coordinate a, int b)
        {
            return new Coordinate(a.X + b, a.Y + b);
        }

        public static Coordinate operator-(Coordinate a, Coordinate b)
        {
            return new Coordinate(a.X - b.X, a.Y - b.Y);
        }

        public static Coordinate operator-(Coordinate a, int b)
        {
            return new Coordinate(a.X - b, a.Y - b);
        }

        public static Coordinate operator*(Coordinate a, Coordinate b)
        {
            return new Coordinate(a.X * b.X, a.Y * b.Y);
        }

        public static Coordinate operator*(Coordinate a, int b)
        {
            return new Coordinate(a.X * b, a.Y * b);
        }

        public static Coordinate operator/(Coordinate a, Coordinate b)
        {
            return new Coordinate(a.X / b.X, a.Y / b.Y);
        }

        public static Coordinate operator/(Coordinate a, int b)
        {
            return new Coordinate(a.X / b, a.Y / b);
        }

        #endregion

        #region Casting Operators

        public static implicit operator Vector2(Coordinate a)
        {
            return new Vector2(a.X, a.Y);
        }

        public static explicit operator Coordinate(Vector2 a)
        {
            return new Coordinate((int)a.X, (int)a.Y);
        }

        public static explicit operator Point(Coordinate a)
        {
            return new Point(a.X, a.Y);
        }

        public static implicit operator Coordinate(Point a)
        {
            return new Coordinate(a.X, a.Y);
        }

        #endregion

        #region Properties

        public static Coordinate Zero { get { return new Coordinate(0); } }

        public static Coordinate One { get { return new Coordinate(1); } }

        public static Coordinate Up { get { return new Coordinate(0, -1); } }

        public static Coordinate Down { get { return new Coordinate(0, 1); } }

        public static Coordinate Right { get { return new Coordinate(1, 0); } }

        public static Coordinate Left { get { return new Coordinate(-1, 0); } }

        #endregion
    }
}
