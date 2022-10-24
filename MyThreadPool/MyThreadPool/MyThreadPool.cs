namespace MyThreadPool;

using System.Collections.Concurrent;

/// <summary>
/// Thread pool on which calculations can be performed via <see cref="IMyTask{TResult}"/>.
/// </summary>
public class MyThreadPool
{
    private readonly object locker = new ();

    private readonly ThreadPoolItem[] threadPoolItems;
    private readonly BlockingCollection<Action> collection;

    private readonly ManualResetEvent resetEvent = new (false);
    private readonly CancellationTokenSource source = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class.
    /// </summary>
    /// <param name="threadCount">Amount of threads on thread pool.</param>
    /// <exception cref="IncorrectThreadCountException">Throws if the amount of threads was lesser than 1</exception>
    public MyThreadPool(int threadCount)
    {
        if (threadCount <= 0)
        {
            throw new ArgumentException("Thread count cannot be lesser than 1", nameof(threadCount));
        }

        this.threadPoolItems = new ThreadPoolItem[threadCount];
        this.collection = new BlockingCollection<Action>();

        for (int i = 0; i < threadCount; i++)
        {
            this.threadPoolItems[i] = new ThreadPoolItem(this.collection, this.resetEvent, this.source.Token);
            this.threadPoolItems[i].Start();
        }
    }

    /// <summary>
    /// Puts <paramref name="operation"/> on the queue and returns <see cref="IMyTask{TResult}"/> with value that will be calculated.
    /// </summary>
    /// <typeparam name="TResult">Value type.</typeparam>
    /// <param name="operation">A calculation to perform.</param>
    /// <returns><see cref="IMyTask{TResult}"/> with value that will be calculated.</returns>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> operation)
    {
        lock (this.locker)
        {
            var task = new MyTask<TResult>(operation, this);
            this.collection.Add(() => task.Start());

            return task;
        }
    }

    /// <summary>
    /// Disables submitting and waits until every calculation in the queue is performed.
    /// </summary>
    public void Shutdown()
    {
        this.collection.CompleteAdding();

        while (true)
        {
            if (this.collection.Count == 0)
            {
                this.source.Cancel();
                foreach (var item in this.threadPoolItems)
                {
                    if (item.IsActive)
                    {
                        this.resetEvent.WaitOne();
                    }

                    item.Join();
                }

                break;
            }
        }
    }

    private class ThreadPoolItem
    {
        private readonly Thread thread;
        private readonly BlockingCollection<Action> collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadPoolItem"/> class.
        /// </summary>
        /// <param name="collection"><see cref="BlockingCollection{T}"/> from <see cref="MyThreadPool"/>.</param>
        /// <param name="resetEvent"><see cref="ManualResetEvent"/> used to indicate whether <see cref="thread"/> is performing a calculation.</param>
        /// <param name="token">Cancellation token from <see cref="source"/>.</param>
        public ThreadPoolItem(BlockingCollection<Action> collection, ManualResetEvent resetEvent, CancellationToken token)
        {
            this.collection = collection;

            this.thread = new Thread(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    if (this.collection.TryTake(out var action))
                    {
                        resetEvent.Reset();
                        this.IsActive = true;

                        action();

                        resetEvent.Set();
                        this.IsActive = false;
                    }
                }
            });
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="ThreadPoolItem"/>'s thread is performing a calculation.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Starts the stored <see cref="thread"/>.
        /// </summary>
        public void Start()
            => this.thread.Start();

        /// <summary>
        /// Stops the stored <see cref="thread"/>.
        /// </summary>
        public void Join()
            => this.thread.Join();
    }
}