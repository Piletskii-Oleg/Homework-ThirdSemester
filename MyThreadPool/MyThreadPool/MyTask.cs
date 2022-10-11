namespace MyThreadPool;

public class MyTask<T> : IMyTask<T>
{
    private readonly Func<T> operation;
    private readonly MyThreadPool threadPool;
    private readonly object locker = new ();

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
            if (IsCompleted)
            {
                return Result;
            }
            else
            {
                throw new NotImplementedException(); //NotCalculatedYet
            }
        }

        private set { }
    }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<T, TNewResult> operation)
    {
        while (!IsCompleted)
        {
            
        }

        var task = new MyTask<TNewResult>(new Func<TNewResult>(() => operation(Result)), threadPool);
        threadPool.Submit(task.operation);
        return task;
    }

    public void Start()
    {
        lock (locker)
        {
            Result = operation();
            IsCompleted = true;
        }
    }
}