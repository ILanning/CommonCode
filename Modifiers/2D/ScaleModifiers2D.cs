using Microsoft.Xna.Framework;
using System;

namespace CommonCode.Modifiers
{
    /// <summary>
    /// Scales the owner to a specific size over time.
    /// </summary>
    public class ScaleToModifier2D : IModifier2D
    {
        public IModifiable2D owner { get; set; }

        /// <summary>
        /// A string representing this modifier.
        /// </summary>
        public string ID { get; set; }
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
        int frames;

        /// <summary>
        /// Creates a new ScaleTo Modifier.
        /// </summary>
        /// <param name="targetScale">Size that the owner will scale to.</param>
        /// <param name="owner">The object the modifier will be applied to.</param>
        ///  <param name="removeIfComplete">Set to true to delete this modifier when Active is false.</param>
        /// <param name="time">Time, in frames, it will take to scale.  Set to 1 for immediate rescaling.</param>
        public ScaleToModifier2D(Vector2 targetScale, IModifiable2D owner, bool removeIfComplete, int time)
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
        }

        public void Update()
        {
            if (!Paused && Active)
            {
                owner.Scale -= lerpSpeed;
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

        public ScaleToModifier2D ShallowCopy()
        {
            throw new NotImplementedException();
        }
    }
}
