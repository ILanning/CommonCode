using Microsoft.Xna.Framework;
using System;

namespace CommonCode.Modifiers
{
    /// <summary>
    /// Causes the owner to orbit around a certain point in 3D space.  Something of a hack job at the moment.
    /// </summary>
    public class OrbitModifier3D : IModifier3D
    {
        public IModifiable3D Owner { get; set; }

        /// <summary>
        /// A string representing this modifier.
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// The vector perpendicular to the orbital plane.
        /// </summary>
        public Vector3 orbitPivot;
        /// <summary>
        /// The object the owner will orbit about.
        /// </summary>
        public IModifiable3D Target;
        /// <summary>
        /// The point the owner will orbit about.
        /// </summary>
        public Vector3 TargetPosition;
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
        /// If true, this modifier will override all other position modifiers.
        /// </summary>
        public bool StrictOrbit = false;
        private Vector3 currentOrbitLocation;
        /// <summary>
        /// The last calculated position the owner should have been at.
        /// </summary>
        private Vector3 lastOwnerPosition;
        /// <summary>
        /// Remaining time before this modifier completes its task.
        /// </summary>
        private int frames;
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
        public bool Active { get; private set; }

        public event EventHandler Complete;
        public event EventHandler Pause;

        /// <summary>
        /// Creates a new OrbitModifier3D.
        /// </summary>
        /// <param name="targetObject">The object the owner will orbit about.</param>
        /// <param name="startingPosition">The Vector3 the owner will start from.</param>
        /// <param name="speed">How fast, in radians, the owner will move along its path.</param>
        /// <param name="owner">The object the modifier will be applied to.</param>
        /// <param name="removeIfComplete">Set to true to delete this modifier when Active is false.</param>
        /// <param name="time">Time, in frames, it will move along the orbit.  Set to -1 to orbit forever.</param>
        public OrbitModifier3D(IModifiable3D targetObject, Vector3 startingPosition, float speed, IModifiable3D owner, bool removeIfComplete, int time)
        {
            Target = targetObject;
            TargetPosition = Target.WorldPosition;
            Vector3 startingVector = (startingPosition - TargetPosition);
            Vector3 startingRightNormal = Vector3.Cross(startingVector, Vector3.Up);
            orbitPivot = Vector3.Cross(startingRightNormal, startingVector);
            orbitSpeed = speed;
            orbitDist = startingVector.Length();
            this.Owner = owner;

            Vector3 startingOrbitLocation = new Vector3((float)(orbitDist * startingRightNormal.X), //orbitAngle = 0, cos(orbitAngle) = 1, sin(orbitAngle) = 0.
                                                        (float)(orbitDist * startingRightNormal.Y),
                                                        (float)(orbitDist * startingRightNormal.Z)) + TargetPosition;
            orbitAngle = (float)Math.Acos(Vector3.Dot(startingPosition, startingOrbitLocation));

            Vector3 orbitRightNormal;
            if (orbitPivot != Vector3.Up)
                orbitRightNormal = Vector3.Cross(orbitPivot, Vector3.Up);
            else
                orbitRightNormal = Vector3.Right;
            Vector3 orbitUpNormal = Vector3.Cross(orbitRightNormal, orbitPivot);

            currentOrbitLocation = new Vector3((float)(orbitDist * Math.Cos(orbitAngle) * orbitRightNormal.X + orbitDist * Math.Sin(orbitAngle) * orbitUpNormal.X),
                                               (float)(orbitDist * Math.Cos(orbitAngle) * orbitRightNormal.Y + orbitDist * Math.Sin(orbitAngle) * orbitUpNormal.Y),
                                               (float)(orbitDist * Math.Cos(orbitAngle) * orbitRightNormal.Z + orbitDist * Math.Sin(orbitAngle) * orbitUpNormal.Z)) + TargetPosition;
            owner.WorldPosition = currentOrbitLocation;
            lastOwnerPosition = currentOrbitLocation;
            Active = true;
            RemoveIfComplete = removeIfComplete;
            frames = time;
        }


