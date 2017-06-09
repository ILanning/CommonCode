using CommonCode.Collision;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CommonCode
{
    public class QuadMap<T> : IBoundable where T : IBoundable
    {
        public Rectangle Bounds
        {
            get { return head.Bounds; }
        }

        QuadNode<T> head;

        public QuadMap()
        {
            
        }

        public QuadMap(T[] items)
        {

        }
    }

    public class QuadNode<T> where T : IBoundable
    {
        /// <summary>
        /// The physical size of the node
        /// </summary>
        public Rectangle Bounds { get { return new Rectangle(Top, Left, Bottom - Top, Right - Left); } }
        public int Top;
        public int Bottom;
        public int Left;
        public int Right;
        /// <summary>
        /// The maximum number of items before subdivision
        /// </summary>
        public int Limit = 1000;

        List<T> items;
        QuadNode<T> TopLeft;
        QuadNode<T> TopRight;
        QuadNode<T> BottomLeft;
        QuadNode<T> BottomRight;

        public QuadNode(Rectangle bounds)
        {
            Top = bounds.X;
            Bottom = bounds.Bottom;
            Left = bounds.Y;
            Right = bounds.Right;
        }

        public QuadNode(List<T> items)
        {
            Top = int.MaxValue;
            Left = int.MaxValue;
            Bottom = int.MinValue;
            Right = int.MinValue;

            foreach(T item in items)
            {
                int itemMinX = item.Bounds.X, itemMinY = item.Bounds.Y, itemMaxX = item.Bounds.Bottom, itemMaxY = item.Bounds.Right;
                if (itemMinX < Top)
                    itemMinX = Top;
                if (itemMinY < Left)
                    itemMinY = Left;
                if (itemMaxX > Bottom)
                    itemMaxX = Bottom;
                if (itemMaxY > Right)
                    itemMaxY = Right;
            }
        }
        public QuadNode(Rectangle bounds, List<T> items, out List<T> rejected)
        {
            Top = bounds.X;
            Bottom = bounds.Bottom;
            Left = bounds.Y;
            Right = bounds.Right;
            RejectExterior(items, out rejected, out this.items);
        }
        /// <summary>
        /// Adds items to this node, distributing among children or subdividing if needed
        /// </summary>
        /// <param name="items"></param>
        public void Add(List<T> items)
        {
            this.items.AddRange(items);
            if (TopLeft == null && this.items.Count > Limit)
                Subdivide();
        }
        /// <summary>
        /// Creates and populates the children of this node
        /// </summary>
        public void Subdivide()
        {
            if (TopLeft != null)
                return;
            int midX = (Bottom - Top) / 2 + Top, midY = (Right - Left) / 2 + Left;


        }
        /// <summary>
        /// Destroys this node's children after adding thier items to this node
        /// </summary>
        public void Collapse()
        {
            items.AddRange(TopLeft.items);
            items.AddRange(TopRight.items);
            items.AddRange(BottomLeft.items);
            items.AddRange(BottomRight.items);
            TopLeft = null;
            TopRight = null;
            BottomLeft = null;
            BottomRight = null;
        }
        /// <summary>
        /// Returns a list of the items that aren't within the bounds of this node
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public void RejectExterior(List<T> items, out List<T> rejected, out List<T> accepted)
        {
            int minX = Bounds.X, minY = Bounds.Y, maxX = Bounds.Bottom, maxY = Bounds.Right;
            rejected = new List<T>();
            accepted = new List<T>();

            foreach (T item in items)
            {
                int itemMinX = item.Bounds.X, itemMinY = item.Bounds.Y, itemMaxX = item.Bounds.Bottom, itemMaxY = item.Bounds.Right;
                if (itemMinX < minX || itemMinY < minY || itemMaxX > maxX || itemMaxY > maxY)
                    rejected.Add(item);
                else
                    accepted.Add(item);
            }
        }
    }
}
