using Microsoft.Xna.Framework;
using System;

namespace CommonCode.Modifiers
{
    /// <summary>
    /// Moves the owner to the target location over a certain period of time.
    /// </summary>
    public class MoveToModifier3D : IModifier3D
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

        IModifiable3D target;
        Vector3 targetPosition = Vector3.Zero;
        int framesLeft;

        /// <summary>
        /// Creates a new MoveTo Modifier.
        /// </summary>
        /// <param name="targetPosition">Position the object will move to.</param>
        /// <param name="owner">The object the modifier will be applied to.</param>
        /// <param name="removeIfComplete">Set to true to delete this modifier when Active is false.</param>
        /// <param name="time">Time, in frames, it will take to move.  Set to 1 for immediate movement.</param>
        public MoveToModifier3D(Vector3 targetPosition, IModifiable3D owner, bool removeIfComplete, int time)
        {
            if (time < 1)
                throw new ArgumentException("This modifier takes at least 1 frame to execute.", "time");
            framesLeft = time;
            Active = true;
            RemoveIfComplete = removeIfComplete;
            this.targetPosition = targetPosition;
        }

        /// <summary>
        /// Creates a new MoveTo Modifier.
        /// </summary>
        /// <param name="target">Object the owner will move to.</param>
        /// <param name="owner">The object the modifier will be applied to.</param>
        /// <param name="removeIfComplete">Set to true to delete this modifier when Active is false.</param>
        /// <param name="time">Time, in frames, it will take to move.  Set to 1 for immediate movement.</param>
        public MoveToModifier3D(IModifiable3D target, IModifiable3D owner, bool removeIfComplete, int time)
        {
            if (time < 1)
                throw new ArgumentException("This modifier takes at least 1 frame to execute.", "time");
            framesLeft = time;
            Active = true;
            RemoveIfComplete = removeIfComplete;
            this.target = target;
        }

        public void Update()
        {
            if (!Paused && Active)
            {
                if (target != null)
                    targetPosition = target.WorldPosition;
                Vector3 lerpSpeed = Vector3.Zero;
                if (targetPosition.X != -1)
                    lerpSpeed.X = (Owner.WorldPosition.X - targetPosition.X) / framesLeft;
                if (targetPosition.Y != -1)
                    lerpSpeed.Y = (Owner.WorldPosition.Y - targetPosition.Y) / framesLeft;
                if (targetPosition.Z != -1)
                    lerpSpeed.Z = (Owner.WorldPosition.Z - targetPosition.Z) / framesLeft;

                Owner.WorldPosition -= lerpSpeed;
                framesLeft--;
                if (framesLeft == 0)
                {
                    Vector3 tempVector = Vector3.Zero;
                    if (targetPosition.X != -1)
                        tempVector.X = targetPosition.X;
                    else
                        tempVector.X = Owner.WorldPosition.X;
                    if (targetPosition.Y != -1)
                        tempVector.Y = targetPosition.Y;
                    else
                        tempVector.Y = Owner.WorldPosition.Y;
                    if (targetPosition.Z != -1)
                        tempVector.Z = targetPosition.Z;
                    else
                        tempVector.Z = Owner.WorldPosition.Z;
                    Owner.WorldPosition = tempVector;

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
                if (removed && i < Owner.Modifiers.Length - 1)
                    Owner.Modifiers[i] = Owner.Modifiers[i + 1];
            }
            Owner.Modifiers[Owner.Modifiers.Length - 1] = null;
        }

        public IModifier3D DeepCopy(IModifiable3D newOwner)
        {
            MoveToModifier3D clone = null;
            clone = new MoveToModifier3D(targetPosition, Owner, RemoveIfComplete, 2);
            clone.framesLeft = framesLeft;
            if (target != null)
                clone.target = target;
            clone.targetPosition = targetPosition;
            if (newOwner != null)
                clone.Owner = newOwner;
            else
                clone.Owner = Owner;
            clone.Paused = Paused;
            clone.Active = Active;
            return clone;
        }
    }

    /// <summary>
    /// Uses MathHelper.SmoothStep to move the owner to the target location over time.
    /// </summary>
    public class SmoothStepModifier3D : IModifier3D
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

        Vector3 startPoint;
        Vector3 endPoint;
        int frames;
        float framesSpent;

