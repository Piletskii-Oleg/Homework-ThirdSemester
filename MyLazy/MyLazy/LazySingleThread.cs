namespace MyLazy;

public class LazySingleThread<T> : ILazy<T>
{
    private readonly Func<T> function;
    private Func<T> result = () => throw new InvalidOperationException();
    private bool isCalculated = false;

    public LazySingleThread(Func<T> function)
        => this.function = function;

    public T Get()
    {
        if (!isCalculated)
        {
            var calculatedValue = function();
            result = () => calculatedValue;
            isCalculated = true;
        }

        return result();
    }
}