        /// <summary>
        /// Creates a new OrbitModifier3D.
        /// </summary>
        /// <param name="targetPoint">The point the owner will orbit about.</param>
        /// <param name="startingPosition">The Vector3 the owner will start from.</param>
        /// <param name="speed">How fast, in radians, the owner will move along its path.</param>
        /// <param name="owner">The object the modifier will be applied to.</param>
        /// <param name="removeIfComplete">Set to true to delete this modifier when Active is false.</param>
        /// <param name="time">Amount of time, in frames, that it will move along the orbit.  Set to -1 to orbit forever.</param>
        public OrbitModifier3D(Vector3 targetPoint, Vector3 startingPosition, float speed, IModifiable3D owner, bool removeIfComplete, int time)
        {
            TargetPosition = targetPoint;
            Vector3 startingVector = (startingPosition - TargetPosition);
            Vector3 startingRightNormal = Vector3.Cross(startingVector, Vector3.Up);
            orbitPivot = Vector3.Cross(startingRightNormal, startingVector);
            orbitSpeed = speed;
            orbitDist = startingVector.Length();
            this.Owner = owner;

            Vector3 startingOrbitLocation = new Vector3((float)(orbitDist * startingRightNormal.X), //orbitAngle = 0, cos(orbitAngle) = 1, sin(orbitAngle) = 0.
                                                        (float)(orbitDist * startingRightNormal.Y),
                                                        (float)(orbitDist * startingRightNormal.Z)) + TargetPosition;
            orbitAngle = (float)Math.Acos(Vector3.Dot(startingPosition, startingOrbitLocation));

            Vector3 orbitRightNormal;
            if (orbitPivot != Vector3.Up)
                orbitRightNormal = Vector3.Cross(orbitPivot, Vector3.Up);
            else
                orbitRightNormal = Vector3.Right;
            Vector3 orbitUpNormal = Vector3.Cross(orbitRightNormal, orbitPivot);

            currentOrbitLocation = new Vector3((float)(orbitDist * Math.Cos(orbitAngle) * orbitRightNormal.X + orbitDist * Math.Sin(orbitAngle) * orbitUpNormal.X),
                                               (float)(orbitDist * Math.Cos(orbitAngle) * orbitRightNormal.Y + orbitDist * Math.Sin(orbitAngle) * orbitUpNormal.Y),
                                               (float)(orbitDist * Math.Cos(orbitAngle) * orbitRightNormal.Z + orbitDist * Math.Sin(orbitAngle) * orbitUpNormal.Z)) + TargetPosition;
            owner.WorldPosition = currentOrbitLocation;
            lastOwnerPosition = currentOrbitLocation;
            Active = true;
            RemoveIfComplete = removeIfComplete;
            frames = time;
        }

        /// <summary>
        /// Creates a new OrbitModifier3D.
        /// </summary>
        /// <param name="targetPoint">The point the owner will orbit about.</param>
        /// <param name="perpedicularAxis">The Vector3 perpendicular to the orbital plane.</param>
        /// <param name="distance">How far from the target point the owner will be.</param>
        /// <param name="initialAngle">The initial angle of the owner along the orbital path.</param>
        /// <param name="speed">How fast, in radians, the owner will move along its path.</param>
        /// <param name="owner">The object the modifier will be applied to.</param>
        /// <param name="removeIfComplete">Set to true to delete this modifier when Active is false.</param>
        /// <param name="time">Time, in frames, it will move along the orbit.  Set to -1 to orbit forever.</param>
        public OrbitModifier3D(Vector3 targetPoint, Vector3 perpedicularAxis, float distance, float initialAngle, float speed, IModifiable3D owner, bool removeIfComplete, int time)
        {
            TargetPosition = targetPoint;
            orbitPivot = perpedicularAxis;
            orbitSpeed = speed;
            orbitDist = distance;
            orbitAngle = initialAngle;
            this.Owner = owner;
            Vector3 orbitRightNormal;
            if (orbitPivot != Vector3.Up)
                orbitRightNormal = Vector3.Cross(orbitPivot, Vector3.Up);
            else
                orbitRightNormal = Vector3.Right;
            Vector3 orbitUpNormal = Vector3.Cross(orbitRightNormal, orbitPivot);

            currentOrbitLocation = new Vector3((float)(orbitDist * Math.Cos(orbitAngle) * orbitRightNormal.X + orbitDist * Math.Sin(orbitAngle) * orbitUpNormal.X),
                                               (float)(orbitDist * Math.Cos(orbitAngle) * orbitRightNormal.Y + orbitDist * Math.Sin(orbitAngle) * orbitUpNormal.Y),
                                               (float)(orbitDist * Math.Cos(orbitAngle) * orbitRightNormal.Z + orbitDist * Math.Sin(orbitAngle) * orbitUpNormal.Z)) + TargetPosition;
            owner.WorldPosition = currentOrbitLocation;
            lastOwnerPosition = currentOrbitLocation;
            Active = true;
            RemoveIfComplete = removeIfComplete;
            frames = time;
        }

