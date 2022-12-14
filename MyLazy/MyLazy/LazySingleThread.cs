namespace MyLazy;

/// <summary>
/// Represents an object whose value is calculated only when it is called. Only for single-thread programs.
/// </summary>
/// <typeparam name="T">Variable type.</typeparam>
public class LazySingleThread<T> : ILazy<T>
{
    private Func<T?>? function;
    private T? result;

    private bool isCalculated;

    /// <summary>
    /// Initializes a new instance of the <see cref="LazySingleThread{T}"/> class.
    /// </summary>
    /// <param name="function">Calculation that should be performed later.</param>
    public LazySingleThread(Func<T?>? function)
        => this.function = function;

    /// <inheritdoc/>
    public T? Get()
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

        return this.result;
    }
}