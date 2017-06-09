using CommonCode.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CommonCode.Drawing
{
    public class TexturedPlane : IModifiable3D, ICopyable<TexturedPlane>, IDrawable3D
    {
        Vector3 position;
        Quaternion rotation = Quaternion.Identity; 
        Vector2 scale;
        Color color;
        Texture2D texture;
        public float depthBias = 0;
        VertexPositionColorTexture[] vertexArray;

        public bool billboard;

        public TexturedPlane() { }

        public TexturedPlane(TexturedPlaneBuilder builder)
        {
            position = builder.Position;
            if(builder.Rotation != new Quaternion(0, 0, 0, 0))
                rotation = builder.Rotation;
            scale = builder.Scale;
            texture = ScreenManager.Content.Load<Texture2D>(builder.ImagePath);
            color = new Color(builder.Color);

            vertexArray = new VertexPositionColorTexture[4];
            vertexArray[0] = new VertexPositionColorTexture(new Vector3(scale.X, 0, scale.Y), color, new Vector2(1, 0));
            vertexArray[1] = new VertexPositionColorTexture(new Vector3(-scale.X, 0, scale.Y), color, new Vector2(1, 1));
            vertexArray[2] = new VertexPositionColorTexture(new Vector3(scale.X, 0, -scale.Y), color, new Vector2(0, 0));
            vertexArray[3] = new VertexPositionColorTexture(new Vector3(-scale.X, 0, -scale.Y), color, new Vector2(0, 1));
        }

        public TexturedPlane(string filePath, DynamicContentManager Content)
        {
            TexturedPlaneBuilder builder = TexturedPlaneBuilder.BuilderRead(filePath);

            position = builder.Position;
            if (builder.Rotation != new Quaternion(0, 0, 0, 0))
                rotation = builder.Rotation;
            scale = builder.Scale;
            texture = ScreenManager.Content.Load<Texture2D>(builder.ImagePath);
            color = new Color(builder.Color);

            vertexArray = new VertexPositionColorTexture[4];
            vertexArray[0] = new VertexPositionColorTexture(new Vector3(scale.X, 0, scale.Y), color, new Vector2(1, 0));
            vertexArray[1] = new VertexPositionColorTexture(new Vector3(-scale.X, 0, scale.Y), color, new Vector2(1, 1));
            vertexArray[2] = new VertexPositionColorTexture(new Vector3(scale.X, 0, -scale.Y), color, new Vector2(0, 0));
            vertexArray[3] = new VertexPositionColorTexture(new Vector3(-scale.X, 0, -scale.Y), color, new Vector2(0, 1));
        }

        public TexturedPlane(Vector3 location, Quaternion direction, Vector2 size, Color color, bool rotateToCamera, Texture2D image)
        {
            position = location;
            rotation = direction;
            scale = size;
            size *= 0.5f;
            this.color = color;
            texture = image;
            billboard = rotateToCamera;

            vertexArray = new VertexPositionColorTexture[4];
            vertexArray[0] = new VertexPositionColorTexture(new Vector3( size.X, 0,  size.Y), color, new Vector2(1, 1));
            vertexArray[1] = new VertexPositionColorTexture(new Vector3(-size.X, 0,  size.Y), color, new Vector2(0, 1));
            vertexArray[2] = new VertexPositionColorTexture(new Vector3( size.X, 0, -size.Y), color, new Vector2(1, 0));
            vertexArray[3] = new VertexPositionColorTexture(new Vector3(-size.X, 0, -size.Y), color, new Vector2(0, 0));
        }

        public void Update(GameTime gameTime)
        {
            if (rotation.LengthSquared() > 1.01f || rotation.LengthSquared() < 0.99f)
                rotation.Normalize();

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

        public void Draw(Effect effect, GraphicsDevice graphics)
        {
            Vector3 biasOffset = Vector3.Zero;
            if (billboard)
                Billboard();
            biasOffset = Vector3.Transform(new Vector3(0, -depthBias, 0), rotation);
            if (effect is BasicEffect)
            {
                BasicEffect be = (BasicEffect)effect;
                be.Texture = texture;
                be.VertexColorEnabled = true;
                be.World = Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position + biasOffset);
                be.TextureEnabled = true;
            }
            else if (effect is AlphaTestEffect)
            {
                AlphaTestEffect ate = (AlphaTestEffect)effect;
                ate.Texture = texture;
                ate.VertexColorEnabled = true;
                ate.World = Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position + biasOffset);
            }

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleStrip,
                    vertexArray, 0, 4, new int[] { 0, 1, 2, 3, 1, 2 }, 0, 2);
            }
        }

        public void Billboard()
        {
            Vector3 cameraDirection = ScreenManager.Globals.Camera.CameraPosition - position;
            cameraDirection.Normalize();
            Vector3 rightNormal = Vector3.Cross(Vector3.Up, cameraDirection);
            rightNormal.Normalize();

            //Align plane with camera's right normal
            float angleToRight = (float)Math.Acos(Vector3.Dot(Vector3.Right, rightNormal));
            if (rightNormal.Z >= 0)
                angleToRight = MathHelper.TwoPi - angleToRight;
            //Align plane with camera's direction
            float angleToUp = (float)Math.Acos(Vector3.Dot(Vector3.Up, cameraDirection));
            //Combine into a single quaternion
            rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, angleToRight) * Quaternion.CreateFromAxisAngle(Vector3.Right, angleToUp);
        }

        #region Properties

        public Vector3 WorldPosition { get { return position; } set { position = value; } }
        public Quaternion Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                if (rotation.LengthSquared() > 1.001f || rotation.LengthSquared() < 0.999f)
                    rotation.Normalize();
            }
        }
        public Vector3 Scale { get { return new Vector3(scale, 0); } set { scale = new Vector2(value.X, value.Y); } }
        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                for (int i = 0; i < vertexArray.Length; i++)
                    vertexArray[i].Color = color;
            }
        }
        public float DepthBias { get { return depthBias; } set { depthBias = value; } }

        #endregion

        #region IModifiable3D Members

        IModifier3D[] modifiers = new IModifier3D[4];

        public IModifier3D[] Modifiers
        {
            get { return modifiers; }
        }

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

        #endregion

        #region ICopyable<TexturedPlane> Members

        public TexturedPlane ShallowCopy()
        {
            TexturedPlane clone = new TexturedPlane();
            clone.billboard = billboard;
            clone.color = color;
            clone.modifiers = modifiers;
            clone.vertexArray = vertexArray;
            clone.texture = texture;
            clone.scale = scale;
            clone.rotation = rotation;
            clone.position = position;
            return clone;
        }

        public TexturedPlane ShallowCopy(LoadArgs l)
        {
            return ShallowCopy();
        }

        public TexturedPlane DeepCopy()
        {
            TexturedPlane clone = new TexturedPlane(position, rotation, scale, color, billboard, texture);
            for (int i = 0; i < modifiers.Length; i++)
                if (modifiers[i] != null)
                    clone.AddModifier(modifiers[i].DeepCopy(clone));
            return clone;
        }

        public TexturedPlane DeepCopy(LoadArgs l)
        {
            return DeepCopy();
        }

        #endregion
    }
}