        /// <summary>
        /// Creates a new OrbitModifier3D.
        /// </summary>
        /// <param name="targetObject">The object the owner will orbit about.</param>
        /// <param name="perpedicularAxis">The Vector3 perpendicular to the orbital plane.</param>
        /// <param name="distance">How far from the target the owner will be.</param>
        /// <param name="initialAngle">The initial angle of the owner along the orbital path.</param>
        /// <param name="speed">How fast, in radians, the owner will move along its path.</param>
        /// <param name="owner">The object the modifier will be applied to.</param>
        /// <param name="removeIfComplete">Set to true to delete this modifier when Active is false.</param>
        /// <param name="time">Time, in frames, it will move along the orbit.  Set to -1 to orbit forever.</param>
        public OrbitModifier3D(IModifiable3D targetObject, Vector3 perpedicularAxis, float distance, float initialAngle, float speed, IModifiable3D owner, bool removeIfComplete, int time)
        {
            Target = targetObject;
            TargetPosition = Target.WorldPosition;
            orbitPivot = perpedicularAxis;
            orbitSpeed = speed;
            orbitDist = distance;
            orbitAngle = initialAngle;
            Vector3 orbitRightNormal;
            this.Owner = owner;
            if (orbitPivot != Vector3.Up)
                orbitRightNormal = Vector3.Cross(orbitPivot, Vector3.Up);
            else
                orbitRightNormal = Vector3.Right;
            Vector3 orbitUpNormal = Vector3.Cross(orbitRightNormal, orbitPivot);

            currentOrbitLocation = new Vector3((float)(orbitDist * Math.Cos(orbitAngle) * orbitRightNormal.X + orbitDist * Math.Sin(orbitAngle) * orbitUpNormal.X),
                                               (float)(orbitDist * Math.Cos(orbitAngle) * orbitRightNormal.Y + orbitDist * Math.Sin(orbitAngle) * orbitUpNormal.Y),
                                               (float)(orbitDist * Math.Cos(orbitAngle) * orbitRightNormal.Z + orbitDist * Math.Sin(orbitAngle) * orbitUpNormal.Z)) + TargetPosition;
            owner.WorldPosition = currentOrbitLocation;
            lastOwnerPosition = currentOrbitLocation;
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
                    Vector3 orbitRightNormal;
                    if (orbitPivot != Vector3.Up)
                        orbitRightNormal = Vector3.Cross(orbitPivot, Vector3.Up);
                    else
                        orbitRightNormal = Vector3.Right;
                    Vector3 orbitUpNormal = Vector3.Cross(orbitRightNormal, orbitPivot);

                    if (Target != null)
                        TargetPosition = Target.WorldPosition;
                    else if (!StrictOrbit && currentOrbitLocation != Owner.WorldPosition)
                    {
                        TargetPosition += Owner.WorldPosition - currentOrbitLocation;
                        currentOrbitLocation = Owner.WorldPosition;
                    }

                    orbitAngle += orbitSpeed;
                    Vector3 nextOrbitLocation = new Vector3((float)(orbitDist * Math.Cos(orbitAngle) * orbitRightNormal.X + orbitDist * Math.Sin(orbitAngle) * orbitUpNormal.X),
                                                            (float)(orbitDist * Math.Cos(orbitAngle) * orbitRightNormal.Y + orbitDist * Math.Sin(orbitAngle) * orbitUpNormal.Y),
                                                            (float)(orbitDist * Math.Cos(orbitAngle) * orbitRightNormal.Z + orbitDist * Math.Sin(orbitAngle) * orbitUpNormal.Z)) + TargetPosition;

                    if (StrictOrbit)
                        Owner.WorldPosition = nextOrbitLocation;
                    else
                        Owner.WorldPosition += nextOrbitLocation - currentOrbitLocation;

                    currentOrbitLocation = nextOrbitLocation;
                    //orbitAngle += orbitSpeed;
                    //Vector3 orbitLocation = new Vector3((float)(orbitDist * Math.Cos(orbitAngle)),
                    //                                    0,
                    //                                    (float)(orbitDist * Math.Sin(orbitAngle)));
                    //orbitPivot.Y = owner.WorldPosition.Y;
                    //owner.WorldPosition = orbitLocation + orbitPivot;

                    if (orbitAngle > MathHelper.TwoPi)
                        orbitAngle -= MathHelper.TwoPi;
                    else if (orbitAngle < 0)
                        orbitAngle += MathHelper.TwoPi;
                    if (frames > -1)
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
            OrbitModifier3D clone;
            if (Target != null)
                clone = new OrbitModifier3D(Target, orbitPivot, orbitDist, orbitAngle, orbitSpeed, newOwner, RemoveIfComplete, frames);
            else
                clone = new OrbitModifier3D(TargetPosition, orbitPivot, orbitDist, orbitAngle, orbitSpeed, newOwner, RemoveIfComplete, frames);
            if (Owner != null)
                clone.Owner = Owner;
            else
                clone.Owner = this.Owner;
            clone.StrictOrbit = StrictOrbit;
            clone.Paused = Paused;
            clone.Active = Active;
            return clone;
        }
    }
}
