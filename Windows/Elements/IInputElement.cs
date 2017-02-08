namespace CommonCode.Windows
{
    public interface IInputElement
    {
        string FullName { get; }
        string[] Events { get; }
        void HandleInput();
    }
}
