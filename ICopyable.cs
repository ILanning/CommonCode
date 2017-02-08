namespace CommonCode
{
    public interface ICopyable<T>
    {
        T ShallowCopy();
        T ShallowCopy(LoadArgs l);
        T DeepCopy();
        T DeepCopy(LoadArgs l);
    }
}
