using System;

namespace CommonCode.Modifiers
{
    /// <summary>
    /// Rotates the owner to a specific angle over time.
    /// </summary>
    public class RotateToModifier2D : IModifier2D
    {
        public IModifiable2D owner { get; set; }

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
        /// <summary>
        /// Set to true to temporarily stop this modifier.
        /// </summary>
        public bool Paused { get; set; }
        /// <summary>
        /// If set to false, this modifier has completed its task.
        /// </summary>
        public bool Active { get; set; }

        float lerpSpeed = 0f;

        /// <summary>
        /// Creates a new RotateTo Modifier.
        /// </summary>
        /// <param name="targetRotation">Rotation, in radians, that the owner will rotate to.</param>
        /// <param name="owner">The object the modifier will be applied to.</param>
        ///  <param name="removeIfComplete">Set to true to delete this modifier when Active is false.</param>
        /// <param name="time">Time, in frames, it will take to rotate.  Set to 1 for immediate rotation.</param>
        public RotateToModifier2D(float targetRotation, IModifiable2D owner, bool removeIfComplete, int time)
        {
            if (time < 1)
                throw new ArgumentException("This modifier takes at least 1 frame to execute.", "time");
            frames = time;
            this.owner = owner;
            Active = true;
            if (targetRotation != -1)
                lerpSpeed = (owner.Rotation - targetRotation) / time;
        }

        public void Reset(float targetRotation, bool removeIfComplete, int time)
        {
            if (time < 1)
                throw new ArgumentException("This modifier takes at least 1 frame to execute.", "time");
            frames = time;
            Active = true;
            if (targetRotation != -1)
                lerpSpeed = (owner.Rotation - targetRotation) / time;
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

        public RotateToModifier2D ShallowCopy()
        {
            throw new NotImplementedException();
        }

        public RotateToModifier2D DeepCopy()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Rotates the owner continuously at a certain rate.
    /// </summary>
    public class RotateModifier2D : IModifier2D
    {
        public IModifiable2D owner { get; set; }

        public float rotationalMomentum;
        /// <summary>
        /// A string representing this modifier.
        /// </summary>
        public string ID { get; set; }
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
        public RotateModifier2D(float rotationSpeed, bool removeIfComplete, int time)
        {
            rotationalMomentum = rotationSpeed;
            frames = time;
            RemoveIfComplete = removeIfComplete;
            Active = true;
        }

        public void Reset(float rotationSpeed, bool removeIfComplete, int time)
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
                if (removed && i != owner.Modifiers.Length-2)
                    owner.Modifiers[i] = owner.Modifiers[i + 1];
            }
            owner.Modifiers[owner.Modifiers.Length - 1] = null;
        }

        public RotateModifier2D ShallowCopy()
        {
            RotateModifier2D temp = new RotateModifier2D(rotationalMomentum, RemoveIfComplete, frames);
            temp.owner = owner;
            return temp;
        }

        public RotateModifier2D DeepCopy()
        {
            return new RotateModifier2D(rotationalMomentum, RemoveIfComplete, frames);
        }
    }
}
