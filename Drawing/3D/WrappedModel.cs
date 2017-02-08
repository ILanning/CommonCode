using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using CommonCode.Content;

namespace CommonCode.Drawing
{
    public class WrappedModel : IModifiable3D, ICopyable<WrappedModel>
    {
        public Model model;
        private Matrix[] transforms;
        public bool[] meshIsAlphaEnabled;
        public Matrix[] OriginalBoneTransforms;
        public Vector3[] BoneRotations;

        private WrappedModel(WrappedModel other, bool deepCopy) 
        {
            model = other.model;
            if (deepCopy)
            {
                transforms = other.transforms.ToArray();
                meshIsAlphaEnabled = other.meshIsAlphaEnabled.ToArray();
                OriginalBoneTransforms = other.OriginalBoneTransforms.ToArray();
                BoneRotations = other.BoneRotations.ToArray();
                Scale = other.Scale;
                Rotation = other.Rotation;
            }
            else
            {
                transforms = other.transforms;
                meshIsAlphaEnabled = other.meshIsAlphaEnabled;
                OriginalBoneTransforms = other.OriginalBoneTransforms;
                BoneRotations = other.BoneRotations;
                Scale = other.Scale;
                Rotation = other.Rotation;
            }
        }

        public WrappedModel(string filePath, string baseDirectory, ContentManager Content)
        {
            WrappedModelBuilder builder = WrappedModelBuilder.BuilderRead(filePath, baseDirectory);

            model = Content.Load<Model>(builder.ModelPath);
            Scale = builder.Scale;
            transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            meshIsAlphaEnabled = builder.MeshAlpha;
            OriginalBoneTransforms = new Matrix[model.Meshes.Count];
            BoneRotations = new Vector3[model.Meshes.Count];
            for (int i = 0; i < BoneRotations.Length; i++)
                BoneRotations[0] = Vector3.Zero;

            for (int i = 0; i < model.Meshes.Count; i++)
            {
                foreach (BasicEffect effect in model.Meshes[i].Effects)
                {
                    effect.Projection = ScreenManager.Globals.Camera.Projection;
                    //effect.CommitChanges();
                }
                OriginalBoneTransforms[i] = model.Meshes[i].ParentBone.Transform;
            }
        }

        public WrappedModel(ContentManager Content, bool[] meshAlphas, string filePath)
        {
            model = Content.Load<Model>(filePath);
            transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            meshIsAlphaEnabled = meshAlphas;
            OriginalBoneTransforms = new Matrix[model.Meshes.Count];
            BoneRotations = new Vector3[model.Meshes.Count];
            for (int i = 0; i < BoneRotations.Length; i++)
                BoneRotations[0] = Vector3.Zero;

            for (int i = 0; i < model.Meshes.Count; i++ )
            {
                foreach (BasicEffect effect in model.Meshes[i].Effects)
                {
                    effect.Projection = ScreenManager.Globals.Camera.Projection;
                    //effect.CommitChanges();
                }
                OriginalBoneTransforms[i] = model.Meshes[i].ParentBone.Transform;
            }
        }

        public void RotateMesh(int meshNum, Vector3 angle)
        {
            //bool foundMesh = false;
            //for (int i = 0; i < model.Meshes.Count; i++ )
            //{
            //    if (model.Meshes[i].Name == meshName)
            //    {
            //        foundMesh = true;
                    BoneRotations[meshNum] += angle;
            //    }
            //}
        }

        public void Draw(BasicEffect effect, GraphicsDevice graphics)
        {
            for (int i = 0; i < model.Meshes.Count; i++)
            {
                foreach (BasicEffect meshEffect in model.Meshes[i].Effects)
                {
                    //float scale = 0;
                    meshEffect.View = effect.View;
                    meshEffect.Projection = effect.Projection;
                    //if (extension == ".X")
                    //    scale = 0.1f;
                    //else
                    //    scale = 0.0393f;
                    meshEffect.World = transforms[model.Meshes[i].ParentBone.Index] * Matrix.CreateScale(Scale)
                            * Matrix.CreateFromYawPitchRoll(BoneRotations[i].X, BoneRotations[i].Y, BoneRotations[i].Z)
                            * Matrix.CreateFromQuaternion(Rotation)
                            * Matrix.CreateTranslation(WorldPosition);
                    meshEffect.LightingEnabled = false;
                    //meshEffect.CommitChanges();
                    graphics.RasterizerState.MultiSampleAntiAlias = false;
                    if (true)//meshIsAlphaEnabled[i] != graphics.RasterizerState.AlphaBlendEnable)
                    {
                        if (meshIsAlphaEnabled[i])
                        {
                            //graphics.RasterizerState.AlphaBlendEnable = true;
                            //graphics.RenderState.DepthBufferEnable = false;
                            graphics.RasterizerState.CullMode = CullMode.None;
                            //graphics.RasterizerState.SourceBlend = Blend.BlendFactor;
                            //graphics.RasterizerState.DestinationBlend = Blend.One;
                        }
                        else
                        {
                            //graphics.RasterizerState.AlphaBlendEnable = false;
                            //graphics.RasterizerState.DepthBufferEnable = true;
                            graphics.RasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
                            //graphics.RasterizerState.SourceBlend = Blend.One;
                            //graphics.RasterizerState.DestinationBlend = Blend.Zero;
                        }
                    }
                }
                model.Meshes[i].Draw();
            }
            //graphics.VertexDeclaration = new VertexDeclaration(graphics, VertexPositionColor.VertexElements);
        }

        #region ICopyable<WrappedModel> Members

        public WrappedModel ShallowCopy()
        {
            WrappedModel temp = new WrappedModel(this, false);
            return temp;
        }

        public WrappedModel ShallowCopy(LoadArgs l)
        {
            WrappedModel temp = new WrappedModel(this, false);
            return temp;
        }

        public WrappedModel DeepCopy()
        {
            WrappedModel temp = new WrappedModel(this, true);
            return temp;
        }

        public WrappedModel DeepCopy(LoadArgs l)
        {
            WrappedModel temp = new WrappedModel(this, true);
            return temp;
        }

        #endregion

        #region IModifiable3D Members

        public Vector3 WorldPosition { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public Color Color { get; set; }

        public IModifier3D[] Modifiers { get { return modifiers; } }
        IModifier3D[] modifiers = new IModifier3D[4];

        public void AddModifier(IModifier3D modifier)
        {
            modifier.Owner = this;
            for (int i = 0; i <= modifiers.Length; i++)
            {
                if (i == modifiers.Length)
                {
                    IModifier3D[] newModifiersArray = new IModifier3D[modifiers.Length + 4];
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
            modifiers = new IModifier3D[4];
        }

        #endregion
    }
}
