namespace PriorityQueue.Tests;

public class Tests
{
    private PriorityQueue<int> queue = new();
    
    [SetUp]
    public void Setup()
    {
        queue = new PriorityQueue<int>();
    }

    [Test]
    public void SequentialEnqueueShouldWork()
    {
        queue.Enqueue(3, 1);
        queue.Enqueue(3, 4);
        queue.Enqueue(5, 2);
        queue.Enqueue(4, 2);
        queue.Enqueue(4, 0);
        
        Assert.That(queue.Size(), Is.EqualTo(5));
    }

    [Test]
    public void SequentialDequeueShouldWork()
    {
        queue.Enqueue(1, 1);
        queue.Enqueue(4, 4);
        Assert.That(queue.Dequeue(), Is.EqualTo(4));
        Assert.That(queue.Dequeue(), Is.EqualTo(1));
    }

    [Test]
    public void ParallelEnqueueShouldWork()
    {
        Parallel.Invoke(() => queue.Enqueue(1, 1),
            () => queue.Enqueue(2, 2));
        
        Assert.That(queue.Dequeue(), Is.EqualTo(2));
        Assert.That(queue.Dequeue(), Is.EqualTo(1));
    }

    [Test]
    public void DequeueCanTakeElementThatWasAddedAfterItsCall()
    {
        int value = 0;
        Task.Run(() => value = queue.Dequeue());
        
        queue.Enqueue(3, 1);
        
        Thread.Sleep(100);
        Assert.That(value, Is.EqualTo(3));
    }
}