namespace MyThreadPool.Tests;

public class Tests
{
    MyThreadPool pool = new (10);

    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void ValueCalculatedInTaskShouldBeAvailable()
    {
        pool = new MyThreadPool(3);
        var task = pool.Submit(() => 2 * 2);
        Thread.Sleep(100);

        Assert.Multiple(() =>
        {
            Assert.That(task.IsCompleted, Is.True);
            Assert.That(task.Result, Is.EqualTo(4));
        });
    }

    [Test]
    public void MultipleContinuedTasksAreCalculatedCorrectly()
    {
        pool = new MyThreadPool(3);
        var task = pool.Submit(() => 2 * 2);
        Thread.Sleep(10);

        var continued = task.ContinueWith(x => x + 4.0);
        var continuedString = task.ContinueWith(x => x.ToString());

        Assert.Multiple(() =>
        {
            Assert.That(continued.Result, Is.EqualTo(8.0));
            Assert.That(continuedString.Result, Is.EqualTo("4"));
        });
    }

    [Test]
    public void SubmittingInMultipleThreadsGivesCorrectResults()
    {
        pool = new MyThreadPool(5);
        var threads = new Thread[10];
        var results = new IMyTask<int>[10];
        for (int i = 0; i < 10; i++)
        {
            var localI = i;
            threads[i] = new Thread(() => results[localI] = pool.Submit(() => localI * 10));
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        Thread.Sleep(100);
        for (int i = 0; i < 10; i++)
        {
            Assert.That(results[i].Result, Is.EqualTo(i * 10));
        }
    }

    [Test]
    public void Test()
    {
        pool = new MyThreadPool(5);
        var task = pool.Submit(() => 4 * 4);
    }
}