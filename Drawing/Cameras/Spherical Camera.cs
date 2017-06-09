using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace CommonCode
{
    /// <summary>
    /// A 3 dimensional camera based on the spherical coordinate system.  Ideal for 3rd person views.
    /// </summary>
    public class SphericalCamera : Camera
    {
        /// <summary>
        /// The closest you can be to the LookAtPosition.
        /// </summary>
        public float MinZoomLevel = 1;
        /// <summary>
        /// The farthest you can be from the LookAtPosition.
        /// </summary>
        public float MaxZoomLevel = -1;
        private float zoomLevel = 1;
        public Vector3 Movement = Vector3.Zero;

        public Vector2 MinCameraAngle = new Vector2(-1);
        public Vector2 MaxCameraAngle = new Vector2(-1);
        private Vector2 cameraAngle = new Vector2(0, 90);

        public Vector3 MaxWorldPosition = new Vector3(-1);
        public Vector3 MinWorldPosition = new Vector3(-1);

        private Vector3 directionalMomentum = Vector3.Zero;
        private Vector2 angularMomentum = Vector2.Zero;
        private float zoomMomentum = 0f;

        public SphericalCamera() { }

        public SphericalCamera(Vector3 lookAtPos, Vector2 startAngle, float startZoom)
        {
            LookAtPosition = lookAtPos;
            CameraAngle = startAngle;
            ZoomLevel = startZoom;
        }

        public override void Initialize()
        {
            viewport = ScreenManager.StaticGame.GraphicsDevice.Viewport;
            float aspectRatio = (float)viewport.Width / (float)viewport.Height;
            //"Normal" 3D camera lens
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 2f, FarPlaneDist);
            //Orthographic camera lens, for 2D compatability
            //Matrix Projection = Matrix.CreateOrthographic(viewport.Width, viewport.Height, -0.5f, 1);
            view = Matrix.CreateLookAt(Vector3.Zero, lookAtPosition, Vector3.Up);
        }

        public override void RemakeProjection()
        {
            viewport = ScreenManager.StaticGame.GraphicsDevice.Viewport;
            float aspectRatio = (float)viewport.Width / (float)viewport.Height;
            //"Normal" 3D camera lens
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 0.5f, FarPlaneDist);
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
                LookAtPosition += directionalMomentum;
                CameraAngle -= angularMomentum;

                directionalMomentum *= 0.9f;
                if (directionalMomentum.LengthSquared() < 0.00010f)
                    directionalMomentum = Vector3.Zero;
                angularMomentum *= 0.8f;
                if (angularMomentum.LengthSquared() < 0.00020f)
                    angularMomentum = Vector2.Zero;
                zoomMomentum *= 0.8f;
                if (angularMomentum.LengthSquared() < 0.00020f)
                    angularMomentum = Vector2.Zero;

                if (cameraAngle.Y > 179.9999f)
                    cameraAngle.Y = 179.9999f;
                else if (cameraAngle.Y < 0.0001f)
                    cameraAngle.Y = 0.0001f;

                if (cameraAngle.X > 360f)
                    cameraAngle.X -= 360f;
                else if (cameraAngle.X < 0f)
                    cameraAngle.X += 360f;

                //Formula (In terms of spherical): Camera = (r*Cos(phi)*Sin(theta)+LookAt.X, r*Cos(theta)+LookAt.Y, r*Sin(phi)*Sin(theta)+LookAt.Z)
                CameraPosition = new Vector3(((float)Math.Cos(MathHelper.ToRadians(CameraAngle.X)) * (float)Math.Sin(MathHelper.ToRadians(CameraAngle.Y)) * ZoomLevel) + LookAtPosition.X,
                                    ((float)Math.Cos(MathHelper.ToRadians(CameraAngle.Y)) * ZoomLevel) + LookAtPosition.Y,
                                    ((float)Math.Sin(MathHelper.ToRadians(CameraAngle.X)) * (float)Math.Sin(MathHelper.ToRadians(CameraAngle.Y)) * ZoomLevel) + LookAtPosition.Z);

                view = Matrix.CreateLookAt(CameraPosition, LookAtPosition, Vector3.Up);

                //Formula (In terms of spherical): Camera = (r*Cos(theta)+LookAt.X, r*Sin(phi)+LookAt.Y, r*Sin(theta)+LookAt.Z)
                //cameraPosition = new Vector3((float)Math.Cos(MathHelper.ToRadians(CameraAngle.Y)*ZoomLevel) + LookAtPosition.X,
                //                    (float)Math.Sin(MathHelper.ToRadians(CameraAngle.X) * ZoomLevel) + LookAtPosition.Y,
                //                    (float)Math.Sin(MathHelper.ToRadians(CameraAngle.Y) * ZoomLevel) + LookAtPosition.Z);
            }
        }

        public override void HandleInput()
        {
            if (!Immobile && !InputIndependent)
            {
                Vector3 movementVector = Vector3.Zero;

                //TODO: Figure out why I wrote this this way 4 years ago.  Some sort of speed check?
                //Speed is related to the camera's zoom level.  This prevents the camera from exceeding the 
                if (directionalMomentum.LengthSquared() < 0.01f * (zoomLevel * zoomLevel) / 2)
                {
                    Vector3 cameraDirection = new Vector3(CameraPosition.X - LookAtPosition.X, 0, CameraPosition.Z - LookAtPosition.Z);
                    cameraDirection.Normalize();
                    Vector3 rightNormal = Vector3.Cross(cameraDirection, Vector3.Up);
                    rightNormal.Normalize();

                    if (InputManager.IsKeyDown(Keys.Down) || InputManager.IsKeyDown(Keys.S))
                        movementVector += cameraDirection;
                    if (InputManager.IsKeyDown(Keys.Up) || InputManager.IsKeyDown(Keys.W))
                        movementVector -= cameraDirection;
                    if (InputManager.IsKeyDown(Keys.Left) || InputManager.IsKeyDown(Keys.A))
                        movementVector += rightNormal;
                    if (InputManager.IsKeyDown(Keys.Right) || InputManager.IsKeyDown(Keys.D))
                        movementVector -= rightNormal;
                    if (InputManager.IsKeyDown(Keys.PageUp) || InputManager.IsKeyDown(Keys.Q))
                        movementVector.Y++;
                    if (InputManager.IsKeyDown(Keys.PageDown) || InputManager.IsKeyDown(Keys.E))
                        movementVector.Y--;

                    if (movementVector != Vector3.Zero)
                    {
                        movementVector.Normalize();
                        directionalMomentum += movementVector * 0.01f * (zoomLevel / 2);
                    }
                }
                Movement = movementVector;

                if (InputManager.HasScrolledDown)
                    ZoomLevel += (float)Math.Sqrt(zoomLevel);
                else if (InputManager.HasScrolledUp)
                    ZoomLevel -= (float)Math.Sqrt(zoomLevel);

                if (MaxZoomLevel != -1 && zoomLevel > MaxZoomLevel)
                    zoomLevel = MaxZoomLevel;
                else if (MinZoomLevel != -1 && zoomLevel < MinZoomLevel)
                    zoomLevel = MinZoomLevel;

                if (InputManager.IsMouseButtonDown(MouseButtons.MMB))
                {
                    if (InputManager.MouseMovement.Y > 0 && cameraAngle.Y < 180)
                        angularMomentum.Y += InputManager.MouseMovement.Y / 10;
                    else if (InputManager.MouseMovement.Y < 0 && cameraAngle.Y > 0)
                        angularMomentum.Y += InputManager.MouseMovement.Y / 10;
                    angularMomentum.X -= InputManager.MouseMovement.X / 10;
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
                Vector3 leftNormal = Vector3.Cross(cameraDirection, Vector3.Down); //Not entirely sure why the left normal is needed, but it works
                leftNormal.Normalize();
                Quaternion returnValue = Quaternion.CreateFromAxisAngle(leftNormal, MathHelper.ToRadians(CameraAngle.Y));
                returnValue.Normalize();
                return returnValue;
            }
            set
            {
                Vector3 coordinate = Vector3.Transform(Vector3.UnitY, value);
                coordinate.Normalize();
                if (coordinate.X == 0f)
                    coordinate.X = 0.0001f;
                float theta = (float)Math.Acos(coordinate.Y);
                float phi = (float)Math.Atan2(coordinate.Z, coordinate.X);
                CameraAngle = new Vector2(MathHelper.ToDegrees(phi), MathHelper.ToDegrees(theta));
            }
        }
        public override Vector3 Scale { get { return new Vector3(ZoomLevel, 0, 0); } set { ZoomLevel = value.X; } }

        //public Quaternion Rotation 
        //{ 
        //    get 
        //    { 
        //        Vector3 cameraDirection = CameraPosition - lookAtPosition;
        //        cameraDirection.Normalize();
        //        return Quaternion.CreateFromAxisAngle(cameraDirection, MathHelper.PiOver2);
        //    }
        //    set
        //    {
        //        float halfAngleSine = (float)Math.Sin(Math.Acos(value.W));
        //        Vector3 axis = new Vector3(value.X / halfAngleSine, value.Y / halfAngleSine, value.Z / halfAngleSine);
        //        cameraAngle = new Vector2((float)Math.Acos(axis.Z / axis.Length()),
        //                                    (float)Math.Atan(axis.Y / axis.X));
        //    } 
        //}

        #endregion

        #region Properties

        public float ZoomLevel 
        {
            get { return zoomLevel; }
            set
            {
                if (MinZoomLevel != -1 && value < MinZoomLevel)
                    zoomLevel = MinZoomLevel;
                else if (MaxZoomLevel != -1 && value > MaxZoomLevel)
                    zoomLevel = MaxZoomLevel;
                else
                    zoomLevel = value;
            }
        }

        public Vector2 CameraAngle
        {
            get { return cameraAngle; }
            set
            {
                if (MaxCameraAngle.X != -1 && value.X > MaxCameraAngle.X)
                    cameraAngle.X = MaxCameraAngle.X;
                else if (MinCameraAngle.X != -1 && value.X < MinCameraAngle.X)
                    cameraAngle.X = MinCameraAngle.X;
                else if (value.X == 0)
                    cameraAngle.X = 0.0001f;
                else
                    cameraAngle.X = value.X;

                if (MaxCameraAngle.Y != -1 && value.Y > MaxCameraAngle.Y)
                    cameraAngle.Y = MaxCameraAngle.Y;
                else if (MinCameraAngle.Y != -1 && value.Y < MinCameraAngle.Y)
                    cameraAngle.Y = MinCameraAngle.Y;
                else if (value.Y == 0)
                    cameraAngle.Y = 0.0001f;
                else
                    cameraAngle.Y = value.Y;
                if (value.X == float.NaN)
                { throw new NotFiniteNumberException("Something set the camera angle to NaN."); }
            }
        }

        public override Vector3 LookAtPosition
        {
            get { return lookAtPosition; }
            protected set
            {
                if (MaxWorldPosition.X != -1 && value.X > MaxWorldPosition.X)
                    lookAtPosition.X = MaxWorldPosition.X;
                else if (MinWorldPosition.X != -1 && value.X < MinWorldPosition.X)
                    lookAtPosition.X = MinWorldPosition.X;
                else
                    lookAtPosition.X = value.X;

                if (MaxWorldPosition.Y != -1 && value.Y > MaxWorldPosition.Y)
                    lookAtPosition.Y = MaxWorldPosition.Y;
                else if (MinWorldPosition.Y != -1 && value.Y < MinWorldPosition.Y)
                    lookAtPosition.Y = MinWorldPosition.Y;
                else
                    lookAtPosition.Y = value.Y;

                if (MaxWorldPosition.Z != -1 && value.Z > MaxWorldPosition.Z)
                    lookAtPosition.Z = MaxWorldPosition.Z;
                else if (MinWorldPosition.Z != -1 && value.Z < MinWorldPosition.Z)
                    lookAtPosition.Z = MinWorldPosition.Z;
                else
                    lookAtPosition.Z = value.Z;
                if (value.X == float.NaN)
                { throw new NotFiniteNumberException("Something set the camera look at position to NaN."); }
            }
        }

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
