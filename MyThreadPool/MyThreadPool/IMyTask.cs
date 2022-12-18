namespace MyThreadPool;

/// <summary>
/// Represents an operation performed on <see cref="MyThreadPool"/> that returns a value.
/// </summary>
/// <typeparam name="TResult">Variable type.</typeparam>
public interface IMyTask<TResult>
{
    /// <summary>
    /// Gets a value indicating whether result is calculated.
    /// </summary>
    bool IsCompleted { get; }

    /// <summary>
    /// Gets result of the calculation.
    /// </summary>
    TResult Result { get; }

    /// <summary>
    /// Performs another operation using <see cref="Result"/>
    /// and returns <see cref="IMyTask{TNewResult}"/> with the new type and value..
    /// </summary>
    /// <typeparam name="TNewResult">Type of the resulting value. Can be same as previous.</typeparam>
    /// <param name="operation">A calculation to make.</param>
    /// <returns><see cref="IMyTask{TNewResult}"/> with the new type and value.</returns>
    IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> operation);
}