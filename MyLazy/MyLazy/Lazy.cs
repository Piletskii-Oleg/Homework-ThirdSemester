namespace MyLazy;

public class Lazy<T> : ILazy<T>
{
    private Func<T> function;

    public T Get()
    {
        throw new NotImplementedException();
    }
}