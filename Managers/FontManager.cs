using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace CommonCode
{
    /// <summary>
    /// A ScreenManager level manager that tracks all fonts that the program has loaded.
    /// </summary>
    public class FontManager
    {
        public Dictionary<string, SpriteFont> FontLibrary = new Dictionary<string, SpriteFont>();
        public static DynamicContentManager Content;

        public FontManager() { }

        public SpriteFont this[string key]
        {
            get { return FontLibrary[key]; }
            private set { FontLibrary[key] = value; }
        }

        public void AddFont(string fontName, string path)
        {
            if (!FontLibrary.ContainsKey(fontName))
                FontLibrary.Add(fontName, Content.Load<SpriteFont>(path));
            //else
            //    throw new ArgumentException("The key '" + fontName + "' already exists in this FontManager.", "fontName");
        }
    }
}
