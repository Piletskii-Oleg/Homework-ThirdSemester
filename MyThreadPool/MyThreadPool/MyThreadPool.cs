namespace MyThreadPool;

using System.Collections.Concurrent;

/// <summary>
/// Thread pool on which calculations can be performed via <see cref="IMyTask{TResult}"/>.
/// </summary>
public class MyThreadPool : IDisposable
{
    private readonly object locker = new ();

    private readonly BlockingCollection<Action> collection;
    private readonly CountdownEvent countdownEvent;

    private volatile bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class.
    /// </summary>
    /// <param name="threadCount">Amount of threads on thread pool.</param>
    public MyThreadPool(int threadCount)
    {
        if (threadCount <= 0)
        {
            throw new ArgumentException("Thread count cannot be lesser than 1", nameof(threadCount));
        }

        this.collection = new BlockingCollection<Action>();
        this.countdownEvent = new (threadCount);

        for (int i = 0; i < threadCount; i++)
        {
            new ThreadPoolItem(this.collection, this.countdownEvent).Start();
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
        if (this.isDisposed)
        {
            return;
        }

        lock (this.locker)
        {
            this.collection.CompleteAdding();

            this.countdownEvent.Wait();
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Shutdown();

        lock (this.locker)
        {
            this.isDisposed = true;
            this.collection.Dispose();
            this.countdownEvent.Dispose();
        }
    }

    /// <summary>
    /// Represents an operation performed on <see cref="MyThreadPool"/> that returns a value.
    /// </summary>
    /// <typeparam name="T">Variable type.</typeparam>
    private class MyTask<T> : IMyTask<T>
    {
        private readonly Func<T?> operation;
        private readonly MyThreadPool threadPool;
        private readonly object locker = new ();

        private readonly ManualResetEvent resetEvent = new (false);

        private T? result;
        private volatile bool isCompleted;

        private AggregateException? aggregateException;

        private readonly BlockingCollection<Action> continuedActions = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="MyTask{T}"/> class.
        /// </summary>
        /// <param name="operation">A calculation to perform.</param>
        /// <param name="threadPool"><see cref="MyThreadPool"/> on which calculation is performed.</param>
        public MyTask(Func<T?> operation, MyThreadPool threadPool)
        {
            this.operation = operation;
            this.threadPool = threadPool;
        }

        /// <inheritdoc/>
        public bool IsCompleted => this.isCompleted;

        /// <summary>
        /// Gets result of the calculation or <see cref="AggregateException"/> if it was thrown during calculation.
        /// </summary>
        public T? Result
        {
            get
            {
                this.resetEvent.WaitOne();

                return this.aggregateException != null ? throw this.aggregateException : this.result;
            }
        }

        /// <inheritdoc/>
        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<T?, TNewResult> newOperation)
        {
            if (!this.IsCompleted)
            {
                this.resetEvent.WaitOne();
            }

            return this.threadPool.Submit(() => newOperation(this.Result));
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
                    this.result = this.operation.Invoke();
                }
                catch (Exception exception)
                {
                    this.aggregateException = new AggregateException(exception);
                }
                finally
                {
                    this.isCompleted = true;
                    this.resetEvent.Set();
                }
            }
        }
    }

    private class ThreadPoolItem
    {
        private readonly Thread thread;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadPoolItem"/> class.
        /// </summary>
        /// <param name="collection"><see cref="BlockingCollection{T}"/> from <see cref="MyThreadPool"/>.</param>
        /// <param name="countdownEvent"><see cref="CountdownEvent"/> used to indicate whether <see cref="thread"/> is finished.</param>
        public ThreadPoolItem(BlockingCollection<Action> collection, CountdownEvent countdownEvent)
        {
            this.thread = new Thread(() =>
            {
                foreach (var action in collection.GetConsumingEnumerable())
                {
                    action();
                }

                countdownEvent.Signal();
            });
        }

        /// <summary>
        /// Starts the stored <see cref="thread"/>.
        /// </summary>
        public void Start()
            => this.thread.Start();
    }
}