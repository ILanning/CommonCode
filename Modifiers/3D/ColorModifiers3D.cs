using Microsoft.Xna.Framework;
using System;

namespace CommonCode.Modifiers
{
    /// <summary>
    /// Interpolates the owner between two colors.
    /// </summary>
    public class ColorModifier3D : IModifier3D
    {
        public IModifiable3D Owner { get; set; }
        private Color prevColor;
        private Color targetColor;
        private int frames;
        private int framesSpent = 0;
        public bool RemoveIfComplete { get; set; }

        /// <summary>
        /// A string representing this modifier.
        /// </summary>
        public string ID { get; set; }
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
        public bool Active { get; private set; }

        public event EventHandler Complete;
        public event EventHandler Pause;

        public ColorModifier3D(Color color, bool removeIfComplete, IModifiable3D owner, int time)
        {
            prevColor = owner.Color;
            targetColor = color;
            frames = time;
            Active = true;
            RemoveIfComplete = removeIfComplete;
        }

        public void Reset(Color color, bool removeIfComplete, int time)
        {
            prevColor = Owner.Color;
            targetColor = color;
            frames = time;
            framesSpent = 0;
            Active = true;
            RemoveIfComplete = removeIfComplete;
        }

        public void Update()
        {
            if (!Paused && Active)
            {
                if (framesSpent != frames)
                {
                    Owner.Color = Color.Lerp(prevColor, targetColor, ((float)framesSpent) / frames);
                    framesSpent++;
                }
                else
                {
                    Active = false;
                    if (Complete != null)
                        Complete(this, EventArgs.Empty);
                    Owner.Color = targetColor;
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
                if (removed && i < Owner.Modifiers.Length - 1)
                    Owner.Modifiers[i] = Owner.Modifiers[i + 1];
            }
            Owner.Modifiers[Owner.Modifiers.Length - 1] = null;
        }

        public IModifier3D DeepCopy(IModifiable3D newOwner)
        {
            ColorModifier3D clone = new ColorModifier3D(targetColor, RemoveIfComplete, Owner, frames);
            if (newOwner != null)
                clone.Owner = newOwner;
            else
                clone.Owner = Owner;
            clone.prevColor = prevColor;
            clone.framesSpent = framesSpent;
            clone.Paused = Paused;
            return clone;
        }
    }
}
