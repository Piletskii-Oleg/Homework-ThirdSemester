namespace MyLazy;

/// <summary>
/// Represents an object whose value is calculated only when it is called. Is thread-safe.
/// </summary>
/// <typeparam name="T">Variable type.</typeparam>
public class LazyMultiThread<T> : ILazy<T>
{
    private readonly object lockObject = new ();

    private Func<T?>? function;
    private T? result;

    private volatile bool isCalculated;

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyMultiThread{T}"/> class.
    /// </summary>
    /// <param name="function">Calculation that should be performed later.</param>
    public LazyMultiThread(Func<T?>? function)
        => this.function = function;

    /// <inheritdoc/>
    public T? Get()
    {
        if (this.isCalculated)
        {
            return this.result;
        }

        lock (this.lockObject)
        {
            if (!this.isCalculated)
            {
                if (this.function == null)
                {
                    throw new InvalidOperationException();
                }

                this.result = this.function();
                this.function = null;
                this.isCalculated = true;
            }
        }

        return this.result;
    }
}