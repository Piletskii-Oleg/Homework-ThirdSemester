namespace PriorityQueue;

public class PriorityQueue<T>
{
    private List<QueueElement> queue;
    private volatile int size;

    private readonly object lockObject = new object();

    public PriorityQueue()
    {
        queue = new ();
    }

    public void Enqueue(T value, int priority)
    {
        lock (this.lockObject)
        {
            var indexToAdd = 0;
            for (int i = 0; i < queue.Count; i++)
            {
                if (priority > queue[i].Priority)
                {
                    indexToAdd = i;
                }
            }
            
            queue.Insert(indexToAdd, new QueueElement(value, priority));
            Interlocked.Increment(ref size);
            Monitor.PulseAll(lockObject);
        }
    }

    public T Dequeue()
    {
        lock (lockObject)
        {
            if (queue.Count == 0)
            {
                Monitor.Wait(lockObject);
            }

            int maxPriority = 0;
            int elementIndex = 0;
            for (int i = 0; i < queue.Count; i++)
            {
                if (queue[i].Priority > maxPriority)
                {
                    maxPriority = queue[i].Priority;
                    elementIndex = i;
                }
            }

            var element = queue[elementIndex];
            queue.RemoveAt(elementIndex);
            
            Interlocked.Decrement(ref size);
            return element.Value;
        }
    }

    public int Size()
        => size;
    
    private class QueueElement
    {
        public T Value { get; }

        public int Priority { get; }

        public QueueElement(T value, int priority)
        {
            this.Value = value;
            this.Priority = priority;
        }
    }
}