namespace MyThreadPool;

using System.Collections.Concurrent;

public class MyThreadPool
{
    private readonly Thread[] threads;

    private readonly BlockingCollection<Action> collection;

    public MyThreadPool(int threadCount)
    {
        this.threads = new Thread[threadCount];
        
        this.collection = new BlockingCollection<Action>();

        for (int i = 0; i < threadCount; i++)
        {
            threads[i] = new Thread(() =>
            {
                while (true)
                {
                    if (collection.TryTake(out Action action))
                    {
                        action();
                    }
                }
            });
        }

        foreach (Thread thread in threads)
        {
            thread.Start();
        }
    }

    public IMyTask<TResult> Submit<TResult>(Func<TResult> operation)
    {
        var task = new MyTask<TResult>(operation, this);
        collection.Add(() => task.Start());
        return task;
    }

    public void Shutdown()
    {
        collection.CompleteAdding();
        while (true)
        {
            if (!collection.TryTake(out Action _))
            {
                break;
            }
        }
    }

    private class ThreadPoolItem
    {
        private readonly Thread thread;
        private readonly BlockingCollection<Action> collection;

        public ThreadPoolItem(BlockingCollection<Action> collection)
        {
            this.collection = collection;

            thread = new Thread(() =>
            {
                while (true)
                {
                    if (collection.TryTake(out Action action))
                    {
                        action();
                    }
                }
            });
        }
    }
}