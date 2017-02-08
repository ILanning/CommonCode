using System;

namespace CommonCode
{
    /// <summary>
    /// An interface that restricts 3D modifiers to only working on 3D objects.
    /// </summary>
    public interface IModifier3D
    {
        IModifiable3D Owner { get; set; }

        string ID { get; set; }
        bool RemoveIfComplete { get; set; }
        bool Paused { get; set; }
        bool Active { get; }

        event EventHandler Complete;
        event EventHandler Pause;

        void Update();
        void Remove();

        /// <summary>
        /// Creates a copy of this modifier.
        /// </summary>
        /// <param name="owner">The object this modifier will operate on.  Use null to keep the original owner.</param>
        /// <returns></returns>
        IModifier3D DeepCopy(IModifiable3D newOwner);
    }
}