        /// <summary>
        /// Creates a new SmoothStep Modifier.
        /// </summary>
        /// <param name="targetPosition">Position the object will move to.</param>
        /// <param name="owner">The object the modifier will be applied to.</param>
        /// <param name="removeIfComplete">Set to true to delete this modifier when Active is false.</param>
        /// <param name="time">Time, in frames, it will take to move.  Set to 1 for immediate movement.</param>
        public SmoothStepModifier3D(Vector3 targetPosition, IModifiable3D owner, bool removeIfComplete, int time)
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
            if (targetPosition.Z != -1)
                endPoint.Z = targetPosition.Z;
        }

        public void Update()
        {
            if (!Paused && Active)
            {
                Vector3 temp = Owner.WorldPosition;
                if (startPoint.X != endPoint.X)
                    temp.X = MathHelper.SmoothStep(startPoint.X, endPoint.X, framesSpent / frames);
                if (startPoint.Y != endPoint.Y)
                    temp.Y = MathHelper.SmoothStep(startPoint.Y, endPoint.Y, framesSpent / frames);
                if (startPoint.Z != endPoint.Z)
                    temp.Z = MathHelper.SmoothStep(startPoint.Z, endPoint.Z, framesSpent / frames);
                Owner.WorldPosition = temp;
                framesSpent++;
                if (frames == framesSpent)
                {
                    Owner.WorldPosition = endPoint;
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
                if (removed && i < Owner.Modifiers.Length - 1)
                    Owner.Modifiers[i] = Owner.Modifiers[i + 1];
            }
            Owner.Modifiers[Owner.Modifiers.Length - 1] = null;
        }

        public IModifier3D DeepCopy(IModifiable3D newOwner)
        {
            SmoothStepModifier3D clone = null;
            clone = new SmoothStepModifier3D(endPoint, Owner, RemoveIfComplete, 2);
            clone.framesSpent = framesSpent;
            clone.frames = frames;
            clone.startPoint = startPoint;
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
    /// Follows a certain point in 3D space.
    /// </summary>
    public class FollowModifier3D : IModifier3D
    { 
        public IModifiable3D Owner { get; set; }

        int frames;

        /// <summary>
        /// A string representing this modifier.
        /// </summary>
        public string ID { get; set; }
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

        IModifiable3D target;

        bool followX = false;
        bool followY = false;
        bool followZ = false;

        /// <summary>
        /// If true, this modifier will override all that update before it.
        /// </summary>
        bool strictFollow = false;
        Vector3 targetLastPosition;
        /// <summary>
        /// Creates a new FollowModifier3D.
        /// </summary>
        /// <param name="target">Object to follow.</param>
        /// <param name="strict">If true, this modifier will override all others.</param>
        /// <param name="removeIfComplete">Set to true to delete this modifier when Active is false.</param>
        /// <param name="time">How long to follow it for.</param>
        public FollowModifier3D(IModifiable3D target, bool strict, bool removeIfComplete, int time)
        {
            this.target = target;
            targetLastPosition = target.WorldPosition;
            followX = true;
            followY = true;
            followZ = true;
            strictFollow = strict;
            RemoveIfComplete = removeIfComplete;
            frames = time;
            Active = true;
        }

        /// <summary>
        /// Creates a new FollowModifier3D.
        /// </summary>
        /// <param name="target">Object to follow.</param>
        /// <param name="followAxes">Sets which axes to follow the target on.  Set a value to -1 to ignore that axis.</param>
        /// <param name="strict">If true, this modifier will override all others.</param>
        /// <param name="removeIfComplete">Set to true to delete this modifier when Active is false.</param>
        /// <param name="time">How long to follow it for.</param>
        public FollowModifier3D(IModifiable3D target, Vector3 followAxes, bool strict, bool removeIfComplete, int time)
        {
            this.target = target;
            targetLastPosition = target.WorldPosition;
            if (followAxes.X != -1)
                followX = true;
            if (followAxes.Y != -1)
                followY = true;
            if (followAxes.Z != -1)
                followZ = true;
            strictFollow = strict;
            RemoveIfComplete = removeIfComplete;
            frames = time;
            Active = true;
        }

        public void Update()
        {
            if (!Paused && Active)
            {
                if (frames > 0)
                    frames--;
                if (frames == 0)
                {
                    Active = false;
                    if (Complete != null)
                        Complete(this, EventArgs.Empty);
                }

                Vector3 newPosition = Vector3.Zero;
                if (strictFollow)
                {
                    if (followX)
                        newPosition.X = target.WorldPosition.X - Owner.WorldPosition.X;
                    if (followY)
                        newPosition.Y = target.WorldPosition.Y - Owner.WorldPosition.Y;
                    if (followZ)
                        newPosition.Z = target.WorldPosition.Z - Owner.WorldPosition.Z;
                }
                else
                {
                    if (followX)
                        newPosition.X = target.WorldPosition.X - targetLastPosition.X;
                    if (followY)
                        newPosition.Y = target.WorldPosition.Y - targetLastPosition.Y;
                    if (followZ)
                        newPosition.Z = target.WorldPosition.Z - targetLastPosition.Z;
                }
                targetLastPosition = target.WorldPosition;
                Owner.WorldPosition += newPosition;
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
            FollowModifier3D clone = null;
            clone = new FollowModifier3D(target, strictFollow, RemoveIfComplete, 2);
            clone.frames = frames;
            clone.followX = followX;
            clone.followY = followY;
            clone.followZ = followZ;
            clone.targetLastPosition = targetLastPosition;
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
