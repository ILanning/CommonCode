using Microsoft.Xna.Framework;
using System;

namespace CommonCode.Modifiers
{
    /// <summary>
    /// Interpolates the owner between two colors.
    /// </summary>
    public class ColorModifier2D : IModifier2D
    {
        public IModifiable2D owner { get; set; }

        private Color prevColor;
        private Color targetColor;
        private int frames;
        private int framesSpent = 0;
        public bool RemoveIfComplete { get; set; }

        /// <summary>
        /// A string representing this modifier.
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Set to true to temporarily stop this modifier.
        /// </summary>
        public bool Paused { get; set; }
        /// <summary>
        /// If set to false, this modifier has completed its task.
        /// </summary>
        public bool Active { get; private set; }

        public ColorModifier2D(Color color, bool removeIfComplete, IModifiable2D owner, int time)
        {
            prevColor = owner.Color;
            targetColor = color;
            frames = time;
            Active = true;
            RemoveIfComplete = removeIfComplete;
        }

        public void Reset(Color color, bool removeIfComplete, int time)
        {
            prevColor = owner.Color;
            targetColor = color;
            frames = time;
            framesSpent = 0;
            Active = true;
            RemoveIfComplete = removeIfComplete;
        }
            //if ((prevColor.R + prevColor.G + prevColor.B) - (targetColor.R + targetColor.G + targetColor.B) > 0)
            //    colorWasSmaller = false;
            //else
            //    colorWasSmaller = true;

        public void Update()
        {
            if (!Paused && Active)
            {
                if (framesSpent != frames)
                {
                    owner.Color = Color.Lerp(prevColor, targetColor, (float)framesSpent / (float)frames);
                    framesSpent++;
                }
                else
                {
                    Active = false;
                    owner.Color = targetColor;
                }
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
                if (removed && i < owner.Modifiers.Length - 1)
                    owner.Modifiers[i] = owner.Modifiers[i + 1];
            }
            owner.Modifiers[owner.Modifiers.Length - 1] = null;
        }

        public ColorModifier2D ShallowCopy()
        {
            throw new NotImplementedException();
        }

        public ColorModifier2D DeepCopy()
        {
            throw new NotImplementedException();
        }
    }
}
