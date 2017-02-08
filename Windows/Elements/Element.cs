using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonCode.Windows
{
    public abstract class Element
    {
        protected Rectangle targetArea;
        public virtual Coordinate Position 
        { 
            get { return targetArea.Location; }
            set { Move(targetArea.Location - value); }
        }
        public virtual Coordinate Size 
        {
            get { return new Coordinate(targetArea.Width, targetArea.Height); }
            set { Resize(new Rectangle(targetArea.Location.X, targetArea.Location.Y, value.X, value.Y)); } 
        }
        /// <summary>
        /// The minimum size for this element that its container should try to adhere to.  This size is not guaranteed.
        /// </summary>
        public virtual Coordinate MinimumSize { get; set; }
        public virtual Coordinate MaximumSize { get; set; }
        protected Vector2 idealDimensions = Vector2.Zero;
        /// <summary>
        /// The ideal ratio of dimensions for this object to be at.
        /// </summary>
        public virtual Vector2 IdealDimensions
        {
            get { return idealDimensions; }
            set
            {
                if (value.X == 0 || value.Y == 0)
                    idealDimensions = Vector2.Zero;
                else
                    idealDimensions = value;
            }
        }
        public string Name { get; protected set; }
        public string FullName { get; protected set; }
        /// <summary>
        /// Defines which side, if any, the object will stick to in its given space
        /// </summary>
        public SideTack SideAttachment { get; set; }
        /// <summary>
        /// Defines how the object will react to being given more or less space.
        /// </summary>
        public ResizeKind ResizeBehavior { get; set; }

        public Element() { MaximumSize = new Coordinate(int.MaxValue); }

        public abstract void Move(Coordinate movement);
        public abstract void Resize(Rectangle targetSpace);
        public virtual void Update(GameTime gameTime) { }
        public abstract void Draw(SpriteBatch sb);

        public virtual void BuildNames(string containerName)
        {
            //containers change names of contained when their names are assigned
            FullName = containerName + "." + Name;
        }

        //Takes the area that an element was given and the area that an element will use, 
        //  and places one within the other according to the sidestick value.
        protected Coordinate SideStick(Rectangle givenArea, Rectangle usedArea)
        {
            Coordinate spareSpace = new Coordinate(givenArea.Width, givenArea.Height) - new Coordinate(usedArea.Width, usedArea.Height);
            if (SideAttachment == SideTack.UpperLeft || (spareSpace.X <= 0 && spareSpace.Y <= 0))
                return givenArea.Location;
            Coordinate result = Coordinate.Zero;
            //if it's sticking to the left, result.X = 0 (so do nothing)
            //if it's sticking to the right, result.X = sparespace.X
            //if it's sticking to the top, result.Y = 0 (so do nothing)
            //if it's sticking to the bottom, result.Y = sparespace.Y
            if (SideAttachment == SideTack.LowerRight || SideAttachment == SideTack.Right || SideAttachment == SideTack.UpperRight)
                result.X = spareSpace.X;
            if (SideAttachment == SideTack.LowerLeft || SideAttachment == SideTack.Down || SideAttachment == SideTack.LowerRight)
                result.Y = spareSpace.Y;
            if (SideAttachment == SideTack.Up || SideAttachment == SideTack.Center || SideAttachment == SideTack.Down)
                result.X = spareSpace.X / 2;
            if (SideAttachment == SideTack.Left || SideAttachment == SideTack.Center || SideAttachment == SideTack.Right)
                result.Y = spareSpace.Y / 2;

            return result + givenArea.Location;
        }
    }
}
