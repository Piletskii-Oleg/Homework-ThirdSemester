namespace MyThreadPool;

using System.Collections.Concurrent;

public class MyThreadPool
{
    private readonly object locker = new ();

    private readonly ThreadPoolItem[] threadPollItems;

    private readonly BlockingCollection<Action> collection;

    public MyThreadPool(int threadCount)
    {
        this.threadPollItems = new ThreadPoolItem[threadCount];
        this.collection = new BlockingCollection<Action>();

        for (int i = 0; i < threadCount; i++)
        {
            threadPollItems[i] = new ThreadPoolItem(collection);
            threadPollItems[i].Start();
        }
    }

    public IMyTask<TResult> Submit<TResult>(Func<TResult> operation)
    {
        lock (locker)
        {
            var task = new MyTask<TResult>(operation, this);
            collection.Add(() => task.Start());

            return task;
        }
    }

    public void Shutdown()
    {
        collection.CompleteAdding();
        while (true)
        {
            if (!collection.TryTake(out Action _))
            {
                foreach (var item in threadPollItems)
                {
                    item.Join();
                }
            }
        }
    }

    private class ThreadPoolItem
    {
        private readonly Thread thread;

        public ThreadPoolItem(BlockingCollection<Action> collection)
        {
            thread = new Thread(() =>
            {
                while (!collection.IsAddingCompleted)
                {
                    if (collection.TryTake(out var action))
                    {
                        action();
                    }
                }
            });
        }

        public void Start()
            => thread.Start();

        public void Join()
            => thread.Join();
    }
}