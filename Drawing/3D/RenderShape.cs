using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonCode.Drawing
{
    public abstract class RenderShape : IModifiable3D
    {
        public float depthBias = 0;
        public virtual Vector3 WorldPosition { get { return position; } set { position = value; } }
        public virtual Quaternion Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                if (rotation.LengthSquared() > 1.001f || rotation.LengthSquared() < 0.999f)
                    rotation.Normalize();
            }
        }
        public virtual Vector3 Scale { get { return scale; } set { scale = value; } }
        public virtual Color Color { get { return color; } set { color = value; } }
        public IModifier3D[] Modifiers { get { return modifiers; } }

        public void AddModifier(IModifier3D modifier)
        {
            modifier.Owner = this;
            for (int i = 0; i <= modifiers.Length; i++)
            {
                if (i == modifiers.Length)
                {
                    IModifier3D[] newModifiersArray = new IModifier3D[modifiers.Length + 4];
                    for (int h = 0; h < modifiers.Length; h++)
                        newModifiersArray[h] = modifiers[h];
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
            modifiers = new IModifier3D[4];
        }

        public abstract void Update();
        public abstract void Draw(Effect effect, GraphicsDevice graphics);
        public abstract void DrawGuidelines(Effect effect, GraphicsDevice graphics);

        protected Vector3 position;
        protected Quaternion rotation = Quaternion.Identity;
        protected Vector3 scale;
        protected Color color;
        protected IModifier3D[] modifiers = new IModifier3D[4];
    }
}
