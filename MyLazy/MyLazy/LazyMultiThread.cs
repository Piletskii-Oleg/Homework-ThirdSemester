namespace MyLazy;

/// <summary>
/// Represents an object whose value is calculated only when it is called. Is thread-safe.
/// </summary>
/// <typeparam name="T">Variable type.</typeparam>
public class LazyMultiThread<T> : ILazy<T>
{
    private readonly Func<T> function;
    private readonly object lockObject = new ();
    private Func<T> result = () => throw new InvalidOperationException();
    private bool isCalculated;

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyMultiThread{T}"/> class.
    /// </summary>
    /// <param name="function">Calculation that should be performed later.</param>
    public LazyMultiThread(Func<T> function)
        => this.function = function;

    /// <inheritdoc/>
    public T Get()
    {
        lock (this.lockObject)
        {
            if (!this.isCalculated)
            {
                var calculatedValue = this.function();
                this.result = () => calculatedValue;
                this.isCalculated = true;
            }
        }

        return this.result();
    }
}