using Microsoft.Xna.Framework;
using System;

namespace CommonCode.Modifiers
{
    /// <summary>
    /// Scales the owner to a specific size over time.
    /// </summary>
    public class ScaleToModifier3D : IModifier3D
    {
        public IModifiable3D Owner { get; set; }

        /// <summary>
        /// A string representing this modifier.
        /// </summary>
        public string ID { get; set; }
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

        Vector3 lerpSpeed = Vector3.Zero;
        int frames;

        /// <summary>
        /// Creates a new ScaleTo Modifier.
        /// </summary>
        /// <param name="targetRotation">Size that the owner will scale to.</param>
        /// <param name="owner">The object the modifier will be applied to.</param>
        ///  <param name="removeIfComplete">Set to true to delete this modifier when Active is false.</param>
        /// <param name="time">Time, in frames, it will take to scale.  Set to 1 for immediate rescaling.</param>
        public ScaleToModifier3D(Vector3 targetScale, IModifiable3D owner, bool removeIfComplete, int time)
        {
            if (time < 1)
                throw new ArgumentException("This modifier takes at least 1 frame to execute.", "time");
            frames = time;
            Active = true;
            RemoveIfComplete = removeIfComplete;

            if (targetScale.X != -1)
                lerpSpeed.X = (owner.Scale.X - targetScale.X) / time;
            if (targetScale.Y != -1)
                lerpSpeed.Y = (owner.Scale.Y - targetScale.Y) / time;
            if (targetScale.Z != -1)
                lerpSpeed.Z = (owner.Scale.Z - targetScale.Z) / time;
        }

        private ScaleToModifier3D() { }

        public void Update()
        {
            if (!Paused && Active)
            {
                Owner.Scale -= lerpSpeed;
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
            ScaleToModifier3D clone = new ScaleToModifier3D();
            clone.lerpSpeed = lerpSpeed;
            clone.frames = frames;
            if (newOwner != null)
                clone.Owner = newOwner;
            else
                clone.Owner = Owner;
            clone.RemoveIfComplete = RemoveIfComplete;
            clone.ID = ID;
            clone.Active = Active;
            clone.Paused = Paused;
            return clone;
        }
    }
}
