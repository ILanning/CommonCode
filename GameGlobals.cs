using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonCode
{
    public class GameGlobals
    {
        public GraphicsDevice Graphics;
        public BasicEffect DefaultEffect;
        public SpriteBatch sb;
        public FontManager Fonts;
        public Camera Camera;
        public Texture2D White1By1;
        public RasterizerState ClipState = new RasterizerState() { ScissorTestEnable = true };
        public bool Initialized { get; private set; }

        public GameGlobals() 
        {
            Fonts = new FontManager();
        }

        public GameGlobals(GraphicsDevice graphics, BasicEffect defaultEffect, SpriteBatch spriteBatch, Camera camera)
        {
            Graphics = graphics;
            DefaultEffect = defaultEffect;
            sb = spriteBatch;
            Camera = camera;
            Fonts = new FontManager();
            White1By1 = new Texture2D(Graphics, 1, 1);
            White1By1.SetData<Color>(new Color[] { Color.White });
        }

        public void SetData()
        {
            Camera.Initialize();
            // Create the effect which will be used to set matrices, and colors for rendering
            if(DefaultEffect == null)
                DefaultEffect = new BasicEffect(Graphics);
            DefaultEffect.Projection = Camera.Projection;
            Fonts.AddFont("Default", ".\\Fonts\\DefaultFont.xnb");
            Initialized = true;
        }
    }
}
