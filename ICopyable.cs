namespace CommonCode
{
    public interface ICopyable<T>
    {
        T ShallowCopy();
        T DeepCopy();
    }
}
