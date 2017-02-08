namespace CommonCode
{
    /// <summary>
    /// An interface that restricts 2D modifiers to only working on 2D objects.
    /// </summary>
    public interface IModifier2D
    {
        IModifiable2D owner { get; set; }

        string ID { get; set; }
        bool RemoveIfComplete { get; set; }
        bool Paused { get; set; }
        bool Active { get; }

        void Update();
        void Remove();
    }
}
