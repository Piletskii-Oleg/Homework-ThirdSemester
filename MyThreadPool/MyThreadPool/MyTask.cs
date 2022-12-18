namespace MyThreadPool;

/// <summary>
/// Represents an operation performed on <see cref="MyThreadPool"/> that returns a value.
/// </summary>
/// <typeparam name="T">Variable type.</typeparam>
public class MyTask<T> : IMyTask<T>
{
    private readonly Func<T> operation;
    private readonly MyThreadPool threadPool;
    private readonly object locker = new ();

    private readonly ManualResetEvent resetEvent = new (false);

    private T result;
    private AggregateException aggregateException;
    private bool hasThrownException;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyTask{T}"/> class.
    /// </summary>
    /// <param name="operation">A calculation to perform.</param>
    /// <param name="threadPool"><see cref="MyThreadPool"/> on which calculation is performed.</param>
    public MyTask(Func<T> operation, MyThreadPool threadPool)
    {
        this.operation = operation;
        this.threadPool = threadPool;
    }

    /// <inheritdoc/>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// Gets result of the calculation or <see cref="AggregateException"/> if it was thrown during calculation.
    /// </summary>
    public T Result
    {
        get
        {
            this.resetEvent.WaitOne();
            return this.hasThrownException ? throw this.aggregateException : this.result;
        }

        private set => this.result = value;
    }

    /// <inheritdoc/>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<T, TNewResult> operation)
    {
        if (!this.IsCompleted)
        {
            this.resetEvent.WaitOne();
        }

        return this.threadPool.Submit(new Func<TNewResult>(() => operation(this.Result)));
    }

    /// <summary>
    /// Performs the stored operation.
    /// </summary>
    public void Start()
    {
        lock (this.locker)
        {
            try
            {
                this.Result = this.operation.Invoke();
            }
            catch (Exception exception)
            {
                this.hasThrownException = true;
                this.aggregateException = new AggregateException(exception);
            }
            finally
            {
                this.IsCompleted = true;
                this.resetEvent.Set();
            }
        }
    }
}