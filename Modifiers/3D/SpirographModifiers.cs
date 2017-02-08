using System;
using Microsoft.Xna.Framework;
using CommonCode.Drawing;

namespace CommonCode.Modifiers
{
    /// <summary>
    /// Changes one spirograph into another over time.
    /// </summary>
    public class SpirographLerpModifier : IModifier3D
    {
        
        protected Spirograph owner;
        public IModifiable3D Owner 
        {
            get { return owner; }
            set
            {
                if (value is Spirograph)
                    owner = (Spirograph)value;
                else
                    throw new ArgumentException("This modifier can only be applied to a Spirograph.");
            }
        }
        public string ID { get; set; }

        protected float targetSmallRadius;
        protected float targetLargeRadius;
        protected float targetRadians;
        public float targetDist;

        protected Vector4 lerpSpeed = Vector4.Zero;

        protected bool sRadiusWasSmaller;
        protected bool lRadiusWasSmaller;
        protected bool distWasSmaller;
        protected bool radiansWasSmaller;

        protected int frames = 0;

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

        private SpirographLerpModifier() { }

        /// <summary>
        /// This causes the owner spirograph to change to the passed-in values over [time] frames.
        /// </summary>
        /// <param name="smallerRadius">Value to interpolate the outer radius to. 0 or less to keep the old value.</param>
        /// <param name="largerRadius">Value to interpolate the inner radius to. 0 or less to keep the old value.</param>
        /// <param name="radians">Value to interpolate the radians to. 0 or less to keep the old value.</param>
        /// <param name="distanceToPoint">Value to interpolate the distance to point to. 0 or less to keep the old value.</param>
        /// <param name="time">Time, in frames, that this function will take to complete.  Must be greater than -1.</param>
        public SpirographLerpModifier(float smallerRadius, float largerRadius, float radians, float distanceToPoint, float scalar, bool removeIfComplete, Spirograph owner, int time)
        {
            if (time > 0)
            {
                RemoveIfComplete = removeIfComplete;
                Owner = owner;
                if (smallerRadius > 0)
                    targetSmallRadius = smallerRadius * scalar;
                else
                    targetSmallRadius = owner.smallerRadius;
                if (distanceToPoint > 0)
                    targetDist = distanceToPoint * scalar;
                else
                    targetDist = owner.distToPoint;
                if (largerRadius > 0)
                    targetLargeRadius = largerRadius * scalar;
                else
                    targetLargeRadius = owner.largerRadius;
                if (radians > 0)
                    targetRadians = radians;
                else
                    targetRadians = owner.radians;
                frames = time;

                lerpSpeed = new Vector4((owner.smallerRadius - targetSmallRadius) / (float)time,
                                            (owner.largerRadius - targetLargeRadius) / (float)time,
                                            (owner.radians - targetRadians) / (float)time,
                                            (owner.distToPoint - targetDist) / (float)time);
                if (lerpSpeed.X > 0)
                    sRadiusWasSmaller = false;
                else
                    sRadiusWasSmaller = true;
                if (lerpSpeed.Y > 0)
                    lRadiusWasSmaller = false;
                else
                    lRadiusWasSmaller = true;
                if (lerpSpeed.Z > 0)
                    radiansWasSmaller = false;
                else
                    radiansWasSmaller = true;
                if (lerpSpeed.W > 0)
                    distWasSmaller = false;
                else
                    distWasSmaller = true;

                Active = true;
            }
        }

        public void Reset(float smallerRadius, float largerRadius, float radians, float distanceToPoint, float scalar, int time)
        {
            if (time > 0)
            {
                if (smallerRadius > 0)
                    targetSmallRadius = smallerRadius * scalar;
                else
                    targetSmallRadius = owner.smallerRadius;
                if (distanceToPoint > 0)
                    targetDist = distanceToPoint * scalar;
                else
                    targetDist = owner.distToPoint;
                if (largerRadius > 0)
                    targetLargeRadius = largerRadius * scalar;
                else
                    targetLargeRadius = owner.largerRadius;
                if (radians > 0)
                    targetRadians = radians;
                else
                    targetRadians = owner.radians;
                frames = time;

                lerpSpeed = new Vector4((owner.smallerRadius - targetSmallRadius) / (float)time,
                                            (owner.largerRadius - targetLargeRadius) / (float)time,
                                            (owner.radians - targetRadians) / (float)time,
                                            (owner.distToPoint - targetDist) / (float)time);
                if (lerpSpeed.X > 0)
                    sRadiusWasSmaller = false;
                else
                    sRadiusWasSmaller = true;
                if (lerpSpeed.Y > 0)
                    lRadiusWasSmaller = false;
                else
                    lRadiusWasSmaller = true;
                if (lerpSpeed.Z > 0)
                    radiansWasSmaller = false;
                else
                    radiansWasSmaller = true;
                if (lerpSpeed.W > 0)
                    distWasSmaller = false;
                else
                    distWasSmaller = true;

                Active = true;
            }
        }

        public void Update()
        {
            if (!Paused && Active)
            {
                if (frames != 0)
                {
                    if (owner.smallerRadius != targetSmallRadius)
                        owner.smallerRadius -= lerpSpeed.X;
                    if (owner.largerRadius != targetLargeRadius)
                        owner.largerRadius -= lerpSpeed.Y;
                    if (owner.radians != targetRadians)
                        owner.radians -= lerpSpeed.Z;
                    if (owner.distToPoint != targetDist)
                        owner.distToPoint -= lerpSpeed.W;

                    owner.Generate(owner.lineVertices.Length);

                    if ((sRadiusWasSmaller && owner.smallerRadius > targetSmallRadius) ||
                       (!sRadiusWasSmaller && owner.smallerRadius < targetSmallRadius))
                        owner.smallerRadius = targetSmallRadius;

                    if ((distWasSmaller && owner.distToPoint > targetDist) ||
                       (!distWasSmaller && owner.distToPoint < targetDist))
                        owner.distToPoint = targetDist;

                    if ((lRadiusWasSmaller && owner.largerRadius > targetLargeRadius) ||
                       (!lRadiusWasSmaller && owner.largerRadius < targetLargeRadius))
                        owner.largerRadius = targetLargeRadius;

                    if ((radiansWasSmaller && owner.radians > targetRadians) ||
                       (!radiansWasSmaller && owner.radians < targetRadians))
                        owner.radians = targetRadians;

                    if (owner.smallerRadius == targetSmallRadius &&
                        owner.distToPoint == targetDist &&
                        owner.largerRadius == targetLargeRadius &&
                        owner.radians == targetRadians)
                    {
                        Active = false;
                        if (Complete != null)
                            Complete(this, EventArgs.Empty);
                    }

                    frames--;
                }
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

        public IModifier3D DeepCopy(IModifiable3D newOwner)
        {
            SpirographLerpModifier clone = null;
            clone = new SpirographLerpModifier();
            clone.distWasSmaller = distWasSmaller;
            clone.lRadiusWasSmaller = lRadiusWasSmaller;
            clone.sRadiusWasSmaller = sRadiusWasSmaller;
            clone.radiansWasSmaller = radiansWasSmaller;
            clone.targetDist = targetDist;
            clone.targetLargeRadius = targetLargeRadius;
            clone.targetRadians = targetRadians;
            clone.targetSmallRadius = targetSmallRadius;
            clone.lerpSpeed = lerpSpeed;
            clone.frames = frames;
            clone.ID = ID;
            if (newOwner != null)
                clone.Owner = newOwner;
            else
                clone.owner = owner;
            clone.Paused = Paused;
            clone.Active = Active;
            return clone;
        }
    }
}
