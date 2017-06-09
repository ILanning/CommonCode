using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace CommonCode
{
    /// <summary>
    /// A camera for use with SpriteBatch.
    /// </summary>
    public class FlatCamera : Camera
    {
        public float minZoomLevel = 3;
        public float maxZoomLevel = 450;
        public float zoomLevel = 1;

        private Vector3 cameraDirectionalMomentum = Vector3.Zero;

        public FlatCamera() { }

        public FlatCamera(Vector3 cameraPos, Vector3 lookAtPos)
        {
            lookAtPosition = lookAtPos;
            CameraPosition = cameraPos;
            zoomLevel = (lookAtPosition - cameraPos).Length();
            //projection = Matrix.CreateOrthographic(viewport.Width, viewport.Height, 1, FarPlaneDist);
            //view = Matrix.CreateLookAt(cameraPos, lookAtPos, Vector3.Up);
        }

        //public FlatCamera(Vector3 lookAtPos, Vector2 startAngle, float startZoom)
        //{
        //    lookAtPosition = lookAtPos;
        //    cameraAngle = startAngle;
        //    zoomLevel = startZoom;
        //}

        public override void Initialize()
        {
            viewport = ScreenManager.StaticGame.GraphicsDevice.Viewport;
            float aspectRatio = (float)viewport.Width / (float)viewport.Height;
            //projection = Matrix.CreateOrthographic
            zoomLevel = 25;
            projection = Matrix.CreateOrthographic(zoomLevel * aspectRatio, zoomLevel, 0.1f, FarPlaneDist);
            view = Matrix.CreateLookAt(CameraPosition, LookAtPosition, Vector3.Up);
        }

        public override void RemakeProjection()
        {
            viewport = ScreenManager.StaticGame.GraphicsDevice.Viewport;
            float aspectRatio = (float)viewport.Width / (float)viewport.Height;
            projection = Matrix.CreateOrthographic(zoomLevel * aspectRatio, zoomLevel, 0.1f, FarPlaneDist);
        }

        public override void Update()
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

            if (!Immobile)
            {
                LookAtPosition += cameraDirectionalMomentum;
                CameraPosition += cameraDirectionalMomentum;

                cameraDirectionalMomentum *= 0.9f;
                if (cameraDirectionalMomentum.LengthSquared() < 0.00010f)
                    cameraDirectionalMomentum = Vector3.Zero;

                view = Matrix.CreateLookAt(CameraPosition, LookAtPosition, Vector3.Up);
            }
        }

        public override void HandleInput()
        {
            if (!Immobile && !InputIndependent)
            {
                Vector3 cameraDirection = new Vector3(CameraPosition.X - LookAtPosition.X, 0, CameraPosition.Z - LookAtPosition.Z);
                cameraDirection.Normalize();
                Vector3 rightNormal = Vector3.Cross(cameraDirection, Vector3.Up);
                rightNormal.Normalize();
                Vector3 movementVector = Vector3.Zero;

                if ((InputManager.IsKeyDown(Keys.Down) && !InputManager.IsKeyDown(Keys.Up)) && cameraDirectionalMomentum.LengthSquared() < 0.01f * (zoomLevel * zoomLevel) / 2)
                    movementVector += cameraDirection;
                else if ((InputManager.IsKeyDown(Keys.Up) && !InputManager.IsKeyDown(Keys.Down)) && cameraDirectionalMomentum.LengthSquared() < 0.01f * (zoomLevel * zoomLevel) / 2)
                    movementVector -= cameraDirection;
                if (InputManager.IsKeyDown(Keys.Left) && !InputManager.IsKeyDown(Keys.Right) && cameraDirectionalMomentum.LengthSquared() < 0.01f * (zoomLevel * zoomLevel) / 2)
                    movementVector += rightNormal;
                else if (InputManager.IsKeyDown(Keys.Right) && !InputManager.IsKeyDown(Keys.Left) && cameraDirectionalMomentum.LengthSquared() < 0.01f * (zoomLevel * zoomLevel) / 2)
                    movementVector -= rightNormal;
                if (InputManager.IsKeyDown(Keys.PageUp) && !InputManager.IsKeyDown(Keys.PageDown) && cameraDirectionalMomentum.LengthSquared() < 0.01f * (zoomLevel * zoomLevel) / 2)
                    movementVector.Y++;
                else if (InputManager.IsKeyDown(Keys.PageDown) && !InputManager.IsKeyDown(Keys.PageUp) && cameraDirectionalMomentum.LengthSquared() < 0.01f * (zoomLevel * zoomLevel) / 2)
                    movementVector.Y--;

                if (InputManager.HasScrolledDown)
                    zoomLevel += (float)Math.Sqrt(zoomLevel);
                else if (InputManager.HasScrolledUp)
                    zoomLevel -= (float)Math.Sqrt(zoomLevel);

                if (maxZoomLevel != -1 && zoomLevel > maxZoomLevel)
                    zoomLevel = maxZoomLevel;
                else if (minZoomLevel != -1 && zoomLevel < minZoomLevel)
                    zoomLevel = minZoomLevel;

                if (movementVector != Vector3.Zero)
                {
                    movementVector.Normalize();
                    cameraDirectionalMomentum += movementVector * 0.01f * (zoomLevel / 2);
                }
            }
        }

        public override void Draw(Effect effect, GraphicsDevice graphics)
        {
            if (!(effect is BasicEffect))
                throw new ArgumentException("effect", "Cameras only support the BasicEffect");
            ((BasicEffect)effect).TextureEnabled = false;
            ((BasicEffect)effect).VertexColorEnabled = true;
            //effect.CommitChanges();
            DrawBox(lookAtPosition, 0.1f, Color.White, graphics);
        }

        public void DrawBox(Vector3 position, float radius, Color color, GraphicsDevice graphics)
        {
            VertexPositionColor[] lineVertexArray = new VertexPositionColor[] { 
                            new VertexPositionColor(new Vector3( radius + position.X,  radius + position.Y,  radius + position.Z), color), 
                            new VertexPositionColor(new Vector3(-radius + position.X,  radius + position.Y,  radius + position.Z), color), 
                            new VertexPositionColor(new Vector3( radius + position.X, -radius + position.Y,  radius + position.Z), color), 
                            new VertexPositionColor(new Vector3( radius + position.X,  radius + position.Y, -radius + position.Z), color), 
                            new VertexPositionColor(new Vector3(-radius + position.X, -radius + position.Y,  radius + position.Z), color), 
                            new VertexPositionColor(new Vector3( radius + position.X, -radius + position.Y, -radius + position.Z), color),
                            new VertexPositionColor(new Vector3(-radius + position.X,  radius + position.Y, -radius + position.Z), color), 
                            new VertexPositionColor(new Vector3(-radius + position.X, -radius + position.Y, -radius + position.Z), color)};
            int[] vertexOrderArray = new int[24] { 0, 1, 1, 4, 4, 2, 2, 0, 0, 3, 1, 6, 4, 7, 2, 5, 3, 6, 6, 7, 7, 5, 5, 3 };
            //graphics.VertexDeclaration = new VertexDeclaration(graphics, VertexPositionColor.VertexElements);
            graphics.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList,
                       lineVertexArray, 0, 8, vertexOrderArray, 0, 12);
        }
        
        #region IModifiable3D Members

        public override Vector3 WorldPosition { get { return lookAtPosition; } set { lookAtPosition = value; } }
        public override Quaternion Rotation
        {
            get
            {
                Vector3 cameraDirection = CameraPosition - lookAtPosition;
                cameraDirection.Normalize();
                return Quaternion.CreateFromAxisAngle(cameraDirection, MathHelper.PiOver2);
            }
            set
            {
                throw new NotImplementedException();
                //float halfAngleSine = (float)Math.Sin(Math.Acos(value.W));
                //Vector3 axis = new Vector3(value.X / halfAngleSine, value.Y / halfAngleSine, value.Z / halfAngleSine);
                //cameraAngle = new Vector2((float)Math.Acos(axis.Z / axis.Length()),
                //                            (float)Math.Atan(axis.Y / axis.X));
            }
        }
        public override Vector3 Scale { get { return new Vector3(zoomLevel, 0, 0); } set { zoomLevel = value.X; } }

        #endregion
    }

    #region Obsolete Rotation Modifiers
    /*
    class RotateToModifier : IModifier
    {
        public IModifiable owner { get; set; }

        /// <summary>
        /// Speed, per frame, at which the rotation changes.
        /// </summary>
        public int frames;
        /// <summary>
        /// Set to true to delete this modifier when Active is false.
        /// </summary>
        public bool RemoveIfComplete { get; set; }
        /// <summary>
        /// Set to true to temporarily stop this modifier.
        /// </summary>
        public bool Paused { get; set; }
        /// <summary>
        /// If set to false, this modifier has completed its task.
        /// </summary>
        public bool Active { get; set; }

        Vector2 lerpSpeed = Vector2.Zero;

        /// <summary>
        /// Creates a new RotateTo Modifier.
        /// </summary>
        /// <param name="targetRotation">Rotation, in radians, that the owner will rotate to.</param>
        /// <param name="owner">The object the modifier will be applied to.</param>
        ///  <param name="removeIfComplete">Set to true to delete this modifier when Active is false.</param>
        /// <param name="time">Time, in frames, it will take to rotate.  Set to 1 for immediate rotation.</param>
        public RotateToModifier(Vector3 targetRotation, IModifiable owner, bool removeIfComplete, int time)
        {
            if (time < 1)
                throw new ArgumentException("This modifier takes at least 1 frame to execute.", "time");
            frames = time;
            Active = true;
            if (targetRotation.X != -1)
                lerpSpeed.X = (owner.Rotation.X - targetRotation.X) / time;
            if (targetRotation.Y != -1)
                lerpSpeed.Y = (owner.Rotation.Y - targetRotation.Y) / time;
        }

        public void Reset(Vector3 rotationSpeed, bool removeIfComplete, int time)
        {
            frames = time;
            RemoveIfComplete = removeIfComplete;
            Active = true;
        }

        public void Update()
        {
            if (!Paused && Active)
            {
                owner.Rotation -= lerpSpeed;
                frames--;
                if (frames == 0)
                    Active = false;
            }
        }

        public void Remove()
        {
            bool removed = false;
            for (int i = 0; i < owner.Modifiers.Length; i++)
            {
                if (owner.Modifiers[i] == this)
                {
                    owner.Modifiers[i] = null;
                    removed = true;
                }
                if (removed && i != owner.Modifiers.Length - 2)
                    owner.Modifiers[i] = owner.Modifiers[i + 1];
            }
            owner.Modifiers[owner.Modifiers.Length - 1] = null;
        }

        public RotateToModifier ShallowCopy()
        {
            throw new NotImplementedException();
        }

        public void DeepCopy()
        {
            throw new NotImplementedException();
        }
    }

    class RotateModifier : IModifier
    {
        public IModifiable owner { get; set; }

        public Vector2 rotationalMomentum;
        /// <summary>
        /// Speed, per frame, at which the rotation changes.
        /// </summary>
        public int frames;

        public bool RemoveIfComplete { get; set; }
        /// <summary>
        /// Set to true to temporarily stop this modifier.
        /// </summary>
        public bool Paused { get; set; }
        /// <summary>
        /// If set to false, this modifier has completed its task.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Creates a new Rotate Modifier.
        /// </summary>
        /// <param name="rotationSpeed">Speed, in radians, at which the owner will rotate.</param>
        /// <param name="time">Time, in frames, it will rotate for.  Set to -1 to rotate forever.</param>
        public RotateModifier(Vector2 rotationSpeed, bool removeIfComplete, int time)
        {
            rotationalMomentum = rotationSpeed;
            frames = time;
            RemoveIfComplete = removeIfComplete;
            Active = true;
        }

        public void Reset(Vector2 rotationSpeed, bool removeIfComplete, int time)
        {
            rotationalMomentum = rotationSpeed;
            frames = time;
            RemoveIfComplete = removeIfComplete;
            Active = true;
        }

        public void Update()
        {
            if (!Paused && Active)
            {
                if (frames != 0)
                    owner.Rotation += rotationalMomentum;
                else
                    Active = false;
            }
        }

        public void Remove()
        {
            bool removed = false;
            for (int i = 0; i < owner.Modifiers.Length; i++)
            {
                if (owner.Modifiers[i] == this)
                {
                    owner.Modifiers[i] = null;
                    removed = true;
                }
                if (removed && i != owner.Modifiers.Length - 2)
                    owner.Modifiers[i] = owner.Modifiers[i + 1];
            }
            owner.Modifiers[owner.Modifiers.Length - 1] = null;
        }

        public RotateModifier ShallowCopy()
        {
            return new RotateModifier(rotationalMomentum, RemoveIfComplete, frames);
        }

        public void DeepCopy()
        {
            throw new NotImplementedException();
        }
    }
    */
    #endregion
}
