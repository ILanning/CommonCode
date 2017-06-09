using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonCode.Drawing
{
    public class Sprite : IModifiable2D
    {
        protected Vector2 screenPosition;
        protected Vector2 positionOffset;
        protected float rotation;
        protected Vector2 scale;
        public Vector2 Origin;
        protected Color color;
        protected IModifier2D[] modifiers = new IModifier2D[4];
        public Rectangle textureSource;
        protected Texture2D texture;

        public Sprite() { }

        public Sprite(Vector2 position, Color color, Texture2D image)
            : this(position, Vector2.One, 0, color, new Rectangle(0, 0, image.Width, image.Height), image) { }

        public Sprite(Vector2 position, Color color, Rectangle textureSample, Texture2D image)
            : this(position, Vector2.One, 0, color, textureSample, image) { }

        public Sprite(Vector2 position, Vector2 scaleFactor, float angle, Color color, Texture2D image)
            : this(position, scaleFactor, angle, color, new Rectangle(0, 0, image.Width, image.Height), image) { }

        public Sprite(Vector2 position, Vector2 scaleFactor, float angle, Color color, Rectangle textureSample, Texture2D image) 
        {
            screenPosition = position;
            scale = scaleFactor;
            rotation = angle;
            this.color = color;
            textureSource = textureSample;
            Origin = new Vector2((textureSource.Width - textureSource.X)/2, (textureSource.Height -textureSource.Y)/2);
            texture = image;
        }

        public virtual void Update()
        {
            if (rotation > MathHelper.TwoPi)
                rotation -= MathHelper.TwoPi;
            else if (rotation < 0)
                rotation += MathHelper.TwoPi;

            for (int i = 0; i < modifiers.Length; i++)
            {
                if (modifiers[i] != null)
                {
                    modifiers[i].Update();
                    if (modifiers[i].RemoveIfComplete && !modifiers[i].Active)
                    {
                        modifiers[i].Remove();
                        i--;
                    }
                }
            }
        }

        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, screenPosition + positionOffset + Origin, textureSource, color, rotation, Origin, scale, SpriteEffects.None, 0);
        }

        public virtual void Draw(SpriteBatch sb, Vector2 customOffset)
        {
            sb.Draw(texture, screenPosition + positionOffset + customOffset + Origin, textureSource, color, rotation, Origin, scale, SpriteEffects.None, 0);
        }

        #region IModifiable2D Members        
        public IModifier2D[] Modifiers { get { return modifiers; } }

        public void AddModifier(IModifier2D modifier)
        {
            modifier.owner = this;
            for (int i = 0; i <= modifiers.Length; i++)
            {
                if (i == modifiers.Length)
                {
                    IModifier2D[] newModifiersArray = new IModifier2D[modifiers.Length + 4];
                    for (int h = 0; h < modifiers.Length; h++)
                    {
                        newModifiersArray[h] = modifiers[h];
                    }
                    newModifiersArray[modifiers.Length] = modifier;
                    modifiers = newModifiersArray;
                }
                if (modifiers[i] == null)
                {
                    modifiers[i] = modifier;
                    break;
                }
            }
        }

        public void ClearModifiers()
        {
            modifiers = new IModifier2D[4];
        }
        
        #endregion

        #region Properties

        public virtual Vector2 WorldPosition { get { return screenPosition; } set { screenPosition = value; } }

        public virtual Vector2 Offset { get { return positionOffset; } set { positionOffset = value; } }

        public float Rotation { get { return rotation; } set { rotation = value; } }

        public Vector2 Scale { get { return scale; } set { scale = value; } }

        public virtual Color Color { get { return color; } set { color = value; } }

        //public IModifier2D[] Modifiers { get { return modifiers; } }

        #endregion
    }
}
