namespace MyThreadPool;

using System.Collections.Concurrent;

public class MyTask<T> : IMyTask<T>
{
    private readonly Func<T> operation;
    private readonly MyThreadPool threadPool;
    private readonly object locker = new ();

    private readonly ManualResetEvent resetEvent = new (false);

    private T result;

    public MyTask(Func<T> operation, MyThreadPool threadPool)
    {
        this.operation = operation;
        this.threadPool = threadPool;
    }

    public bool IsCompleted { get; private set; }

    public T Result
    {
        get
        {
            resetEvent.WaitOne();
            return result;
        }

        private set => result = value;
    }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<T, TNewResult> operation)
    {
        if (!IsCompleted)
        {
            resetEvent.WaitOne();
        }

        return threadPool.Submit(new Func<TNewResult>(() => operation(Result)));
    }

    public void Start()
    {
        lock (locker)
        {
            Result = operation.Invoke();
            IsCompleted = true;
            resetEvent.Set();
        }
    }
}