namespace MyLazy;

public class LazyMultiThread<T> : ILazy<T>
{
    private readonly object lockObject = new ();
    private Func<T> result = () => throw new InvalidOperationException();
    private bool isCalculated = false;
    private readonly Func<T> function;

    public LazyMultiThread(Func<T> function)
        => this.function = function;

    public T Get()
    {
        lock (lockObject)
        {
            if (!isCalculated)
            {
                var calculatedValue = function();
                result = () => calculatedValue;
                isCalculated = true;
            }
        }

        return result();
    }
}