using Microsoft.Xna.Framework;
using System;

namespace CommonCode.Modifiers
{
    /// <summary>
    /// Moves the owner to the target location over a certain period of time.
    /// </summary>
    public class MoveToModifier2D : IModifier2D
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
        /// Creates a new MoveTo Modifier.
        /// </summary>
        /// <param name="targetPosition">Position the object will move to.</param>
        /// <param name="owner">The object the modifier will be applied to.</param>
        /// <param name="removeIfComplete">Set to true to delete this modifier when Active is false.</param>
        /// <param name="time">Time, in frames, it will take to move.  Set to 1 for immediate movement.</param>
        public MoveToModifier2D(Vector2 targetPosition, IModifiable2D owner, bool removeIfComplete, int time)
        {
            if (time < 1)
                throw new ArgumentException("This modifier takes at least 1 frame to execute.", "time");
            frames = time;
            Active = true;
            RemoveIfComplete = removeIfComplete;
            if(targetPosition.X != -1)
                lerpSpeed.X = (owner.WorldPosition.X - targetPosition.X) / time;
            if (targetPosition.Y != -1)
                lerpSpeed.Y = (owner.WorldPosition.Y - targetPosition.Y) / time;
        }

        public void Update()
        {
            if (!Paused && Active)
            {
                owner.WorldPosition -= lerpSpeed;
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
                if (removed && i < owner.Modifiers.Length - 1)
                    owner.Modifiers[i] = owner.Modifiers[i + 1];
            }
            owner.Modifiers[owner.Modifiers.Length - 1] = null;
        }

        public MoveToModifier2D ShallowCopy()
        {
            throw new NotImplementedException();
        }

        public MoveToModifier2D DeepCopy()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Uses MathHelper.SmoothStep to move the owner to the target location over time.
    /// </summary>
    public class SmoothStepModifier2D : IModifier2D
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

        Vector2 startPoint;
        Vector2 endPoint;
        int frames;
        float framesSpent;

        /// <summary>
        /// Creates a new SmoothStep Modifier.
        /// </summary>
        /// <param name="targetPosition">Position the object will move to.</param>
        /// <param name="owner">The object the modifier will be applied to.</param>
        /// <param name="removeIfComplete">Set to true to delete this modifier when Active is false.</param>
        /// <param name="time">Time, in frames, it will take to move.  Set to 1 for immediate movement.</param>
        public SmoothStepModifier2D(Vector2 targetPosition, IModifiable2D owner, bool removeIfComplete, int time)
        {
            if (time < 1)
                throw new ArgumentException("This modifier takes at least 1 frame to execute.", "time");
            frames = time;
            Active = true;
            startPoint = owner.WorldPosition;
            RemoveIfComplete = removeIfComplete;
            endPoint = startPoint;
            if (targetPosition.X != -1)
                endPoint.X = targetPosition.X;
            if (targetPosition.Y != -1)
                endPoint.Y = targetPosition.Y;
        }

        public void Update()
        {
            if (!Paused && Active)
            {
                Vector2 temp = owner.WorldPosition;
                if (startPoint.X != endPoint.X)
                    temp.X = MathHelper.SmoothStep(startPoint.X, endPoint.X, framesSpent / frames);
                if (startPoint.Y != endPoint.Y)
                    temp.Y = MathHelper.SmoothStep(startPoint.Y, endPoint.Y, framesSpent / frames);
                owner.WorldPosition = temp;
                framesSpent++;
                if (frames == framesSpent)
                {
                    owner.WorldPosition = endPoint;
                    Active = false;
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

        public SmoothStepModifier2D ShallowCopy()
        {
            throw new NotImplementedException();
        }

        public SmoothStepModifier2D DeepCopy()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Follows a certain point in 2D space.
    /// </summary>
    public class FollowModifier2D : IModifier2D
    { 
        public IModifiable2D owner { get; set; }

        /// <summary>
        /// Remaining time before this modifier completes its task.
        /// </summary>
        int frames;

        /// <summary>
        /// A string representing this modifier.
        /// </summary>
        public string ID { get; set; }
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
        /// Object the owner will be following.
        /// </summary>
        IModifiable2D target;

        bool followX = false;
        bool followY = false;

        /// <summary>
        /// Causes the owner to follow the target for a set period of time.
        /// </summary>
        /// <param name="targetPosition">Point to follow.</param>
        /// <param name="time">How long to follow it for.  Set to -1 for forever.</param>
        public FollowModifier2D(IModifiable2D target, int time)
        {
            this.target = target;
            frames = time;
            Active = true;
            followX = true;
            followY = true;
        }

        /// <summary>
        /// Causes the owner to follow the target for a set period of time.
        /// </summary>
        /// <param name="target">Other object to follow.</param>
        /// <param name="followAxes">Axes to follow the target on. Set to -1 for false, and anything else for true.</param>
        /// <param name="time">Time, in frames, to follow the other object for.  Set to -1 to follow forever.</param>
        public FollowModifier2D(IModifiable2D target, Vector2 followAxes, int time)
        {
            this.target = target;
            frames = time;
            Active = true;
            if (followAxes.X != -1)
                followX = true;
            if (followAxes.Y != -1)
                followY = true;
        }

        public void Update()
        {
            if (!Paused && Active)
            {
                if (frames > 0)
                    frames--;
                if (frames == 0)
                    Active = false;
                Vector2 newPosition = Vector2.Zero;
                if(followX)
                    newPosition.X = target.WorldPosition.X;
                else
                    newPosition.X = owner.WorldPosition.X;
                if (followY)
                    newPosition.Y = target.WorldPosition.Y;
                else
                    newPosition.Y = owner.WorldPosition.Y;

                owner.WorldPosition = newPosition;
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

        public void ShallowCopy()
        {
            throw new NotImplementedException();
        }
    }
}
