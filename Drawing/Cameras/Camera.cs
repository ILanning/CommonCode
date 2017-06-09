using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonCode
{
    public abstract class Camera : IModifiable3D
    {
        protected Vector3 lookAtPosition = Vector3.Zero;

        protected Matrix projection;
        protected Matrix view;
        protected Viewport viewport;

        /// <summary>
        /// If set to true, this camera will not respont to player input.
        /// </summary>
        public bool InputIndependent = false;
        /// <summary>
        /// While this is true, the camera will not move for any reason.
        /// </summary>
        public bool Immobile = false;
        /// <summary>
        /// The farthest an in-world poly can be from the camera before being cut off.
        /// </summary>
        public static float FarPlaneDist = 2000;

        public abstract void Initialize();
        public abstract void RemakeProjection();
        public abstract void Update();
        public abstract void HandleInput();
        public abstract void Draw(Effect effect, GraphicsDevice graphics);

        #region Properties

        /// <summary>
        /// The location of the camera in 3D space.
        /// </summary>
        public virtual Vector3 CameraPosition { get; protected set; }
        /// <summary>
        /// The point in 3D space the camera is looking at.
        /// </summary>
        public virtual Vector3 LookAtPosition { get { return lookAtPosition; } protected set { lookAtPosition = value; } }
        /// <summary>
        /// The 'camera lens' for this camera.
        /// </summary>
        public Matrix Projection
        {
            get { return projection; }
            protected set { projection = value; }
        }
        /// <summary>
        /// The direction the camera is looking.
        /// </summary>
        public Matrix View
        {
            get { return view; }
            protected set { view = value; }
        }
        /// <summary>
        /// The size of the window the game is running in.
        /// </summary>
        public Viewport Viewport { get { return viewport; } set { viewport = value; } }

        #endregion

        #region IModifiable3D Members

        /// <summary>
        /// Position of the actual camera (as opposed to the point it is looking at.)
        /// </summary>
        public abstract Vector3 WorldPosition { get; set; }
        public abstract Quaternion Rotation { get; set; }
        public abstract Vector3 Scale { get; set; }
        /// <summary>
        /// Color of the debug cube the camera can draw.
        /// </summary>
        public Color Color { get { return color; } set { color = value; } }
        protected Color color;

        protected IModifier3D[] modifiers = new IModifier3D[4];

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

    /// <summary>
    /// Determines what kind of projection matrix is used when this camera draws.
    /// </summary>
    enum CameraMode
    { 
        Orthographic,
        Perspective
    }
}
