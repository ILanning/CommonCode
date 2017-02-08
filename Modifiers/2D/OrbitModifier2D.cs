using Microsoft.Xna.Framework;
using System;

namespace CommonCode.Modifiers
{
    /// <summary>
    /// Causes the owner to orbit around a certain point in 2D space.
    /// </summary>
    public class OrbitModifier2D : IModifier2D
    {
        public IModifiable2D owner { get; set; }

        /// <summary>
        /// A string representing this modifier.
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// The point the owner is orbitting about.
        /// </summary>
        public Vector2 orbitPivot;
        /// <summary>
        /// Current angle from 'zero'.  Not usually very meaningful.
        /// </summary>
        private float orbitAngle;
        /// <summary>
        /// Speed, in radians, at which this object orbits around the orbitPivot.
        /// </summary>
        private float orbitSpeed;
        /// <summary>
        /// Distance from owner to orbitPivot.
        /// </summary>
        private float orbitDist;
        /// <summary>
        /// Remaining time before this modifier completes its task.
        /// </summary>
        private int frames;
        public bool RemoveIfComplete { get; set; }

        /// <summary>
        /// Set to true to temporarily stop this modifier.
        /// </summary>
        public bool Paused { get; set; }
        /// <summary>
        /// If set to false, this modifier has completed its task.
        /// </summary>
        public bool Active { get; private set; }

        public OrbitModifier2D(Vector2 pivot, float distance, float initialAngle, float speed, bool removeIfComplete, int time)
        {
            orbitPivot = pivot;
            orbitSpeed = speed;
            orbitDist = distance;
            orbitAngle = initialAngle;
            Active = true;
            RemoveIfComplete = removeIfComplete;
            frames = time;
        }

        public void Update()
        {
            if (!Paused && Active)
            {
                if (frames != 0)
                {
                    orbitAngle += orbitSpeed;
                    Vector2 orbitLocation = new Vector2((float)(orbitDist * Math.Cos(orbitAngle)),
                                                        (float)(orbitDist * Math.Sin(orbitAngle)));

                    owner.WorldPosition = orbitLocation + orbitPivot;

                    if (orbitAngle > MathHelper.TwoPi)
                        orbitAngle -= MathHelper.TwoPi;
                    else if (orbitAngle < 0)
                        orbitAngle += MathHelper.TwoPi;
                    if (frames > -1)
                        frames--;
                }
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
                if (removed && i < owner.Modifiers.Length - 1)
                    owner.Modifiers[i] = owner.Modifiers[i + 1];
            }
            owner.Modifiers[owner.Modifiers.Length - 1] = null;
        }

        public void ShallowCopy()
        {
            
        }

        public OrbitModifier2D DeepCopy()
        {
            OrbitModifier2D clone = new OrbitModifier2D(orbitPivot, orbitDist, orbitAngle, orbitSpeed, RemoveIfComplete, frames);
            return clone;
        }

        public OrbitModifier2D DeepCopy(IModifiable2D newOwner)
        {
            OrbitModifier2D clone = new OrbitModifier2D(orbitPivot, orbitDist, orbitAngle, orbitSpeed, RemoveIfComplete, frames);
            clone.owner = newOwner;
            return clone;
        }
    }
}
