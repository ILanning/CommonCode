using Microsoft.Xna.Framework;
using System;

namespace CommonCode.Modifiers
{
    /// <summary>
    /// Rotates the owner to a specific angle over time.
    /// </summary>
    public class RotateToModifier3D : IModifier3D
    {
        public IModifiable3D Owner { get; set; }

        /// <summary>
        /// A string representing this modifier.
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Speed, per frame, at which the rotation changes.
        /// </summary>
        public int frames;
        /// <summary>
        /// Set to true to delete this modifier when Active is false.
        /// </summary>
        public bool RemoveIfComplete { get; set; }
        bool paused = false;
        /// <summary>
        /// Set to true to temporarily stop this modifier.
        /// </summary>
        public bool Paused
        {
            get { return paused; }
            set
            {
                paused = value;
                if (Pause != null)
                    Pause(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// If set to false, this modifier has completed its task.
        /// </summary>
        public bool Active { get; set; }

        public event EventHandler Complete;
        public event EventHandler Pause;

        Quaternion lerpSpeed = Quaternion.Identity;

        /// <summary>
        /// Creates a new RotateTo Modifier.
        /// </summary>
        /// <param name="targetRotation">Quaternion that the owner will rotate to.</param>
        /// <param name="owner">The object the modifier will be applied to.</param>
        ///  <param name="removeIfComplete">Set to true to delete this modifier when Active is false.</param>
        /// <param name="time">Time, in frames, it will take to rotate.  Set to 1 for immediate rotation.</param>
        public RotateToModifier3D(Quaternion targetRotation, IModifiable3D owner, bool removeIfComplete, int time)
        {
            if (time < 1)
                throw new ArgumentException("This modifier takes at least 1 frame to execute.", "time");
            frames = time;
            this.Owner = owner;
            Active = true;
            targetRotation.Normalize();
            Vector3 rotationAxis = Vector3.Transform(Vector3.UnitY, owner.Rotation);
            rotationAxis.Normalize();
            Vector3 targetAxis = Vector3.Transform(Vector3.UnitY, targetRotation);
            targetAxis.Normalize();

            float angleToTarget = (float)Math.Acos(Vector3.Dot(rotationAxis, targetAxis));
            if (float.IsNaN(angleToTarget))
                angleToTarget = 0;
            if (angleToTarget != 0)
            {
                float lerpAngularSpeed = angleToTarget / time;
                Vector3 pivotAxis = Vector3.Cross(rotationAxis, targetAxis);
                pivotAxis.Normalize();
                lerpSpeed = Quaternion.CreateFromAxisAngle(pivotAxis, lerpAngularSpeed);
            }
            else
                lerpSpeed = Quaternion.Identity;
        }

        private RotateToModifier3D() { }

        public void Reset(Quaternion targetRotation, bool removeIfComplete, int time)
        {
            if (time < 1)
                throw new ArgumentException("This modifier takes at least 1 frame to execute.", "time");
            frames = time;
            Active = true;
            targetRotation.Normalize();
            Vector3 rotationAxis = Vector3.Transform(Vector3.UnitY, Owner.Rotation);
            rotationAxis.Normalize();
            Vector3 targetAxis = Vector3.Transform(Vector3.UnitY, targetRotation);
            targetAxis.Normalize();

            float angleToTarget = (float)Math.Acos(Vector3.Dot(rotationAxis, targetAxis));
            if (float.IsNaN(angleToTarget))
                angleToTarget = 0;
            if (angleToTarget != 0)
            {
                float lerpAngularSpeed = angleToTarget / time;
                Vector3 pivotAxis = Vector3.Cross(rotationAxis, targetAxis);
                pivotAxis.Normalize();
                lerpSpeed = Quaternion.CreateFromAxisAngle(pivotAxis, lerpAngularSpeed);
            }
            else
                lerpSpeed = Quaternion.Identity;
        }

        public void Update()
        {
            if (!Paused && Active)
            {
                Owner.Rotation = lerpSpeed * Owner.Rotation;
                frames--;
                if (frames <= 0)
                {
                    Active = false;
                    if (Complete != null)
                        Complete(this, EventArgs.Empty);
                }
            }
        }

        public void Remove()
        {
            bool removed = false;
            for (int i = 0; i < Owner.Modifiers.Length; i++)
            {
                if (Owner.Modifiers[i] == this)
                {
                    Owner.Modifiers[i] = null;
                    removed = true;
                }
                if (removed && i != Owner.Modifiers.Length - 2)
                    Owner.Modifiers[i] = Owner.Modifiers[i + 1];
            }
            Owner.Modifiers[Owner.Modifiers.Length - 1] = null;
        }

        public IModifier3D DeepCopy(IModifiable3D newOwner)
        {
            RotateToModifier3D clone = new RotateToModifier3D();
            clone.frames = frames;
            clone.lerpSpeed = lerpSpeed;
            if (newOwner != null)
                clone.Owner = newOwner;
            else
                clone.Owner = Owner;
            clone.ID = ID;
            clone.Paused = Paused;
            clone.Active = Active;
            return clone;
        }
    }
    
    /// <summary>
    /// Rotates the owner continuously at a certain rate.
    /// </summary>
    public class RotateModifier3D : IModifier3D
    {
        public IModifiable3D Owner { get; set; }

        public Quaternion rotationalMomentum;
        /// <summary>
        /// A string representing this modifier.
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Time, in frames, the owner will rotate for.
        /// </summary>
        public int frames;

        public bool RemoveIfComplete { get; set; }
        bool paused = false;
        /// <summary>
        /// Set to true to temporarily stop this modifier.
        /// </summary>
        public bool Paused
        {
            get { return paused; }
            set
            {
                paused = value;
                if (Pause != null)
                    Pause(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// If set to false, this modifier has completed its task.
        /// </summary>
        public bool Active { get; set; }

        public event EventHandler Complete;
        public event EventHandler Pause;

        /// <summary>
        /// Creates a new Rotate Modifier.
        /// </summary>
        /// <param name="rotationSpeed">Quaternion by which the owner will rotate each frame.</param>
        /// <param name="time">Time, in frames, it will rotate for.  Set to -1 to rotate forever.</param>
        public RotateModifier3D(Quaternion rotationSpeed, bool removeIfComplete, int time)
        {
            rotationalMomentum = rotationSpeed;
            frames = time;
            RemoveIfComplete = removeIfComplete;
            Active = true;
        }

        /// <summary>
        /// Reinitializes this Rotate Modifier.
        /// </summary>
        /// <param name="rotationSpeed">Quaternion by which the owner will rotate each frame.</param>
        /// <param name="time">Time, in frames, it will rotate for.  Set to -1 to rotate forever.</param>
        public void Reset(Quaternion rotationSpeed, bool removeIfComplete, int time)
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
                    Owner.Rotation = rotationalMomentum * Owner.Rotation;
                else
                {
                    Active = false;
                    if (Complete != null)
                        Complete(this, EventArgs.Empty);
                }
            }
        }

        public void Remove()
        {
            bool removed = false;
            for (int i = 0; i < Owner.Modifiers.Length; i++)
            {
                if (Owner.Modifiers[i] == this)
                {
                    Owner.Modifiers[i] = null;
                    removed = true;
                }
                if (removed && i != Owner.Modifiers.Length-2)
                    Owner.Modifiers[i] = Owner.Modifiers[i + 1];
            }
            Owner.Modifiers[Owner.Modifiers.Length - 1] = null;
        }

        public IModifier3D DeepCopy(IModifiable3D newOwner)
        {
            RotateModifier3D clone = new RotateModifier3D(rotationalMomentum, RemoveIfComplete, frames);
            if (newOwner != null)
                clone.Owner = newOwner;
            else
                clone.Owner = Owner;
            clone.ID = ID;
            clone.Paused = Paused;
            clone.Active = Active;
            return clone;
        }
    }

    /// <summary>
    /// Rotates the owner to face the camera at all times.
    /// </summary>
    public class BillboardModifier3D : IModifier3D
    {
        public IModifiable3D Owner { get; set; }

        /// <summary>
        /// A string representing this modifier.
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Remaining time, in frames, the owner will face the camera for.
        /// </summary>
        public int frames;
        /// <summary>
        /// If true, this modifier will override all previously applied rotation modifiers.
        /// </summary>
        public bool Strict;

        public bool RemoveIfComplete { get; set; }
        bool paused = false;
        /// <summary>
        /// Set to true to temporarily stop this modifier.
        /// </summary>
        public bool Paused
        {
            get { return paused; }
            set
            {
                paused = value;
                if (Pause != null)
                    Pause(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// If set to false, this modifier has completed its task.
        /// </summary>
        public bool Active { get; set; }
        Quaternion prevRotation;

        public event EventHandler Complete;
        public event EventHandler Pause;

        /// <summary>
        /// Creates a new Billboard Modifier.
        /// </summary>
        /// <param name="strict">If true, this modifier will override all previously applied rotation modifiers.</param>
        /// <param name="removeIfComplete">If true, this modifier will be removed from the owner once Active is false.</param>
        /// <param name="time">Time, in frames, it will face the camera for.  Set to -1 to rotate forever.</param>
        public BillboardModifier3D(bool strict, bool removeIfComplete, int time)
        {
            Strict = strict;
            frames = time;
            RemoveIfComplete = removeIfComplete;
            Active = true;
            prevRotation = Quaternion.Identity;
        }

        public void Update()
        {
            if (!Paused && Active)
            {
                Vector3 cameraDirection = ScreenManager.Globals.Camera.CameraPosition - Owner.WorldPosition;
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
                Quaternion newRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, angleToRight) * Quaternion.CreateFromAxisAngle(Vector3.Right, angleToUp);
                if (Strict)
                    Owner.Rotation = newRotation;
                else
                    Owner.Rotation = Owner.Rotation * new Quaternion(prevRotation.X, prevRotation.Y, prevRotation.Z, -prevRotation.W) * newRotation;
                prevRotation = newRotation;
                if (frames > -1)
                    frames--;
                if (frames == 0)
                {
                    Active = false;
                    if (Complete != null)
                        Complete(this, EventArgs.Empty);
                }
            }
        }

        public void Remove()
        {
            bool removed = false;
            for (int i = 0; i < Owner.Modifiers.Length; i++)
            {
                if (Owner.Modifiers[i] == this)
                {
                    Owner.Modifiers[i] = null;
                    removed = true;
                }
                if (removed && i != Owner.Modifiers.Length - 2)
                    Owner.Modifiers[i] = Owner.Modifiers[i + 1];
            }
            Owner.Modifiers[Owner.Modifiers.Length - 1] = null;
        }

        public IModifier3D DeepCopy(IModifiable3D newOwner)
        {
            BillboardModifier3D clone = new BillboardModifier3D(Strict, RemoveIfComplete, frames);
            clone.prevRotation = prevRotation;
            if (newOwner != null)
                clone.Owner = newOwner;
            else
                clone.Owner = Owner;
            clone.ID = ID;
            clone.Paused = Paused;
            clone.Active = Active;
            return clone;
        }
    }

    /// <summary>
    /// Rotates the owner to face the camera at all times.
    /// </summary>
    public class BillboardRotateModifier3D : IModifier3D
    {
        public IModifiable3D Owner { get; set; }
        
        public float rotationalMomentum;
        float currentAngle;
        /// <summary>
        /// A string representing this modifier.
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Remaining time, in frames, the owner will face the camera for.
        /// </summary>
        public int frames;
        /// <summary>
        /// If true, this modifier will override all previously applied rotation modifiers.
        /// </summary>
        public bool Strict;

        public bool RemoveIfComplete { get; set; }
        bool paused = false;
        /// <summary>
        /// Set to true to temporarily stop this modifier.
        /// </summary>
        public bool Paused
        {
            get { return paused; }
            set
            {
                paused = value;
                if (Pause != null)
                    Pause(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// If set to false, this modifier has completed its task.
        /// </summary>
        public bool Active { get; set; }
        Quaternion prevRotation;

        public event EventHandler Complete;
        public event EventHandler Pause;

        /// <summary>
        /// Creates a new Billboard Modifier.
        /// </summary>
        /// <param name="strict">If true, this modifier will override all previously applied rotation modifiers.</param>
        /// <param name="removeIfComplete">If true, this modifier will be removed from the owner once Active is false.</param>
        /// <param name="time">Time, in frames, it will face the camera for.  Set to -1 to rotate forever.</param>
        public BillboardRotateModifier3D(float rotation, bool strict, bool removeIfComplete, int time)
        {
            rotationalMomentum = rotation;
            frames = time;
            RemoveIfComplete = removeIfComplete;
            Active = true;
            Strict = strict;
            currentAngle = 0;
            prevRotation = Quaternion.Identity;
        }

        public void Update()
        {
            if (!Paused && Active)
            {
                Vector3 cameraDirection = ScreenManager.Globals.Camera.CameraPosition - Owner.WorldPosition;
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
                currentAngle += rotationalMomentum;
                if (currentAngle > MathHelper.TwoPi)
                    currentAngle -= MathHelper.TwoPi;
                else if (currentAngle < 0)
                    currentAngle += MathHelper.TwoPi;
                Quaternion newRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, angleToRight) * Quaternion.CreateFromAxisAngle(Vector3.Right, angleToUp);//
                Quaternion secondaryRotation = Quaternion.CreateFromAxisAngle(cameraDirection, currentAngle);
                if (Strict)
                    Owner.Rotation = secondaryRotation * newRotation;//
                else
                    Owner.Rotation = Owner.Rotation * new Quaternion(prevRotation.X, prevRotation.Y, prevRotation.Z, -prevRotation.W) * (secondaryRotation * newRotation);
                prevRotation = newRotation;
                if (frames > -1)
                    frames--;
                if (frames == 0)
                {
                    Active = false;
                    if (Complete != null)
                        Complete(this, EventArgs.Empty);
                }
            }
        }

        public void Remove()
        {
            bool removed = false;
            for (int i = 0; i < Owner.Modifiers.Length; i++)
            {
                if (Owner.Modifiers[i] == this)
                {
                    Owner.Modifiers[i] = null;
                    removed = true;
                }
                if (removed && i != Owner.Modifiers.Length - 2)
                    Owner.Modifiers[i] = Owner.Modifiers[i + 1];
            }
            Owner.Modifiers[Owner.Modifiers.Length - 1] = null;
        }

        public IModifier3D DeepCopy(IModifiable3D newOwner)
        {
            BillboardRotateModifier3D clone = new BillboardRotateModifier3D(rotationalMomentum, Strict, RemoveIfComplete, frames);
            clone.prevRotation = prevRotation;
            clone.currentAngle = currentAngle;
            if (newOwner != null)
                clone.Owner = newOwner;
            else
                clone.Owner = Owner;
            clone.ID = ID;
            clone.Paused = Paused;
            clone.Active = Active;
            return clone;
        }
    }
}
