using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace CommonCode
{
    public class RandomCamera : Camera
    {
        private float zoomLevel, zoomMin = 0, zoomMax = float.MaxValue;
        private float yawMin = 0, yawMax = MathHelper.TwoPi;
        private float pitchMin = 0, pitchMax = MathHelper.TwoPi;
        private float rollMin = 0, rollMax = MathHelper.TwoPi;
        private float yaw, pitch, roll;
        private bool aboutLookAt = true;
        private Random random;

        private Vector3 cameraDirectionalMomentum = Vector3.Zero;

        public RandomCamera() { }

        /// <summary>
        /// Creates a new RandomCamera.
        /// </summary>
        /// <param name="pos">Position that the camera will be locked to.</param>
        /// <param name="zoom">zoom distance that the camera will be locked to. -1 for random.</param>
        /// <param name="isLookAt">If true</param>
        public RandomCamera(Vector3 pos, float zoom, bool isLookAt)
        {
            if(isLookAt)
                lookAtPosition = pos;
            else
                CameraPosition = pos;
            aboutLookAt = isLookAt;
            if (zoom != -1f)
            {
                zoomMax = zoom;
                zoomMin = zoom;
                zoomLevel = zoom;
            }
            random = new Random();
            //zoomLevel = (lookAtPosition - cameraPos).Length();
            //projection = Matrix.CreateOrthographic(viewport.Width, viewport.Height, 1, FarPlaneDist);
            //view = Matrix.CreateLookAt(cameraPos, lookAtPos, Vector3.Up);
        }

        public override void Initialize()
        {
            viewport = ScreenManager.StaticGame.GraphicsDevice.Viewport;
            float aspectRatio = (float)viewport.Width / (float)viewport.Height;
            //projection = Matrix.CreateOrthographic
            float zoomLevel = 25;
            projection = Matrix.CreateOrthographic(zoomLevel * aspectRatio, zoomLevel, 0.1f, FarPlaneDist);
            Randomize();
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

                //view = Matrix.CreateLookAt(CameraPosition, LookAtPosition, new Vector3((float)Math.Sin(roll), (float)Math.Cos(roll), 0));
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

                if (zoomMax != -1 && zoomLevel > zoomMax)
                    zoomLevel = zoomMax;
                else if (zoomMin != -1 && zoomLevel < zoomMin)
                    zoomLevel = zoomMin;

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
            DrawBox(lookAtPosition, 0.1f, Color.White, graphics);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Randomize()
        {
            if (yawMax != yawMin)
                yaw = ((float)random.NextDouble()) * (yawMax - yawMin) + yawMin;
            else
                yaw = yawMax;
            if (pitchMax != pitchMin)
                pitch = ((float)random.NextDouble()) * (pitchMax - pitchMin) + pitchMin;
            else
                pitch = pitchMax;
            if (rollMax != rollMin)
                roll = ((float)random.NextDouble()) * (rollMax - rollMin) + rollMin;
            else
                roll = rollMax;
            Matrix rotationScale = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            rotationScale *= zoomLevel;
            if (aboutLookAt)
            {
                CameraPosition = Vector3.Transform(Vector3.UnitX, rotationScale) + lookAtPosition;
                view = Matrix.CreateLookAt(CameraPosition, lookAtPosition, new Vector3((float)Math.Sin(roll), (float)Math.Cos(roll), 0));//Vector3.Up);
            }
            else
            {
                lookAtPosition = Vector3.Transform(Vector3.UnitX, rotationScale) + CameraPosition;
                view = Matrix.CreateLookAt(CameraPosition, lookAtPosition, new Vector3((float)Math.Sin(roll), (float)Math.Cos(roll), 0));
            }
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
                return Quaternion.CreateFromAxisAngle(cameraDirection, roll);
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public override Vector3 Scale { get { return new Vector3(zoomLevel, 0, 0); } set { zoomLevel = value.X; } }

        #endregion
    }
}
