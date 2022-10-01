namespace MyThreadPool;

public class MyTask<T> : IMyTask<T>
{
    private Func<T> operation;

    public MyTask(Func<T> operation)
    {
        this.operation = operation;
    }

    public bool IsCompleted { get; private set; }

    public T Result { get; private set; }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<T, TNewResult> operation)
    {
        return new MyTask<TNewResult>(new Func<TNewResult>(() => operation(Result)));
    }
}