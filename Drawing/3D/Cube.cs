using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CommonCode.Drawing
{
    public class Cube : RenderShape, IDrawable3D
    {
        VertexPositionColorTexture[] cubeVertices = new VertexPositionColorTexture[24];
        VertexPositionColor[] frameVertices = new VertexPositionColor[20];
        static int[] drawOrder = new int[] { 0, 1, 2, 3, 1, 2,        4, 5, 6, 7, 5, 6,        8, 9, 10, 11, 9, 10, 
                                             12, 13, 14, 15, 13, 14,  16, 17, 18, 19, 17, 18,  20, 21, 22, 23, 21, 22};
        /// <summary>
        /// Contains the vertex color for each face.  Order: z, -x, x, -y, y, -z
        /// </summary>
        Color[] faceColors;
        Texture2D[] faceTextures;

        public float DepthBias { get; set; }

        public Cube(Vector3 position, Quaternion rotation, float radius, Color[] faceColors = null, Texture2D[] faceTextures = null)
        {
            WorldPosition = position;
            Rotation = rotation;
            Scale = new Vector3(radius, radius, radius);
            if (faceColors == null)
            {
                faceColors = new Color[6];
                for (int i = 0; i < 6; i++)
                    faceColors[i] = Color.White;
            }
            else if(faceColors.Length < 6)
            {
                Color[] newColors = new Color[6];
                if (faceColors.Length == 1)
                    for (int i = 0; i < 6; i++)
                        newColors[i] = faceColors[0];
                else
                {
                    int i = 0;
                    for (; i < faceTextures.Length; i++)
                    {
                        if (faceTextures[i] != null)
                            newColors[i] = faceColors[i];
                        else
                            newColors[i] = Color.White;
                    }
                    for (; i < 6; i++)
                        newColors[i] = Color.White;
                }
                faceColors = newColors;

            }
            else
            {
                for (int i = 0; i < faceColors.Length; i++)
                    if (faceColors[i] == null)
                        faceColors[i] = Color.White;
            }
            Texture2D blank = new Texture2D(ScreenManager.Globals.Graphics, 1, 1);
            blank.SetData<Color>(new Color[] { Color.White });
            if (faceTextures == null)
            {
                faceTextures = new Texture2D[6];
                for (int i = 0; i < 6; i++)
                    faceTextures[i] = blank;
            }
            else if (faceTextures.Length < 6)
            {
                Texture2D[] newTextures = new Texture2D[6];
                if (faceTextures.Length == 1)
                    for (int i = 0; i < 6; i++)
                        newTextures[i] = faceTextures[0];
                else
                {
                    int i = 0;
                    for (; i < faceTextures.Length; i++)
                    {
                        if (faceTextures[i] != null)
                            newTextures[i] = faceTextures[i];
                        else
                            newTextures[i] = blank;
                    }
                    for (; i < 6; i++)
                        newTextures[i] = blank;
                }
                faceTextures = newTextures;
            }
            else
            {
                for (int i = 0; i < 6; i++)
                    if (faceTextures[i] == null)
                        faceTextures[i] = blank;
            }
            this.faceColors = faceColors;
            this.faceTextures = faceTextures;

            cubeVertices[0] = new VertexPositionColorTexture(new Vector3(radius, radius, radius), faceColors[0], new Vector2(1, 1));
            cubeVertices[1] = new VertexPositionColorTexture(new Vector3(-radius, radius, radius), faceColors[0], new Vector2(0, 1));
            cubeVertices[2] = new VertexPositionColorTexture(new Vector3(radius, -radius, radius), faceColors[0], new Vector2(1, 0));
            cubeVertices[3] = new VertexPositionColorTexture(new Vector3(-radius, -radius, radius), faceColors[0], new Vector2(0, 0));

            cubeVertices[4] = new VertexPositionColorTexture(new Vector3(-radius, radius, radius), faceColors[1], new Vector2(1, 1));
            cubeVertices[5] = new VertexPositionColorTexture(new Vector3(-radius, radius, -radius), faceColors[1], new Vector2(1, 0));
            cubeVertices[6] = new VertexPositionColorTexture(new Vector3(-radius, -radius, radius), faceColors[1], new Vector2(0, 1));
            cubeVertices[7] = new VertexPositionColorTexture(new Vector3(-radius, -radius, -radius), faceColors[1], new Vector2(0, 0));

            cubeVertices[8] = new VertexPositionColorTexture(new Vector3(radius, radius, radius), faceColors[2], new Vector2(1, 1));
            cubeVertices[9] = new VertexPositionColorTexture(new Vector3(radius, radius, -radius), faceColors[2], new Vector2(1, 0));
            cubeVertices[10] = new VertexPositionColorTexture(new Vector3(radius, -radius, radius), faceColors[2], new Vector2(0, 1));
            cubeVertices[11] = new VertexPositionColorTexture(new Vector3(radius, -radius, -radius), faceColors[2], new Vector2(0, 0));

            cubeVertices[12] = new VertexPositionColorTexture(new Vector3(radius, -radius, radius), faceColors[3], new Vector2(1, 1));
            cubeVertices[13] = new VertexPositionColorTexture(new Vector3(-radius, -radius, radius), faceColors[3], new Vector2(0, 1));
            cubeVertices[14] = new VertexPositionColorTexture(new Vector3(radius, -radius, -radius), faceColors[3], new Vector2(1, 0));
            cubeVertices[15] = new VertexPositionColorTexture(new Vector3(-radius, -radius, -radius), faceColors[3], new Vector2(0, 0));

            cubeVertices[16] = new VertexPositionColorTexture(new Vector3(radius, radius, radius), faceColors[4], new Vector2(1, 1));
            cubeVertices[17] = new VertexPositionColorTexture(new Vector3(-radius, radius, radius), faceColors[4], new Vector2(0, 1));
            cubeVertices[18] = new VertexPositionColorTexture(new Vector3(radius, radius, -radius), faceColors[4], new Vector2(1, 0));
            cubeVertices[19] = new VertexPositionColorTexture(new Vector3(-radius, radius, -radius), faceColors[4], new Vector2(0, 0));

            cubeVertices[20] = new VertexPositionColorTexture(new Vector3(radius, radius, -radius), faceColors[5], new Vector2(1, 1));
            cubeVertices[21] = new VertexPositionColorTexture(new Vector3(-radius, radius, -radius), faceColors[5], new Vector2(0, 1));
            cubeVertices[22] = new VertexPositionColorTexture(new Vector3(radius, -radius, -radius), faceColors[5], new Vector2(1, 0));
            cubeVertices[23] = new VertexPositionColorTexture(new Vector3(-radius, -radius, -radius), faceColors[5], new Vector2(0, 0));
            float frameRadius = radius + radius / 100;
            frameVertices[0] = new VertexPositionColor(new Vector3(frameRadius, frameRadius, 0), Color.Gray);
            frameVertices[1] = new VertexPositionColor(new Vector3(-frameRadius, frameRadius, 0), Color.Gray);
            frameVertices[2] = new VertexPositionColor(new Vector3(-frameRadius, -frameRadius, 0), Color.Gray);
            frameVertices[3] = new VertexPositionColor(new Vector3(frameRadius, -frameRadius, 0), Color.Gray);
            frameVertices[4] = new VertexPositionColor(new Vector3(frameRadius, 0, frameRadius), Color.Gray);
            frameVertices[5] = new VertexPositionColor(new Vector3(-frameRadius, 0, frameRadius), Color.Gray);
            frameVertices[6] = new VertexPositionColor(new Vector3(-frameRadius, 0, -frameRadius), Color.Gray);
            frameVertices[7] = new VertexPositionColor(new Vector3(frameRadius, 0, -frameRadius), Color.Gray);
            frameVertices[8] = new VertexPositionColor(new Vector3(0, frameRadius, frameRadius), Color.Gray);
            frameVertices[9] = new VertexPositionColor(new Vector3(0, frameRadius, -frameRadius), Color.Gray);
            frameVertices[10] = new VertexPositionColor(new Vector3(0, -frameRadius, -frameRadius), Color.Gray);
            frameVertices[11] = new VertexPositionColor(new Vector3(0, -frameRadius, frameRadius), Color.Gray);

            frameVertices[12] = new VertexPositionColor(new Vector3(frameRadius, frameRadius, frameRadius), Color.Black);
            frameVertices[13] = new VertexPositionColor(new Vector3(-frameRadius, frameRadius, frameRadius), Color.Black);
            frameVertices[14] = new VertexPositionColor(new Vector3(-frameRadius, -frameRadius, frameRadius), Color.Black);
            frameVertices[15] = new VertexPositionColor(new Vector3(frameRadius, -frameRadius, frameRadius), Color.Black);
            frameVertices[16] = new VertexPositionColor(new Vector3(frameRadius, frameRadius, -frameRadius), Color.Black);
            frameVertices[17] = new VertexPositionColor(new Vector3(-frameRadius, frameRadius, -frameRadius), Color.Black);
            frameVertices[18] = new VertexPositionColor(new Vector3(-frameRadius, -frameRadius, -frameRadius), Color.Black);
            frameVertices[19] = new VertexPositionColor(new Vector3(frameRadius, -frameRadius, -frameRadius), Color.Black);
        }

        public void SetColor(int face, Color color)
        {
            for (int i = face * 4; i < face * 4 + 4; i++)
                cubeVertices[i].Color = color;
        }

        public void SetColors(Color plusX, Color minusX, Color plusY, Color minusY, Color plusZ, Color minusZ)
        {
            for (int i = 0; i < 4; i++)
            {
                cubeVertices[i].Color = plusZ;
                cubeVertices[i + 4].Color = minusX;
                cubeVertices[i + 8].Color = plusX;
                cubeVertices[i + 12].Color = minusY;
                cubeVertices[i + 16].Color = plusY;
                cubeVertices[i + 20].Color = minusZ;
            }
        }

        public override void Update()
        { }

        public override void Draw(Effect effect, GraphicsDevice graphics)
        {
            if (effect is BasicEffect)
                draw((BasicEffect)effect, graphics);
            if (effect is AlphaTestEffect)
                Draw((AlphaTestEffect)effect, graphics);
        }

        void draw(BasicEffect effect, GraphicsDevice graphics)
        {
            effect.TextureEnabled = true;
            effect.VertexColorEnabled = true;
            effect.World = Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                effect.Texture = faceTextures[0];
                pass.Apply();
                graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList,
                           cubeVertices, 0, 4, drawOrder, 0, 2);
                effect.Texture = faceTextures[1];
                pass.Apply();
                graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList,
                           cubeVertices, 0, 4, drawOrder, 6, 2);
                effect.Texture = faceTextures[2];
                pass.Apply();
                graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList,
                           cubeVertices, 0, 4, drawOrder, 12, 2);
                effect.Texture = faceTextures[3];
                pass.Apply();
                graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList,
                           cubeVertices, 0, 4, drawOrder, 18, 2);
                effect.Texture = faceTextures[4];
                pass.Apply();
                graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList,
                           cubeVertices, 0, 4, drawOrder, 24, 2);
                effect.Texture = faceTextures[5];
                pass.Apply();
                graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList,
                           cubeVertices, 0, 4, drawOrder, 30, 2);
            }
        }

        void draw(AlphaTestEffect effect, GraphicsDevice graphics)
        {
            effect.VertexColorEnabled = true;
            effect.World = Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                effect.Texture = faceTextures[0];
                pass.Apply();
                graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList,
                           cubeVertices, 0, 4, drawOrder, 0, 2);
                effect.Texture = faceTextures[1];
                pass.Apply();
                graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList,
                           cubeVertices, 0, 4, drawOrder, 6, 2);
                effect.Texture = faceTextures[2];
                pass.Apply();
                graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList,
                           cubeVertices, 0, 4, drawOrder, 12, 2);
                effect.Texture = faceTextures[3];
                pass.Apply();
                graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList,
                           cubeVertices, 0, 4, drawOrder, 18, 2);
                effect.Texture = faceTextures[4];
                pass.Apply();
                graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList,
                           cubeVertices, 0, 4, drawOrder, 24, 2);
                effect.Texture = faceTextures[5];
                pass.Apply();
                graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList,
                           cubeVertices, 0, 4, drawOrder, 30, 2);
            }
        }

        public override void DrawGuidelines(Effect effect, GraphicsDevice graphics)
        {
            if (effect is BasicEffect)
            {
                ((BasicEffect)effect).TextureEnabled = false;
                ((BasicEffect)effect).VertexColorEnabled = true;
                ((BasicEffect)effect).World = Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
            }
            if (effect is AlphaTestEffect)
            {
                ((AlphaTestEffect)effect).VertexColorEnabled = true;
                ((AlphaTestEffect)effect).World = Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
            }
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList,
                           frameVertices, 0, 4, new Int16[] { 0, 1, 1, 2, 2, 3, 3, 0 }, 0, 4);
                graphics.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList,
                           frameVertices, 0, 4, new Int16[] { 4, 5, 5, 6, 6, 7, 7, 4 }, 0, 4);
                graphics.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList,
                           frameVertices, 0, 4, new Int16[] { 8, 9, 9, 10, 10, 11, 11, 8 }, 0, 4);
                graphics.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList,
                           frameVertices, 0, 4, new Int16[] { 12, 13, 13, 14, 14, 15, 15, 12 }, 0, 4);
                graphics.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList,
                           frameVertices, 0, 4, new Int16[] { 16, 17, 17, 18, 18, 19, 19, 16 }, 0, 4);
                graphics.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList,
                           frameVertices, 0, 4, new Int16[] { 12, 16, 13, 17, 14, 18, 15, 19 }, 0, 4);
            }
        }
    }
}
