using System.Collections.Concurrent;

namespace MyThreadPool;

public class MyThreadPool
{
    private readonly int threadNumber;
    private readonly Thread[] threads;
    private readonly ConcurrentQueue<Action> queue = new ();

    public MyThreadPool(int threadNumber)
    {
        this.threadNumber = threadNumber;
        this.threads = new Thread[threadNumber];
    }

    public void Submit<TResult>(Func<TResult> operation)
    {
        queue.Enqueue(new Action(() => operation()));
    }

    public void Shutdown()
    {

    }
}