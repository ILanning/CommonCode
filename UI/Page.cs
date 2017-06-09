using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CommonCode.Drawing;

namespace CommonCode.UI
{
    /// <summary>
    /// Acts as a container for UI objects.
    /// </summary>
    public class Page : IModifiable2D
    {
        public StringPositionColor[] lines;
        public Sprite[] images;
        Vector2 position;
        float rotation;
        Vector2 scale;
        IModifier2D[] modifiers;

        public Page() 
        {
            images = new Sprite[0];
            modifiers = new IModifier2D[4];
        }

        public Page(Vector2 location, StringPositionColor[] text)
        {
            position = location;
            lines = text;
            scale = Vector2.One;
            images = new Sprite[0];
            modifiers = new IModifier2D[4];
        }

        public Page(Vector2 location, StringPositionColor[] text, Sprite[] textures)
        {
            position = location;
            lines = text;
            images = textures;
            scale = Vector2.One;
            modifiers = new IModifier2D[4];
        }

        public void Initialize()
        { 
        
        }

        public void LoadContent()
        { 
        
        }

        public void Update()
        {
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
            for (int i = 0; i < images.Length; i++)
            {
                images[i].Offset = position;
                images[i].Update();
            }
        }

        public void Draw2D()
        {
            for (int i = 0; i < lines.Length; i++)
            {
                ScreenManager.Globals.sb.DrawString(ScreenManager.Globals.Fonts["Default"], lines[i].Text,
                                        (lines[i].Position * scale) + position + scale / 2,
                                        lines[i].Color, rotation, scale / 2, 1, SpriteEffects.None, 0);
            }
            for (int i = 0; i < images.Length; i++)
            {
                images[i].Draw(ScreenManager.Globals.sb);
            }
        }

        #region IModifiable2D Members

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

        public Vector2 WorldPosition { get { return position; } set { position = value; } }

        public float Rotation { get { return rotation; } set { rotation = value; } }

        public Vector2 Scale { get { return scale; } set { scale = value; } }

        public Color Color { get; set; }

        public IModifier2D[] Modifiers { get { return modifiers; } }

        #endregion
    }
}
